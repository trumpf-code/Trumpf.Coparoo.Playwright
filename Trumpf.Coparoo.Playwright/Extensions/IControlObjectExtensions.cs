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
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extensions for IControlObject.
/// </summary>
public static class IControlObjectExtensions
{
    /// <summary>
    /// Click the control.
    /// </summary>
    /// <param name="source">The control object to click.</param>
    public static async Task ClickAsync(this IControlObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        await source.Locator.ScrollIntoViewIfNeededAsync();
        await source.Locator.ClickAsync();
    }

    /// <summary>
    /// <para>Set a value to the input field.</para>
    /// <para>**Usage**</para>
    /// <code>await page.GetByRole(AriaRole.Textbox).FillAsync("example value");</code>
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
    /// <para>To send fine-grained keyboard events, use <see cref="ILocator.PressSequentiallyAsync"/>.</para>
    /// </summary>
    /// <param name="source">The control object to fill.</param>
    /// <param name="value">
    /// Value to set for the <c>&lt;input&gt;</c>, <c>&lt;textarea&gt;</c> or <c>[contenteditable]</c>
    /// element.
    /// </param>
    /// <param name="options">Call options</param>
    public static Task FillAsync(this IControlObject source, string value, LocatorFillOptions? options = default)
    {
        return source.Locator.FillAsync(value, options);
    }

    /// <summary>
    /// Returns the <c>textContent</c> of the element, or <c>null</c> if the element is not found.
    /// See: https://playwright.dev/dotnet/docs/api/class-locator#locator-text-content
    /// </summary>
    public static Task<string?> TextContentAsync(this IControlObject source)
    {
        return source.Locator.TextContentAsync();
    }

    /// <summary>
    /// Returns the <c>innerText</c> of the element.
    /// See: https://playwright.dev/dotnet/docs/api/class-locator#locator-inner-text
    /// </summary>
    public static Task<string> InnerTextAsync(this IControlObject source)
    {
        return source.Locator.InnerTextAsync();
    }

    /// <summary>
    /// Returns the value of the <c>&lt;input&gt;</c>, <c>&lt;textarea&gt;</c> or <c>[contenteditable]</c> element.
    /// Throws if the element is not an <c>&lt;input&gt;</c>, <c>&lt;textarea&gt;</c> or <c>[contenteditable]</c> element.
    /// See: https://playwright.dev/dotnet/docs/api/class-locator#locator-input-value
    /// </summary>
    public static Task<string> InputValueAsync(this IControlObject source)
    {
        return source.Locator.InputValueAsync();
    }
}
