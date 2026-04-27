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

## Control Access Rules

PageObjects and ControlObjects expose child controls as **properties** — never as wrapper methods. Tests interact with controls directly.

```csharp
// ✅ Good — control exposed as property, test interacts directly
public Span UserName => Find<Span>(By.TestId("user-name"));
// Test: var name = await page.UserName.TextContentAsync();

// ❌ Bad — deflated method hides the control, duplicates API
public async Task<string> GetUserNameTextAsync()
    => await Locator.Locator("[data-testid='user-name']").TextContentAsync();
```

When multiple controls form a logical unit, group them in a **composite ControlObject**:

```csharp
// ✅ Good — composite control groups related elements
public sealed class CurrentUserControlObject : ControlObject
{
    protected override By SearchPattern => By.TestId("current-user");
    public Span DisplayName => Find<Span>(By.TestId("user-display-name"));
    public Span Initials => Find<Span>(By.TestId("user-initials"));
}

// PageObject exposes the composite
public CurrentUserControlObject CurrentUser
    => Find<CurrentUserControlObject>(By.TestId("current-user"));

// Test reads naturally:
var initials = await header.CurrentUser.Initials.TextContentAsync();
```

**Rule:** If you find yourself writing `GetXyzTextAsync()`, `IsXyzVisibleAsync()`, or similar on a PageObject or ControlObject — stop. Expose the control as a property and let the caller use extension methods. When generating tests against existing page models that contain deflated methods, refactor the page model first.

### Concrete vs Interface in `Find<T>()`

Use `Find<ConcreteType>()` when the concrete type lives in the **same assembly** as the caller. Use `Find<IInterface>()` only when the implementation lives in a **different assembly** and you need decoupling (see [Interface-Based Decoupling](#interface-based-decoupling-advanced)).

```csharp
// ✅ Good — concrete type argument; return type can be the interface
public IBody Content => Find<Body>();

// ✅ Also good — concrete return type works too
public Body Content => Find<Body>();

// ✅ Good — test uses interface because implementation is in a separate assembly
var settings = tab.Goto<ISettings>();

// ❌ Bad — interface as the generic argument for a type in the same assembly
// This triggers assembly scanning; if the assembly isn't loaded yet, resolution fails
public IBody Content => Find<IBody>();
```

**Why:** `Find<IInterface>()` relies on runtime assembly scanning (`AppDomain.CurrentDomain.GetAssemblies()`) to discover which concrete class implements the interface. If the assembly containing the implementation hasn't been loaded by the CLR yet (due to lazy loading), the resolver won't find it. `Find<ConcreteType>()` skips scanning entirely — the CLR loads the type directly. The return type doesn't matter — only the **generic argument** to `Find<T>()` determines whether scanning occurs.

## Naming Conventions

Class names must end with their tier suffix: `TabObject`, `PageObject`, `ControlObject`.

| Tier | Class example | Interface example |
|---|---|---|
| Tab | `MyAppTabObject` | `IMyAppTabObject` |
| Page | `SettingsPageObject` | `ISettingsPageObject` |
| Control | `TypeAheadControlObject` | `ITypeAheadControlObject` |

### Variable Naming

Use **full, descriptive variable names** — never abbreviate page object or control object variables. Abbreviated names hurt readability and make tests harder to understand at a glance.

```csharp
// ✅ Good — full names
var settings = await tab.Goto<SettingsPageObject>();
var dashboard = await tab.Goto<DashboardPageObject>();
var userProfile = await tab.Goto<UserProfilePageObject>();

// ❌ Bad — abbreviated names
var sett = await tab.Goto<SettingsPageObject>();
var dbPage = await tab.Goto<DashboardPageObject>();
var upPage = await tab.Goto<UserProfilePageObject>();
```

## XML Documentation

Every PageObject, ControlObject, and TabObject class **must** have complete XML documentation. This applies to the class itself and all its public members.

### Required Documentation

| Element | Required |
|---|---|
| Class declaration | `<summary>` describing what UI area or widget the class represents |
| Public control properties (`Find<T>(...)`) | `<summary>` describing what the control is for |
| Public methods | `<summary>` describing what the method does |
| `Goto()` overrides | `<summary>` describing the navigation steps performed |

### Examples

```csharp
/// <summary>
/// Represents the application header bar containing navigation links,
/// workspace selector, theme toggle, and the current user display.
/// </summary>
public sealed class HeaderNavigationPageObject : PageObject
{
    protected override By SearchPattern => By.TestId("header-nav");

    /// <summary>
    /// Logo link that navigates to the home page when clicked.
    /// </summary>
    public Link LogoLink => Find<Link>(By.TestId("logo-link"));

    /// <summary>
    /// Button that opens the workspace selector dropdown.
    /// </summary>
    public Button WorkspaceButton => Find<Button>(By.TestId("workspace-button"));

    /// <summary>
    /// Navigates to the header by scrolling to the top of the page.
    /// </summary>
    public override async Task Goto() { /* ... */ }
}
```

```csharp
/// <summary>
/// Reusable dropdown selector control with search-and-filter capability.
/// Used across configuration and filter panels.
/// </summary>
public sealed class TypeAheadControlObject : ControlObject
{
    protected override By SearchPattern => By.TestId("typeahead");

    /// <summary>
    /// Text input field where the user types to filter options.
    /// </summary>
    public TextInput SearchInput => Find<TextInput>(By.TestId("typeahead-input"));
}
```

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

### When to use `Goto` vs `On`

| Scenario | Method | Why |
|---|---|---|
| Test **starts** and must reach a page | `Goto<T>()` | You need the page's navigation logic to run |
| A **click or action** already triggered navigation | `On<T>()` | The page is appearing; calling `Goto` would redundantly re-navigate |
| Accessing a **second page** that's already visible | `On<T>()` | No navigation needed — the page is on screen |
| Verifying a page **appeared** after an action | `On<T>()` + `WaitForVisibleAsync()` | Wait for the result, don't cause it |

**Rule of thumb:** `Goto` = *make it happen*. `On` = *it already happened, give me a reference*.

```csharp
// ✅ Good — Goto at test entry point, need navigation
await tab.Open();
var dashboard = await tab.Goto<DashboardPageObject>();

// ✅ Good — On after a click that already triggered navigation
await dashboard.SettingsLink.ClickAsync();
var settings = tab.On<SettingsPageObject>();
await settings.WaitForVisibleAsync();

// ❌ Bad — Goto after a click redundantly navigates
await dashboard.SettingsLink.ClickAsync();
var settings = await tab.Goto<SettingsPageObject>(); // DON'T — click already did it
```

## Project Organization

Define your TabObjects, PageObjects, and ControlObjects in a **separate project** from your test project. This allows them to be published as a NuGet package and consumed by multiple test suites — e.g., UI tests living alongside the UI code, integration tests in a different repository, or E2E tests maintained by a separate team.

```
MyApp.PageObjects/          # Tab, Page, Control definitions → publish as NuGet
MyApp.UITests/              # UI tests → references MyApp.PageObjects
MyApp.IntegrationTests/     # Integration tests → also references MyApp.PageObjects
```

### Domain-Aligned Subfolders

As page model projects grow, organize PageObjects and ControlObjects into domain-aligned subfolders. Keep namespaces flat — subfolder structure is purely for discoverability:

```
PageObjects/
├── Shell/           # MainShell, HeaderNavigation, ChatWindow
├── Dashboard/       # IndexPage
└── Landing/         # AboutPage

ControlObjects/
├── Shell/           # MainFooter, SettingsDropdown
└── Configuration/   # ConfigActionBar, ModuleSelector, RevisionInfo
```

## TestId Contract Project (Decoupling Page Objects from Productive Code)

Page objects should **never** reference the productive UI project directly. Instead, extract all `data-testid` constants into a dedicated **contract project** that both the productive UI and the page objects reference. This ensures:

- Page objects depend only on string constants, not on UI controllers, Blazor components, or any runtime type.
- The productive UI and tests can evolve independently — renaming a controller class never breaks tests.
- The contract project has **zero dependencies**, keeping it lightweight and fast to compile.

```
MyApp.UI.TestIds/           # Contract: data-testid string constants only (no dependencies)
MyApp.UI.Blazor/            # Productive UI → references TestIds
MyApp.PageObjects/          # Page objects → references TestIds (NOT Blazor)
MyApp.UITests/              # Tests → references PageObjects
```

### Defining Constants

Group constants by page or component in flat static classes:

```csharp
namespace MyApp.UI.TestIds;

public static class LoginPageTestIds
{
    public const string PAGE = "login-page";
    public const string USERNAME_INPUT = "login-username";
    public const string PASSWORD_INPUT = "login-password";
    public const string SUBMIT_BUTTON = "login-submit";
}
```

### Using in Productive UI (e.g., in Blazor)

Reference the contract constants directly in `.razor` files via a global `@using` in `_Imports.razor` — no intermediate inner class needed:

```razor
@* _Imports.razor *@
@using MyApp.UI.TestIds

@* LoginPage.razor — reference constants directly *@
<form data-testid="@LoginPageTestIds.PAGE">
    <input data-testid="@LoginPageTestIds.USERNAME_INPUT" />
</form>
```

### No Magic Strings in Tests

Every `data-testid` value that originates from the application must be defined in the contract project — including values used in CSS attribute selectors within test code. Generic HTML/Bootstrap selectors (tag names, CSS classes) may remain as inline strings.

```csharp
// ✅ Good — test IDs from constants
page.Locator($"[data-testid^='{AlarmClusterTestIds.CARD}']");
page.Locator($"[data-testid='{TicketOverviewTestIds.TABLE}'] tbody tr");
await Assertions.Expect(card).ToHaveAttributeAsync("data-testid", AlarmClusterTestIds.CARD_SELECTED);

// ❌ Bad — hardcoded test IDs in tests
page.Locator("[data-testid^='alarm-cluster-card']");
page.Locator("[data-testid='ticket-table'] tbody tr");

// ✅ OK — generic CSS selectors can remain inline
card.Locator(".badge").First;
page.Locator(".alarms-split-right tr");
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
    await using var session = await TestSession.CreateAsync();

    var tab = session.CreateTab();
    await tab.Open();
    var settingsPage = tab.Goto<ISettings>();
    await settingsPage.EnableNotifications.Check();
    (await settingsPage.EnableNotifications.IsChecked).Should().BeTrue();
    tab.Goto<IPreferences>();
    // tab is closed automatically when the session is disposed
}
```

## Writing Maintainable Tests

Tests break when UI implementation changes. The key to maintainability is ensuring every UI interaction goes through the page model so that when the UI changes, you fix **one method** — not dozens of tests.

### Rule: No Raw `IPage` or `ILocator` in Tests or Scenarios

Test methods and reusable scenario classes must never access `tab.Page`, `page.Locator(...)`, `page.GetByTestId(...)`, or any raw Playwright API. All interactions flow through PageObject/ControlObject methods.

```csharp
// ❌ Bad — raw IPage access in a test/scenario
var page = await tab.Page;
await page.ReloadAsync(new PageReloadOptions { ... });
await page.Locator("[data-testid='remove-user-123']").ClickAsync();
var dialog = page.GetByTestId("confirm-dialog");
await dialog.Locator("button:has-text('Remove')").ClickAsync();

// ✅ Good — page model encapsulates the flow
await userList.ReloadAndWaitForItemsAsync();
await userList.RemoveItemAsync(itemId);
```

### Rule: Encapsulate Multi-Step UI Flows in Page Model Methods

When a test performs multiple sequential Playwright interactions to achieve one logical action (e.g. fill form → click create → read modal → dismiss), wrap the entire flow in a single page model method.

```csharp
// ❌ Bad — multi-step flow duplicated across tests
await nameInput.FillAsync(patName);
await firstCheckbox.ClickAsync();
await createButton.ClickAsync();
var modal = page.GetByTestId("token-modal");
await modal.WaitForAsync(...);
var token = await page.GetByTestId("token-value").TextContentAsync();
await page.GetByTestId("close-modal").ClickAsync();

// ✅ Good — one method, one responsibility
await createForm.FillNameAsync(itemName);
await createForm.SelectFirstOptionAsync();
var result = await createForm.SubmitAndReadResultAsync();
```

### Rule: State Queries Belong on the Page Model

Boolean checks like "is this button visible?", "is this element enabled?", or "does this row exist?" belong on the page object — not inline in tests.

```csharp
// ❌ Bad — raw locator count check in a test
(await page.Locator("[data-testid='remove-user-123']").CountAsync()).Should().Be(0);

// ✅ Good — semantic method on the page object
(await itemList.IsRemoveButtonVisibleAsync(itemId)).Should().BeFalse();
```

### When to Add a Method vs Use Existing Controls

- **Add a method** when the interaction involves multiple steps, conditional logic, or waits that would be duplicated across tests.
- **Use existing controls** when a single control property + extension method is sufficient (e.g. `await page.SaveButton.ClickAsync()`).

## Tab Lifecycle — Auto-Cleanup over try/finally

Manual `try/finally` blocks for `tab.Close()` are error-prone and create massive boilerplate. Instead, have your test fixture or session track created tabs and close them automatically via `IAsyncDisposable`.

```csharp
// ❌ Bad — try/finally in every test
var tab = CreateTab();
try
{
    await tab.Open();
    // ... test body ...
}
finally
{
    try { await tab.Close(); } catch { }
}

// ✅ Good — session tracks tabs and closes them on dispose
await using var session = await TestSession.CreateAsync();
var tab = session.CreateTab();   // tracked internally
await tab.Open();
// ... test body ...
// tab.Close() called automatically in session.DisposeAsync()
```

Implement this by having your session/fixture maintain a list of created tabs and close them all in `DisposeAsync()`:

```csharp
public sealed class TestSession : IAsyncDisposable
{
    private readonly List<TabObject> _trackedTabs = new();

    public MyTabObject CreateTab()
    {
        var tab = new MyTabObject(/* ... */);
        _trackedTabs.Add(tab);
        return tab;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var tab in _trackedTabs)
        {
            try { await tab.Close(); } catch { }
        }
        _trackedTabs.Clear();
    }
}
```

## Test Rules

- Use `await using` on your test session/fixture so tabs are closed automatically
- Use interface types for page references (`ISettings` not `Settings`)
- Test code must only interact with PageObjects and ControlObjects — never use `ILocator` or `IPage` directly in test methods
- Don't create UI object instances with `new` — use `Find<T>()`
- Use FluentAssertions (`.Should().BeTrue()`)
- Add `await Task.Delay()` only in headed tests for visualization

## Structuring Page Objects by Visual Region

When a PageObject accumulates many flat properties spanning different visual regions of the UI (e.g., a header with a nav bar, project picker, and user area), it becomes hard to navigate and no longer mirrors the layout. Decompose it into ControlObjects that match what the user sees.

**Signal to refactor:** A PageObject has 15+ flat control properties, and they belong to visually distinct regions.

```csharp
// ❌ Bad — flat list mixes unrelated UI regions
public sealed class HeaderPageObject : PageObject
{
    public Link LogoLink => Find<Link>(By.TestId("logo"));
    public Button ThemeToggle => Find<Button>(By.TestId("theme-toggle"));
    public Button ProjectButton => Find<Button>(By.TestId("project-btn"));
    public TextInput ProjectSearch => Find<TextInput>(By.TestId("project-search"));
    public Link HomeLink => Find<Link>(By.TestId("home-link"));
    public Link ReportsLink => Find<Link>(By.TestId("reports-link"));
    public Span UserInitials => Find<Span>(By.TestId("user-initials"));
    // ... 20 more properties spanning 4 different UI regions
}

// ✅ Good — grouped by visual region
public sealed class HeaderPageObject : PageObject
{
    // Top bar (always visible)
    public Link LogoLink => Find<Link>(By.TestId("logo"));
    public Button ThemeToggle => Find<Button>(By.TestId("theme-toggle"));

    // Sub-regions as ControlObjects
    public ProjectPickerControlObject ProjectPicker
        => Find<ProjectPickerControlObject>(By.TestId("project-picker"));
    public NavBarControlObject NavBar
        => Find<NavBarControlObject>(By.TestId("nav-bar"));

    // Current user
    public Span UserInitials => Find<Span>(By.TestId("user-initials"));
}

// Tests read like the visual layout:
await header.ProjectPicker.SelectProjectAsync("production");
await header.NavBar.ReportsLink.ClickAsync();
var initials = await header.UserInitials.TextContentAsync();
```

**Guidelines:**
- Each ControlObject wraps a visually distinct region with its own `SearchPattern`
- A developer looking at the UI should intuitively know which sub-object to use
- Convenience methods on the parent (e.g., `SelectProjectAsync`) can delegate to the sub-object for common flows
- Don't over-decompose: if all properties belong to a single visual region, a flat PageObject is fine

## Anti-Patterns

❌ Using concrete types in tests — use interfaces
❌ Accessing `ILocator` or `IPage` directly in tests — interact through PageObject/ControlObject APIs
❌ Creating UI objects with `new` — use `Find<T>()` or `FindAll<T>()`
❌ Hardcoding page relationships — use `ChildOf<,>()` in TabObject
❌ Referencing the productive UI project from page objects — use a TestId contract project (see above)
❌ Hardcoding `data-testid` strings in page objects or tests — use constants from the contract project
❌ Writing `try/finally { tab.Close() }` in every test — use auto-cleanup via `IAsyncDisposable` session/fixture
❌ Flat PageObjects with 15+ properties spanning multiple visual regions — decompose into ControlObjects by region
❌ Exposing `ILocator` in public properties of PageObjects/ControlObjects — wrap as typed controls via `Find<T>()`
❌ Putting app-specific TestIds or CSS in a shared controls library — keep those in the page model project
❌ Using `Task.Delay()` in tests — use dynamic waits (see below)
❌ Duplicating multi-step UI flows across tests/scenarios — extract to a page model method so UI changes require fixing one place
❌ Mirroring TestId constants via inner `TestIds` classes in controllers — reference contract constants directly in `.razor` files
❌ Interpolating untrusted values into `:has-text()` selectors — use `Locator.Filter()` instead
❌ Deflating control interactions onto PageObjects (e.g., `GetUserNameTextAsync()`, `GetUserInitialsTextAsync()`) — expose a composite ControlObject with child controls instead, so tests read `page.CurrentUser.Initials.TextContentAsync()`
❌ Abbreviating variable names (e.g., `settMgr`, `dbPage`, `upPage`) — use full descriptive names (`settings`, `dashboard`)
❌ Omitting XML documentation on PageObject/ControlObject classes, properties, or methods — every public member must have a `<summary>`
❌ Using control-specific `Text` members — use the universal `TextContentAsync()` / `InnerTextAsync()` extension methods available on all `IUIObject`

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
