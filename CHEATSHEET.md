# Coparoo Test Writing Cheat Sheet

Concise reference for writing tests with the Trumpf.Coparoo.Playwright library.

> **AI-ready by design:** Coparoo's rigid hierarchy, interface-first approach, and typed controls make tests structurally easy for AI agents to generate correctly and hard to generate incorrectly. Point your AI agent at this cheat sheet — it will produce robust, maintainable tests.

## Three-Tier Hierarchy

1. **TabObject** — browser tab / application root
2. **PageObject** — unique UI area (page, panel, dialog)
3. **ControlObject** — reusable widget (button, checkbox, input)

## PageObject vs ControlObject

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

## Navigation

- **`tab.Goto<T>()`** — navigates to a page: opens the tab (if not yet open) and calls `T.Goto()` to perform whatever navigation logic the page defines.
- **`tab.On<T>()`** — returns the page reference *without* navigation. Use when the page is already visible.
- Controls are **never** navigated to directly — they are accessed as properties of their parent page (e.g., `settingsPage.SaveButton`).

```csharp
await tab.Open();                                    // open the browser tab
var settings = tab.Goto<ISettings>();                 // navigate to the Settings page
await settings.EnableNotifications.Check();           // interact with a control on that page
await tab.On<ISettings>().SaveButton.ClickAsync();    // access control without re-navigating
```

## Project Organization

Define your TabObjects, PageObjects, and ControlObjects in a **separate project** from your test project. This allows them to be published as a NuGet package and consumed by multiple test suites — e.g., UI tests living alongside the UI code, integration tests in a different repository, or E2E tests maintained by a separate team.

```
MyApp.PageObjects/          # Tab, Page, Control definitions → publish as NuGet
MyApp.UITests/              # UI tests → references MyApp.PageObjects
MyApp.IntegrationTests/     # Integration tests → also references MyApp.PageObjects
```

## Interface-Based Decoupling (Advanced)

When page object implementations ship with the system under test (SUT) and tests are deployed separately, use interface-based decoupling. This is useful when:
- Tests should work across **multiple UI versions** without recompilation — only the page object implementation changes (e.g., different locators), not the tests.
- Page object implementations are **loaded dynamically** at runtime (e.g., via `Assembly.LoadFrom`).
- Multiple teams develop pages independently and register them via `ChildOf<,>()`.

The pattern uses a three-assembly separation:
1. **Interface assembly** — defines `ISettings`, `ILoginPage`, etc. with control properties and operations
2. **Implementation assembly** — ships with the SUT; contains concrete `Settings : PageObject, ISettings` with locators
3. **Test assembly** — references only the interface assembly; implementations are resolved at runtime

```csharp
// Interface (in interface assembly)
public interface ISettings : IPageObject
{
    ICheckbox EnableNotifications { get; }
    IButton SaveButton { get; }
}

// Implementation (ships with SUT — loaded dynamically)
public sealed class Settings : PageObject, ISettings
{
    protected override By SearchPattern => By.TestId("settings-page");
    public ICheckbox EnableNotifications => Find<ICheckbox>(By.TestId("enable-notifications"));
    public IButton SaveButton => Find<IButton>(By.TestId("save-button"));
}

// Test (references only interfaces — works across UI versions)
var settings = tab.Goto<ISettings>();
await settings.EnableNotifications.Check();
```

Key rules:
- Use `tab.Goto<ISettings>()` / `tab.On<ISettings>()` — never concrete types in tests
- Control properties return interfaces (`ICheckbox`, `IButton`) — resolved at runtime via `Find<T>()`
- Use `TabObject.Resolve<IMyTab>()` when the tab itself must be resolved from its interface
- Register relationships with `ChildOf<TChild, TParent>()` in the TabObject constructor

For the full decoupling pattern and step-by-step guide, see [DECOUPLING.md](DECOUPLING.md).

## By Selectors

```csharp
By.TestId("my-element")                                          // preferred — most stable
By.TagName("button").And(By.ClassName("primary"))                 // combine with .And()
By.TagName("input").And(By.Id("username")).And(By.ClassName("x")) // order: tag → ID → class → attr → pseudo
```

- Prefer `By.TestId()` for stability
- Use `.And()` to combine — never spaces or commas
- One tag and one ID max per combined selector; multiple classes allowed

## Test Example

```csharp
[TestMethod]
public async Task DemonstrateFeature_Headless()
{
    var tab = new MyAppTab(headless: true);
    try
    {
        await tab.Open();
        var settingsPage = tab.Goto<ISettings>();
        await settingsPage.EnableNotifications.Check();
        (await settingsPage.EnableNotifications.IsChecked).Should().BeTrue();
        tab.Goto<IPreferences>();
    }
    finally
    {
        await tab.Close();
    }
}
```

## Test Rules

- Always use `try/finally` to ensure `tab.Close()` is called
- Use interface types for page references (`ISettings` not `Settings`)
- Test code must only interact with PageObjects and ControlObjects — never use `ILocator` or `IPage` directly in test methods
- Don't create UI object instances with `new` — use `Find<T>()`
- Use FluentAssertions (`.Should().BeTrue()`)
- Add `await Task.Delay()` only in headed tests for visualization

## Anti-Patterns

❌ Using concrete types in tests — use interfaces
❌ Accessing `ILocator` or `IPage` directly in tests — interact through PageObject/ControlObject APIs
❌ Creating UI objects with `new` — use `Find<T>()` or `FindAll<T>()`
❌ Hardcoding page relationships — use `ChildOf<,>()` in TabObject
❌ Forgetting `tab.Close()` — always wrap in `try/finally`
❌ Exposing `ILocator` in public properties of PageObjects/ControlObjects — wrap as typed controls via `Find<T>()`
❌ Using `Task.Delay()` in tests — use dynamic waits (see below)

## Dynamic Waits (Avoiding Fixed Delays)

Never use `Task.Delay()` in headless tests. Replace every fixed sleep with a wait for the specific condition the test needs next.

### Wait Strategy by Scenario

| Scenario | Wait approach |
|---|---|
| Data loads after navigation | `await page.MyTable.WaitForVisibleAsync(timeout)` |
| Element appears after click | `await locator.WaitForAsync(new() { Timeout = ... })` |
| Attribute/state change | `await Assertions.Expect(locator).ToHaveAttributeAsync(...)` |

Define timeouts once; reference everywhere.

## Debugging

- `WriteTree()` — visualize page object hierarchy
- `.HighlightAsync()` — visually identify elements
- `headless: false` — watch interactions live
- `await Task.Delay()` — slow down headed tests for observation
- Playwright traces — detailed execution logs

## Further Reading

- [README.md](README.md) — Quick start and overview
- [PATTERN.md](PATTERN.md) — Coparoo pattern theory (DOM structure, search optimization)
- [DECOUPLING.md](DECOUPLING.md) — Interface-based decoupling for cooperative projects
- [DEMO.md](DEMO.md) — Step-by-step code walkthrough
- [Demo Project](Trumpf.Coparoo.Playwright.Demo/README.md) — Full working demo
