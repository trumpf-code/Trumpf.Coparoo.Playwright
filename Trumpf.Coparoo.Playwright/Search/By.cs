using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Search pattern wrapper.
/// </summary>
public class By
{
    private readonly List<string> _selectors = new();
    private SelectorType _selectorType;
    private string _selector;

    private const string PatternEscapeSymbols = @"(?<!\\)([.#:>[\](){}+~^$|=])";

    private enum SelectorType
    {
        CssSelector,
        XPath,
        Id,
        TagName,
        Name,
        ClassName,
        TestId,
        Free
    }

    private By(string selector, SelectorType selectorType)
    {
        _selector = selector;
        _selectorType = selectorType;
        _selectors.Add(selector);
    }

    private By()
    {
    }

    private By(IEnumerable<string> selectors, SelectorType selectorType)
    {
        _selectors.AddRange(selectors);
        _selectorType = selectorType;
        _selector = selectors.LastOrDefault();
    }

    /// <summary>
    /// Combines this selector with another selector using CSS AND (concatenation).
    /// Tag selectors are always placed first to ensure valid CSS syntax.
    /// Only one tag selector and one ID selector are allowed. Multiple class selectors are permitted.
    /// </summary>
    /// <param name="other">The other By selector to combine with.</param>
    /// <returns>A new By instance representing the combined selector.</returns>
    /// <exception cref="InvalidOperationException">Thrown when multiple tag selectors or multiple ID selectors are combined.</exception>
    public By And(By other)
    {
        // Collect all selectors
        var allSelectors = _selectors.Concat(other._selectors).ToList();
        
        // Categorize selectors
        var tagSelectors = new List<string>();
        var idSelectors = new List<string>();
        var classSelectors = new List<string>();
        var attributeSelectors = new List<string>();
        var pseudoSelectors = new List<string>();
        
        foreach (var selector in allSelectors)
        {
            if (string.IsNullOrEmpty(selector))
                continue;
                
            if (selector.StartsWith("#"))
                idSelectors.Add(selector);
            else if (selector.StartsWith("."))
                classSelectors.Add(selector);
            else if (selector.StartsWith("["))
                attributeSelectors.Add(selector);
            else if (selector.StartsWith(":"))
                pseudoSelectors.Add(selector);
            else
                tagSelectors.Add(selector);
        }
        
        // Validate: only one tag selector allowed
        if (tagSelectors.Count > 1)
        {
            throw new InvalidOperationException(
                $"Cannot combine multiple tag selectors: {string.Join(", ", tagSelectors)}. " +
                "Only one tag selector is allowed per combined selector.");
        }
        
        // Validate: only one ID selector allowed
        if (idSelectors.Count > 1)
        {
            throw new InvalidOperationException(
                $"Cannot combine multiple ID selectors: {string.Join(", ", idSelectors)}. " +
                "Only one ID selector is allowed per combined selector.");
        }
        
        // Check for duplicate attribute selectors (same attribute name)
        var attributeNames = attributeSelectors
            .Select(a => Regex.Match(a, @"\[([^\]=]+)").Groups[1].Value)
            .ToList();
        
        var duplicateAttributes = attributeNames
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
            
        if (duplicateAttributes.Any())
        {
            throw new InvalidOperationException(
                $"Cannot combine duplicate attribute selectors for: {string.Join(", ", duplicateAttributes)}. " +
                "Each attribute can only appear once.");
        }
        
        // Build combined list: tags first, then ID, then classes, then attributes, then pseudo
        var combinedSelectors = new List<string>();
        combinedSelectors.AddRange(tagSelectors);
        combinedSelectors.AddRange(idSelectors);
        combinedSelectors.AddRange(classSelectors);
        combinedSelectors.AddRange(attributeSelectors);
        combinedSelectors.AddRange(pseudoSelectors);
        
        return new By(combinedSelectors, SelectorType.CssSelector);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element using a CSS selector.
    /// </summary>
    /// <param name="selector">The CSS selector string used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided CSS selector.</returns>
    public static By CssSelector(string selector)
    {
        return new By(selector, SelectorType.CssSelector);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element using an XPath expression.
    /// </summary>
    /// <param name="xpath">The XPath expression used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided XPath expression.</returns>
    public static By XPath(string xpath)
    {
        return new By(xpath, SelectorType.XPath);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element by its ID attribute.
    /// </summary>
    /// <param name="id">The ID attribute value used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on.</returns>
    public static By Id(string id)
    {
        var escapedId = Regex.Replace(id, PatternEscapeSymbols, @"\$1");
        return new By($"#{escapedId}", SelectorType.Id);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element by its tag name.
    /// </summary>
    /// <param name="tagName">The tag name used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided tag name.</returns>
    public static By TagName(string tagName)
    {
        return new By(tagName, SelectorType.TagName);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element by its class name.
    /// </summary>
    /// <param name="className">The class name used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided class name.</returns>
    public static By ClassName(string className)
    {
        var escapedClassName = Regex.Replace(className, PatternEscapeSymbols, @"\$1");
        return new By($".{escapedClassName}", SelectorType.ClassName);
    }

    /// <summary>
    /// Creates a <see cref="By"/> object that locates an element by its data-testid attribute.
    /// </summary>
    /// <param name="testId">The test ID attribute value used to find the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided test ID.</returns>
    public static By TestId(string testId)
    {
        // Treat TestId as a CSS selector for combinability
        return new By($"[data-testid=\"{testId}\"]", SelectorType.TestId);
    }

    /// <summary>
    /// Converts the current object to a locator string that can be used to find the element.
    /// </summary>
    /// <returns>A string representation of the locator, suitable for locating the element.</returns>
    public string ToLocator()
    {
        if (_selectors.Count > 1)
        {
            // Combine all selectors with no space (CSS AND)
            return string.Concat(_selectors);
        }
        switch (_selectorType)
        {
            case SelectorType.CssSelector:
                return _selector;
            case SelectorType.XPath:
                return _selector;
            case SelectorType.Id:
                return _selector;
            case SelectorType.TagName:
                return _selector;
            case SelectorType.Name:
                return _selector;
            case SelectorType.ClassName:
                return _selector;
            case SelectorType.TestId:
                return _selector;
            case SelectorType.Free:
                return _selector;
            default:
                throw new InvalidOperationException("Unknown selector type.");
        }
    }

    /// <summary>
    /// Implicitly converts a string selector into a <see cref="By"/> object.
    /// </summary>
    /// <param name="selector">The selector string used to locate the element.</param>
    /// <returns>A <see cref="By"/> object that can be used to locate the element based on the provided selector string.</returns>
    public static implicit operator By(string selector) => new By(new List<string> { selector }, SelectorType.Free);

    /// <summary>
    /// Implicitly converts a <see cref="By"/> object to a string representation of its selector.
    /// </summary>
    /// <param name="by">The <see cref="By"/> object to convert.</param>
    /// <returns>The selector string.</returns>
    public static implicit operator string(By by)
    {
        if (by == null)
            throw new ArgumentNullException(nameof(by));

        return by.ToLocator();
    }
}