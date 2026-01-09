# Smart Connection Pooling for CDP

This directory contains the implementation of validation-based connection pooling for Playwright CDP (Chrome DevTools Protocol) connections.

## Overview

The Smart Connection Pooling system solves memory issues in WPF applications with CefSharp dialogs by:
- Reusing validated connections instead of creating new ones
- Automatically detecting and recovering from stale connections
- Retrying connection attempts when CEF subprocess is still starting
- Isolating connections per dialog for parallel usage

## Components

### SmartPlaywrightConnectionPool

The singleton connection pool that manages all CDP connections.

**Key Features:**
- Validation-based caching (no timeout-based eviction)
- Automatic retry logic with configurable attempts and delays
- Per-dialog or per-endpoint caching strategies
- Multi-layer connection validation
- Statistics and monitoring support

**Configuration:**
```csharp
var pool = SmartPlaywrightConnectionPool.Instance;
pool.MaxRetryAttempts = 3;           // Default: 3
pool.RetryDelay = TimeSpan.FromMilliseconds(500); // Default: 500ms
pool.EnablePageCaching = true;       // Default: true (per-dialog caching)
```

### PooledPageConnection

Wrapper class for a single CDP connection, encapsulating:
- IPlaywright instance
- IBrowser instance
- IPage instance
- Metadata (creation time, last used time, cache key)

Implements `IAsyncDisposable` for proper resource cleanup.

### CdpTabObject

Specialized `TabObject` that enforces pool usage through a sealed `Creator()` method.

**Basic Usage:**
```csharp
public class SettingsDialogTab : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    protected override string PageIdentifier => "settings_dialog";
    protected override string Url => "https://myapp.local/settings";
    
    public SettingsDialogTab()
    {
        ChildOf<SettingsPage, SettingsDialogTab>();
    }
}
```

**Finding Existing Pages by URL:**

When connecting to applications where pages are already opened (e.g., existing WPF apps with CefSharp), you can search for existing pages by URL instead of creating new ones:

```csharp
public class ExistingDialogTab : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    protected override string Url => "https://myapp.local/existing-dialog";
    protected override bool FindExistingPageByUrl => true; // Search for existing page
    
    public ExistingDialogTab()
    {
        ChildOf<DialogPage, ExistingDialogTab>();
    }
}
```

When `FindExistingPageByUrl` is `true`, the pool will search through existing pages in the browser's contexts to find a page with matching URL:
```csharp
// Internal logic equivalent to:
page = browser.Contexts.First().Pages.First(p => p.Url.Equals(targetUrl));
```

This is useful when:
- Pages are already loaded in the application (not created by test)
- You want to connect to existing dialogs/windows
- The application manages page lifecycle independently

### SmartPoolStatistics

Provides statistics about the pool state for monitoring and debugging:
- Total number of active connections
- Per-connection details (cache key, idle time, validation state)

## Connection Lifecycle

1. **First Access**: `GetOrCreatePageAsync()` creates new connection with retry logic
2. **Caching**: Connection stored with cache key (endpoint + optional page identifier)
3. **Reuse**: Subsequent accesses validate connection before reuse
4. **Validation Failure**: Stale connections are automatically disposed and recreated
5. **Manual Invalidation**: `InvalidateConnectionAsync()` forces recreation

## Validation Layers

Connection validation uses four layers:
1. **Browser.IsConnected** - Check if browser is still connected
2. **Page.IsClosed** - Check if page is still open
3. **Browser.Contexts.Any()** - Check if browser contexts exist
4. **Page.EvaluateAsync** - Check if page is responsive

## Retry Logic

When connecting to CDP endpoint, the pool automatically retries on failure:
- Default: 3 attempts with 500ms delay between attempts
- Handles scenarios where CEF subprocess is still starting
- Throws `InvalidOperationException` after exhausting all attempts

## Monitoring

Get pool statistics for debugging:
```csharp
var stats = SmartPlaywrightConnectionPool.Instance.GetStatistics();

Console.WriteLine($"Total Connections: {stats.TotalConnections}");
foreach (var conn in stats.ConnectionDetails)
{
    Console.WriteLine($"  {conn.PageUrl}: Valid={conn.IsValid}, Idle={conn.IdleTime.TotalSeconds:F1}s");
}
```

## Testing

Unit tests are located in `Trumpf.Coparoo.Playwright.Tests/Pooling/`:
- `SmartPlaywrightConnectionPoolTests.cs` - Pool functionality tests
- `PooledPageConnectionTests.cs` - Connection wrapper tests
- `CdpTabObjectTests.cs` - CdpTabObject behavior tests

## Thread Safety

The pool uses:
- `ConcurrentDictionary` for connection storage
- `SemaphoreSlim` for critical section protection
- Lazy singleton initialization with thread safety

## .NET Standard 2.0 Compatibility

All components target .NET Standard 2.0 for maximum compatibility:
- Uses `IndexOf()` with `StringComparison` instead of `Contains()`
- Compatible with .NET Framework 4.6.1+ and .NET Core 2.0+
