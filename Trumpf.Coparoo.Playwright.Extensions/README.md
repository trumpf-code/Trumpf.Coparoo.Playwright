# Trumpf.Coparoo.Playwright.Extensions

Extension methods that augment the core `Trumpf.Coparoo.Playwright` API with convenience helpers:

- UI object helpers: existence, visibility, attribute access, waiting, role and test id lookup
- Control object helpers: click, fill, read text / value
- Tab object helpers: fluent page injection

## Installation

Install via NuGet (package is versioned by Git tags via MinVer):
```
dotnet add package Trumpf.Coparoo.Playwright.Extensions
```

## Example
```csharp
var myControl = pageObject.Control<MyButton>();
await myControl.ClickAsync();
await myControl.FillAsync("Hello");
bool shown = await myControl.IsVisibleAsync();
```

## Versioning
Versions are derived from Git tags in the repository using MinVer. Pre-release versions use the `preview` tag until a stable tag is created.

## License
Apache 2.0
