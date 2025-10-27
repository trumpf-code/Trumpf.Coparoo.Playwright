// Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#nullable enable

using System;
using System.Collections.Generic;
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extension methods providing query and state checking methods for IUIObject.
/// These methods allow you to inspect element state, content, and properties.
/// Works with all UI objects: controls, pages, and tabs.
/// </summary>
public static class IUIObjectQueryExtensions
{
    #region State Checking Methods

    /// <summary>
    /// Returns whether the element is visible.
    /// Note: Consider using Expect(locator).ToBeVisibleAsync() for assertions to avoid flakiness.
    /// </summary>
    /// <param name="source">The UI object to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is visible, false otherwise.</returns>
    public static async Task<bool> IsVisibleAsync(this IUIObject source, LocatorIsVisibleOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsVisibleAsync(options);
    }

    /// <summary>
    /// Returns whether the element is hidden.
    /// </summary>
    /// <param name="source">The UI object to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is hidden, false otherwise.</returns>
    public static async Task<bool> IsHiddenAsync(this IUIObject source, LocatorIsHiddenOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsHiddenAsync(options);
    }

    /// <summary>
    /// Returns whether the checkbox or radio button is checked.
    /// </summary>
    /// <param name="source">The UI object (checkbox/radio) to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is checked, false otherwise.</returns>
    public static async Task<bool> IsCheckedAsync(this IUIObject source, LocatorIsCheckedOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsCheckedAsync(options);
    }

    /// <summary>
    /// Returns whether the element is disabled.
    /// </summary>
    /// <param name="source">The UI object to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is disabled, false otherwise.</returns>
    public static async Task<bool> IsDisabledAsync(this IUIObject source, LocatorIsDisabledOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsDisabledAsync(options);
    }

    /// <summary>
    /// Returns whether the element is enabled.
    /// </summary>
    /// <param name="source">The UI object to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is enabled, false otherwise.</returns>
    public static async Task<bool> IsEnabledAsync(this IUIObject source, LocatorIsEnabledOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsEnabledAsync(options);
    }

    /// <summary>
    /// Returns whether the element is editable (input/textarea that is not readonly).
    /// </summary>
    /// <param name="source">The UI object to check.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>True if the element is editable, false otherwise.</returns>
    public static async Task<bool> IsEditableAsync(this IUIObject source, LocatorIsEditableOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.IsEditableAsync(options);
    }

    #endregion

    #region Content Methods

    /// <summary>
    /// Returns the element's inner HTML.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The innerHTML of the element.</returns>
    public static async Task<string> InnerHTMLAsync(this IUIObject source, LocatorInnerHTMLOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.InnerHTMLAsync(options);
    }

    /// <summary>
    /// Returns the textContent of the element, or null if not found.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The textContent of the element, or null.</returns>
    public static async Task<string?> TextContentAsync(this IUIObject source, LocatorTextContentOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.TextContentAsync(options);
    }

    /// <summary>
    /// Returns the innerText of the element.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The innerText of the element.</returns>
    public static async Task<string> InnerTextAsync(this IUIObject source, LocatorInnerTextOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.InnerTextAsync(options);
    }

    /// <summary>
    /// Returns the input value for input, textarea, or select elements.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The input value.</returns>
    public static async Task<string> InputValueAsync(this IUIObject source, LocatorInputValueOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.InputValueAsync(options);
    }

    /// <summary>
    /// Returns an array of inner texts for all matching elements.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>Array of inner texts.</returns>
    public static async Task<IReadOnlyList<string>> AllInnerTextsAsync(this IUIObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.AllInnerTextsAsync();
    }

    /// <summary>
    /// Returns an array of text contents for all matching elements.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>Array of text contents.</returns>
    public static async Task<IReadOnlyList<string?>> AllTextContentsAsync(this IUIObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.AllTextContentsAsync();
    }

    #endregion

    #region Element Information

    /// <summary>
    /// Returns the number of elements matching the locator.
    /// Useful for checking existence: count > 0 means element exists.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>The count of matching elements.</returns>
    public static async Task<int> CountAsync(this IUIObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.CountAsync();
    }

    /// <summary>
    /// Returns the element's attribute value.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="name">Attribute name to get the value for.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The attribute value, or null if the attribute is not present.</returns>
    public static async Task<string?> GetAttributeAsync(this IUIObject source, string name, LocatorGetAttributeOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.GetAttributeAsync(name, options);
    }

    /// <summary>
    /// Returns the bounding box of the element, or null if not visible.
    /// The box includes x, y, width, and height in pixels.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    /// <returns>The bounding box, or null if element is not visible.</returns>
    public static async Task<LocatorBoundingBoxResult?> BoundingBoxAsync(this IUIObject source, LocatorBoundingBoxOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.BoundingBoxAsync(options);
    }

    /// <summary>
    /// Locate element by the test id.
    /// By default, the data-testid attribute is used as a test id.
    /// </summary>
    /// <param name="source">The UI object to search within.</param>
    /// <param name="testId">Id to locate the element by.</param>
    /// <returns>A locator for the element with the specified test id.</returns>
    public static ILocator GetByTestId(this IUIObject source, string testId)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Locator.GetByTestId(testId);
    }

    /// <summary>
    /// Locate elements by their ARIA role, ARIA attributes, and accessible name.
    /// </summary>
    /// <param name="source">The UI object to search within.</param>
    /// <param name="role">Required aria role.</param>
    /// <param name="options">Optional locator options including name, exact, etc.</param>
    /// <returns>A locator for elements with the specified role.</returns>
    public static ILocator GetByRole(this IUIObject source, AriaRole role, LocatorGetByRoleOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Locator.GetByRole(role, options);
    }

    #endregion

    #region Advanced Methods

    /// <summary>
    /// Wait for the element to reach a specific state.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Wait options including state (attached, detached, visible, hidden) and timeout.</param>
    public static async Task WaitForAsync(this IUIObject source, LocatorWaitForOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.WaitForAsync(options);
    }

    /// <summary>
    /// Takes a screenshot of the element.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Screenshot options including path, type, quality, etc.</param>
    /// <returns>The screenshot as a byte array.</returns>
    public static async Task<byte[]> ScreenshotAsync(this IUIObject source, LocatorScreenshotOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.ScreenshotAsync(options);
    }

    /// <summary>
    /// Remove focus from the element.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout settings.</param>
    public static async Task BlurAsync(this IUIObject source, LocatorBlurOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.BlurAsync(options);
    }

    /// <summary>
    /// Clear the input field. Works for input and textarea elements.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <param name="options">Optional timeout and force settings.</param>
    public static async Task ClearAsync(this IUIObject source, LocatorClearOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.ClearAsync(options);
    }

    /// <summary>
    /// Checks if the element exists by verifying the count is greater than zero.
    /// This is a convenience method equivalent to: await uiObject.CountAsync() > 0
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>True if the element exists (count > 0), false otherwise.</returns>
    public static async Task<bool> ExistsAsync(this IUIObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.CountAsync() > 0;
    }

    #endregion
}
