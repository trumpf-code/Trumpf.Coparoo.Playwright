# GitHub Copilot Instructions for Trumpf.Coparoo.Playwright

## Project Overview

Trumpf.Coparoo.Playwright is a .NET library that implements the **Co**ntrol/**Pa**ge/**Ro**ot-**O**bject (Coparoo) pattern for writing maintainable, robust, and fluent Playwright-driven web UI tests. The framework's structured patterns and interface-first design make it particularly well-suited for AI-assisted test generation and maintenance.

### Architecture Pattern

The framework follows a three-tier hierarchy:
1. **TabObject** - Represents a browser tab/application root
2. **PageObject** - Represents individual pages/views  
3. **ControlObject** - Represents UI elements (buttons, checkboxes, inputs, etc.)

### Choosing Between PageObject and ControlObject

| Aspect | PageObject | ControlObject |
|---|---|---|
| **Purpose** | Wraps a *unique* composite UI area (page, panel, dialog, sidebar) | Wraps a *reusable* single UI widget (button, checkbox, input, dropdown) |
| **Navigation** | Supports `Goto<T>()` with an overridable `Goto()` method | Cannot be navigated to — accessed as a property of its parent via `Find<T>()` |
| **Tree position** | Interior nodes — can contain child pages and child controls | Leaf nodes — declared via `Find<T>()` or `FindAll<T>()` |
| **Reuse** | Typically unique to a specific part of the application | Highly reusable across many pages (e.g., built-in `Button`, `Checkbox`, `Table`) |

**Rule of thumb:** Use `PageObject` for things that appear **once** in the UI (a settings page, a navigation sidebar, a login dialog). Use `ControlObject` for things that appear **many times** across the application (buttons, checkboxes, text inputs, table rows).

**Examples:**
- Settings page, user profile panel, navigation menu → `PageObject`
- Save button, search text box, "Enable notifications" checkbox, table row → `ControlObject`

If you need a `Goto()` override to make the element appear on screen (e.g., clicking a menu item to open a page), or the element contains child controls, use `PageObject`. If the element is a self-contained widget accessed from a parent, use `ControlObject`.

### Navigation: `Goto<T>()` vs `On<T>()`

- **`tab.Goto<T>()`** — navigates to a page: opens the tab (if not yet open) and calls `T.Goto()` to perform whatever navigation logic the page defines.
- **`tab.On<T>()`** — returns the page reference *without* navigation. Use when the page is already visible.
- Controls are **never** navigated to directly — they are accessed as properties of their parent page (e.g., `settingsPage.SaveButton`).

```csharp
await tab.Open();                                    // open the browser tab
var settings = tab.Goto<ISettings>();                 // navigate to the Settings page
await settings.EnableNotifications.Check();           // interact with a control on that page
await tab.On<ISettings>().SaveButton.ClickAsync();    // access control without re-navigating
```

### Interface-Based Decoupling

All PageObjects and ControlObjects should be accessed through interfaces to decouple test code from implementations. This enables independent team development and allows tests to be written before pages are implemented.

The pattern uses a three-assembly separation:
1. **Interface assembly** — defines `ISettings`, `ILoginPage`, etc. with control properties and operations
2. **Implementation assembly** — contains concrete `Settings : PageObject, ISettings` classes with locators and navigation logic
3. **Test assembly** — references only the interface assembly; never depends on implementations directly

```csharp
// Interface (in interface assembly)
public interface ISettings : IPageObject
{
    ICheckbox EnableNotifications { get; }
    IButton SaveButton { get; }
}

// Implementation (in implementation assembly)
public sealed class Settings : PageObject, ISettings
{
    protected override By SearchPattern => By.TestId("settings-page");
    public ICheckbox EnableNotifications => Find<ICheckbox>(By.TestId("enable-notifications"));
    public IButton SaveButton => Find<IButton>(By.TestId("save-button"));
}

// Test (in test assembly — only references interfaces)
var settings = tab.Goto<ISettings>();
await settings.EnableNotifications.Check();
```

Key rules:
- Use `tab.Goto<ISettings>()` / `tab.On<ISettings>()` — never concrete types in tests
- Control properties return interfaces (`ICheckbox`, `IButton`) — resolved at runtime via `Find<T>()`
- Use `TabObject.Resolve<IMyTab>()` when the tab itself must be resolved from its interface
- Register relationships with `ChildOf<TChild, TParent>()` in the TabObject constructor

For the full decoupling pattern and step-by-step guide, see [DECOUPLING.md](../DECOUPLING.md).

## Coding Standards

### General C# Conventions

- **Language version**: C# 10.0 (for .NET Standard 2.0 projects) or latest for .NET 8 projects
- **Null handling**: Use nullable reference types (`#nullable enable`) where appropriate, especially in new code
- **Naming**: PascalCase for public types/constants, `_camelCase` for private fields, camelCase for locals
- **File organization**: One primary type per file, matching the filename

### XML Documentation

All public APIs **must** have XML documentation (`<summary>`, `<param>`, `<returns>`).
Required for: all public classes, interfaces, methods, properties, and all protected members in base classes.
Start with a verb for methods. Document exceptions with `<exception>` tags. Reference types with `<see cref=""/>`.

### Copyright Headers

All `.cs` files must start with the Apache 2.0 license header:

```csharp
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
```

## Framework-Specific Patterns

### TabObject Implementation

```csharp
public class MyAppTab : TabObject
{
    protected override string Url => "https://example.com";
    
    protected override async Task<IPage> Creator()
    {
        // Browser setup logic
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        return await browser.NewPageAsync();
    }
    
    public MyAppTab()
    {
        // Register dynamic page relationships (decouples teams)
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();
    }
}
```

**TabObject Rules**:
- Always override `Url` property if navigating to a specific URL
- Use `ChildOf<TChild, TParent>()` for dynamic page relationships
- Avoid hardcoding page hierarchies - let teams register independently
- Clean up resources in `Close()` if custom setup in `Creator()`

### PageObject Implementation

```csharp
public sealed class Settings : PageObject, ISettings
{
    protected override By SearchPattern => By.TestId("settings-page");
    
    // Control properties using Find<T>
    public Checkbox EnableNotifications => Find<Checkbox>(By.TestId("enable-notifications"));
    public Button SaveButton => Find<Button>(By.TestId("save-button"));
    
    // Override Goto for custom navigation logic
    public override async Task Goto()
    {
        if (!await this.IsVisibleAsync())
        {
            // Navigate via menu or other mechanism
            await On<IShell>().Menu.NavigateToAsync(this);
            await this.WaitForVisibleAsync();
        }
    }
}
```

**PageObject Rules**:
- Always implement an interface (e.g., `ISettings`) for decoupling
- Use `sealed` for concrete page implementations
- Define `SearchPattern` with a unique, stable locator (prefer `data-testid`)
- Expose controls as properties (not methods) for natural API
- Use `Find<TControl>(By selector)` to locate child controls — never expose raw `ILocator`
- Override `Goto()` only if custom navigation is needed
- Use `On<IPageInterface>()` to access other pages in the hierarchy

### ControlObject Implementation

```csharp
public sealed class CustomButton : ControlObject, ICustomButton
{
    protected override By SearchPattern => By.TagName("button").And(By.ClassName("custom-btn"));
    
    public async Task<bool> IsEnabledAsync()
    {
        return await this.IsEnabledAsync();
    }
}
```

**ControlObject Rules**:
- Inherit from `ControlObject` or use built-in controls from `Trumpf.Coparoo.Playwright.Controls`
- Implement an interface if the control will be used across teams
- Define `SearchPattern` with combinable `By` selectors
- Expose behavior as async methods (e.g., `ClickAsync`, `GetTextAsync`)
- Reuse extension methods from `Trumpf.Coparoo.Playwright.Extensions` where possible
- **Never expose `ILocator` in public properties** — wrap child elements as `ControlObject` (via `Find<T>()`) or use built-in controls; use extension methods like `ClickAsync()` for interactions

### By Selector Best Practices

```csharp
By.TestId("my-element")                                          // preferred — most stable
By.TagName("button").And(By.ClassName("primary"))                 // combine with .And()
By.TagName("input").And(By.Id("username")).And(By.ClassName("x")) // order: tag → ID → class → attr → pseudo
```

- Prefer `By.TestId()` for test-specific selectors (most stable)
- Use `.And()` to combine selectors (NOT spaces or commas)
- Only one tag selector and one ID selector per combined selector
- Multiple class and attribute selectors are allowed
- Order is enforced: tag → ID → classes → attributes → pseudo-selectors

### Extension Method Patterns

```csharp
public static async Task<string> GetTextAsync(this IUIObject source)
{
    if (source == null) throw new ArgumentNullException(nameof(source));
    return await source.Locator.TextContentAsync() ?? string.Empty;
}
```

- Place in `Trumpf.Coparoo.Playwright.Extensions` project
- Always validate `source` parameter with null check
- Ensure element is scrolled into view for actions (`.ScrollIntoViewIfNeededAsync()`)
- Return meaningful defaults (e.g., empty string instead of null)
- Document with XML comments including usage examples

## Project Structure

```
Trumpf.Coparoo.Playwright/               # Core framework (.NET Standard 2.0)
Trumpf.Coparoo.Playwright.Controls/      # Built-in controls (.NET Standard 2.0)
Trumpf.Coparoo.Playwright.Extensions/    # Extension methods (.NET Standard 2.0)
Trumpf.Coparoo.Playwright.Tests/         # Unit tests (.NET 8)
Trumpf.Coparoo.Playwright.Controls.Tests/ # Control tests (.NET 8)
Trumpf.Coparoo.Playwright.Demo/          # Demo project (.NET 8)
```

- Target .NET Standard 2.0 for library projects (Core, Controls, Extensions)
- Use .NET 8 for test and demo projects
- Version management via **MinVer** from Git tags
- Core dependencies: **Microsoft.Playwright** 1.54.0, **System.Linq.Async** 6.0.1

## Anti-Patterns to Avoid

❌ Hardcode page relationships — use `ChildOf<,>()`
❌ Use concrete page types in tests — use interfaces
❌ Create UI object instances with `new` — use `Find<T>()`
❌ Expose `ILocator` or `IPage` in public properties of PageObjects/ControlObjects — wrap as typed controls via `Find<T>()`
❌ Use spaces in combined selectors — use `.And()`
❌ Skip null checks in extension methods
❌ Skip XML documentation on public APIs

## Contributing

1. All new features require XML documentation
2. Add unit tests for new functionality
3. Follow existing code style and patterns
4. Update README.md if adding major features
5. Ensure builds pass before submitting changes

## Related Documentation

- [README.md](../README.md) — Quick start and overview
- [PATTERN.md](../PATTERN.md) — Coparoo pattern theory (DOM structure, search optimization, PageObject vs ControlObject)
- [DECOUPLING.md](../DECOUPLING.md) — Interface-based decoupling for cooperative projects
- [DEMO.md](../DEMO.md) — Step-by-step code walkthrough
- [CHEATSHEET.md](../CHEATSHEET.md) — Concise test-writing reference for library consumers
- [Demo Project README](../Trumpf.Coparoo.Playwright.Demo/README.md) — Full working demo with dynamic relationships
