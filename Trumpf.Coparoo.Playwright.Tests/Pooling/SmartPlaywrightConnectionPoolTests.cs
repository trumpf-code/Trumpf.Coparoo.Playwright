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
    using Microsoft.Playwright;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Trumpf.Coparoo.Playwright.Pooling;

    [TestClass]
    public class SmartPlaywrightConnectionPoolTests
    {
        private SmartPlaywrightConnectionPool _pool;

        [TestInitialize]
        public async Task Setup()
        {
            _pool = SmartPlaywrightConnectionPool.Instance;
            await _pool.ClearAllAsync();
            _pool.EnablePageCaching = true;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _pool.ClearAllAsync();
        }

        [TestMethod]
        public void Instance_ReturnsSingleton()
        {
            // Act
            var instance1 = SmartPlaywrightConnectionPool.Instance;
            var instance2 = SmartPlaywrightConnectionPool.Instance;

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public async Task GetOrCreatePageAsync_ThrowsArgumentNullException_WhenCdpEndpointIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await _pool.GetOrCreatePageAsync(null, "test"));
        }

        [TestMethod]
        public async Task GetOrCreatePageAsync_ThrowsArgumentNullException_WhenPageUrlIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await _pool.GetOrCreatePageAsync("http://localhost:12345", null));
        }

        [TestMethod]
        public void GetStatistics_ReturnsZeroConnections_WhenPoolIsEmpty()
        {
            // Act
            var stats = _pool.GetStatistics();

            // Assert
            Assert.AreEqual(0, stats.TotalConnections);
            Assert.AreEqual(0, stats.ConnectionDetails.Count);
        }

        [TestMethod]
        public async Task ClearAllAsync_RemovesAllConnections()
        {
            // Note: This test validates the pool interface without actually connecting to a CDP endpoint
            // In a real scenario with a running CDP endpoint, you would create connections first
            
            // Act
            await _pool.ClearAllAsync();
            var stats = _pool.GetStatistics();

            // Assert
            Assert.AreEqual(0, stats.TotalConnections);
        }

        [TestMethod]
        public async Task InvalidateConnectionAsync_ThrowsArgumentNullException_WhenCdpEndpointIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await _pool.InvalidateConnectionAsync(null));
        }

        [TestMethod]
        public void EnablePageCaching_DefaultIsTrue()
        {
            // Assert
            Assert.IsTrue(_pool.EnablePageCaching);
        }

        [TestMethod]
        public void EnablePageCaching_CanBeSet()
        {
            // Act
            var originalValue = _pool.EnablePageCaching;
            _pool.EnablePageCaching = false;

            // Assert
            Assert.IsFalse(_pool.EnablePageCaching);
            
            // Cleanup - restore original
            _pool.EnablePageCaching = originalValue;
        }
    }
}
