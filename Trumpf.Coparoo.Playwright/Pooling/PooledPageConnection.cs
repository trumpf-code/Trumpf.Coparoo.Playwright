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
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Represents a pooled connection wrapper for Playwright Chrome DevTools Protocol connections.
    /// Encapsulates IPlaywright, IBrowser, and IPage instances with metadata for connection pooling.
    /// </summary>
    public sealed class PooledPageConnection : IAsyncDisposable
    {
        /// <summary>
        /// Gets the unique cache key for this connection (format: "endpoint::pageUrl").
        /// </summary>
        public string CacheKey { get; }

        /// <summary>
        /// Gets the Chrome DevTools Protocol endpoint URL (e.g., "http://localhost:12345").
        /// </summary>
        public string ChromeDevToolsProtocolEndpoint { get; }

        /// <summary>
        /// Gets the page identifier or URL for this connection.
        /// </summary>
        public string PageUrl { get; }

        /// <summary>
        /// Gets the Playwright instance for this connection.
        /// </summary>
        public IPlaywright Playwright { get; }

        /// <summary>
        /// Gets the Chrome DevTools Protocol browser instance for this connection.
        /// </summary>
        public IBrowser Browser { get; }

        /// <summary>
        /// Gets the page instance for this connection.
        /// </summary>
        public IPage Page { get; }

        /// <summary>
        /// Gets a value indicating whether this connection owns the page lifecycle.
        /// When true, the pool created the page and will close it on dispose.
        /// When false, the page was found/attached (e.g., via Chrome DevTools Protocol) and must not be closed by the pool.
        /// </summary>
        public bool OwnsPage { get; }

        /// <summary>
        /// Gets the timestamp when this connection was last used.
        /// </summary>
        public DateTime LastUsed { get; private set; }

        /// <summary>
        /// Gets the timestamp when this connection was created.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledPageConnection"/> class.
        /// </summary>
        /// <param name="cacheKey">The unique cache key for this connection.</param>
        /// <param name="chromeDevToolsProtocolEndpoint">The Chrome DevTools Protocol endpoint URL.</param>
        /// <param name="pageUrl">The page identifier or URL.</param>
        /// <param name="playwright">The Playwright instance.</param>
        /// <param name="browser">The Chrome DevTools Protocol browser instance.</param>
        /// <param name="page">The page instance.</param>
        /// <param name="ownsPage">Optional flag indicating whether the page was created by the pool (defaults to true). When false, the page will not be closed on dispose.</param>
        public PooledPageConnection(
            string cacheKey,
            string chromeDevToolsProtocolEndpoint,
            string pageUrl,
            IPlaywright playwright,
            IBrowser browser,
            IPage page,
            bool ownsPage = true)
        {
            CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
            ChromeDevToolsProtocolEndpoint = chromeDevToolsProtocolEndpoint ?? throw new ArgumentNullException(nameof(chromeDevToolsProtocolEndpoint));
            PageUrl = pageUrl ?? throw new ArgumentNullException(nameof(pageUrl));
            Playwright = playwright ?? throw new ArgumentNullException(nameof(playwright));
            Browser = browser ?? throw new ArgumentNullException(nameof(browser));
            Page = page ?? throw new ArgumentNullException(nameof(page));
            OwnsPage = ownsPage;
            
            CreatedAt = DateTime.UtcNow;
            LastUsed = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the last used timestamp to the current time.
        /// </summary>
        public void UpdateLastUsed()
        {
            LastUsed = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the time elapsed since this connection was last used.
        /// </summary>
        /// <returns>The idle time as a <see cref="TimeSpan"/>.</returns>
        public TimeSpan GetIdleTime()
        {
            return DateTime.UtcNow - LastUsed;
        }

        /// <summary>
        /// Gets the total age of this connection since creation.
        /// </summary>
        /// <returns>The age as a <see cref="TimeSpan"/>.</returns>
        public TimeSpan GetAge()
        {
            return DateTime.UtcNow - CreatedAt;
        }

        /// <summary>
        /// Disposes all resources associated with this connection.
        /// Ensures that Page, Browser, and Playwright are properly cleaned up without throwing exceptions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            // Close Page (only when we own it)
            try
            {
                if (Page != null && !Page.IsClosed && OwnsPage)
                {
                    await Page.CloseAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing page: {ex.Message}");
            }

            // Close Browser
            try
            {
                if (Browser != null && Browser.IsConnected)
                {
                    await Browser.CloseAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing browser: {ex.Message}");
            }

            // Dispose Playwright
            try
            {
                Playwright?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error disposing Playwright: {ex.Message}");
            }
        }
    }
}
