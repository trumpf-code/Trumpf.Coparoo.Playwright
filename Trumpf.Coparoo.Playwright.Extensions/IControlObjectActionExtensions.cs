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
/// Extension methods providing all Playwright action methods for IControlObject.
/// These methods delegate to the underlying ILocator to perform user interactions.
/// </summary>
public static class IControlObjectActionExtensions
{
    /// <summary>
    /// Click the control.
    /// This method waits for actionability checks, scrolls the element into view if needed,
    /// and clicks in the center of the element.
    /// </summary>
    /// <param name="source">The control object to click.</param>
    /// <param name="options">Click options such as button, click count, delay, position, modifiers, etc.</param>
    public static async Task ClickAsync(this IControlObject source, LocatorClickOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.ClickAsync(options);
    }

    /// <summary>
    /// Double-click the control.
    /// </summary>
    /// <param name="source">The control object to double-click.</param>
    /// <param name="options">Double-click options.</param>
    public static async Task DblClickAsync(this IControlObject source, LocatorDblClickOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.DblClickAsync(options);
    }

    /// <summary>
    /// Tap the control (mobile/touch gesture).
    /// </summary>
    /// <param name="source">The control object to tap.</param>
    /// <param name="options">Tap options.</param>
    public static async Task TapAsync(this IControlObject source, LocatorTapOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.TapAsync(options);
    }

    /// <summary>
    /// <para>Set a value to the input field.</para>
    /// <para>**Usage**</para>
    /// <code>await control.FillAsync("example value");</code>
    /// <para>**Details**</para>
    /// <para>
    /// This method waits for <a href="https://playwright.dev/dotnet/docs/actionability">actionability</a>
    /// checks, focuses the element, fills it and triggers an <c>input</c> event after filling.
    /// Note that you can pass an empty string to clear the input field.
    /// </para>
    /// <para>
    /// If the target element is not an <c>&lt;input&gt;</c>, <c>&lt;textarea&gt;</c> or
    /// <c>[contenteditable]</c> element, this method throws an error. However, if the element
    /// is inside the <c>&lt;label&gt;</c> element that has an associated <a href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLLabelElement/control">control</a>,
    /// the control will be filled instead.
    /// </para>
    /// <para>To send fine-grained keyboard events, use <see cref="PressSequentiallyAsync"/>.</para>
    /// </summary>
    /// <param name="source">The control object to fill.</param>
    /// <param name="value">
    /// Value to set for the <c>&lt;input&gt;</c>, <c>&lt;textarea&gt;</c> or <c>[contenteditable]</c>
    /// element.
    /// </param>
    /// <param name="options">Fill options.</param>
    public static async Task FillAsync(this IControlObject source, string value, LocatorFillOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.FillAsync(value, options);
    }

    /// <summary>
    /// Type text into the control character by character (simulates user typing).
    /// Deprecated: use <see cref="FillAsync"/> or <see cref="PressSequentiallyAsync"/> instead.
    /// </summary>
    /// <param name="source">The control object to type into.</param>
    /// <param name="text">Text to type.</param>
    /// <param name="options">Type options including delay between key presses.</param>
    [Obsolete("Use FillAsync for most cases, or PressSequentiallyAsync for precise key events.")]
    public static async Task TypeAsync(this IControlObject source, string text, LocatorTypeOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.TypeAsync(text, options);
    }

    /// <summary>
    /// Press a key on the control.
    /// </summary>
    /// <param name="source">The control object to press keys on.</param>
    /// <param name="key">Key name or character to press (e.g., "Enter", "ArrowLeft", "a", "A").</param>
    /// <param name="options">Press options including delay.</param>
    public static async Task PressAsync(this IControlObject source, string key, LocatorPressOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.PressAsync(key, options);
    }

    /// <summary>
    /// Press text sequentially, character by character (sends fine-grained keyboard events).
    /// Unlike <see cref="FillAsync"/>, this method emits keyboard events for each character.
    /// </summary>
    /// <param name="source">The control object to type into.</param>
    /// <param name="text">Text to press sequentially.</param>
    /// <param name="options">Press sequentially options including delay between key presses.</param>
    public static async Task PressSequentiallyAsync(this IControlObject source, string text, LocatorPressSequentiallyOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.PressSequentiallyAsync(text, options);
    }

    /// <summary>
    /// Check a checkbox or radio button.
    /// Ensures the control is checked. If already checked, does nothing.
    /// </summary>
    /// <param name="source">The control object to check.</param>
    /// <param name="options">Check options.</param>
    public static async Task CheckAsync(this IControlObject source, LocatorCheckOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.CheckAsync(options);
    }

    /// <summary>
    /// Uncheck a checkbox.
    /// Ensures the control is unchecked. If already unchecked, does nothing.
    /// </summary>
    /// <param name="source">The control object to uncheck.</param>
    /// <param name="options">Uncheck options.</param>
    public static async Task UncheckAsync(this IControlObject source, LocatorUncheckOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.UncheckAsync(options);
    }

    /// <summary>
    /// Set the checked state of a checkbox or radio button.
    /// </summary>
    /// <param name="source">The control object.</param>
    /// <param name="checkedState">True to check, false to uncheck.</param>
    /// <param name="options">Set checked options.</param>
    public static async Task SetCheckedAsync(this IControlObject source, bool checkedState, LocatorSetCheckedOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SetCheckedAsync(checkedState, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by value.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">Value(s) to select.</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, string values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by values.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">Multiple values to select.</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, IEnumerable<string> values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by SelectOptionValue.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">SelectOptionValue to select (can specify by value, label, or index).</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, SelectOptionValue values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by multiple SelectOptionValue objects.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">Multiple SelectOptionValue objects.</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, IEnumerable<SelectOptionValue> values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by element handle.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">Element handle of the option to select.</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, IElementHandle values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Select one or more options in a &lt;select&gt; element by multiple element handles.
    /// </summary>
    /// <param name="source">The control object (select element).</param>
    /// <param name="values">Element handles of options to select.</param>
    /// <param name="options">Select option options.</param>
    /// <returns>List of selected option values.</returns>
    public static async Task<IReadOnlyList<string>> SelectOptionAsync(this IControlObject source, IEnumerable<IElementHandle> values, LocatorSelectOptionOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return await source.Locator.SelectOptionAsync(values, options);
    }

    /// <summary>
    /// Set input files for a file input element.
    /// </summary>
    /// <param name="source">The control object (file input).</param>
    /// <param name="files">File path to upload.</param>
    /// <param name="options">Set input files options.</param>
    public static async Task SetInputFilesAsync(this IControlObject source, string files, LocatorSetInputFilesOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SetInputFilesAsync(files, options);
    }

    /// <summary>
    /// Set input files for a file input element (multiple files).
    /// </summary>
    /// <param name="source">The control object (file input).</param>
    /// <param name="files">File paths to upload.</param>
    /// <param name="options">Set input files options.</param>
    public static async Task SetInputFilesAsync(this IControlObject source, IEnumerable<string> files, LocatorSetInputFilesOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SetInputFilesAsync(files, options);
    }

    /// <summary>
    /// Set input files for a file input element using FilePayload.
    /// </summary>
    /// <param name="source">The control object (file input).</param>
    /// <param name="files">File payload with name, mime type, and buffer.</param>
    /// <param name="options">Set input files options.</param>
    public static async Task SetInputFilesAsync(this IControlObject source, FilePayload files, LocatorSetInputFilesOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SetInputFilesAsync(files, options);
    }

    /// <summary>
    /// Set input files for a file input element using multiple FilePayload objects.
    /// </summary>
    /// <param name="source">The control object (file input).</param>
    /// <param name="files">Multiple file payloads.</param>
    /// <param name="options">Set input files options.</param>
    public static async Task SetInputFilesAsync(this IControlObject source, IEnumerable<FilePayload> files, LocatorSetInputFilesOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SetInputFilesAsync(files, options);
    }

    /// <summary>
    /// Hover over the control.
    /// </summary>
    /// <param name="source">The control object to hover over.</param>
    /// <param name="options">Hover options including position, modifiers, force, etc.</param>
    public static async Task HoverAsync(this IControlObject source, LocatorHoverOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.HoverAsync(options);
    }

    /// <summary>
    /// Focus the control.
    /// </summary>
    /// <param name="source">The control object to focus.</param>
    /// <param name="options">Focus options.</param>
    public static async Task FocusAsync(this IControlObject source, LocatorFocusOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.FocusAsync(options);
    }

    /// <summary>
    /// Drag the control to a target locator (drag and drop).
    /// </summary>
    /// <param name="source">The control object to drag.</param>
    /// <param name="target">Target locator to drop onto.</param>
    /// <param name="options">Drag to options.</param>
    public static async Task DragToAsync(this IControlObject source, ILocator target, LocatorDragToOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.DragToAsync(target, options);
    }

    /// <summary>
    /// Scroll the element into view if needed.
    /// </summary>
    /// <param name="source">The control object to scroll into view.</param>
    /// <param name="options">Scroll into view options.</param>
    public static async Task ScrollIntoViewIfNeededAsync(this IControlObject source, LocatorScrollIntoViewIfNeededOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.ScrollIntoViewIfNeededAsync(options);
    }

    /// <summary>
    /// Dispatch a DOM event on the control.
    /// </summary>
    /// <param name="source">The control object.</param>
    /// <param name="type">Event type (e.g., "click", "dragstart").</param>
    /// <param name="eventInit">Optional event initialization object.</param>
    /// <param name="options">Dispatch event options.</param>
    public static async Task DispatchEventAsync(this IControlObject source, string type, object? eventInit = null, LocatorDispatchEventOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.DispatchEventAsync(type, eventInit, options);
    }

    /// <summary>
    /// Highlight the control for debugging purposes (draws a red border around it).
    /// </summary>
    /// <param name="source">The control object to highlight.</param>
    public static async Task HighlightAsync(this IControlObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.HighlightAsync();
    }

    /// <summary>
    /// Select all text in the control (typically for input or textarea elements).
    /// </summary>
    /// <param name="source">The control object.</param>
    /// <param name="options">Select text options.</param>
    public static async Task SelectTextAsync(this IControlObject source, LocatorSelectTextOptions? options = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await source.Locator.SelectTextAsync(options);
    }
}
