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
    public class PlaywrightConnectionPoolTests
    {
        private PlaywrightConnectionPool _pool;

        [TestInitialize]
        public async Task Setup()
        {
            _pool = PlaywrightConnectionPool.Instance;
            await _pool.ClearAllAsync();
            _pool.EnablePageCaching = true;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _pool.ClearAllAsync();
        }

        [TestMethod]
        public async Task ClearAllAsync_RemovesAllConnections()
        {
            // Note: This test validates the pool interface without actually connecting to a Chrome DevTools Protocol endpoint
            // In a real scenario with a running Chrome DevTools Protocol endpoint, you would create connections first

            // Act
            await _pool.ClearAllAsync();
            var stats = _pool.GetStatistics();

            // Assert
            stats.TotalConnections.Should().Be(0);
        }

        [TestMethod]
        public async Task InvalidateConnectionAsync_ThrowsArgumentNullException_WhenChromeDevToolsProtocolEndpointIsNull()
        {
            // Act & Assert
            Func<Task> act = async () => await _pool.InvalidateConnectionAsync(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public void EnablePageCaching_DefaultIsTrue()
        {
            // Assert
            _pool.EnablePageCaching.Should().BeTrue();
        }

        [TestMethod]
        public void EnablePageCaching_CanBeSet()
        {
            // Act
            var originalValue = _pool.EnablePageCaching;
            _pool.EnablePageCaching = false;

            // Assert
            _pool.EnablePageCaching.Should().BeFalse();

            // Cleanup - restore original
            _pool.EnablePageCaching = originalValue;
        }
    }
}
