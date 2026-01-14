# CDP Connection Pooling - Usage Example

This example demonstrates how to use the `CdpTabObject` for WPF applications with CefSharp dialogs.

## Basic Usage

```csharp
using Trumpf.Coparoo.Playwright;

// Define your dialog tab using CdpTabObject
public class SettingsDialogTab : CdpTabObject
{
    // REQUIRED: Specify the CDP endpoint (CefSharp remote debugging port)
    protected override string CdpEndpoint => "http://localhost:12345";
    
    // OPTIONAL: Specify a unique identifier for this dialog (for per-dialog caching)
    protected override string PageIdentifier => "settings_dialog";
    
    // REQUIRED: Specify the page URL
    protected override string Url => "https://myapp.local/settings";
    
    public SettingsDialogTab()
    {
        // Register page relationships
        ChildOf<SettingsPage, SettingsDialogTab>();
    }
}

// Define your page objects as usual
public class SettingsPage : PageObject, ISettingsPage
{
    protected override By SearchPattern => By.TestId("settings-page");
    
    public Button SaveButton => Find<Button>(By.TestId("save-button"));
    public CheckBox EnableNotifications => Find<CheckBox>(By.TestId("enable-notifications"));
}
```

## Using the Dialog in WPF

```csharp
public async Task OpenSettingsDialog()
{
    var dialog = new SettingsDialogTab();
    
    try
    {
        // First time: Creates new CDP connection (with retry logic)
        await dialog.Open();
        
        // Navigate to settings page
        var settingsPage = dialog.Goto<ISettingsPage>();
        
        // Interact with the page
        await settingsPage.EnableNotifications.Check();
        await settingsPage.SaveButton.Click();
        
        // Close the dialog (connection stays in pool!)
        await dialog.Close();
    }
    catch (Exception ex)
    {
        // Handle errors
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// Second time opening the dialog
public async Task OpenSettingsDialogAgain()
{
    var dialog = new SettingsDialogTab();
    
    try
    {
        // This time: Reuses existing connection from pool! 🎉
        await dialog.Open();
        
        // ... interact with the page ...
        
        await dialog.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

## Configuring the Pool in App.xaml.cs

```csharp
using Trumpf.Coparoo.Playwright.Pooling;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Optional: Configure the connection pool
        var pool = PlaywrightConnectionPool.Instance;
        pool.MaxRetryAttempts = 5;  // Increase retries if CEF starts slowly
        pool.RetryDelay = TimeSpan.FromSeconds(1);  // Increase delay between retries
        pool.EnablePageCaching = true;  // Enable per-dialog caching (default)
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Cleanup: Dispose all connections
        PlaywrightConnectionPool.Instance.Dispose();
        base.OnExit(e);
    }
}
```

## Monitoring Pool Statistics

```csharp
public void DisplayPoolStatistics()
{
    var stats = PlaywrightConnectionPool.Instance.GetStatistics();
    
    Console.WriteLine($"Total Connections: {stats.TotalConnections}");
    
    foreach (var conn in stats.ConnectionDetails)
    {
        Console.WriteLine($"Dialog: {conn.PageUrl}");
        Console.WriteLine($"  Endpoint: {conn.Endpoint}");
        Console.WriteLine($"  Last Used: {conn.LastUsed}");
        Console.WriteLine($"  Idle Time: {conn.IdleTime.TotalSeconds:F1}s");
        Console.WriteLine($"  Valid: {conn.IsValid}");
        Console.WriteLine($"  Age: {conn.Age.TotalSeconds:F1}s");
    }
}
```

## Advanced: Custom CDP Options

```csharp
public class AdvancedDialogTab : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    protected override string PageIdentifier => "advanced_dialog";
    protected override string Url => "https://myapp.local/advanced";
    
    // Customize CDP connection options
    protected override BrowserTypeConnectOverCDPOptions CdpOptions => new()
    {
        Timeout = 60000,  // 60 seconds timeout
        SlowMo = 100      // Slow down operations by 100ms (for debugging)
    };
}
```

## Handling Connection Errors

```csharp
public async Task OpenDialogWithErrorHandling()
{
    var dialog = new SettingsDialogTab();
    
    try
    {
        await dialog.Open();
        
        // If you know the connection might be stale, invalidate it manually
        // await dialog.InvalidateConnectionAsync();
        
        var page = dialog.Goto<ISettingsPage>();
        // ... work with page ...
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("CDP endpoint"))
    {
        // Handle CDP connection failure
        Console.WriteLine($"Failed to connect to CDP endpoint: {ex.Message}");
        // Maybe the CefSharp process isn't running?
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
    }
    finally
    {
        await dialog.Close();
    }
}
```

## Benefits

✅ **No Memory Leaks**: Playwright instances are reused, not recreated  
✅ **Automatic Recovery**: Stale connections are detected and recreated  
✅ **CEF Startup Handling**: Retry logic handles delayed subprocess starts  
✅ **Per-Dialog Isolation**: Different dialogs can run in parallel  
✅ **Zero Configuration**: Works out-of-the-box with sensible defaults  
✅ **Thread Safe**: Singleton pool with concurrent access support  

## Comparison: Before vs After

### Before (Memory Leak)
```csharp
protected override async Task<IPage> Creator()
{
    var playwright = await Playwright.CreateAsync();  // NEW INSTANCE EVERY TIME! ❌
    var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:12345");
    return await browser.NewPageAsync();
    // Playwright instance never disposed → OutOfMemoryException
}
```

### After (Pooled)
```csharp
public class MyDialog : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    // Creator() is sealed and uses PlaywrightConnectionPool ✅
}
```

## Troubleshooting

### "Failed to connect to CDP endpoint"
- Ensure your CefSharp application has remote debugging enabled
- Check that the port matches (e.g., `--remote-debugging-port=12345`)
- Verify the CEF subprocess is running

### Connections not being reused
- Check `EnablePageCaching` setting
- Ensure `PageIdentifier` is consistent across dialog instances
- Review pool statistics to see if connections are being validated correctly

### Slow initial connection
- Increase `MaxRetryAttempts` if CEF takes longer to start
- Adjust `RetryDelay` to give CEF more time between attempts
