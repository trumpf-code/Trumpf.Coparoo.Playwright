# Trumpf.Coparoo.Playwright.Controls.Tests

Unit tests for the Trumpf.Coparoo.Playwright.Controls library.

## Overview

This project contains comprehensive tests for all pre-built control objects in the Trumpf.Coparoo.Playwright.Controls library. The tests verify the behavior and functionality of each control type.

## Test Coverage

### Basic HTML Control Tests

- **ButtonTests** - Tests for button control functionality
- **CheckboxTests** - Tests for checkbox state management and interactions
- **LabelTests** - Tests for label element behavior
- **LinkTests** - Tests for hyperlink navigation and properties
- **TextInputTests** - Tests for text input operations, typing, and clearing

### Complex Control Tests

- **SelectTests** - Tests for dropdown/select element behavior
- **OptionTests** - Tests for individual option selection within select elements
- **TableTests** - Tests for table structure and data access
- **ControlTests** - Base control tests for common functionality

## Test Framework

- **MSTest** - Test framework
- **AwesomeAssertions** - Enhanced assertion library
- **FluentAssertions** - Fluent assertion extensions
- **Microsoft.Playwright** - Browser automation

## Running Tests

Run all tests using:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~ButtonTests"
```

## Test Configuration

Tests are configured to run in parallel at the method level for optimal performance. See `Properties\Parallelize.cs` for configuration details.

## Dependencies

- **Trumpf.Coparoo.Playwright** - Core framework being tested
- **Trumpf.Coparoo.Playwright.Controls** - Controls library being tested
- **Microsoft.Playwright** - Browser automation
- **MSTest** - Test framework and adapters
- **AwesomeAssertions** - Assertion library

## License

Licensed under the Apache License, Version 2.0. See [LICENSE](../LICENSE) for details.
