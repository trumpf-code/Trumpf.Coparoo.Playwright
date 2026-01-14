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

namespace Trumpf.Coparoo.Playwright;

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Pooling;

/// <summary>
/// Specialized TabObject for Chrome DevTools Protocol connections.
/// Enforces connection pooling to prevent memory issues in WPF applications with CefSharp dialogs.
/// </summary>
/// <remarks>
/// This class uses the Template Method pattern to enforce pool-based connection management:
/// <list type="bullet">
/// <item>Users must override <see cref="ChromeDevToolsProtocolEndpoint"/> to specify the Chrome DevTools Protocol endpoint URL</item>
/// <item>Users can optionally override <see cref="PageIdentifier"/> for per-dialog caching</item>
/// <item>Users can optionally override <see cref="ChromeDevToolsProtocolOptions"/> for connection configuration</item>
/// <item>The <see cref="Creator"/> method is sealed and automatically uses <see cref="SmartPlaywrightConnectionPool"/></item>
/// </list>
/// <para>
/// <strong>Why sealed Creator()?</strong> This ensures that all Chrome DevTools Protocol connections go through the pool,
/// preventing memory leaks from creating new Playwright instances for each dialog.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class SettingsDialogTab : ChromeDevToolsProtocolTabObject
/// {
///     protected override string ChromeDevToolsProtocolEndpoint => "http://localhost:12345";
///     protected override string PageIdentifier => "settings_dialog";
///     protected override string Url => "https://myapp.local/settings";
///     
///     public SettingsDialogTab()
///     {
///         ChildOf&lt;SettingsPage, SettingsDialogTab&gt;();
///     }
/// }
/// </code>
/// </example>
public abstract class ChromeDevToolsProtocolTabObject : TabObject
{
    /// <summary>
    /// Gets the Chrome DevTools Protocol endpoint URL.
    /// </summary>
    /// <remarks>
    /// This is the WebSocket endpoint exposed by the browser or CEF instance.
    /// For CefSharp applications, this is typically configured via RemoteDebuggingPort.
    /// Example: "http://localhost:12345"
    /// </remarks>
    protected abstract string ChromeDevToolsProtocolEndpoint { get; }

    /// <summary>
    /// Gets the page identifier for connection caching.
    /// When <see cref="SmartPlaywrightConnectionPool.EnablePageCaching"/> is true,
    /// different page identifiers will get separate cached connections.
    /// </summary>
    /// <remarks>
    /// By default, this returns the <see cref="TabObject.Url"/> property.
    /// Override this to provide a custom identifier for your dialog.
    /// Example: "settings_dialog", "preferences_dialog"
    /// </remarks>
    protected virtual string PageIdentifier => Url;

    /// <summary>
    /// Gets a value indicating whether to find an existing page by URL instead of creating a new one.
    /// When true, searches for an existing page with matching URL in the browser's contexts.
    /// When false, creates a new page via an existing browser context (preferred) or as a fallback via Browser.NewPageAsync().
    /// </summary>
    /// <remarks>
    /// Default is true. Set this to true when connecting to applications where pages are already opened
    /// and you want to find them by URL rather than creating new tabs/windows.
    /// Example: Connecting to an existing WPF application with CefSharp where dialogs are already loaded.
    /// </remarks>
    protected virtual bool FindExistingPageByUrl => true;

    /// <summary>
    /// Gets the Chrome DevTools Protocol connection options.
    /// Override this to customize timeout, slow motion, or other connection settings.
    /// </summary>
    /// <remarks>
    /// Default timeout is 30 seconds.
    /// </remarks>
    protected virtual BrowserTypeConnectOverCDPOptions ChromeDevToolsProtocolOptions => new BrowserTypeConnectOverCDPOptions
    {
        Timeout = 30000
    };

    /// <summary>
    /// Creates a new page instance by connecting to the Chrome DevTools Protocol endpoint.
    /// This method is sealed to enforce usage of <see cref="SmartPlaywrightConnectionPool"/>.
    /// </summary>
    /// <returns>A <see cref="Task{IPage}"/> representing the asynchronous operation that returns an <see cref="IPage"/> instance.</returns>
    /// <remarks>
    /// <strong>This method is sealed and cannot be overridden.</strong>
    /// All page creation goes through the connection pool to ensure:
    /// <list type="bullet">
    /// <item>Connections are validated and reused when possible</item>
    /// <item>Stale connections are automatically recreated</item>
    /// <item>Retry logic handles CEF subprocess startup delays</item>
    /// <item>Memory leaks are prevented in dialog-heavy scenarios</item>
    /// </list>
    /// To customize connection behavior, override <see cref="ChromeDevToolsProtocolEndpoint"/>, 
    /// <see cref="PageIdentifier"/>, or <see cref="ChromeDevToolsProtocolOptions"/> instead.
    /// </remarks>
    protected sealed override async Task<IPage> Creator()
    {
        if (string.IsNullOrEmpty(ChromeDevToolsProtocolEndpoint))
        {
            throw new InvalidOperationException(
                $"{GetType().Name}.ChromeDevToolsProtocolEndpoint must not be null or empty. " +
                "Override the ChromeDevToolsProtocolEndpoint property to specify the Chrome DevTools Protocol endpoint URL.");
        }

        var pageIdentifier = !string.IsNullOrWhiteSpace(PageIdentifier)
            ? PageIdentifier
            : (!string.IsNullOrWhiteSpace(Url) ? Url : GetType().Name);

        if (pageIdentifier == GetType().Name)
        {
            System.Diagnostics.Debug.WriteLine($"[ChromeDevToolsProtocolTabObject] WARN: PageIdentifier and Url are empty for {GetType().Name} at connect time. Falling back to class name. Ensure derived tab initializes identifier before connection.");
        }

        System.Diagnostics.Debug.WriteLine($"[ChromeDevToolsProtocolTabObject] {GetType().Name}: Connecting via Chrome DevTools Protocol endpoint='{ChromeDevToolsProtocolEndpoint}', pageIdentifier='{pageIdentifier}', findExistingByUrl={FindExistingPageByUrl}");
        
        return await SmartPlaywrightConnectionPool.Instance
            .GetOrCreatePageAsync(ChromeDevToolsProtocolEndpoint, pageIdentifier, ChromeDevToolsProtocolOptions, FindExistingPageByUrl)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Invalidates the cached connection for this tab, forcing recreation on next access.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Use this method when you know the connection has become invalid and needs to be recreated.
    /// The pool will automatically handle disposal of the old connection.
    /// </remarks>
    protected async Task InvalidateConnectionAsync()
    {
        var pageIdentifier = PageIdentifier ?? Url ?? GetType().Name;
        await SmartPlaywrightConnectionPool.Instance
            .InvalidateConnectionAsync(ChromeDevToolsProtocolEndpoint, pageIdentifier)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Closes the tab without closing the underlying page (which is managed by the pool).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Unlike the base <see cref="TabObject.Close()"/> implementation, this method does NOT call
    /// <c>Page.CloseAsync()</c> because Chrome DevTools Protocol connections are pooled and reused. The page lifecycle
    /// is managed by <see cref="SmartPlaywrightConnectionPool"/>, which will close pages only when
    /// the pool is cleared or when connections are disposed.
    /// <para>
    /// This allows multiple tab instances with the same <see cref="PageIdentifier"/> to share
    /// the same browser page without interfering with each other's lifecycle.
    /// </para>
    /// </remarks>
    public override async Task Close()
    {
        // Do NOT close the page - it's managed by the pool and may be reused
        // Just mark the tab as closed by awaiting a completed task
        await Task.CompletedTask;
    }
}
