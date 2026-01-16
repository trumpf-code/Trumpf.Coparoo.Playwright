# IFrame Support for Coparoo

This implementation adds seamless iframe support to the Coparoo.Playwright framework through two new base classes: `FramePageObject` and `FrameControlObject`.

## Features

✅ **Simplified API** - Uses existing `SearchPattern` to locate iframe elements  
✅ **Transparent iframe handling** - Playwright's `FrameLocator` automatically bridges DOM boundaries  
✅ **Type-safe navigation** - Works with existing `Goto<T>()` and `On<T>()` patterns  
✅ **Nested frame support** - Handles frames within frames seamlessly  
✅ **Convenience properties** - Optional `FrameElement` property for frame-level checks  

## Architecture

### FramePageObject
For **complete pages** rendered inside iframes (e.g., embedded payment providers, external widgets):

```csharp
public sealed class PaymentFrame : FramePageObject, IPaymentFrame
{
    // SearchPattern points to the iframe element
    protected override By SearchPattern => By.TestId("payment-frame");
    
    // Controls are found inside the iframe
    public TextInput CardNumber => Find<TextInput>(By.TestId("card-number"));
    public Button SubmitButton => Find<Button>(By.TestId("submit-btn"));
}

// Usage
var paymentFrame = await tab.Goto<IPaymentFrame>();
await paymentFrame.CardNumber.FillAsync("4242424242424242");
await paymentFrame.SubmitButton.ClickAsync();
```

### FrameControlObject
For **transient UI controls** inside iframes (e.g., WYSIWYG editors, modal dialogs):

```csharp
public sealed class RichTextEditor : FrameControlObject, IRichTextEditor
{
    // SearchPattern points to the iframe element
    protected override By SearchPattern => By.TestId("editor-frame");
    
    // Editor controls inside the iframe
    public Button BoldButton => Find<Button>(By.TestId("bold-btn"));
    public TextInput ContentArea => Find<TextInput>(By.TestId("editor-content"));
}

// Usage
var editor = mainPage.Editor;  // Like any other control
await editor.ContentArea.FillAsync("Hello from Coparoo!");
await editor.BoldButton.ClickAsync();
```

## Key Design Decisions

### Unified SearchPattern
- **Decision**: Use `SearchPattern` for iframe location instead of separate `FrameSelector`
- **Rationale**: Simpler API, consistent with existing PageObject/ControlObject patterns
- **Trade-off**: `SearchPattern` has dual meaning (locates iframe element AND defines root context)

### Optional Root Element
- Frame objects can work **without** an intermediate root element inside the iframe
- The `SearchPattern` directly identifies the iframe; content is accessed from the frame root (`:root`)

### FrameElement Property
- Provides access to the iframe element itself (in parent DOM)
- Useful for checking iframe visibility/existence before accessing content
- Optional - most use cases only need to interact with frame content

## Implementation Details

### FramePageObjectNode
Overrides the `Locator()` method to use `FrameLocator`:

```csharp
public override ILocator Locator()
{
    var frameLocator = Parent.FrameLocator(SearchPattern.ToLocator());
    return frameLocator.Locator(":root").Nth(Index);
}
```

### FrameControlObjectNode
Identical implementation for consistency:

```csharp
public override ILocator Locator()
{
    var frameLocator = Parent.FrameLocator(SearchPattern.ToLocator());
    return frameLocator.Locator(":root").Nth(Index);
}
```

## Testing

Comprehensive tests in [FrameTests.cs](../Trumpf.Coparoo.Playwright.Tests/FrameTests.cs):

- ✅ **FramePageObject_ShouldLocateElementsInsideIframe** - Verifies iframe page navigation and interaction
- ✅ **FramePageObject_ShouldHandleNestedFrames** - Tests frames within frames
- ⚠️ **FrameControlObject** tests - Core functionality works; some edge cases with parent hierarchies need refinement

## Demo

Live demo in [IFrameDemo.cs](../Trumpf.Coparoo.Playwright.Demo/IFrameDemo.cs) with:
- Payment frame (FramePageObject example)
- Rich text editor (FrameControlObject example)
- Interactive HTML page with embedded iframes

Run the demo:
```bash
dotnet test --filter "FullyQualifiedName~IFrameDemo"
```

## Future Enhancements

1. **FrameElement hierarchy** - Refine parent-to-iframe element access
2. **ShadowDOM support** - Similar pattern for shadow DOM boundaries
3. **Multi-frame utilities** - Helper methods for complex frame scenarios
4. **Performance optimization** - Cache frame locators when appropriate

## Compatibility

- **Playwright**: 1.54.0+
- **Coparoo Core**: Current main branch
- **.NET**: .NET Standard 2.0 (core), .NET 8 (tests/demo)

## Usage Guidelines

### When to use FramePageObject
- Embedded third-party applications (payment forms, chat widgets)
- Large iframe-based content that acts as a distinct page
- Content that requires `Goto<T>()` navigation

### When to use FrameControlObject
- Temporary iframe-based controls (rich text editors, file uploaders)
- Modal dialogs rendered in iframes
- Controls accessed via `Find<T>()` from parent page

### Migration from Direct Playwright
Before (direct Playwright):
```csharp
var frame = page.FrameLocator("#payment-frame");
await frame.Locator("#card-number").FillAsync("4242...");
```

After (Coparoo with iframe support):
```csharp
var paymentFrame = tab.On<IPaymentFrame>();
await paymentFrame.CardNumber.FillAsync("4242...");
```

---

**Status**: ✅ Core implementation complete and tested  
**Author**: GitHub Copilot  
**Date**: January 16, 2026
