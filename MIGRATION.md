# Migration Guide

## 2.3.x → 2.4.0

### Breaking: `Text` removed from built-in controls

The `Text` method/property has been removed from `Span`, `Button`, `Label`, `Link`, and `Option` (and their interfaces `ISpan`, `ILabel`, `ILink`, `IOption`).

Use the universal extension methods from `Trumpf.Coparoo.Playwright.Extensions` instead:

| Before (2.3.x) | After (2.4.0) |
|-----------------|---------------|
| `await span.Text()` | `await span.TextContentAsync()` |
| `await button.Text()` | `await button.TextContentAsync()` |
| `await label.Text` | `await label.TextContentAsync()` |
| `await link.Text` | `await link.TextContentAsync()` |
| `await option.Text` | `await option.TextContentAsync()` |

**Required using:** `using Trumpf.Coparoo.Playwright.Extensions;`

**Why:** `TextContentAsync()` and `InnerTextAsync()` already exist as extension methods on every `IUIObject`. The per-control `Text` members were redundant, inconsistent (method vs property), and used different underlying Playwright calls (`InnerTextAsync` vs `TextContentAsync`).

**Migration effort:** The compiler will flag every broken call site. Find-and-replace, then add the using directive.
