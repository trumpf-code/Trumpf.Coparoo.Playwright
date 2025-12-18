# GitHub Copilot Instructions for Trumpf.Coparoo.Playwright

## Project Overview

Trumpf.Coparoo.Playwright is a .NET library that implements the **Co**ntrol/**Pa**ge/**Ro**ot-**O**bject (Coparoo) pattern for writing maintainable, robust, and fluent Playwright-driven web UI tests.

### Architecture Pattern

The framework follows a three-tier hierarchy:
1. **TabObject** - Represents a browser tab/application root
2. **PageObject** - Represents individual pages/views  
3. **ControlObject** - Represents UI elements (buttons, checkboxes, inputs, etc.)

### Core Principles

- **Interface-based navigation**: Use `tab.Goto<ISettings>()` instead of concrete types for team decoupling
- **Dynamic relationships**: Register page hierarchies via `ChildOf<TChild, TParent>()` in TabObject
- **Fluent API**: Methods should read like natural language (e.g., `await checkbox.Check()`)
- **Type safety**: Prefer compile-time errors over runtime failures

## Coding Standards

### General C# Conventions

- **Language version**: C# 10.0 (for .NET Standard 2.0 projects) or latest for .NET 8 projects
- **Null handling**: Use nullable reference types (`#nullable enable`) where appropriate, especially in new code
- **Naming**:
  - Public types: PascalCase
  - Private fields: camelCase with underscore prefix (e.g., `_fieldName`)
  - Local variables: camelCase
  - Constants: PascalCase
- **File organization**: One primary type per file, matching the filename

### XML Documentation

All public APIs **must** have XML documentation:

```csharp
/// <summary>
/// Brief description of what the member does.
/// </summary>
/// <param name="paramName">Description of parameter.</param>
/// <returns>Description of return value.</returns>
/// <remarks>
/// Additional details, usage notes, or warnings (optional).
/// </remarks>
```

**Required for**:
- All public classes, interfaces, methods, properties
- All protected members in extensible base classes (TabObject, PageObject, ControlObject)

**Guidelines**:
- Start with a verb for methods (e.g., "Gets", "Creates", "Validates")
- Be concise but complete - explain the "what" and "why", not the "how"
- Document exceptions with `<exception>` tags
- Use `<code>` blocks for usage examples where helpful
- Reference related types with `<see cref="TypeName"/>`

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
- Use `Find<TControl>(By selector)` to locate child controls
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

### By Selector Best Practices

```csharp
// Prefer TestId for stability
By.TestId("my-element")

// Combine selectors for specificity
By.TagName("button").And(By.ClassName("primary"))

// Available selector types
By.CssSelector(".my-class")
By.XPath("//div[@id='test']")
By.Id("element-id")
By.ClassName("my-class")
By.TagName("div")

// Combine multiple (order matters: tag first, then ID, then classes, attributes, pseudo)
By.TagName("input").And(By.Id("username")).And(By.ClassName("form-control"))
```

**By Selector Rules**:
- Prefer `By.TestId()` for test-specific selectors (most stable)
- Use `.And()` to combine selectors (NOT spaces or commas)
- Only one tag selector and one ID selector per combined selector
- Multiple class and attribute selectors are allowed
- Order is enforced: tag ? ID ? classes ? attributes ? pseudo-selectors

### Extension Method Patterns

When creating extension methods:

```csharp
public static async Task<string> GetTextAsync(this IUIObject source)
{
    if (source == null) throw new ArgumentNullException(nameof(source));
    return await source.Locator.TextContentAsync() ?? string.Empty;
}
```

**Extension Method Rules**:
- Place in `Trumpf.Coparoo.Playwright.Extensions` project
- Always validate `source` parameter with null check
- Ensure element is scrolled into view for actions (`.ScrollIntoViewIfNeededAsync()`)
- Return meaningful defaults (e.g., empty string instead of null)
- Document with XML comments including usage examples

### Test Writing Guidelines

```csharp
[TestMethod]
public async Task DemonstrateFeature_Headless()
{
    var tab = new MyAppTab(headless: true);
    
    try
    {
        await tab.Open();
        
        // Use interface-based navigation
        var settingsPage = tab.Goto<ISettings>();
        
        // Use fluent extension methods
        await settingsPage.EnableNotifications.Check();
        
        // Use FluentAssertions for readability
        (await settingsPage.EnableNotifications.IsChecked).Should().BeTrue();
        
        // Navigate between pages
        tab.Goto<IPreferences>();
    }
    finally
    {
        await tab.Close();
    }
}
```

**Test Rules**:
- Always use `try/finally` to ensure `tab.Close()` is called
- Use interface types for page references (e.g., `ISettings` not `Settings`)
- Prefer `tab.Goto<TPage>()` for explicit navigation
- Use `tab.On<TPage>()` for accessing current page without navigation
- Add delays with `await Task.Delay()` only in headed tests for visualization
- Use FluentAssertions for assertions (`.Should().BeTrue()`, `.Should().Be()`)

## Project Structure

### Solution Organization

```
Trumpf.Coparoo.Playwright/               # Core framework (.NET Standard 2.0)
??? Root/
?   ??? TabObject/                       # Browser tab abstraction
?   ??? PageObject/                      # Page abstraction
?   ??? ControlObject/                   # Control abstraction
??? Search/                              # By selectors and locator helpers
??? Internal/                            # Framework internals (not for public use)

Trumpf.Coparoo.Playwright.Controls/      # Built-in controls (.NET Standard 2.0)
??? Button.cs, Checkbox.cs, TextBox.cs   # Standard HTML controls
??? Table.cs, DropDown.cs, etc.

Trumpf.Coparoo.Playwright.Extensions/    # Extension methods (.NET Standard 2.0)
??? IUIObjectActionExtensions.cs         # Click, Fill, etc.
??? IUIObjectStateExtensions.cs          # IsVisible, IsEnabled, etc.
??? TabObjectExtensions.cs               # Tab-specific helpers

Trumpf.Coparoo.Playwright.Tests/         # Unit tests (.NET 8)
Trumpf.Coparoo.Playwright.Controls.Tests/ # Control tests (.NET 8)
Trumpf.Coparoo.Playwright.Demo/          # Demo project (.NET 8)
```

### File Naming Conventions

- Interfaces: `IInterfaceName.cs` (e.g., `ISettings.cs`)
- Implementations: `TypeName.cs` (e.g., `Settings.cs`)
- Tests: `TypeNameTests.cs` (e.g., `ByTests.cs`)
- Extensions: `TypeNameExtensions.cs` (e.g., `IUIObjectActionExtensions.cs`)

## Common Scenarios

### Adding a New Control Type

1. Create interface in `Trumpf.Coparoo.Playwright/Interfaces/`
2. Implement in `Trumpf.Coparoo.Playwright.Controls/`
3. Add extension methods if needed in `Extensions/`
4. Add tests in `Controls.Tests/`

### Adding a New PageObject

1. Define interface (e.g., `IMyPage.cs`)
2. Implement class inheriting `PageObject` and interface
3. Define `SearchPattern` with stable locator
4. Expose controls as properties using `Find<T>()`
5. Register relationship in `TabObject` using `ChildOf<,>()`

### Creating a Custom TabObject

1. Inherit from `TabObject`
2. Override `Url` property
3. Override `Creator()` to configure Playwright browser
4. Register page relationships in constructor with `ChildOf<,>()`
5. Optionally expose configuration options (headless, slowMo, etc.)

## Dependencies and Packages

### Core Dependencies
- **Microsoft.Playwright**: 1.54.0 - Browser automation
- **System.Linq.Async**: 6.0.1 - Async LINQ operations

### Testing Dependencies
- **MSTest.TestFramework**: For unit tests
- **FluentAssertions**: For readable assertions

### Package Management
- Version management via **MinVer** from Git tags
- No hardcoded versions in projects (except dependencies)
- NuGet packages: Core, Controls, Extensions

## Anti-Patterns to Avoid

? **Don't** hardcode page relationships - use `ChildOf<,>()`
? **Don't** use concrete page types in tests - use interfaces
? **Don't** create UI object instances with `new` - use `Find<T>()`
? **Don't** forget XML documentation on public APIs
? **Don't** use spaces in combined selectors - use `.And()`
? **Don't** skip null checks in extension methods
? **Don't** leave tabs open after tests - always call `Close()`

? **Do** register page relationships dynamically
? **Do** use interface-based navigation
? **Do** use `Find<T>()` for child controls
? **Do** document all public APIs
? **Do** use `.And()` for selector combination
? **Do** validate parameters in extension methods
? **Do** clean up resources in `finally` blocks

## Performance Considerations

- Use `IsVisibleAsync()` checks before unnecessary navigation
- Batch multiple locator queries when possible
- Prefer `By.TestId()` for fastest, most stable selectors
- Cache control properties (framework handles this automatically)
- Use headless mode for CI/CD pipelines

## Debugging Tips

- Use `WriteTree()` to visualize page object hierarchy
- Add `await Task.Delay()` in headed tests to observe interactions
- Use `.HighlightAsync()` extension to visually identify elements
- Enable Playwright traces for detailed execution logs
- Set `headless: false` in TabObject constructor for visual debugging

## Questions or Clarifications

When unsure about:
- **Architecture**: Follow the TabObject ? PageObject ? ControlObject hierarchy
- **Interfaces**: Always create interfaces for cross-team usage
- **Locators**: Prefer `data-testid` attributes, then CSS selectors
- **Navigation**: Use `Goto<T>()` for explicit navigation, `On<T>()` for access
- **Documentation**: When in doubt, add more documentation

## Contributing Guidelines

1. All new features require XML documentation
2. Add unit tests for new functionality
3. Follow existing code style and patterns
4. Update README.md if adding major features
5. Ensure builds pass before submitting changes
6. Target .NET Standard 2.0 for library projects (Core, Controls, Extensions)
7. Use .NET 8 for test and demo projects

---

**Remember**: The goal is to write tests that read like natural language and remain robust against UI changes. The Coparoo pattern achieves this through abstraction layers and interface-based design.
