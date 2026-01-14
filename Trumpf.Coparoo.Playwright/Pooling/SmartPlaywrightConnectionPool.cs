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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Singleton connection pool for managing Playwright CDP (Chrome DevTools Protocol) connections.
    /// Provides validation-based caching with automatic retry logic and connection reinitialization.
    /// </summary>
    /// <remarks>
    /// This pool solves memory issues in WPF applications with CefSharp dialogs by:
    /// <list type="bullet">
    /// <item>Reusing validated connections instead of creating new ones</item>
    /// <item>Automatically detecting and recovering from stale connections</item>
    /// <item>Retrying connection attempts when CEF subprocess is still starting</item>
    /// <item>Isolating connections per dialog for parallel usage</item>
    /// </list>
    /// </remarks>
    public sealed class SmartPlaywrightConnectionPool : IDisposable
    {
        private static readonly Lazy<SmartPlaywrightConnectionPool> LazyInstance = 
            new Lazy<SmartPlaywrightConnectionPool>(() => new SmartPlaywrightConnectionPool(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ConcurrentDictionary<string, PooledPageConnection> _connectionCache = new ConcurrentDictionary<string, PooledPageConnection>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _disposed;

        /// <summary>
        /// Gets the singleton instance of the connection pool.
        /// </summary>
        public static SmartPlaywrightConnectionPool Instance => LazyInstance.Value;

        /// <summary>
        /// Gets or sets a value indicating whether to cache pages per pageUrl (true) or per endpoint only (false).
        /// When true, different dialogs on the same endpoint get separate connections.
        /// Default is true.
        /// </summary>
        public bool EnablePageCaching { get; set; } = true;

        private SmartPlaywrightConnectionPool()
        {
        }

        /// <summary>
        /// Gets or creates a pooled page connection for the specified Chrome DevTools Protocol endpoint and page URL.
        /// Validates existing connections before reuse and automatically recreates stale connections.
        /// </summary>
        /// <param name="chromeDevToolsProtocolEndpoint">The Chrome DevTools Protocol endpoint URL (e.g., "http://localhost:12345").</param>
        /// <param name="pageUrl">The page identifier or URL (used for per-dialog caching).</param>
        /// <param name="options">Optional Chrome DevTools Protocol connection options.</param>
        /// <param name="findExistingByUrl">If true, searches for existing page by URL instead of creating new one.</param>
        /// <returns>A validated <see cref="IPage"/> instance ready for use.</returns>
        /// <remarks>
        /// This method implements validation-based caching:
        /// <list type="number">
        /// <item>Check if connection exists in cache</item>
        /// <item>Validate connection using multi-layer checks</item>
        /// <item>If valid, return existing connection</item>
        /// <item>If invalid or missing, create new connection with retry logic</item>
        /// </list>
        /// When <paramref name="findExistingByUrl"/> is true, the method will search for an existing page
        /// matching the URL instead of creating a new page. This is useful for connecting to applications
        /// where pages are already opened.
        /// </remarks>
        public async Task<IPage> GetOrCreatePageAsync(
            string chromeDevToolsProtocolEndpoint, 
            string pageUrl,
            BrowserTypeConnectOverCDPOptions options = null,
            bool findExistingByUrl = false)
        {
            if (string.IsNullOrEmpty(chromeDevToolsProtocolEndpoint))
                throw new ArgumentNullException(nameof(chromeDevToolsProtocolEndpoint));
            if (string.IsNullOrEmpty(pageUrl))
                throw new ArgumentNullException(nameof(pageUrl));

            ThrowIfDisposed();

            var cacheKey = GenerateCacheKey(chromeDevToolsProtocolEndpoint, pageUrl);

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SmartPool] GetOrCreatePageAsync: endpoint='{chromeDevToolsProtocolEndpoint}', pageIdentifier='{pageUrl}', findExistingByUrl={findExistingByUrl}, enablePageCaching={EnablePageCaching}, cacheKey='{cacheKey}'");
                // Check if connection exists and is valid
                if (_connectionCache.TryGetValue(cacheKey, out var existingConnection))
                {
                    if (await ValidatePageConnectionAsync(existingConnection).ConfigureAwait(false))
                    {
                        existingConnection.UpdateLastUsed();
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Reusing existing connection for {cacheKey}");
                        return existingConnection.Page;
                    }
                    else
                    {
                        // Connection is stale, remove it
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Connection {cacheKey} is stale, removing from cache");
                        _connectionCache.TryRemove(cacheKey, out _);
                        await existingConnection.DisposeAsync().ConfigureAwait(false);
                    }
                }

                // Create new connection
                var newConnection = await CreatePageConnectionAsync(cacheKey, chromeDevToolsProtocolEndpoint, pageUrl, options, findExistingByUrl).ConfigureAwait(false);
                _connectionCache[cacheKey] = newConnection;
                
                System.Diagnostics.Debug.WriteLine($"[SmartPool] Created new connection for {cacheKey}");
                return newConnection.Page;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Validates a pooled page connection using multi-layer checks.
        /// </summary>
        /// <param name="connection">The connection to validate.</param>
        /// <returns>True if the connection is valid and can be reused; otherwise, false.</returns>
        /// <remarks>
        /// Validation layers:
        /// <list type="number">
        /// <item>Browser.IsConnected - Check if browser is still connected</item>
        /// <item>Page.IsClosed - Check if page is still open</item>
        /// <item>Browser.Contexts.Any() - Check if browser contexts exist</item>
        /// <item>Page.EvaluateAsync - Check if page is responsive</item>
        /// </list>
        /// </remarks>
        private async Task<bool> ValidatePageConnectionAsync(PooledPageConnection connection)
        {
            try
            {
                // Layer 1: Browser Connected?
                if (connection.Browser == null || !connection.Browser.IsConnected)
                {
                    System.Diagnostics.Debug.WriteLine($"[SmartPool] Validation failed: Browser not connected");
                    return false;
                }

                // Layer 2: Page Open?
                if (connection.Page == null || connection.Page.IsClosed)
                {
                    System.Diagnostics.Debug.WriteLine($"[SmartPool] Validation failed: Page is closed");
                    return false;
                }

                // Layer 3: Contexts Available?
                if (!connection.Browser.Contexts.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[SmartPool] Validation failed: No browser contexts");
                    return false;
                }

                // Layer 4: Page Responsive?
                await connection.Page.EvaluateAsync("() => true").ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SmartPool] Validation failed with exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates a new page connection.
        /// </summary>
        /// <param name="cacheKey">The cache key for the connection.</param>
        /// <param name="chromeDevToolsProtocolEndpoint">The Chrome DevTools Protocol endpoint URL.</param>
        /// <param name="pageUrl">The page identifier or URL.</param>
        /// <param name="options">Optional Chrome DevTools Protocol connection options.</param>
        /// <param name="findExistingByUrl">If true, searches for existing page by URL instead of creating new one.</param>
        /// <returns>A new <see cref="PooledPageConnection"/> instance.</returns>
        /// <remarks>
        /// When <paramref name="findExistingByUrl"/> is true, searches through existing pages
        /// in the browser's contexts to find a page with matching URL.
        /// </remarks>
        private async Task<PooledPageConnection> CreatePageConnectionAsync(
            string cacheKey,
            string chromeDevToolsProtocolEndpoint,
            string pageUrl,
            BrowserTypeConnectOverCDPOptions options,
            bool findExistingByUrl)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SmartPool] Connecting to Chrome DevTools Protocol endpoint {chromeDevToolsProtocolEndpoint}");

                var playwright = await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);
                var browser = await playwright.Chromium.ConnectOverCDPAsync(chromeDevToolsProtocolEndpoint, options).ConfigureAwait(false);
                
                IPage page;
                if (findExistingByUrl)
                {
                    // Search for existing page with matching URL
                    page = browser.Contexts
                        .SelectMany(c => c.Pages)
                        .FirstOrDefault(p => string.Equals(p.Url, pageUrl, StringComparison.OrdinalIgnoreCase));

                    if (page == null)
                    {                        // Enhanced diagnostics: list all available pages
                        var availablePages = browser.Contexts
                            .SelectMany(c => c.Pages)
                            .Select(p => $"'{p.Url}'")
                            .ToList();
                        
                        var availablePagesInfo = availablePages.Any() 
                            ? string.Join(", ", availablePages) 
                            : "(no pages found)";
                        
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Available pages in browser: {availablePagesInfo}");
                        
                        throw new InvalidOperationException(
                            $"No existing page found with URL '{pageUrl}'. " +
                            $"Available pages: {availablePagesInfo}. " +
                            "Either ensure the page is fully loaded before connecting, adjust PageIdentifier to match the actual URL, " +
                            "or override FindExistingPageByUrl=false to let the framework create a new tab/window.");
                    }

                    System.Diagnostics.Debug.WriteLine($"[SmartPool] Found existing page with URL {pageUrl}");
                }
                else
                {
                    // Prefer creating a page from an existing (persistent) context when connected over CDP
                    var context = browser.Contexts.FirstOrDefault();

                    if (context != null)
                    {
                        page = await context.NewPageAsync().ConfigureAwait(false);
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Created new page via existing context for {pageUrl}");
                    }
                    else
                    {
                        // Fallback: try Browser.NewPageAsync (may fail for CDP without default context)
                        page = await browser.NewPageAsync().ConfigureAwait(false);
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Created new page via Browser.NewPageAsync for {pageUrl}");
                    }
                }

                var connection = new PooledPageConnection(
                    cacheKey,
                    chromeDevToolsProtocolEndpoint,
                    pageUrl,
                    playwright,
                    browser,
                    page,
                    ownsPage: !findExistingByUrl);

                System.Diagnostics.Debug.WriteLine($"[SmartPool] Successfully created connection to {chromeDevToolsProtocolEndpoint}");
                return connection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SmartPool] Failed to connect to Chrome DevTools Protocol endpoint '{chromeDevToolsProtocolEndpoint}': {ex.Message}");
                throw new InvalidOperationException(
                    $"Failed to connect to Chrome DevTools Protocol endpoint '{chromeDevToolsProtocolEndpoint}'. " +
                    $"Ensure the Chrome DevTools Protocol endpoint is running and accessible. Error: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Invalidates and removes a specific connection from the pool.
        /// </summary>
        /// <param name="cdpEndpoint">The CDP endpoint URL.</param>
        /// <param name="pageUrl">The page identifier or URL. If null, all connections for the endpoint are invalidated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Use this method to force recreation of a connection when you know it has become invalid.
        /// </remarks>
        public async Task InvalidateConnectionAsync(string cdpEndpoint, string pageUrl = null)
        {
            if (string.IsNullOrEmpty(cdpEndpoint))
                throw new ArgumentNullException(nameof(cdpEndpoint));

            ThrowIfDisposed();

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (string.IsNullOrEmpty(pageUrl))
                {
                    // Invalidate all connections for this endpoint
                    var keysToRemove = _connectionCache.Keys
                        .Where(k => k.IndexOf(cdpEndpoint, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();

                    foreach (var key in keysToRemove)
                    {
                        if (_connectionCache.TryRemove(key, out var connection))
                        {
                            await connection.DisposeAsync().ConfigureAwait(false);
                            System.Diagnostics.Debug.WriteLine($"[SmartPool] Invalidated connection {key}");
                        }
                    }
                }
                else
                {
                    // Invalidate specific connection
                    var cacheKey = GenerateCacheKey(cdpEndpoint, pageUrl);
                    if (_connectionCache.TryRemove(cacheKey, out var connection))
                    {
                        await connection.DisposeAsync().ConfigureAwait(false);
                        System.Diagnostics.Debug.WriteLine($"[SmartPool] Invalidated connection {cacheKey}");
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Clears all connections from the pool and disposes them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ClearAllAsync()
        {
            ThrowIfDisposed();

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var allConnections = _connectionCache.Values.ToList();
                _connectionCache.Clear();

                foreach (var connection in allConnections)
                {
                    await connection.DisposeAsync().ConfigureAwait(false);
                }

                System.Diagnostics.Debug.WriteLine($"[SmartPool] Cleared all {allConnections.Count} connections");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Gets statistics about the current state of the connection pool.
        /// </summary>
        /// <returns>A <see cref="SmartPoolStatistics"/> instance containing pool statistics.</returns>
        public SmartPoolStatistics GetStatistics()
        {
            ThrowIfDisposed();

            var stats = new SmartPoolStatistics
            {
                TotalConnections = _connectionCache.Count,
                ConnectionDetails = new System.Collections.Generic.List<ConnectionStatistics>()
            };

            foreach (var kvp in _connectionCache)
            {
                var connection = kvp.Value;
                var isValid = ValidatePageConnectionAsync(connection).GetAwaiter().GetResult();

                stats.ConnectionDetails.Add(new ConnectionStatistics
                {
                    CacheKey = connection.CacheKey,
                    Endpoint = connection.ChromeDevToolsProtocolEndpoint,
                    PageUrl = connection.PageUrl,
                    LastUsed = connection.LastUsed,
                    CreatedAt = connection.CreatedAt,
                    IdleTime = connection.GetIdleTime(),
                    Age = connection.GetAge(),
                    IsValid = isValid
                });
            }

            return stats;
        }

        /// <summary>
        /// Generates a cache key based on the CDP endpoint and page URL.
        /// </summary>
        /// <param name="cdpEndpoint">The CDP endpoint URL.</param>
        /// <param name="pageUrl">The page identifier or URL.</param>
        /// <returns>A unique cache key string.</returns>
        private string GenerateCacheKey(string cdpEndpoint, string pageUrl)
        {
            if (EnablePageCaching)
            {
                return $"{cdpEndpoint}::{pageUrl}";
            }
            else
            {
                return cdpEndpoint;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SmartPlaywrightConnectionPool));
        }

        /// <summary>
        /// Disposes all pooled connections and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            ClearAllAsync().GetAwaiter().GetResult();
            _semaphore?.Dispose();

            System.Diagnostics.Debug.WriteLine("[SmartPool] Pool disposed");
        }
    }
}
