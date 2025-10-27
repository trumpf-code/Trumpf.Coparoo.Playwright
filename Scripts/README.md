# Utility Scripts

This folder contains PowerShell scripts for maintaining and updating the Trumpf.Coparoo.Playwright project.

## List-ILocatorMethods.ps1

This script inspects the Microsoft.Playwright assembly and lists all methods from the `ILocator` interface. It's useful for:

- Tracking changes in Playwright versions
- Ensuring `IControlObjectActionExtensions` stays synchronized with Playwright's API
- Identifying new action methods that should be added to the extensions
- Documenting the complete Playwright API surface

### Usage

**Basic usage (Summary format):**
```powershell
.\Scripts\List-ILocatorMethods.ps1
```

**Detailed format with full signatures:**
```powershell
.\Scripts\List-ILocatorMethods.ps1 -OutputFormat Detailed
```

**Markdown table format:**
```powershell
.\Scripts\List-ILocatorMethods.ps1 -OutputFormat Markdown
```

**Save markdown output to file:**
```powershell
.\Scripts\List-ILocatorMethods.ps1 -OutputFormat Markdown > ILocatorMethods.md
```

**Specify custom DLL path:**
```powershell
.\Scripts\List-ILocatorMethods.ps1 -PlaywrightDllPath "C:\path\to\Microsoft.Playwright.dll"
```

### Output Formats

- **Summary**: Lists method names, return types, and overload counts (default)
- **Detailed**: Shows full method signatures with parameter types and names
- **Markdown**: Generates a markdown table suitable for documentation

### When to Use

Run this script when:

1. **Upgrading Playwright**: After updating the Microsoft.Playwright NuGet package, run this script to see if new action methods have been added
2. **Updating Extensions**: Compare the output with `IControlObjectActionExtensions.cs` to ensure all action methods are covered
3. **Documentation**: Generate markdown documentation of the Playwright API
4. **Code Review**: Verify that all necessary methods are implemented

### Output Sections

The script provides three key sections:

1. **Summary**: Total method count and unique method names
2. **Method List**: All ILocator methods with overload information
3. **Action Methods**: Verification that all expected action methods exist in Playwright and are implemented in `IControlObjectActionExtensions`

### Prerequisites

- The solution must be built at least once (the script needs access to `Microsoft.Playwright.dll`)
- PowerShell 5.1 or later
- .NET runtime compatible with the Playwright version being inspected

### Example Output

```
========================================
Summary: ILocator Interface Methods
========================================
Total methods (including overloads): 79
Unique method names: 63

Method List:
----------------------------------------
  ClickAsync() -> Task
  FillAsync() -> Task
  SelectOptionAsync() -> Task`1 [6 overloads]
  ...

========================================
Action Methods (for IControlObjectActionExtensions):
========================================
Found action methods: 19/19
  ✓ ClickAsync
  ✓ FillAsync
  ✓ SelectOptionAsync (6 overloads)
  ...
```

### Maintenance

The script includes a predefined list of action methods that should be in `IControlObjectActionExtensions`. If Playwright adds new action methods, update the `$actionMethods` array in the script.
