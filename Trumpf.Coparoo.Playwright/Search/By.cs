using System;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Search pattern wrapper.
/// </summary>
public class By
{
    private string _selector;
    private SelectorType _selectorType;

    private enum SelectorType
    {
        CssSelector,
        XPath,
        Id,
        TagName,
        Name,
        ClassName,
        Free
    }

    private By(string selector, SelectorType selectorType)
    {
        _selector = selector;
        _selectorType = selectorType;
    }

    private By()
    {
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
        return new By($"#{id}", SelectorType.Id);
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
        return new By($".{className}", SelectorType.ClassName);
    }

    /// <summary>
    /// Converts the current object to a locator string that can be used to find the element.
    /// </summary>
    /// <returns>A string representation of the locator, suitable for locating the element.</returns>
    public string ToLocator()
    {
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
    public static implicit operator By(string selector) => new() { _selector = selector, _selectorType = SelectorType.Free };
}
