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

namespace Trumpf.Coparoo.Playwright.Pooling
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides statistics about the connection pool state.
    /// </summary>
    public sealed class SmartPoolStatistics
    {
        /// <summary>
        /// Gets the total number of active connections in the pool.
        /// </summary>
        public int TotalConnections { get; internal set; }

        /// <summary>
        /// Gets the list of connection details for all pooled connections.
        /// </summary>
        public List<ConnectionStatistics> ConnectionDetails { get; internal set; } = new List<ConnectionStatistics>();
    }

    /// <summary>
    /// Provides detailed statistics for a single pooled connection.
    /// </summary>
    public sealed class ConnectionStatistics
    {
        /// <summary>
        /// Gets the unique cache key for this connection.
        /// </summary>
        public string CacheKey { get; internal set; }

        /// <summary>
        /// Gets the CDP endpoint URL.
        /// </summary>
        public string Endpoint { get; internal set; }

        /// <summary>
        /// Gets the page identifier or URL.
        /// </summary>
        public string PageUrl { get; internal set; }

        /// <summary>
        /// Gets the timestamp when the connection was last used.
        /// </summary>
        public DateTime LastUsed { get; internal set; }

        /// <summary>
        /// Gets the timestamp when the connection was created.
        /// </summary>
        public DateTime CreatedAt { get; internal set; }

        /// <summary>
        /// Gets the time elapsed since the connection was last used.
        /// </summary>
        public TimeSpan IdleTime { get; internal set; }

        /// <summary>
        /// Gets the total age of the connection since creation.
        /// </summary>
        public TimeSpan Age { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the connection is currently valid.
        /// </summary>
        public bool IsValid { get; internal set; }
    }
}
