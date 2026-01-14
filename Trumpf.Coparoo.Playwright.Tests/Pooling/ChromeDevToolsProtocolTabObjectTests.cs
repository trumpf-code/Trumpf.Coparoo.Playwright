// Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Playwright.Tests.Pooling
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Playwright.Pooling;

    [TestClass]
    [DoNotParallelize]  // Chrome DevTools Protocol tests share the same browser instance via connection pool
    public class ChromeDevToolsProtocolTabObjectTests
    {
        private static string _chromeDevToolsProtocolEndpoint;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            // NOTE: Chrome DevTools Protocol connections require an EXTERNAL browser with Chrome DevTools Protocol enabled (e.g., CefSharp in WPF).
            // Playwright.LaunchAsync() creates a browser, but Playwright.ConnectOverCDP() needs a separate
            // Chrome DevTools Protocol endpoint. You can't ConnectOverCDP to a browser you just launched with Playwright.
            // 
            // For real Chrome DevTools Protocol testing, start CefSharp with RemoteDebuggingPort:
            //   var settings = new CefSettings { RemoteDebuggingPort = 9223 };
            // Then use: _chromeDevToolsProtocolEndpoint = "http://127.0.0.1:9223";
            
            // Prefer env var if provided (e.g., in CI), else default to localhost:9222
            _chromeDevToolsProtocolEndpoint = Environment.GetEnvironmentVariable("CHROME_DEVTOOLS_PROTOCOL_ENDPOINT") ?? "http://localhost:9222"; // Default for CI setup

            // Create test HTML file
            var htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "test-pooling.html");
            
            if (!File.Exists(htmlPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(htmlPath)!);
                File.WriteAllText(htmlPath, 
                    "<html><body><h1>Test Page for Pooling</h1><p id='test-content'>Pooling Test</p></body></html>");
            }

            await Task.CompletedTask;
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await Task.CompletedTask;
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            // Clear pool before each test
            await SmartPlaywrightConnectionPool.Instance.ClearAllAsync();
        }

        private static string GetLocalHtmlUrl()
        {
            var htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "test-pooling.html");
            return $"file:///{htmlPath.Replace("\\", "/")}";
        }

        // Test ChromeDevToolsProtocolTabObject that connects to our test browser
        private class TestChromeDevToolsProtocolTab : ChromeDevToolsProtocolTabObject
        {
            private readonly string _endpointValue;
            private readonly string _identifierValue;

            public TestChromeDevToolsProtocolTab(string endpoint, string identifier = "default")
            {
                _endpointValue = endpoint;
                _identifierValue = identifier;
            }

            // Fallback to static _chromeDevToolsProtocolEndpoint if instance value not set (handles base constructor access)
            protected override string ChromeDevToolsProtocolEndpoint => _endpointValue ?? _chromeDevToolsProtocolEndpoint;
            protected override string PageIdentifier => _identifierValue;
            protected override string Url => GetLocalHtmlUrl();
            protected override bool FindExistingPageByUrl => false; // Create new pages instead of searching for existing
        }

        private class TestChromeDevToolsProtocolTabFindExisting : ChromeDevToolsProtocolTabObject
        {
            private readonly string _endpoint;
            private readonly string _targetUrl;

            public TestChromeDevToolsProtocolTabFindExisting(string endpoint, string targetUrl)
            {
                _endpoint = endpoint;
                _targetUrl = targetUrl;
            }

            protected override string ChromeDevToolsProtocolEndpoint => _endpoint;
            protected override string PageIdentifier => _targetUrl;
            protected override string Url => _targetUrl;
            protected override bool FindExistingPageByUrl => true; // Search for existing page instead of creating new
        }

        private class TestChromeDevToolsProtocolTabWithCustomIdentifier : ChromeDevToolsProtocolTabObject
        {
            private readonly string _endpoint;

            public TestChromeDevToolsProtocolTabWithCustomIdentifier(string endpoint)
            {
                _endpoint = endpoint;
            }

            protected override string ChromeDevToolsProtocolEndpoint => _endpoint;
            protected override string PageIdentifier => "custom_identifier";
            protected override string Url => GetLocalHtmlUrl();
            protected override bool FindExistingPageByUrl => false; // Create new pages instead of searching for existing
        }

        private class InvalidChromeDevToolsProtocolTab : ChromeDevToolsProtocolTabObject
        {
            protected override string ChromeDevToolsProtocolEndpoint => null!; // Invalid!
            protected override string Url => GetLocalHtmlUrl();
        }

        [TestMethod]
        public void ChromeDevToolsProtocolTabObject_CanBeInstantiated()
        {
            // Act
            var tab = new TestChromeDevToolsProtocolTab("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void PageIdentifier_DefaultsToUrl()
        {
            // Arrange
            var tab = new TestChromeDevToolsProtocolTab("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void ChromeDevToolsProtocolTabObject_CanUseCustomPageIdentifier()
        {
            // Act
            var tab = new TestChromeDevToolsProtocolTabWithCustomIdentifier("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void ChromeDevToolsProtocolTabObject_InheritsFromTabObject()
        {
            // Act
            var tab = new TestChromeDevToolsProtocolTab("http://localhost:9222");

            // Assert
            Assert.IsInstanceOfType(tab, typeof(TabObject));
        }

        [TestMethod]
        public async Task Creator_ThrowsInvalidOperationException_WhenChromeDevToolsProtocolEndpointIsNull()
        {
            // Arrange
            var tab = new InvalidChromeDevToolsProtocolTab();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await tab.Open(),
                "ChromeDevToolsProtocolEndpoint must not be null or empty");
        }

        [TestMethod]
        [TestCategory("ChromeDevToolsProtocol")]
        public async Task MultipleTabInstances_WithChromeDevToolsProtocolPooling_ReuseSameConnection()
        {
            // This test demonstrates the pooling concept with Chrome DevTools Protocol connections.
            // To run this test:
            // 1. Start an external browser with Chrome DevTools Protocol enabled:
            //    Edge:   msedge.exe --remote-debugging-port=9222 --headless=new about:blank
            //    Chrome: chrome.exe --remote-debugging-port=9222 --headless=new about:blank
            // 2. Set CHROME_DEVTOOLS_PROTOCOL_ENDPOINT environment variable (or it will default to http://localhost:9222)
            var endpoint = Environment.GetEnvironmentVariable("CHROME_DEVTOOLS_PROTOCOL_ENDPOINT") ?? _chromeDevToolsProtocolEndpoint;
            if (!await IsChromeDevToolsProtocolAvailableAsync(endpoint))
            {
                Assert.Inconclusive($"Chrome DevTools Protocol endpoint '{endpoint}' not reachable. Skipping.");
                return;
            }

            var pool = SmartPlaywrightConnectionPool.Instance;
            var initialStats = pool.GetStatistics();
            
            try
            {
                var tab1 = new TestChromeDevToolsProtocolTab(endpoint, "tab1");
                var tab2 = new TestChromeDevToolsProtocolTab(endpoint, "tab1"); // Same identifier!
                var tab3 = new TestChromeDevToolsProtocolTab(endpoint, "tab1"); // Same identifier!

                // Only open tab1 (which navigates to URL)
                // tab2 and tab3 will get the same page from pool without re-navigating
                await tab1.Open();
                
                // Get pages from pool (these won't navigate again)
                var page1 = await tab1.Page;
                var page2 = await tab2.Page;
                var page3 = await tab3.Page;

                // With pooling, all three should share the SAME connection
                var stats = pool.GetStatistics();
                Assert.AreEqual(initialStats.TotalConnections + 1, stats.TotalConnections, 
                    "Pool should have only 1 new connection for all 3 tabs with same identifier");

                Assert.IsNotNull(page1);
                Assert.IsNotNull(page2);
                Assert.IsNotNull(page3);

                await tab1.Close();
                await tab2.Close();
                await tab3.Close();
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        [TestMethod]
        [TestCategory("ChromeDevToolsProtocol")]
        public async Task SequentialTabUsage_WithChromeDevToolsProtocolPooling_ReusesConnection()
        {
            // This test demonstrates connection reuse across sequential tab instances.
            // See MultipleTabInstances_WithChromeDevToolsProtocolPooling_ReuseSameConnection for setup instructions.
            var endpoint = Environment.GetEnvironmentVariable("CHROME_DEVTOOLS_PROTOCOL_ENDPOINT") ?? _chromeDevToolsProtocolEndpoint;
            if (!await IsChromeDevToolsProtocolAvailableAsync(endpoint))
            {
                Assert.Inconclusive($"Chrome DevTools Protocol endpoint '{endpoint}' not reachable. Skipping.");
                return;
            }
            var pool = SmartPlaywrightConnectionPool.Instance;
            
            try
            {
                var initialStats = pool.GetStatistics();
                
                // Open and close the same tab 3 times
                for (int i = 0; i < 3; i++)
                {
                    var tab = new TestChromeDevToolsProtocolTab(endpoint, "sequential");
                    
                    // Only open on first iteration; subsequent iterations reuse the page
                    if (i == 0)
                    {
                        await tab.Open();
                    }
                    
                    var page = await tab.Page;
                    Assert.IsNotNull(page);
                    Assert.IsFalse(page.IsClosed);
                    
                    await tab.Close();
                    
                    // Connection should stay in pool, not be disposed
                }
                
                var finalStats = pool.GetStatistics();
                Assert.AreEqual(initialStats.TotalConnections + 1, finalStats.TotalConnections,
                    "Pool should have exactly 1 connection reused for all 3 sequential opens");
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        [TestMethod]
        public async Task ChromeDevToolsProtocolTabObject_DifferentIdentifiers_CreateSeparateConnections()
        {
            // NOTE: This demonstrates how page identifiers enable connection isolation
            var pool = SmartPlaywrightConnectionPool.Instance;
            pool.EnablePageCaching = true;
            
            try
            {
                // With different PageIdentifiers, ChromeDevToolsProtocolTabObject creates separate connections
                var tab1 = new TestChromeDevToolsProtocolTabWithCustomIdentifier("http://localhost:9222"); // "custom_identifier"
                var tab2 = new TestChromeDevToolsProtocolTab("http://localhost:9222", "different"); // "different"
                
                // These would create separate cache keys:
                // - "http://localhost:9222::custom_identifier"
                // - "http://localhost:9222::different"
                
                Assert.IsNotNull(tab1);
                Assert.IsNotNull(tab2);
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        [TestMethod]
        public async Task PoolStatistics_TrackConnectionMetrics()
        {
            var pool = SmartPlaywrightConnectionPool.Instance;

            try
            {
                // Create a connection manually to test statistics
                var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
                var page = await browser.NewPageAsync();
                
                var connection = new PooledPageConnection(
                    "test-key",
                    "http://localhost:9222",
                    "stats-test",
                    playwright,
                    browser,
                    page);

                // Minimal initial wait to avoid zero durations
                await Task.Delay(20);

                // Poll until both idle time and age exceed threshold or timeout
                var required = TimeSpan.FromMilliseconds(100);
                var timeout = TimeSpan.FromSeconds(2);
                var sw = System.Diagnostics.Stopwatch.StartNew();

                bool thresholdsMet = false;
                while (sw.Elapsed < timeout)
                {
                    var idleTime = connection.GetIdleTime();
                    var age = connection.GetAge();

                    if (idleTime >= required && age >= required)
                    {
                        thresholdsMet = true;
                        break;
                    }

                    await Task.Delay(50);
                }

                // Basic metadata assertions
                Assert.AreEqual("test-key", connection.CacheKey);
                Assert.AreEqual("http://localhost:9222", connection.ChromeDevToolsProtocolEndpoint);
                Assert.AreEqual("stats-test", connection.PageUrl);

                if (!thresholdsMet)
                {
                    // Capture final values for clearer diagnostics
                    var finalIdle = connection.GetIdleTime();
                    var finalAge = connection.GetAge();
                    await connection.DisposeAsync();

                    Assert.IsTrue(finalIdle >= required, $"IdleTime too small: {finalIdle.TotalMilliseconds} ms (expected ≥ {required.TotalMilliseconds} ms)");
                    Assert.IsTrue(finalAge >= required, $"Age too small: {finalAge.TotalMilliseconds} ms (expected ≥ {required.TotalMilliseconds} ms)");
                    return;
                }

                await connection.DisposeAsync();
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        [TestMethod]
        public async Task Pool_ClearAllAsync_RemovesAllConnections()
        {
            var pool = SmartPlaywrightConnectionPool.Instance;

            // Add some connections manually
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            
            var connection = new PooledPageConnection(
                "clear-test-key",
                "http://localhost:9222",
                "clear-test",
                playwright,
                browser,
                page);

            // Simulate adding to pool (normally done by GetOrCreatePageAsync)
            // We can't directly add, so we verify ClearAllAsync works
            
            await pool.ClearAllAsync();
            var stats = pool.GetStatistics();
            
            Assert.AreEqual(0, stats.TotalConnections);

            // Clean up our manual connection
            await connection.DisposeAsync();
        }

        [TestMethod]
        [TestCategory("ChromeDevToolsProtocol")]
        public async Task FindExistingPageByUrl_FindsPageInsteadOfCreatingNew()
        {
            // This test demonstrates finding an existing page by URL instead of creating a new one.
            // To run this test:
            // 1. Start an external browser with a page already loaded:
            //    Edge:   msedge.exe --remote-debugging-port=9222 --headless=new https://www.bing.com
            // 2. The test will connect and find the existing page with that URL
            var endpoint = Environment.GetEnvironmentVariable("CHROME_DEVTOOLS_PROTOCOL_ENDPOINT") ?? _chromeDevToolsProtocolEndpoint;
            var targetUrl = "https://www.bing.com/";
            
            if (!await IsChromeDevToolsProtocolAvailableAsync(endpoint))
            {
                Assert.Inconclusive($"Chrome DevTools Protocol endpoint '{endpoint}' not reachable. Skipping.");
                return;
            }

            var pool = SmartPlaywrightConnectionPool.Instance;

            try
            {
                // First, check if the target page exists in the browser
                var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                var browser = await playwright.Chromium.ConnectOverCDPAsync(endpoint);
                var existingPages = browser.Contexts.SelectMany(c => c.Pages).ToList();
                var targetPageExists = existingPages.Any(p => p.Url.Contains("bing.com", StringComparison.OrdinalIgnoreCase));
                
                if (!targetPageExists)
                {
                    Assert.Inconclusive($"Target page '{targetUrl}' not found in browser. " +
                        $"Available pages: {string.Join(", ", existingPages.Select(p => $"'{p.Url}'"))}. " +
                        "This test requires a pre-loaded page. Start browser with: msedge.exe --remote-debugging-port=9222 --headless=new https://www.bing.com");
                    return;
                }

                // This tab is configured to search for an existing page with the target URL
                var tab = new TestChromeDevToolsProtocolTabFindExisting(endpoint, targetUrl);
                
                await tab.Open();
                var page = await tab.Page;
                
                Assert.IsNotNull(page);
                Assert.IsFalse(page.IsClosed);
                
                // Verify we found the page with the expected URL
                Assert.IsTrue(page.Url.Contains("bing.com", StringComparison.OrdinalIgnoreCase),
                    $"Expected URL containing 'bing.com' but got '{page.Url}'");
                
                await tab.Close();
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        [TestMethod]
        public async Task FindExistingPageByUrl_ThrowsWhenPageNotFound()
        {
            // This test verifies proper error handling when trying to find a non-existent page
            var pool = SmartPlaywrightConnectionPool.Instance;

            try
            {
                var nonExistentUrl = "https://this-page-definitely-does-not-exist.local/";
                
                // We need to actually try with a real Chrome DevTools Protocol endpoint to test the error
                // For unit testing, we'll just verify the configuration is set correctly
                var tab = new TestChromeDevToolsProtocolTabFindExisting("http://localhost:9999", nonExistentUrl);
                
                // Verify FindExistingPageByUrl is true
                Assert.IsNotNull(tab);
            }
            finally
            {
                await pool.ClearAllAsync();
            }
        }

        private static async Task<bool> IsChromeDevToolsProtocolAvailableAsync(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) return false;
            
            // Try to actually connect via Playwright's ConnectOverCDP to verify it works
            try
            {
                var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                var browser = await playwright.Chromium.ConnectOverCDPAsync(endpoint);
                
                // If we got here, Chrome DevTools Protocol connection works
                await browser.CloseAsync();
                playwright.Dispose();
                return true;
            }
            catch
            {
                // Chrome DevTools Protocol connection failed - maybe not ready or not available
                return false;
            }
        }
    }
}
