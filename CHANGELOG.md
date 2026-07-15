# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-07-15

### Changed (BREAKING)

- All element **state waits** are unified under the composable `ElementCondition` handle. Every wait now
  reads `control.State().WaitForAsync()` (wait until the state holds) or `control.State().WaitForNotAsync()`
  (wait until it clears), where `State()` is one of `Enabled()`, `Checked()`, `Editable()`, `Visible()`,
  or `Attached()`. An optional `TimeSpan` timeout is supported, e.g. `control.Visible().WaitForAsync(timeout)`.

### Removed (BREAKING)

- The four standalone `IUIObject` wait extensions were removed. Migrate as follows:

  | Removed (2.x)                      | Replacement (3.0)                        |
  | ---------------------------------- | ---------------------------------------- |
  | `await ctrl.WaitForVisibleAsync()`  | `await ctrl.Visible().WaitForAsync()`     |
  | `await ctrl.WaitForHiddenAsync()`   | `await ctrl.Visible().WaitForNotAsync()`  |
  | `await ctrl.WaitForAttachedAsync()` | `await ctrl.Attached().WaitForAsync()`    |
  | `await ctrl.WaitForDetachedAsync()` | `await ctrl.Attached().WaitForNotAsync()` |

### Notes

- State waits now run on Playwright's web-first assertions (`Expect(locator).ToBeVisibleAsync()` and
  friends). Two behaviours therefore change: the default timeout is Playwright's **assertion** timeout
  (5 s, previously 30 s for the selector-state waits), and a timeout throws `PlaywrightException`
  (previously `TimeoutException`). Pass an explicit timeout for known-slow waits.
- One-shot reads (`IsVisibleAsync()`, `IsEnabledAsync()`, `IsCheckedAsync()`, `IsEditableAsync()`, …) are
  unchanged.

## Earlier releases

See the [GitHub releases](https://github.com/trumpf-code/Trumpf.Coparoo.Playwright/releases) and
[tags](https://github.com/trumpf-code/Trumpf.Coparoo.Playwright/tags) for the 2.x history.

[3.0.0]: https://github.com/trumpf-code/Trumpf.Coparoo.Playwright/releases/tag/3.0.0
