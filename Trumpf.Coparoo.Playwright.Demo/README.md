# Trumpf.Coparoo.Playwright Demo

This demo project showcases the powerful capabilities of the **Trumpf.Coparoo.Playwright** framework through a practical example that demonstrates real-world patterns for building maintainable, modular web test automation.

## � Demo in Action

![Coparoo Playwright Demo](demo.gif)

*The demo above shows the framework in action: navigating between pages, interacting with checkboxes and buttons, all through modular, dynamically-composed page objects.*

## �🎯 Key Concepts Demonstrated

### 1. Dynamic Page Object Composition

The demo illustrates how page objects can be composed dynamically without requiring explicit parent-child relationship declarations in the page object classes themselves.

**Traditional Approach (Rigid):**
```csharp
public class SettingsPage : PageObject, IChildOf<ApplicationShell> { }
```

**Coparoo Approach (Flexible):**
```csharp
// SettingsPage has NO explicit parent declaration
public class SettingsPage : PageObject, ISettingsPage { }

// Relationships are registered dynamically in the TabObject
public DemoTabObject()
{
    ChildOf<SettingsPage, ApplicationShell>();
    ChildOf<PreferencesPage, ApplicationShell>();
}
```

### 2. Team-Independent Development

This pattern enables multiple teams to work on different parts of the application independently:

- **Team A (Core)**: Maintains `DemoTabObject` and `ApplicationShell`
- **Team B (Settings)**: Develops `SettingsPage` in isolation
- **Team C (Preferences)**: Develops `PreferencesPage` in isolation

None of the teams need to modify each other's code. Integration happens through convention-based registration at runtime.

### 3. Convention-Based Navigation

The framework uses naming conventions to enable type-safe navigation without tight coupling:

```csharp
// HTML menu item
<button data-page="SettingsPage">Settings</button>

// C# navigation - type name matches data-page attribute
var page = tab.Goto<ISettingsPage>();
```

This convention allows:
- New pages to be added without modifying navigation code
- Type-safe navigation with IntelliSense support
- Clear mapping between UI and code

### 4. Interface-Based Testing

Tests interact with page objects through interfaces, not concrete implementations:

```csharp
ISettingsPage settingsPage = tab.Goto<ISettingsPage>();
await settingsPage.EnableNotifications.Check();
```

Benefits:
- Tests are decoupled from implementation details
- Easier to mock for unit testing
- Supports multiple implementations (e.g., different themes/layouts)

## 🏗️ Project Structure

```
Trumpf.Coparoo.Playwright.Demo/
├── TabObjects/
│   └── DemoTabObject.cs          # Root object, configures browser
├── PageObjects/
│   ├── Interfaces/
│   │   ├── IApplicationShell.cs  # Interface for main app shell
│   │   ├── ISettingsPage.cs      # Interface for settings page
│   │   └── IPreferencesPage.cs   # Interface for preferences page
│   ├── ApplicationShell.cs       # Main app container with menu
│   ├── SettingsPage.cs           # Settings page (checkboxes)
│   └── PreferencesPage.cs        # Preferences page (buttons)
├── ControlObjects/
│   ├── Interfaces/
│   │   └── INavigationMenu.cs    # Interface for menu control
│   └── NavigationMenu.cs         # Navigation menu implementation
├── wwwroot/
│   └── demo.html                 # Test HTML application
├── DynamicCompositionDemo.cs     # Test demonstrations
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
dotnet test --filter "FullyQualifiedName~DemonstrateModularPageComposition_Headless"
```

## 📝 Test Scenarios

### 1. `DemonstrateModularPageComposition_Headless`

Runs in headless mode (no visible browser). Demonstrates:
- Opening the application
- Navigating to Settings page via `Goto<T>` pattern
- Interacting with multiple checkboxes
- Navigating to Preferences page
- Clicking action buttons
- Verifying page state

**Perfect for:** CI/CD pipelines, automated regression testing

### 2. `DemonstrateModularPageComposition_Headed`

Identical functionality but runs with visible browser window. Includes intentional delays for observation.

**Perfect for:** Development, debugging, demonstrations

### 3. `DemonstrateMultiPageNavigation_Headless`

Shows rapid navigation between pages to verify state management and navigation reliability.

## 🔑 Key Design Patterns

### Pattern 1: No Explicit Parent-Child Declarations

```csharp
// ❌ Traditional - tightly coupled
public class SettingsPage : PageObject, IChildOf<ApplicationShell> { }

// ✅ Coparoo - loosely coupled
public class SettingsPage : PageObject, ISettingsPage
{
    // No parent declaration - can be used anywhere
}
```

### Pattern 2: Dynamic Registration

```csharp
public DemoTabObject()
{
    // Register relationships at runtime
    ChildOf<ApplicationShell, DemoTabObject>();
    ChildOf<SettingsPage, ApplicationShell>();
    ChildOf<PreferencesPage, ApplicationShell>();
}
```

### Pattern 3: Convention-Based Navigation

```csharp
public override async Task Goto()
{
    if (!await IsActiveAsync())
    {
        var shell = Goto<IApplicationShell>();
        await shell.NavigationMenu.NavigateToAsync<ISettingsPage>();
        await WaitForVisibleAsync();
    }
}
```

### Pattern 4: Interface Segregation

```csharp
public interface ISettingsPage : IPageObject
{
    Checkbox EnableNotifications { get; }
    Checkbox EnableAutoSave { get; }
    Checkbox EnableDarkMode { get; }
    Task<bool> IsActiveAsync();
}
```

## 🌟 Real-World Benefits

### 1. **Modularity**
Page objects can be distributed as separate NuGet packages. Each team can version and release independently.

### 2. **Scalability**
New pages can be added without modifying existing code. Simply register the relationship in the TabObject constructor.

### 3. **Maintainability**
Clear separation of concerns. Each page object focuses on its own functionality.

### 4. **Testability**
Interface-based design enables easy mocking and unit testing.

### 5. **Flexibility**
Different teams can work in parallel without merge conflicts in page object code.

## 🎨 Control Types Showcased

- **Checkboxes** (Settings Page): Demonstrates state management (checked/unchecked)
- **Buttons** (Preferences Page): Demonstrates action triggering with visual feedback
- **Navigation Menu**: Custom control with convention-based page switching

## 📚 Learning Path

1. **Start with**: `DemoTabObject.cs` - understand browser configuration and dynamic registration
2. **Then read**: `ApplicationShell.cs` - see the main container structure
3. **Explore**: `SettingsPage.cs` and `PreferencesPage.cs` - notice the absence of parent declarations
4. **Understand**: `NavigationMenu.cs` - see how convention-based navigation works
5. **Run**: `DynamicCompositionDemo.cs` - see everything in action

## 🔧 Extending the Demo

To add a new page:

1. Create interface in `PageObjects/Interfaces/`
2. Implement page object in `PageObjects/`
3. Add menu button in `wwwroot/demo.html` with correct `data-page` attribute
4. Register relationship in `DemoTabObject` constructor:
   ```csharp
   ChildOf<YourNewPage, ApplicationShell>();
   ```

No need to modify any existing page objects!

## 📖 Additional Resources

- [Main Coparoo.Playwright Documentation](../../README.md)
- [Pattern Overview](../../PATTERN.md)
- [Decoupling Concepts](../../DECOUPLING.md)

## 📄 License

Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG

Licensed under the Apache License, Version 2.0. See [LICENSE](../LICENSE) for details.
