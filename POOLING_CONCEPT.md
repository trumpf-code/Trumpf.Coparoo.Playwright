# Trumpf.Coparoo.Playwright - CDP Connection Pooling Konzept

## ?? �bersicht

Dieses Dokument beschreibt das Konzept f�r **Smart Connection Pooling** zur L�sung von Memory-Problemen in WPF-Anwendungen mit CefSharp-Dialogen.

---

## ?? Problem

### Szenario: WPF-Anwendung mit CefSharp-Dialogen

**Symptome**:
- `OutOfMemoryException` bei h�ufigem Dialog-�ffnen/Schlie�en
- CEF-Subprozesse (`cefSharp.BrowserSubprocess.exe 32bit`) starten dynamisch
- Playwright/Browser-Instanzen werden nicht freigegeben

**Ursache**:
```csharp
// ? Jeder Dialog erstellt neue Instanzen
protected override async Task<IPage> Creator()
{
    var playwright = await Playwright.CreateAsync();  // NEU bei jedem Dialog
    var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:12345");
    return await browser.NewPageAsync();
    // ? Playwright-Instanz bleibt im Speicher
    // ? Browser-Connection bleibt offen
    // ? OutOfMemoryException!
}
```

---

## ?? L�sung: Validation-Based Connection Pooling

### Kern-Prinzipien

1. **Validierung statt Timeout**: Connections werden bei jedem Zugriff validiert
2. **Automatische Reinitialisierung**: Stale Connections werden automatisch neu erstellt
3. **Per-Dialog-Caching**: Jeder Dialog bekommt eigene Connection
4. **Retry-Logik**: Automatische Wiederholung wenn CEF-Subprocess noch startet

---

## ??? Architektur

### Komponenten

```
PlaywrightConnectionPool (Singleton)
??? PooledPageConnection (Connection-Wrapper)
?   ??? IPlaywright playwright
?   ??? IBrowser browser
?   ??? IPage page
?   ??? Metadata (LastUsed, CacheKey, etc.)
?
??? CdpTabObject (Spezialisierter TabObject)
    ??? sealed Creator() ? Erzwingt Pool-Nutzung
```

---

## ?? Ablaufdiagramm

### Connection Lifecycle

```
Dialog �ffnen
    ?
GetOrCreatePageAsync(cdpEndpoint, pageUrl)
    ?
Ist gecached?
    ? JA
ValidatePageConnectionAsync()
    ?? Browser.IsConnected? ?
    ?? Page.IsClosed? ?
    ?? Contexts.Any()? ?
    ?? Page.EvaluateAsync("() => true")? ?
    ? Validation OK?
    ?? JA ? Wiederverwenden ?
    ?? NEIN ? Aus Cache entfernen + Neu erstellen
    ?
Ist gecached?
    ? NEIN
CreatePageWithRetryAsync()
    ?? Versuch 1: ConnectOverCDPAsync()
    ?   ?? Fehler? ? Warte 500ms
    ?? Versuch 2: ConnectOverCDPAsync()
    ?   ?? Fehler? ? Warte 500ms
    ?? Versuch 3: ConnectOverCDPAsync()
        ?? Erfolg ? Im Cache speichern
```

---

## ?? Key Features

### 1. Multi-Layer Validation

```csharp
private async Task<bool> ValidatePageConnectionAsync(connection)
{
    // Layer 1: Browser Connected?
    if (!connection.Browser.IsConnected) return false;
    
    // Layer 2: Page Open?
    if (connection.Page.IsClosed) return false;
    
    // Layer 3: Contexts Available?
    if (!connection.Browser.Contexts.Any()) return false;
    
    // Layer 4: Page Responsive?
    await connection.Page.EvaluateAsync("() => true");
    
    return true;
}
```

**Vorteil**: Erkennt automatisch wenn CEF-Subprocess neu gestartet wurde

---

### 2. Cache-Key-Strategie

```csharp
// Per-Dialog-Caching (EnablePageCaching = true)
"http://localhost:12345::settings_dialog"
"http://localhost:12345::preferences_dialog"

// Pro-Endpoint-Caching (EnablePageCaching = false)
"http://localhost:12345"
```

**Vorteil**: Verschiedene Dialoge interferieren nicht

---

### 3. Retry-Mechanismus

```csharp
for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
{
    try
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.ConnectOverCDPAsync(cdpEndpoint);
        // Success!
        return page;
    }
    catch (Exception) when (attempt < MaxRetryAttempts)
    {
        await Task.Delay(RetryDelay); // Default: 500ms
    }
}
```

**Vorteil**: Wartet auf CEF-Subprocess-Start

---

### 4. Automatic Retry bei CDP-Fehlern

```csharp
public override async Task Goto()
{
    try
    {
        await base.Goto();
    }
    catch (CDP-Connection-Error)
    {
        // Connection failed ? Invalidate cache
        await pool.InvalidateConnectionAsync();
        
        // Retry with fresh connection
        await base.Goto();
    }
}
```

**Vorteil**: Transparent f�r den User

---

## ?? Datei-Struktur

### Zu erstellende Dateien

```
Trumpf.Coparoo.Playwright/
??? Pooling/
?   ??? PlaywrightConnectionPool.cs  (Kern-Pool)
?   ??? PooledPageConnection.cs           (Connection-Wrapper)
?
??? Root/TabObject/
    ??? CdpTabObject.cs                   (Spezialisierter TabObject)
```

---

## ?? Code-Implementierung

### 1. PlaywrightConnectionPool.cs

**Zweck**: Singleton-Pool f�r CDP-Verbindungen

**Kern-Methoden**:
- `GetOrCreatePageAsync()` - Haupt-Entry-Point
- `ValidatePageConnectionAsync()` - Multi-Layer-Validation
- `CreatePageWithRetryAsync()` - Retry-Logik
- `InvalidateConnectionAsync()` - Manuelle Cache-Invalidierung
- `GetStatistics()` - Monitoring

**Konfiguration**:
```csharp
var pool = PlaywrightConnectionPool.Instance;
pool.MaxRetryAttempts = 3;           // Standard: 3
pool.RetryDelay = TimeSpan.FromMilliseconds(500); // Standard: 500ms
pool.EnablePageCaching = true;       // Standard: true
```

---

### 2. PooledPageConnection.cs

**Zweck**: Wrapper f�r eine CDP-Verbindung

**Eigenschaften**:
```csharp
public sealed class PooledPageConnection
{
    public string CacheKey { get; }        // "endpoint::pageUrl"
    public string CdpEndpoint { get; }     // "http://localhost:12345"
    public string PageUrl { get; }         // "settings_dialog"
    public IBrowser Browser { get; }       // CDP-Browser
    public IPage Page { get; }             // Playwright-Page
    public DateTime LastUsed { get; }      // Monitoring
    public DateTime CreatedAt { get; }     // Statistiken
}
```

**Lifecycle**:
```csharp
await DisposeAsync()
    ? Page.CloseAsync()
    ? Browser.CloseAsync()
    ? Playwright.Dispose()
```

**Wichtig**: Alle Schritte mit `try/catch` - kein Fehler stoppt Cleanup

---

### 3. CdpTabObject.cs

**Zweck**: Erzwingt Pool-Nutzung f�r WPF/CefSharp-Szenarien

**Template Method Pattern**:
```csharp
public abstract class CdpTabObject : TabObject
{
    // User MUSS �berschreiben
    protected abstract string CdpEndpoint { get; }
    
    // User KANN �berschreiben
    protected virtual string PageIdentifier => null;
    protected virtual BrowserTypeConnectOverCDPOptions CdpOptions => new() 
    { 
        Timeout = 30000 
    };
    
    // ?? SEALED - User KANN NICHT �berschreiben!
    protected sealed override async Task<IPage> Creator()
    {
        return await PlaywrightConnectionPool.Instance
            .GetOrCreatePageAsync(CdpEndpoint, PageIdentifier ?? Url, CdpOptions);
    }
}
```

**Vorteil**: Pool-Umgehung ist unm�glich

---

## ?? Verwendungs-Beispiele

### WPF-Dialog mit CdpTabObject

```csharp
// User-Code (einfach!)
public class SettingsDialogTab : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    protected override string PageIdentifier => "settings_dialog";
    protected override string Url => "https://myapp.local/settings";
    
    public SettingsDialogTab()
    {
        ChildOf<SettingsPage, SettingsDialogTab>();
    }
    
    // Creator() ist sealed ? automatisch gecacht!
}

// Verwendung in WPF
public async Task OpenSettingsDialog()
{
    var dialog = new SettingsDialogTab();
    
    try
    {
        await dialog.Open();  // 1. Mal: Connection erstellen
        
        var page = dialog.Goto<ISettingsPage>();
        await page.SaveButton.ClickAsync();
        
        await dialog.Close(); // Connection bleibt im Pool!
    }
    catch (Exception ex)
    {
        // Error handling
    }
}

// 2. Mal �ffnen
var dialog2 = new SettingsDialogTab();
await dialog2.Open(); // Connection aus Cache! ?
```

---
### WPF-Dialog mit existierender Page suchen

```csharp
// Für Apps, wo die Page bereits geladen ist und nicht neu erstellt werden soll
public class ExistingDialogTab : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
    protected override string Url => "https://myapp.local/existing-dialog";
    protected override bool FindExistingPageByUrl => true; // Page suchen statt erstellen!
    
    public ExistingDialogTab()
    {
        ChildOf<DialogPage, ExistingDialogTab>();
    }
}

// Verwendung
public async Task ConnectToExistingDialog()
{
    // Dialog ist bereits in der App geöffnet mit URL "https://myapp.local/existing-dialog"
    var dialog = new ExistingDialogTab();
    
    try
    {
        await dialog.Open(); // Sucht existierende Page mit passender URL
        
        // Entspricht intern:
        // page = browser.Contexts.First().Pages.First(p => p.Url.Equals(targetUrl));
        
        var page = dialog.Goto<IDialogPage>();
        await page.DoSomething();
        
        await dialog.Close(); // Page wird NICHT geschlossen, nur Connection freigegeben
    }
    catch (InvalidOperationException ex)
    {
        // Fehler, wenn keine Page mit dieser URL gefunden wurde
        Console.WriteLine($"Page nicht gefunden: {ex.Message}");
    }
}
```

---
### WPF App.xaml.cs - Pool-Konfiguration

```csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Pool-Konfiguration
        var pool = PlaywrightConnectionPool.Instance;
        pool.MaxConnections = 5;
        pool.IdleTimeout = TimeSpan.FromMinutes(3);
        pool.CleanupInterval = TimeSpan.FromSeconds(30);

        // Optional: Monitoring
        StartPoolMonitoring();
    }

    private void StartPoolMonitoring()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };

        timer.Tick += (s, args) =>
        {
            var stats = PlaywrightConnectionPool.Instance.GetStatistics();
            Debug.WriteLine($"Pool: {stats.TotalConnections} connections");
            
            foreach (var conn in stats.ConnectionDetails)
            {
                Debug.WriteLine(
                    $"  {conn.PageUrl}: Valid={conn.IsValid}, " +
                    $"Idle={conn.IdleTime.TotalSeconds:F1}s");
            }
        };

        timer.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Cleanup
        PlaywrightConnectionPool.Instance.Dispose();
        base.OnExit(e);
    }
}
```

---

## ?? Szenarien-Abdeckung

### ? Szenario 1: Normaler Dialog-Fluss

```
Dialog A �ffnen
    ? Connection erstellt & gecached
Dialog A schlie�en
    ? Connection bleibt im Cache
Dialog A wieder �ffnen
    ? Connection validiert & wiederverwendet ?
```

---

### ? Szenario 2: CEF-Subprocess-Restart

```
Dialog A �ffnen
    ? Connection gecached
CEF crashed/restart
Dialog A wieder �ffnen
    ? Validation FAILS
    ? Auto-Dispose + Reinitialisierung ?
```

---

### ? Szenario 3: Mehrere parallele Dialoge

```
Dialog A (Settings) �ffnen
    ? Connection A gecached
Dialog B (Preferences) �ffnen
    ? Connection B gecached
Beide gleichzeitig aktiv
    ? Separate Connections ?
```

---

### ? Szenario 4: CEF-Subprocess startet verz�gert

```
Dialog �ffnen
    ? CDP-Connect FEHLER
Retry nach 500ms
    ? Subprocess noch nicht da
Retry nach 500ms
    ? Subprocess noch nicht da
Retry nach 500ms
    ? Subprocess bereit ? ERFOLG ?
```

---

### ? Szenario 5: Stale Connection Detection

```
Dialog A lange nicht verwendet
CEF-Subprocess beendet sich (Idle)
Dialog A wieder �ffnen
    ? Validation: Page.IsClosed
    ? Auto-Dispose + Reinitialisierung ?
```

---

## ?? Konfigurationsoptionen

### PlaywrightConnectionPool

```csharp
public sealed class PlaywrightConnectionPool
{
    // Retry-Konfiguration
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);
    
    // Caching-Strategie
    public bool EnablePageCaching { get; set; } = true;
    
    // Methoden
    public async Task<IPage> GetOrCreatePageAsync(string cdpEndpoint, string pageUrl, options);
    public async Task InvalidateConnectionAsync(string cdpEndpoint, string pageUrl = null);
    public async Task ClearAllAsync();
    public SmartPoolStatistics GetStatistics();
    public void Dispose();
}
```

---

### CdpTabObject

```csharp
public abstract class CdpTabObject : TabObject
{
    // MUSS �berschrieben werden
    protected abstract string CdpEndpoint { get; }
    
    // KANN �berschrieben werden
    protected virtual string PageIdentifier => null;
    protected virtual BrowserTypeConnectOverCDPOptions CdpOptions => new() { Timeout = 30000 };
    
    // Utility
    protected async Task InvalidateConnectionAsync();
}
```

---

## ?? Monitoring & Debugging

### Pool-Statistiken abrufen

```csharp
var stats = PlaywrightConnectionPool.Instance.GetStatistics();

Console.WriteLine($"Total Connections: {stats.TotalConnections}");

foreach (var conn in stats.ConnectionDetails)
{
    Console.WriteLine($"Cache Key: {conn.CacheKey}");
    Console.WriteLine($"  Endpoint: {conn.Endpoint}");
    Console.WriteLine($"  Page URL: {conn.PageUrl}");
    Console.WriteLine($"  Last Used: {conn.LastUsed}");
    Console.WriteLine($"  Idle Time: {conn.IdleTime.TotalSeconds:F1}s");
    Console.WriteLine($"  Valid: {conn.IsValid}");
}
```

---

### Debug-Ausgabe

```csharp
// In PlaywrightConnectionPool.cs:
System.Diagnostics.Debug.WriteLine(
    $"CDP connection attempt {attempt}/{MaxRetryAttempts} failed: {ex.Message}");
```

---

## ?? Nicht ben�tigt: LaunchedBrowserPool

### Begr�ndung

**CDP-Szenario (WPF/CefSharp)**: Pooling ist **essentiell** ?
- Externer Browser-Prozess l�uft unabh�ngig
- Viele Dialog-Instanzen zur Laufzeit
- Kein zentrales Lifecycle-Management

**Playwright-Launch-Szenario (Tests)**: Pooling ist **optional** ??
- Developer kontrolliert Lifecycle explizit
- `ClassInitialize`/`OneTimeSetUp` Pattern
- Ein Browser f�r alle Tests

**Standard Test-Pattern (funktioniert ohne Pool)**:
```csharp
[ClassInitialize]
public static async Task Setup()
{
    playwright = await Playwright.CreateAsync();  // EINMAL
    browser = await playwright.Chromium.LaunchAsync();  // EINMAL
}

[TestMethod]
public async Task MyTest()
{
    var page = await browser.NewPageAsync();  // Nur Page pro Test
    // Test...
    await page.CloseAsync();
}

[ClassCleanup]
public static async Task Teardown()
{
    await browser.CloseAsync();
    playwright.Dispose();
}
```

**? LaunchedBrowserPool wird NICHT implementiert** (f�r jetzt)

---

## ?? Implementierungs-Plan (�berarbeitet)

### Phase 1: CDP Pooling (Fokus)

| Step | Status | Beschreibung |
|------|--------|--------------|
| 1 | ? | PlaywrightConnectionPool + PooledPageConnection |
| 2 | ? | CdpTabObject (sealed Creator) |
| 5 | ?? | TabObject.cs updaten (Warnings, Obsolete) |
| 6 | ?? | Copilot Instructions erweitern |
| 7 | ?? | WPF-Demo-Beispiel erstellen |
| 8 | ?? | Unit Tests f�r Pool |
| 9 | ?? | README-Dokumentation |
| 10 | ?? | Build & Verify |

**Steps 3-4 (LaunchedBrowserPool) werden �BERSPRUNGEN**

---

## ?? .NET Standard 2.0 Kompatibilit�t

### Wichtige Anpassungen

#### String.Contains() mit StringComparison

```csharp
// ? .NET Standard 2.0 hat NICHT:
pageUrl.Contains(targetUrl, StringComparison.OrdinalIgnoreCase)

// ? Stattdessen:
pageUrl.IndexOf(targetUrl, StringComparison.OrdinalIgnoreCase) >= 0
```

---

## ?? Anti-Patterns

### ? Was NICHT zu tun

```csharp
// ? Direct Creator() override (umgeht Pool)
protected override async Task<IPage> Creator()
{
    var playwright = await Playwright.CreateAsync();
    return await browser.NewPageAsync();
}

// ? Stattdessen: CdpTabObject verwenden
public class MyDialog : CdpTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";
}
```

---

### ? Connection nicht freigeben

```csharp
// ? Ohne Pool
var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.ConnectOverCDPAsync(...);
// Vergessen zu disposen ? Memory Leak!

// ? Mit Pool
var tab = new MyDialogTab();
await tab.Open();
await tab.Close(); // Pool managed cleanup
```

---

## ?? Test-Szenarien

### Unit Tests f�r PlaywrightConnectionPool

```csharp
[TestMethod]
public async Task GetOrCreatePageAsync_CachesConnection()
{
    var pool = PlaywrightConnectionPool.Instance;
    await pool.ClearAllAsync();

    var page1 = await pool.GetOrCreatePageAsync("http://localhost:12345", "test");
    var page2 = await pool.GetOrCreatePageAsync("http://localhost:12345", "test");

    Assert.AreSame(page1, page2); // Gleiche Instanz!
}

[TestMethod]
public async Task ValidatePageConnectionAsync_DetectsStaleConnection()
{
    // Connection erstellen
    var page = await pool.GetOrCreatePageAsync("http://localhost:12345", "test");
    
    // CEF-Subprocess simuliert Crash
    await page.CloseAsync();
    
    // Erneuter Zugriff sollte neue Connection erstellen
    var newPage = await pool.GetOrCreatePageAsync("http://localhost:12345", "test");
    
    Assert.AreNotSame(page, newPage);
}

[TestMethod]
public async Task CreatePageWithRetryAsync_RetriesOnFailure()
{
    pool.MaxRetryAttempts = 3;
    pool.RetryDelay = TimeSpan.FromMilliseconds(100);
    
    // CEF-Subprocess noch nicht bereit
    // ? Sollte automatisch retries durchf�hren
    var page = await pool.GetOrCreatePageAsync("http://localhost:12345", "test");
    
    Assert.IsNotNull(page);
}
```

---

## ?? Weitere Ressourcen

### Verwandte Patterns

- **Object Pool Pattern**: https://refactoring.guru/design-patterns/object-pool
- **Connection Pooling**: https://en.wikipedia.org/wiki/Connection_pool
- **Template Method Pattern**: https://refactoring.guru/design-patterns/template-method

### Playwright Dokumentation

- **CDP Connection**: https://playwright.dev/dotnet/docs/api/class-browsertype#browser-type-connect-over-cdp
- **Browser Lifecycle**: https://playwright.dev/dotnet/docs/browsers
- **Page API**: https://playwright.dev/dotnet/docs/api/class-page

---

## ?? Offene Fragen & Zukunft

### F�r sp�ter (wenn Bedarf besteht)

1. **LaunchedBrowserPool**: F�r Test-Szenarien mit vielen parallelen Browser-Instanzen
2. **Context-Pooling**: F�r Session-Isolation in Multi-User-Tests
3. **Metrics & Telemetry**: Detaillierte Performance-Metriken
4. **Health Checks**: Proaktive Connection-Validierung

---

## ?? Zusammenfassung

**Problem gel�st**: ?
- OutOfMemoryException in WPF + CefSharp
- CEF-Subprocess-Restart-Handling
- Memory-Leaks bei h�ufigem Dialog-�ffnen

**Kern-Innovation**: ??
- Validation-based Caching (statt Timeout)
- Automatic Retry Logic
- Per-Dialog Connection-Isolation
- Zero-Configuration f�r User

**N�chste Schritte**: ??
1. Dateien erstellen (3 Dateien)
2. Build & Test
3. Dokumentation & Demo
4. Release!

---

## ?? Kontakt & Beitragen

Bei Fragen oder Verbesserungsvorschl�gen:
- GitHub Issues: https://github.com/trumpf-code/Trumpf.Coparoo.Playwright/issues
- Diskussionen: https://github.com/trumpf-code/Trumpf.Coparoo.Playwright/discussions

---

**Stand**: 2025-04-17  
**Autor**: AI Assistant & Team  
**Version**: 1.0 (CDP-Focus)

