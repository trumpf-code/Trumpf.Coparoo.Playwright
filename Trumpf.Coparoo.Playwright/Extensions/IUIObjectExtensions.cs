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
using Trumpf.Coparoo.Playwright.Internal;
using Trumpf.Coparoo.Playwright.Logging.Tree;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extensions.
/// </summary>
public static class IUIObjectExtensions
{
    /// <summary>
    /// Checks if the source exists by verifying if the count of matching elements is greater than zero.
    /// </summary>
    /// <returns>
    /// A boolean indicating whether the source exists (true if count is greater than zero, false otherwise).
    /// </returns>
    public static async Task<bool> ExistsAsync(this IUIObject source)
    {
        return await source.Locator.CountAsync() > 0;
    }

    /// <summary>
    /// <para>Returns whether the element is <a href="https://playwright.dev/dotnet/docs/actionability#visible">visible</a>.</para>
    /// <para>
    /// If you need to assert that element is visible, prefer <see cref="ILocatorAssertions.ToBeVisibleAsync"/>
    /// to avoid flakiness. See <a href="https://playwright.dev/dotnet/docs/test-assertions">assertions
    /// guide</a> for more details.
    /// </para>
    /// <para>**Usage**</para>
    /// <code>Boolean visible = await page.GetByRole(AriaRole.Button).IsVisibleAsync();</code>
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you need to assert that element is visible, prefer <see cref="ILocatorAssertions.ToBeVisibleAsync"/>
    /// to avoid flakiness. See <a href="https://playwright.dev/dotnet/docs/test-assertions">assertions
    /// guide</a> for more details.
    /// </para>
    /// </remarks>
    /// <param name="source">The UI object to check visibility for.</param>
    public static async Task<bool> IsVisibleAsync(this IUIObject source)
    {
        return await source.Locator.IsVisibleAsync();
    }

    /// <summary>
    /// <para>Returns the matching element's attribute value.</para>
    /// </summary>
    /// <param name="source">The UI object to get the attribute from.</param>
    /// <param name="name">Attribute name to get the value for.</param>
    /// <param name="options">Call options</param>
    public static async Task<string?> GetAttributeAsync(this IUIObject source, string name, LocatorGetAttributeOptions? options = default)
    {
        return await source.Locator.GetAttributeAsync(name, options);
    }

    /// <summary>
    /// <para>Locate element by the test id.</para>
    /// <para>**Usage**</para>
    /// <para>Consider the following DOM structure.</para>
    /// <para>You can locate the element by it's test id:</para>
    /// <code>await page.GetByTestId("directions").ClickAsync();</code>
    /// <para>**Details**</para>
    /// <para>
    /// By default, the <c>data-testid</c> attribute is used as a test id. Use <see cref="ISelectors.SetTestIdAttribute"/>
    /// to configure a different test id attribute if necessary.
    /// </para>
    /// </summary>
    /// <param name="source">The UI object to search within.</param>
    /// <param name="testId">Id to locate the element by.</param>
    public static ILocator GetByTestId(this IUIObject source, string testId)
    {
        return source.Locator.GetByTestId(testId);
    }

    /// <summary>
    /// <para>
    /// Allows locating elements by their <a href="https://www.w3.org/TR/wai-aria-1.2/#roles">ARIA
    /// role</a>, <a href="https://www.w3.org/TR/wai-aria-1.2/#aria-attributes">ARIA attributes</a>
    /// and <a href="https://w3c.github.io/accname/#dfn-accessible-name">accessible name</a>.
    /// </para>
    /// <para>**Usage**</para>
    /// <para>Consider the following DOM structure.</para>
    /// <para>You can locate each element by it's implicit role:</para>
    /// <code>
    /// await Expect(Page<br/>
    ///     .GetByRole(AriaRole.Heading, new() { Name = "Sign up" }))<br/>
    ///     .ToBeVisibleAsync();<br/>
    /// <br/>
    /// await page<br/>
    ///     .GetByRole(AriaRole.Checkbox, new() { Name = "Subscribe" })<br/>
    ///     .CheckAsync();<br/>
    /// <br/>
    /// await page<br/>
    ///     .GetByRole(AriaRole.Button, new() {<br/>
    ///         NameRegex = new Regex("submit", RegexOptions.IgnoreCase)<br/>
    ///     })<br/>
    ///     .ClickAsync();
    /// </code>
    /// <para>**Details**</para>
    /// <para>
    /// Role selector **does not replace** accessibility audits and conformance tests, but
    /// rather gives early feedback about the ARIA guidelines.
    /// </para>
    /// <para>
    /// Many html elements have an implicitly <a href="https://w3c.github.io/html-aam/#html-element-role-mappings">defined
    /// role</a> that is recognized by the role selector. You can find all the <a href="https://www.w3.org/TR/wai-aria-1.2/#role_definitions">supported
    /// roles here</a>. ARIA guidelines **do not recommend** duplicating implicit roles
    /// and attributes by setting <c>role</c> and/or <c>aria-*</c> attributes to default
    /// values.
    /// </para>
    /// </summary>
    /// <param name="source">The UI object to search within.</param>
    /// <param name="role">Required aria role.</param>
    /// <param name="options">Call options</param>
    public static ILocator GetByRole(this IUIObject source, AriaRole role, LocatorGetByRoleOptions? options = default)
    {
        return source.Locator.GetByRole(role, options);
    }
}