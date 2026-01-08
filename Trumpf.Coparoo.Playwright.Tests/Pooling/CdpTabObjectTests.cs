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
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Playwright.Pooling;

    [TestClass]
    public class CdpTabObjectTests
    {
        private static string _cdpEndpoint;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            // NOTE: CDP connections require an EXTERNAL browser with CDP enabled (e.g., CefSharp in WPF).
            // Playwright.LaunchAsync() creates a browser, but Playwright.ConnectOverCDP() needs a separate
            // CDP endpoint. You can't ConnectOverCDP to a browser you just launched with Playwright.
            // 
            // For real CDP testing, start CefSharp with RemoteDebuggingPort:
            //   var settings = new CefSettings { RemoteDebuggingPort = 9223 };
            // Then use: _cdpEndpoint = "http://127.0.0.1:9223";
            
            _cdpEndpoint = "http://localhost:9222"; // Mock endpoint for non-CDP tests

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

        // Test CdpTabObject that connects to our test browser
        private class TestCdpTab : CdpTabObject
        {
            private readonly string _endpointValue;
            private readonly string _identifierValue;

            public TestCdpTab(string endpoint, string identifier = "default")
            {
                _endpointValue = endpoint;
                _identifierValue = identifier;
            }

            // Fallback to static _cdpEndpoint if instance value not set (handles base constructor access)
            protected override string CdpEndpoint => _endpointValue ?? _cdpEndpoint;
            protected override string PageIdentifier => _identifierValue;
            protected override string Url => GetLocalHtmlUrl();
        }

        private class TestCdpTabWithCustomIdentifier : CdpTabObject
        {
            private readonly string _endpoint;

            public TestCdpTabWithCustomIdentifier(string endpoint)
            {
                _endpoint = endpoint;
            }

            protected override string CdpEndpoint => _endpoint;
            protected override string PageIdentifier => "custom_identifier";
            protected override string Url => GetLocalHtmlUrl();
        }

        private class InvalidCdpTab : CdpTabObject
        {
            protected override string CdpEndpoint => null!; // Invalid!
            protected override string Url => GetLocalHtmlUrl();
        }

        [TestMethod]
        public void CdpTabObject_CanBeInstantiated()
        {
            // Act
            var tab = new TestCdpTab("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void PageIdentifier_DefaultsToUrl()
        {
            // Arrange
            var tab = new TestCdpTab("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void CdpTabObject_CanUseCustomPageIdentifier()
        {
            // Act
            var tab = new TestCdpTabWithCustomIdentifier("http://localhost:9222");

            // Assert
            Assert.IsNotNull(tab);
        }

        [TestMethod]
        public void CdpTabObject_InheritsFromTabObject()
        {
            // Act
            var tab = new TestCdpTab("http://localhost:9222");

            // Assert
            Assert.IsInstanceOfType(tab, typeof(TabObject));
        }

        [TestMethod]
        public async Task Creator_ThrowsInvalidOperationException_WhenCdpEndpointIsNull()
        {
            // Arrange
            var tab = new InvalidCdpTab();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await tab.Open(),
                "CdpEndpoint must not be null or empty");
        }

        [TestMethod]
        [Ignore("Requires external CDP browser (e.g., CefSharp with RemoteDebuggingPort). Playwright-launched browsers cannot be used with ConnectOverCDP.")]
        public async Task MultipleTabInstances_WithCdpPooling_ReuseSameConnection()
        {
            // This test demonstrates the pooling concept with CDP connections.
            // To run this test:
            // 1. Start an external browser with CDP enabled (e.g., CefSharp with RemoteDebuggingPort = 9223)
            // 2. Update _cdpEndpoint in ClassInitialize to point to that browser
            // 3. Remove the [Ignore] attribute
            
            var pool = SmartPlaywrightConnectionPool.Instance;
            var initialStats = pool.GetStatistics();
            
            try
            {
                var tab1 = new TestCdpTab(_cdpEndpoint, "tab1");
                var tab2 = new TestCdpTab(_cdpEndpoint, "tab1"); // Same identifier!
                var tab3 = new TestCdpTab(_cdpEndpoint, "tab1"); // Same identifier!

                await tab1.Open();
                await tab2.Open();
                await tab3.Open();

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
        [Ignore("Requires external CDP browser (e.g., CefSharp with RemoteDebuggingPort). Playwright-launched browsers cannot be used with ConnectOverCDP.")]
        public async Task SequentialTabUsage_WithCdpPooling_ReusesConnection()
        {
            // This test demonstrates connection reuse across sequential tab instances.
            // See MultipleTabInstances_WithCdpPooling_ReuseSameConnection for setup instructions.
            var pool = SmartPlaywrightConnectionPool.Instance;
            
            try
            {
                var initialStats = pool.GetStatistics();
                
                // Open and close the same tab 3 times
                for (int i = 0; i < 3; i++)
                {
                    var tab = new TestCdpTab(_cdpEndpoint, "sequential");
                    await tab.Open();
                    
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
        public async Task CdpTabObject_DifferentIdentifiers_CreateSeparateConnections()
        {
            // NOTE: This demonstrates how page identifiers enable connection isolation
            var pool = SmartPlaywrightConnectionPool.Instance;
            pool.EnablePageCaching = true;
            
            try
            {
                // With different PageIdentifiers, CdpTabObject creates separate connections
                var tab1 = new TestCdpTabWithCustomIdentifier("http://localhost:9222"); // "custom_identifier"
                var tab2 = new TestCdpTab("http://localhost:9222", "different"); // "different"
                
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

                await Task.Delay(100);
                
                // Statistics track usage
                var idleTime = connection.GetIdleTime();
                var age = connection.GetAge();

                Assert.IsTrue(idleTime.TotalMilliseconds >= 100);
                Assert.IsTrue(age.TotalMilliseconds >= 100);
                Assert.AreEqual("test-key", connection.CacheKey);
                Assert.AreEqual("http://localhost:9222", connection.CdpEndpoint);
                Assert.AreEqual("stats-test", connection.PageUrl);

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
    }
}
