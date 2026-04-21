# Trumpf.Coparoo.Playwright.Controls.Bootstrap

Pre-built [Bootstrap 5](https://getbootstrap.com/) control objects for [Trumpf.Coparoo.Playwright](https://github.com/trumpf-code/Trumpf.Coparoo.Playwright).

## Controls

| Control | Bootstrap Selector | Description |
|---|---|---|
| `Alert` | `.alert` | Bootstrap alert component |
| `CardHeading` | `.card-header h5` | Card header heading |
| `ChartPanel` | `.card` | Card containing a chart canvas |
| `CollapsibleSection` | `fieldset` + `.collapse` | Expandable/collapsible fieldset |
| `DropdownSelector` | `.dropdown` | Bootstrap dropdown with toggle and items |
| `InfoCard` | `.card` | Card with text content |
| `ProgressBarSegment` | `.progress-bar` | Individual progress bar segment |
| `Spinner` | `.spinner-border` | Bootstrap spinner/loading indicator |

## Usage

```csharp
// Find a Bootstrap alert on the page
var alert = page.Find<Alert>(By.CssSelector(".alert-success"));

// Work with a dropdown
var dropdown = page.Find<DropdownSelector>(By.TestId("my-dropdown"));
await dropdown.SelectAsync("Option B");
var selected = await dropdown.GetSelectedValueAsync();

// Check a collapsible section
var section = page.Find<CollapsibleSection>(By.TestId("settings-group"));
await section.ToggleAsync();
Assert.IsTrue(await section.IsExpandedAsync());
```
