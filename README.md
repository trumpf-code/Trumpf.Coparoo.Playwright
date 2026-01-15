# Trumpf.Coparoo.Playwright Library for .NET
![logo640]

## Description
*Trumpf.Coparoo.Playwright is a .NET library for C# that helps you write fast, maintainable, robust and fluent Playwright-driven web tests based on the **co**ntrol/**pa**ge/**ro**ot-**o**bject (Coparoo) pattern.*

## Quick Start Example
An animated walkthrough of a minimal multi-page setup (dynamic relationships, interface-based navigation, checkbox + button interactions):

![Coparoo demo animation](Trumpf.Coparoo.Playwright.Demo/demo.gif)

For a detailed walkthrough of this demo, including the complete source code and step-by-step explanations, see [Demo README](Trumpf.Coparoo.Playwright.Demo/README.md).

Here's the basic pattern illustrated in the demo above - notice how naturally the test reads:

    var tab = new DemoTab();                                   // create the browser tab
    await tab.Open();                                          // open the application

    await tab.On<ISettings>().EnableNotifications.Check();     // interact with a checkbox
    await tab.On<ISettings>().EnableAutoSave.Check();          // check another option

    tab.Goto<IPreferences>();                                  // navigate to Preferences page
    await tab.On<IPreferences>().SavePreferences.ClickAsync(); // click a button
    await tab.On<IPreferences>().ExportSettings.ClickAsync();  // click another button

    await tab.Close();                                         // cleanup

The library ships with built-in control wrappers for common HTML elements, making it easy to interact with various UI components. These include `Checkbox`, `Button`, `TextBox`, `DropDown`, `Table`, `RadioButton`, and many more. In the example above, checkboxes are used for the notification and auto-save settings, demonstrating how these controls provide clean, type-safe APIs for interaction without dealing with raw locators or CSS selectors.

## Chrome DevTools Protocol (CDP) Connection Pooling for WPF/CefSharp Applications

For WPF applications using CefSharp dialogs, the library provides smart connection pooling to prevent memory leaks and improve performance. Use `ChromeDevToolsProtocolTabObject` to automatically manage Playwright connections via Chrome DevTools Protocol (CDP):

```csharp
public class SettingsDialogTab : ChromeDevToolsProtocolTabObject
{
    protected override string CdpEndpoint => "http://localhost:12345";  // CefSharp debugging port
    protected override string PageIdentifier => "settings_dialog";      // Unique dialog identifier
    protected override string Url => "https://myapp.local/settings";    // Page URL
    
    public SettingsDialogTab() => ChildOf<SettingsPage, SettingsDialogTab>();
}

The connection pool validates and reuses existing connections, dramatically reducing memory consumption when repeatedly opening/closing dialogs. Configure pool behavior if needed:

```csharp
var pool = PlaywrightConnectionPool.Instance;
pool.MaxRetryAttempts = 5;                         // Increase for slower CEF startup
pool.RetryDelay = TimeSpan.FromSeconds(1);         // Delay between retry attempts
pool.EnablePageCaching = true;                     // Per-dialog caching (default)
```

For comprehensive documentation on CDP pooling, including monitoring, error handling, and advanced scenarios, see the [Pooling README](Trumpf.Coparoo.Playwright/Pooling/README.md) and [Usage Guide](Trumpf.Coparoo.Playwright/Pooling/USAGE.md).

## NuGet Package Information
To make it easier for you to develop with the *Trumpf Coparoo Web* library we release it as NuGet packages:

- Core: [Trumpf.Coparoo.Playwright](https://www.nuget.org/packages/Trumpf.Coparoo.Playwright)
- Controls library: [Trumpf.Coparoo.Playwright.Controls](https://www.nuget.org/packages/Trumpf.Coparoo.Playwright.Controls)
- Extension helpers: [Trumpf.Coparoo.Playwright.Extensions](https://www.nuget.org/packages/Trumpf.Coparoo.Playwright.Extensions)

Installation examples (Package Manager Console):
`Install-Package Trumpf.Coparoo.Playwright`
`Install-Package Trumpf.Coparoo.Playwright.Controls`
`Install-Package Trumpf.Coparoo.Playwright.Extensions`

## Getting Started
If you want to learn more about the *control/page/root-object pattern*, the idea behind this framework, consider reading [the design pattern introduction](PATTERN.md).
It illustrates how the framework can help you at writing maintainable and fast-running user interface tests.

If you can't wait getting started and want see some code, have a look at [this code example](DEMO.md).

For a richer, multi-page sample illustrating dynamic page object relationships, team decoupling, and interface-based testing, explore the dedicated demo: [Full Demo README](Trumpf.Coparoo.Playwright.Demo/README.md).

Finally, if things are set up and you want to work on user interface tests in a collaborative setup consisting of many possibly independent teams, or write test cases even before the user interfaces ready to execute (say, directly after the UX team is done) consider reading [this tutorial](DECOUPLING.md).
The demo project shows these principles in practice (dynamic ChildOf registration + interface isolation). See: [Demo README](Trumpf.Coparoo.Playwright.Demo/README.md).

## Contributors
Main development by Alexander Kaiser.

Ideas and contributions by many more including
- Gregor Kriwet / *Accenture*
- Daniel Knorreck, Gerald Waldherr / *Additive Manufacturing, TRUMPF Laser- und Systemtechnik GmbH, Ditzingen*
- Jochen Lange, Matthias Wetzel, Markus Ament, Bernd Gschwind, Bernd Theissler, Andreas Alavi, Sebastian Mayer, Daniel Boeck / *TRUMPF Werkzeugmaschinen GmbH + Co. KG, Ditzingen*
- Igor Mikhalev / *Trumpf Laser Marking Systems AG, Schweiz*
- Thanikaivel Natarajan / *India Metamation Software P. Ltd., India*
- Nol Zefaj, Nils Engelbach, Phi Dang, Mattanja Kern, Felix Eisele / *AXOOM GmbH, Karlsruhe*
- Manuel Pfemeter / *AIT – Applied Information Technologies GmbH & Co. KG, Stuttgart*
- Marie Jeutter / *Hochschule Karlsruhe*

## License
Copyright (c) TRUMPF Werkzeugmaschinen GmbH + Co. KG. All rights reserved. 2016 - 2025.

Licensed under the [Apache License Version 2.0](LICENSE) License.

[logo640]: ./Resources/logo640.png "coparoo web logo"