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
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Playwright.Pooling;

    [TestClass]
    public class PooledPageConnectionTests
    {
        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenCacheKeyIsNull()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new PooledPageConnection(null, "endpoint", "page", null, null, null));
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenPlaywrightIsNull()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new PooledPageConnection("key", "endpoint", "page", null, null, null));
        }

        [TestMethod]
        public async Task UpdateLastUsed_UpdatesLastUsedTimestamp()
        {
            // Arrange - Create real Playwright instances
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            
            var connection = new PooledPageConnection(
                "test-key",
                "http://localhost:12345",
                "test-page",
                playwright,
                browser,
                page);

            var initialLastUsed = connection.LastUsed;
            
            // Act - Wait and then update
            await Task.Delay(100);
            connection.UpdateLastUsed();

            // Assert
            Assert.IsTrue(connection.LastUsed > initialLastUsed,
                "LastUsed timestamp should be updated to a later time");
            
            // Cleanup
            await connection.DisposeAsync();
        }

        [TestMethod]
        public async Task DisposeAsync_ClosesAllResources()
        {
            // Arrange - Create real Playwright instances
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            
            var connection = new PooledPageConnection(
                "test-key",
                "http://localhost:12345",
                "test-page",
                playwright,
                browser,
                page);

            // Act
            await connection.DisposeAsync();

            // Assert - Page should be closed
            Assert.IsTrue(page.IsClosed, "Page should be closed after dispose");
        }

        [TestMethod]
        public async Task Properties_ReturnCorrectValues()
        {
            // Arrange - Create real Playwright instances
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            
            var cacheKey = "test-cache-key";
            var endpoint = "http://localhost:12345";
            var pageUrl = "test-page-url";
            
            var connection = new PooledPageConnection(
                cacheKey,
                endpoint,
                pageUrl,
                playwright,
                browser,
                page);

            // Assert
            Assert.AreEqual(cacheKey, connection.CacheKey);
            Assert.AreEqual(endpoint, connection.ChromeDevToolsProtocolEndpoint);
            Assert.AreEqual(pageUrl, connection.PageUrl);
            Assert.AreSame(playwright, connection.Playwright);
            Assert.AreSame(browser, connection.Browser);
            Assert.AreSame(page, connection.Page);
            Assert.IsTrue(connection.CreatedAt <= DateTime.UtcNow);
            Assert.IsTrue(connection.LastUsed <= DateTime.UtcNow);
            
            // Cleanup
            await connection.DisposeAsync();
        }
    }
}
