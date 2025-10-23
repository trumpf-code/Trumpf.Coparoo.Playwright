# Trumpf.Coparoo.Playwright Demo

This demo project showcases the powerful capabilities of the **Trumpf.Coparoo.Playwright** framework through a practical example that demonstrates real-world patterns for building maintainable, modular web test automation.

## 🎬 Demo in Action

![Coparoo Playwright Demo](demo.gif)

*The demo above shows the framework in action: navigating between pages, interacting with checkboxes and buttons, all through modular, dynamically-composed page objects.*

## 💡 Clean Test Code Example

Here's what a typical test looks like with Coparoo.Playwright - notice the clean, readable code with **no delays, no waits, no brittle selectors**:

```csharp
[TestMethod]
public async Task NavigateBetweenPages()
{
    // Arrange: Create browser instance
    var browser = new DemoTab(headless: true);
    try
    {
        await browser.Open();

        // Get reference to Settings page - type-safe, no strings!
        var settings = browser.On<ISettings>();

        // Interact with checkboxes - clean, expressive API
        await settings.EnableNotifications.Check();
        (await settings.EnableNotifications.IsChecked).Should().BeTrue();

        await settings.EnableAutoSave.Check();
        await settings.EnableDarkMode.Check();
        await settings.EnableAutoSave.Uncheck();

        // Navigate to another page - type-safe navigation
        var preferences = browser.Goto<IPreferences>();

        // Click buttons - no selectors, no waiting logic
        await preferences.SavePreferences.ClickAsync();
        await preferences.ResetToDefaults.ClickAsync();
        await preferences.ExportSettings.ClickAsync();

        // Navigate back - convention-based, clean
        browser.Goto<ISettings>();
    }
    finally
    {
        await browser.Close();
    }
}
```

**Key Benefits:**
- ✅ **No CSS selectors** in test code - they're encapsulated in page objects
- ✅ **No explicit waits** - built into the framework
- ✅ **Type-safe navigation** - `browser.Goto<ISettings>()` instead of strings
- ✅ **IntelliSense support** - discover available pages and controls as you type
- ✅ **Readable and maintainable** - tests read like business requirements

## 🎯 Key Concepts Demonstrated

### Dynamic Page Object Composition

The demo illustrates how page objects can be composed dynamically without requiring explicit parent-child relationship declarations in the page object classes themselves.

**Traditional Approach (Rigid):**
```csharp
public class Settings : PageObject, IChildOf<Shell> { }
```

**Coparoo Approach (Flexible):**
```csharp
// Settings has NO explicit parent declaration
public class Settings : PageObject, ISettings { }

// Relationships are registered dynamically in the root object
public DemoTab()
{
    ChildOf<Settings, Shell>();
    ChildOf<Preferences, Shell>();
}
```

### Team-Independent Development

This pattern enables multiple teams to work on different parts of the application independently:

- **Team A (Core)**: Maintains `DemoTab` and `Shell`
- **Team B (Settings)**: Develops `Settings` in isolation
- **Team C (Preferences)**: Develops `Preferences` in isolation

None of the teams need to modify each other's code. Integration happens through convention-based registration at runtime.

### Convention-Based Navigation

The framework uses naming conventions to enable type-safe navigation without tight coupling:

```csharp
// HTML menu item
<button data-page="Settings">Settings</button>

// C# navigation - type name matches data-page attribute
var page = browser.Goto<ISettings>();
```

This convention allows:
- New pages to be added without modifying navigation code
- Type-safe navigation with IntelliSense support
- Clear mapping between UI and code

### Interface-Based Testing

Tests interact with page objects through interfaces, not concrete implementations:

```csharp
ISettings settings = browser.Goto<ISettings>();
await settings.EnableNotifications.Check();
```

Benefits:
- Tests are decoupled from implementation details
- Easier to mock for unit testing
- Supports multiple implementations (e.g., different themes/layouts)

## 🏗️ Project Structure

```
Trumpf.Coparoo.Playwright.Demo/
├── TabObjects/
│   └── DemoTab.cs                # Root object, configures browser
├── PageObjects/
│   ├── Interfaces/
│   │   ├── IShell.cs             # Interface for main app shell
│   │   ├── ISettings.cs          # Interface for settings page
│   │   └── IPreferences.cs       # Interface for preferences page
│   ├── Shell.cs                  # Main app container with menu
│   ├── Settings.cs               # Settings page (checkboxes)
│   └── Preferences.cs            # Preferences page (buttons)
├── ControlObjects/
│   ├── Interfaces/
│   │   └── IMenu.cs              # Interface for menu control
│   └── Menu.cs                   # Menu implementation
├── wwwroot/
│   └── demo.html                 # Test HTML application
├── Demo.cs                       # Test demonstrations
└── README.md                     # This file
```

## 🚀 Running the Demo

### Prerequisites

1. Install Playwright browsers:
   ```bash
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

### Run Tests

**Headless Mode (CI-friendly):**
```bash
dotnet test --filter "TestCategory!=VisualTest"
```

**Headed Mode (Visual Debugging):**
```bash
dotnet test --filter "TestCategory=VisualTest"
```

**Run Specific Test:**
```bash
dotnet test --filter "FullyQualifiedName~NavigateBetweenPages"
```

## 📝 Test Scenarios

### DemonstrateModularPageComposition_Headless

Runs in headless mode (no visible browser). Demonstrates:
- Opening the application
- Navigating to Settings page via `Goto<T>` pattern
- Interacting with multiple checkboxes
- Navigating to Preferences page
- Clicking action buttons
- Verifying page state

**Perfect for:** CI/CD pipelines, automated regression testing

### DemonstrateModularPageComposition_Headed

Identical functionality but runs with visible browser window. Includes intentional delays for observation.

**Perfect for:** Development, debugging, demonstrations

## 🔑 Key Design Patterns

### Pattern: No Explicit Parent-Child Declarations

```csharp
// ❌ Traditional - tightly coupled
public class Settings : PageObject, IChildOf<Shell> { }

// ✅ Coparoo - loosely coupled
public class Settings : PageObject, ISettings
{
    // No parent declaration - can be used anywhere
}
```

### Pattern: Dynamic Registration

```csharp
public DemoTab()
{
    // Register relationships at runtime
    ChildOf<Shell, DemoTab>();
    ChildOf<Settings, Shell>();
    ChildOf<Preferences, Shell>();
}
```

### Pattern: Convention-Based Navigation

```csharp
public override async Task Goto()
{
    if (!await IsActiveAsync())
    {
        var shell = On<IShell>();
        await shell.Menu.NavigateToAsync<ISettings>();
        await WaitForVisibleAsync();
    }
}
```

### Pattern: Interface Segregation

```csharp
public interface ISettings : IPageObject
{
    Checkbox EnableNotifications { get; }
    Checkbox EnableAutoSave { get; }
    Checkbox EnableDarkMode { get; }
    Task<bool> IsActiveAsync();
}
```

## 🌟 Real-World Benefits

### Modularity
Page objects can be distributed as separate NuGet packages. Each team can version and release independently.

### Scalability
New pages can be added without modifying existing code. Simply register the relationship in the TabObject constructor.

### Maintainability
Clear separation of concerns. Each page object focuses on its own functionality.

### Testability
Interface-based design enables easy mocking and unit testing.

### Flexibility
Different teams can work in parallel without merge conflicts in page object code.

## 🎨 Control Types Showcased

- **Checkboxes** (Settings Page): Demonstrates state management (checked/unchecked)
- **Buttons** (Preferences Page): Demonstrates action triggering with visual feedback
- **Navigation Menu**: Custom control with convention-based page switching

## 📚 Learning Path

1. **Start with**: `DemoTab.cs` - understand browser configuration and dynamic registration
2. **Then read**: `Shell.cs` - see the main container structure
3. **Explore**: `Settings.cs` and `Preferences.cs` - notice the absence of parent declarations
4. **Understand**: `Menu.cs` - see how convention-based navigation works
5. **Run**: `Demo.cs` - see everything in action

## 🔧 Extending the Demo

To add a new page:

1. Create interface in `PageObjects/Interfaces/`
2. Implement page object in `PageObjects/`
3. Add menu button in `wwwroot/demo.html` with correct `data-page` attribute
4. Register relationship in `DemoTab` constructor:
   ```csharp
   ChildOf<YourNewPage, Shell>();
   ```

No need to modify any existing page objects!

## 📖 Additional Resources

- [Main Coparoo.Playwright Documentation](../README.md)
- [Pattern Overview](../PATTERN.md)
- [Decoupling Concepts](../DECOUPLING.md)

## 📄 License

Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG

Licensed under the Apache License, Version 2.0. See [LICENSE](../LICENSE) for details.
