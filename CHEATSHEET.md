# Coparoo Test Writing Cheat Sheet

Concise reference for writing tests with the Trumpf.Coparoo.Playwright library.

> **AI-ready by design:** Coparoo's rigid hierarchy, interface-first approach, and typed controls make tests structurally easy for AI agents to generate correctly and hard to generate incorrectly. Point your AI agent at this cheat sheet — it will produce robust, maintainable tests.

## How the rules are labelled

Each rule carries a stable ID `COPA-<TOPIC>-NNN` (**COPA** = Coparoo; topic = `HIR` / `NAV` / `CTL` / `NAM` / `SEL` / `TST`) and a kebab-case slug used as its review alias. Every rule follows the **same shape**: a `` `ID` · `slug` `` heading, a one-line statement, then a ❌/✅ example. IDs are immutable once assigned — retire, never renumber. Suppress a rule with a `// COPA-<TOPIC>-NNN: justified — <reason>` comment. A complete index is in the [Rule registry](#rule-registry).

## Three-Tier Hierarchy

1. **TabObject** — browser tab / application root
2. **PageObject** — unique UI area (page, panel, dialog)
3. **ControlObject** — reusable widget (button, checkbox, input)

---

## Hierarchy & Types

### `COPA-HIR-001` · `page-vs-control`

Model a *unique* composite UI area (page, panel, dialog, sidebar) as a **PageObject**; model a *reusable* single widget (button, checkbox, input, table row) as a **ControlObject**.

| Aspect | PageObject | ControlObject |
|---|---|---|
| **Purpose** | Unique composite UI area | Reusable single widget |
| **Navigation** | Supports `Goto<T>()` (overridable `Goto()`) | Never navigated to — accessed via `Find<T>()` |
| **Tree position** | Interior node — contains child pages/controls | Leaf node — declared via `Find<T>()` / `FindAll<T>()` |
| **Reuse** | Unique to one part of the app | Reused across many pages |

```csharp
// ✅ PageObject — appears once (settings page, nav sidebar, login dialog)
public sealed class SettingsPageObject : PageObject { /* ... */ }

// ✅ ControlObject — appears many times (button, checkbox, input, row)
public sealed class SaveButtonControlObject : ControlObject { /* ... */ }
```

Rule of thumb: if it needs a `Goto()` to appear or contains child controls, it's a **PageObject**; if it's a self-contained widget accessed from a parent, it's a **ControlObject**.

### `COPA-HIR-002` · `no-new-ui-objects`

Never instantiate a UI object with `new` — resolve it via `Find<T>()` / `FindAll<T>()` so Coparoo wires the parent/child relationship and search scope.

```csharp
// ❌ Bad — bypasses the hierarchy and search scoping
var save = new SaveButtonControlObject();

// ✅ Good — resolved through the parent
public Button SaveButton => Find<Button>(By.TestId("save-button"));
```

Use `Find<ConcreteType>()` when the type lives in the **same assembly**; use `Find<IInterface>()` only for cross-assembly decoupling. An interface argument relies on runtime assembly scanning — if the implementation assembly isn't loaded yet, resolution fails; a concrete argument skips scanning. Only the **generic argument** (not the return type) decides whether scanning occurs.

```csharp
// ✅ Good — concrete generic argument; return type may be the interface
public IBody Content => Find<Body>();

// ❌ Bad — interface argument for a same-assembly type triggers scanning
public IBody Content => Find<IBody>();
```

### `COPA-HIR-003` · `childof-not-hardcoded`

Register page relationships with `ChildOf<TChild, TParent>()` in the TabObject constructor — never hardcode a page hierarchy. This lets teams add pages independently.

```csharp
// ✅ Good — relationships registered dynamically
public MyAppTabObject()
{
    ChildOf<SettingsPageObject, ShellPageObject>();
    ChildOf<PreferencesPageObject, ShellPageObject>();
}
```

---

## Navigation

### `COPA-NAV-001` · `goto-vs-on`

Reach a page with `tab.Goto<T>()` (runs its navigation logic); get an already-visible page with `tab.On<T>()`. Never `Goto` a page a click already opened.

| Scenario | Method |
|---|---|
| Test starts and must reach a page | `Goto<T>()` |
| A click/action already triggered navigation | `On<T>()` |
| Accessing a second page already on screen | `On<T>()` |
| Verifying a page appeared after an action | `On<T>()` + `Visible().WaitForAsync()` |

```csharp
// ✅ Good — Goto at the entry point; On after a click already navigated
await tab.Open();
var dashboard = await tab.Goto<DashboardPageObject>();
await dashboard.SettingsLink.ClickAsync();
var settings = tab.On<SettingsPageObject>();
await settings.Visible().WaitForAsync();

// ❌ Bad — Goto after a click redundantly re-navigates
await dashboard.SettingsLink.ClickAsync();
var settings = await tab.Goto<SettingsPageObject>();
```

`Goto` = *make it happen*; `On` = *it already happened, give me a reference*.

### `COPA-NAV-002` · `real-ui-goto`

Implement `Goto()` by clicking the **real** navigation control a user would use — never reconstruct and navigate to a hand-built URL. The app's own link carries a correct `href` for the current base path, tenant/context segment, query state, and auth redirects; a hand-built URL drops all of that and hides genuinely broken navigation.

```csharp
// ❌ Bad — reconstructs a URL, bypassing base path, tenant, and auth routing
public override async Task Goto()
{
    var page = await Root().Page;
    var origin = new Uri(page.Url);
    await page.GotoAsync($"{origin.Scheme}://{origin.Authority}/settings");
    await this.WaitForNavigationAsync();
}

// ✅ Good — clicks the real menu link the user would use
public override async Task Goto()
{
    var menu = await this.Goto<UserMenuPageObject>();
    await menu.SettingsLink.WaitForAsync();
    await menu.SettingsLink.ClickAsync();
    await this.WaitForNavigationAsync();
}
```

### `COPA-NAV-003` · `url-only-entry-points`

Direct URL navigation is legitimate **only** when the URL *is* the user action under test: the app entry point (`tab.Open()` / `TabObject.Url`), deep-link/bookmark routes, and tests deliberately asserting redirect or query-parameter behaviour. Everywhere else, click the control (`COPA-NAV-002`).

```csharp
// ✅ Good — the redirect / query param IS the subject under test
await page.GotoAsync(session.BuildUrl("admin/globalpolicies", tenant: null)); // asserts access control
await page.GotoAsync(currentUrl + "?FullScreen=true");                        // asserts full-screen mode
```

### `COPA-NAV-004` · `no-navigate-to-control`

Never navigate to a ControlObject — access it as a property of its parent page.

```csharp
// ❌ Bad — controls are not navigable
var save = tab.Goto<SaveButtonControlObject>();

// ✅ Good — reach the page, then use its control
var settings = await tab.Goto<SettingsPageObject>();
await settings.SaveButton.ClickAsync();
```

---

## Control Access

### `COPA-CTL-001` · `controls-as-properties`

Expose child controls as **properties** (not wrapper methods) so tests interact with them directly and read like the UI. Group related controls into a composite ControlObject rather than deflating them into `GetXxx` / `IsXxx` methods.

```csharp
// ❌ Bad — deflated method hides the control and duplicates its API
public async Task<string> GetUserNameTextAsync()
    => await Locator.Locator("[data-testid='user-name']").TextContentAsync();

// ✅ Good — control exposed as a property; caller uses extension methods
public Span UserName => Find<Span>(By.TestId("user-name"));
// Test: var name = await page.UserName.TextContentAsync();

// ✅ Good — composite groups related elements
public sealed class CurrentUserControlObject : ControlObject
{
    protected override By SearchPattern => By.TestId("current-user");
    public Span DisplayName => Find<Span>(By.TestId("user-display-name"));
    public Span Initials => Find<Span>(By.TestId("user-initials"));
}
// Test: var initials = await header.CurrentUser.Initials.TextContentAsync();
```

Prefer the universal `TextContentAsync()` / `InnerTextAsync()` extension methods (available on every `IUIObject`) over control-specific `Text` members.

### `COPA-CTL-002` · `no-raw-locator-exposed`

Never expose `ILocator` or `IPage` from a PageObject/ControlObject — wrap child elements as typed controls via `Find<T>()`.

```csharp
// ❌ Bad — leaks a raw locator
public ILocator SaveButton => Locator.Locator("[data-testid='save']");

// ✅ Good — typed control
public Button SaveButton => Find<Button>(By.TestId("save-button"));
```

### `COPA-CTL-003` · `encapsulate-flows`

Encapsulate multi-step UI flows and state queries as page-object methods — never inline raw locators or multi-step sequences in tests. When the UI changes you then fix one method, not dozens of tests.

```csharp
// ❌ Bad — multi-step flow + state check inlined in the test
await nameInput.FillAsync(patName);
await createButton.ClickAsync();
var token = await page.GetByTestId("token-value").TextContentAsync();
(await page.Locator("[data-testid='remove-user-123']").CountAsync()).Should().Be(0);

// ✅ Good — semantic methods on the page model
var result = await createForm.SubmitAndReadResultAsync();
(await itemList.IsRemoveButtonVisibleAsync(itemId)).Should().BeFalse();
```

Add a method when the interaction has multiple steps, conditional logic, or waits that would be duplicated; use an existing control property when a single extension-method call suffices (`await page.SaveButton.ClickAsync()`).

### `COPA-CTL-004` · `collections-as-findall`

Expose a set of repeated, homogeneous elements as a **collection** via `FindAll<T>()` and let the caller count or enumerate it (`await page.Rows.CountAsync()`). Don't deflate a collection into a bare `GetXxxCountAsync()` that returns an `int` — that hides the control and duplicates its API (`COPA-CTL-001`). A thin wrapper is warranted only when the count needs a **wait** (e.g. the list renders empty for a frame during a re-render); keep it race-safe and name it for the wait, not the number.

```csharp
// ❌ Bad — collection deflated into a bare count method
public async Task<int> GetPolicyCheckboxCountAsync()
    => await Locator.Locator("input[type='checkbox']").CountAsync();
// Test: (await page.GetPolicyCheckboxCountAsync()).Should().BeGreaterThan(0);

// ✅ Good — collection exposed as a property; caller counts directly
public IAsyncEnumerable<Checkbox> PolicyCheckboxes => FindAll<Checkbox>();
// Test: (await page.PolicyCheckboxes.CountAsync()).Should().BeGreaterThan(0);

// ✅ Good — thin wait wrapper when the list is empty only until its data first loads
public async Task WaitForRowsAsync()
    => await Assertions.Expect(RowContainer.Locator.Locator("tr"))
        .Not.ToHaveCountAsync(0, new() { Timeout = DefaultTimeout });
```

The built-in `Table` control already models this: `table.Rows` is an `IAsyncEnumerable<IRow>`, so `await table.Rows.CountAsync()` reads like the UI — reach for that shape before writing a `GetRowCountAsync()`.

Reaching into `.Locator` for the wait is fine — an auto-retrying count assertion needs a Playwright locator, which `FindAll<T>()` (an `IAsyncEnumerable`) can't supply, and `COPA-CTL-002` only bans *exposing* raw locators, not using them internally. The wait above is race-safe **only** for a list that stays populated once it appears. If the list can *re-clear* after first rendering — e.g. a page that self-navigates on load — the assertion can pass and then go stale; capture the count atomically inside the browser (`Page.WaitForFunctionAsync`) instead, and mark it with a `// COPA-CTL-004: justified — …` comment.

---

## Naming & Documentation

### `COPA-NAM-001` · `descriptive-names`

Suffix classes with their tier (`…TabObject`, `…PageObject`, `…ControlObject`) and use full, descriptive variable names — never abbreviate.

| Tier | Class | Interface |
|---|---|---|
| Tab | `MyAppTabObject` | `IMyAppTabObject` |
| Page | `SettingsPageObject` | `ISettingsPageObject` |
| Control | `TypeAheadControlObject` | `ITypeAheadControlObject` |

```csharp
// ✅ Good — full names
var settings = await tab.Goto<SettingsPageObject>();
var dashboard = await tab.Goto<DashboardPageObject>();

// ❌ Bad — abbreviated names
var sett = await tab.Goto<SettingsPageObject>();
var dbPage = await tab.Goto<DashboardPageObject>();
```

### `COPA-NAM-002` · `xml-doc-public`

Give every public PageObject/ControlObject/TabObject class, control property, method, and `Goto()` override an XML `<summary>`.

```csharp
/// <summary>
/// Represents the application header bar with navigation links and the current user display.
/// </summary>
public sealed class HeaderNavigationPageObject : PageObject
{
    protected override By SearchPattern => By.TestId("header-nav");

    /// <summary>Logo link that navigates to the home page when clicked.</summary>
    public Link LogoLink => Find<Link>(By.TestId("logo-link"));

    /// <summary>Navigates to the header by scrolling to the top of the page.</summary>
    public override async Task Goto() { /* ... */ }
}
```

---

## Selectors & TestIds

### `COPA-SEL-001` · `prefer-testid`

Prefer `By.TestId(...)`; combine selectors with `.And()` — never spaces or commas. One tag and one ID max per combined selector; multiple classes are allowed.

```csharp
By.TestId("my-element")                                          // preferred — most stable
By.TagName("button").And(By.ClassName("primary"))                 // combine with .And()
By.TagName("input").And(By.Id("username")).And(By.ClassName("x")) // order: tag → ID → class → attr → pseudo
```

### `COPA-SEL-002` · `testid-contract`

Keep every `data-testid` value in a dedicated **contract project** referenced by both the productive UI and the page objects — no magic strings in page objects, tests, or `.razor` files. Page objects must never reference the productive UI project.

```
MyApp.UI.TestIds/   # contract: data-testid constants only (zero dependencies)
MyApp.UI.Blazor/    # productive UI → references TestIds
MyApp.PageObjects/  # page objects → references TestIds (NOT Blazor)
MyApp.UITests/      # tests → references PageObjects
```

```csharp
// contract
public static class TicketOverviewTestIds
{
    public const string TABLE = "ticket-table";
}

// ✅ Good — constants everywhere (Blazor + page objects + tests)
page.Locator($"[data-testid='{TicketOverviewTestIds.TABLE}'] tbody tr");

// ❌ Bad — hardcoded app test IDs
page.Locator("[data-testid='ticket-table'] tbody tr");

// ✅ OK — generic HTML/CSS selectors may stay inline
card.Locator(".badge").First;
```

Reference contract constants directly in `.razor` via a global `@using` — don't mirror them through inner `TestIds` classes in controllers.

### `COPA-SEL-003` · `no-interpolated-has-text`

Never interpolate untrusted values into a `:has-text()` selector — use `Locator.Filter()` instead.

```csharp
// ❌ Bad — interpolates a value into :has-text()
page.Locator($"tr:has-text('{userInput}')");

// ✅ Good — Filter() handles the value safely
page.Locator("tr").Filter(new() { HasText = userInput });
```

---

## Test Hygiene

### `COPA-TST-001` · `only-page-objects`

Test methods and reusable scenarios interact **only** through PageObjects/ControlObjects — never `tab.Page`, `page.Locator(...)`, `page.GetByTestId(...)`, or any raw Playwright API.

```csharp
// ❌ Bad — raw IPage / ILocator in a test
var page = await tab.Page;
await page.Locator("[data-testid='remove-user-123']").ClickAsync();

// ✅ Good — flows through the page model
await userList.RemoveItemAsync(itemId);
```

### `COPA-TST-002` · `interface-page-refs`

Use interface types for page references (`ISettings`, not `Settings`) so tests survive locator changes and dynamically loaded (cross-assembly) implementations.

```csharp
// ✅ Good — interface reference, resolved at runtime
var settings = tab.Goto<ISettings>();
await settings.EnableNotifications.Check();
```

See [DECOUPLING.md](DECOUPLING.md) for the full interface-based decoupling pattern.

### `COPA-TST-003` · `auto-tab-cleanup`

Close tabs via an `IAsyncDisposable` session/fixture (`await using`) that tracks created tabs — never `try/finally { tab.Close() }` in every test.

```csharp
// ❌ Bad — try/finally in every test
var tab = CreateTab();
try { await tab.Open(); /* ... */ }
finally { try { await tab.Close(); } catch { } }

// ✅ Good — session closes tracked tabs on dispose
await using var session = await TestSession.CreateAsync();
var tab = session.CreateTab();
await tab.Open();
// tab.Close() runs automatically in session.DisposeAsync()
```

### `COPA-TST-004` · `dynamic-waits`

Replace every fixed `Task.Delay()` with a wait for the specific condition the test needs next. `Task.Delay()` is allowed only to slow down **headed** runs for observation.

| Scenario | Wait approach |
|---|---|
| Data loads after navigation | `await page.MyTable.Visible().WaitForAsync(timeout)` |
| Element appears after click | `await locator.WaitForAsync(new() { Timeout = ... })` |
| Attribute / state change | `await Assertions.Expect(locator).ToHaveAttributeAsync(...)` |

Define timeouts once; reference everywhere.

### `COPA-TST-005` · `web-first-state-waits`

After an action whose effect lands asynchronously (e.g. a Blazor Server SignalR round-trip), assert the resulting element state with a web-first state handle (`ctrl.Enabled().WaitForAsync()`) — never a one-shot `IsXxxAsync()` snapshot, which samples the pre-update DOM and flakes under latency (passes locally at ~1 ms, fails remotely at ~100 ms). One-shot `IsXxxAsync()` stays valid for already-settled or negative reads.

```csharp
// ❌ Bad — snapshot races the async enable
await form.ThresholdSlider.SetAsync("70%");
(await form.SaveButton.IsEnabledAsync()).Should().BeTrue();

// ✅ Good — web-first wait retries until the state lands
await form.ThresholdSlider.SetAsync("70%");
await form.SaveButton.Enabled().WaitForAsync();
```

State handle: `ctrl.Enabled()` / `.Checked()` / `.Editable()` / `.Visible()` / `.Attached()`, each exposing `.WaitForAsync()` (state holds) and `.WaitForNotAsync()` (state clears — disabled / unchecked / read-only / hidden / detached). One-shot reads use the `IsXxxAsync()` query extensions. Pass an explicit timeout for known-slow (remote) waits.

---

## Reference

### Organizing the page model

Define TabObjects, PageObjects, and ControlObjects in a **separate project** from tests so they can be published as a NuGet package and shared across suites. As it grows, group by domain-aligned subfolders (namespaces stay flat — folders are for discoverability).

```
MyApp.PageObjects/     # Tab / Page / Control definitions → publish as NuGet
├── Shell/             # MainShell, HeaderNavigation, ChatWindow
├── Dashboard/         # IndexPage
└── Landing/           # AboutPage
MyApp.UITests/         # tests → reference MyApp.PageObjects
```

### Structuring a PageObject by visual region

When a PageObject accumulates 15+ flat control properties spanning visually distinct regions, decompose it into ControlObjects that match what the user sees.

```csharp
// ✅ Good — grouped by visual region
public sealed class HeaderPageObject : PageObject
{
    public Link LogoLink => Find<Link>(By.TestId("logo"));
    public ProjectPickerControlObject ProjectPicker => Find<ProjectPickerControlObject>(By.TestId("project-picker"));
    public NavBarControlObject NavBar => Find<NavBarControlObject>(By.TestId("nav-bar"));
}

// Tests read like the layout:
await header.ProjectPicker.SelectProjectAsync("production");
await header.NavBar.ReportsLink.ClickAsync();
```

Each ControlObject wraps one region with its own `SearchPattern`; convenience methods on the parent can delegate to it. Don't over-decompose — a flat PageObject is fine for a single region.

### Interface-based decoupling (advanced)

When page object implementations ship with the system under test and tests deploy separately, split into three assemblies: **interface** (defines `ISettings`, control properties), **implementation** (ships with the SUT, holds locators), and **test** (references only interfaces). Use `tab.Goto<ISettings>()`, return interface control types, resolve the tab with `TabObject.Resolve<IMyTab>()`, and register pages with `ChildOf<,>()`. Full guide: [DECOUPLING.md](DECOUPLING.md).

### Full test example

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

### Debugging

- `WriteTree()` — visualize the page object hierarchy
- `.HighlightAsync()` — visually identify elements
- `headless: false` — watch interactions live
- `await Task.Delay()` — slow down headed tests for observation
- Playwright traces — detailed execution logs

---

## Rule registry

Canonical IDs are immutable once assigned (retire, never renumber); the kebab slug is the review alias.

| ID | Slug | Rule | Topic |
| --- | --- | --- | --- |
| `COPA-HIR-001` | `page-vs-control` | PageObject for unique areas, ControlObject for reusable widgets | Hierarchy |
| `COPA-HIR-002` | `no-new-ui-objects` | Never `new` a UI object — resolve via `Find<T>()` / `FindAll<T>()` | Hierarchy |
| `COPA-HIR-003` | `childof-not-hardcoded` | Register relationships with `ChildOf<,>()`, don't hardcode | Hierarchy |
| `COPA-NAV-001` | `goto-vs-on` | `Goto<T>()` to reach, `On<T>()` when already visible | Navigation |
| `COPA-NAV-002` | `real-ui-goto` | Implement `Goto()` by clicking the real nav control, not a hand-built URL | Navigation |
| `COPA-NAV-003` | `url-only-entry-points` | Direct URL only for entry points / deep-links / redirect tests | Navigation |
| `COPA-NAV-004` | `no-navigate-to-control` | Never navigate to a ControlObject — access via its parent | Navigation |
| `COPA-CTL-001` | `controls-as-properties` | Expose controls as properties, not methods | Control Access |
| `COPA-CTL-002` | `no-raw-locator-exposed` | Never expose `ILocator` / `IPage` — wrap via `Find<T>()` | Control Access |
| `COPA-CTL-003` | `encapsulate-flows` | Multi-step flows & state queries live on the page object | Control Access |
| `COPA-CTL-004` | `collections-as-findall` | Expose repeated elements via `FindAll<T>()` and count with `.CountAsync()` — don't deflate into `GetXxxCountAsync()` | Control Access |
| `COPA-NAM-001` | `descriptive-names` | Tier suffixes + full descriptive names, no abbreviations | Naming |
| `COPA-NAM-002` | `xml-doc-public` | XML `<summary>` on every public member | Naming |
| `COPA-SEL-001` | `prefer-testid` | Prefer `By.TestId(...)`, combine with `.And()` | Selectors |
| `COPA-SEL-002` | `testid-contract` | TestIds in a contract project — no magic strings | Selectors |
| `COPA-SEL-003` | `no-interpolated-has-text` | Use `Locator.Filter()`, never interpolate `:has-text()` | Selectors |
| `COPA-TST-001` | `only-page-objects` | Tests interact only through PageObjects/ControlObjects | Test Hygiene |
| `COPA-TST-002` | `interface-page-refs` | Interface types for page references | Test Hygiene |
| `COPA-TST-003` | `auto-tab-cleanup` | Close tabs via `IAsyncDisposable`, not per-test `try/finally` | Test Hygiene |
| `COPA-TST-004` | `dynamic-waits` | Wait for the specific condition, no fixed `Task.Delay()` | Test Hygiene |
| `COPA-TST-005` | `web-first-state-waits` | Assert state via a web-first `ctrl.Enabled().WaitForAsync()` handle, not a one-shot `IsXxxAsync()` snapshot | Test Hygiene |

## Further Reading

- [README.md](README.md) — Quick start and overview
- [PATTERN.md](PATTERN.md) — Coparoo pattern theory (DOM structure, search optimization)
- [DECOUPLING.md](DECOUPLING.md) — Interface-based decoupling for cooperative projects
- [DEMO.md](DEMO.md) — Step-by-step code walkthrough
- [Demo Project](Trumpf.Coparoo.Playwright.Demo/README.md) — Full working demo
