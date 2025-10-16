# Trumpf.Coparoo.Playwright.Controls

Pre-built control objects for web testing with Playwright based on the Coparoo pattern.

## Overview

This library provides a comprehensive set of ready-to-use control objects that simplify web testing by encapsulating common HTML elements and their behaviors. All controls follow the control/page/root-object (Coparoo) pattern and integrate seamlessly with Playwright-based web tests.

## Features

### Basic HTML Controls

- **Button** - Clickable button elements
- **TextInput** - Text input fields with type and clear operations
- **Checkbox** - Checkbox elements with checked state management
- **Label** - Label elements for displaying text
- **Link** - Hyperlink elements with navigation capabilities
- **Div** - Generic div container elements
- **Span** - Inline span elements
- **Cite** - Citation elements

### Complex Controls

#### Select Control
- **Select** - Dropdown/select elements
- **Option** - Individual options within select elements

#### Table Control
- **Table** - Complete table structure
- **Head** - Table header section
- **Body** - Table body section
- **Foot** - Table footer section
- **Row** - Table rows with cell access
- **Cell** - Individual table cells
- **Segment** - Generic table segment (head/body/foot base class)

## Usage

All controls inherit from `ControlObject` and implement corresponding interfaces (e.g., `IButton`, `ITextInput`). This design allows for:

- **Type safety** - Strong typing for all control interactions
- **Testability** - Easy mocking through interfaces
- **Extensibility** - Simple creation of custom controls
- **Maintainability** - Clear separation of concerns

### Example

```csharp
public class LoginPage : PageObject
{
    public virtual ITextInput Username => Find<TextInput>();
    public virtual ITextInput Password => Find<TextInput>();
    public virtual IButton LoginButton => Find<Button>();
}
```

## Dependencies

- **Trumpf.Coparoo.Playwright** - Core Coparoo framework
- **Microsoft.Playwright** - Playwright for .NET
- **System.Linq.Async** - Async LINQ support

## Documentation

For more information about the Coparoo pattern and usage examples, see:
- [PATTERN.md](../PATTERN.md) - Pattern explanation
- [DEMO.md](../DEMO.md) - Usage demos
- [DECOUPLING.md](../DECOUPLING.md) - Decoupling strategies

## License

Licensed under the Apache License, Version 2.0. See [LICENSE](../LICENSE) for details.

## Contributing

This is part of the Trumpf.Coparoo.Playwright project. For contributions and issues, visit:
https://github.com/trumpf-code/Trumpf.Coparoo.Playwright
