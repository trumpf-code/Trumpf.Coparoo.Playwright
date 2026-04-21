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

using static Microsoft.Playwright.Assertions;

namespace Trumpf.Coparoo.Playwright.Controls.Bootstrap;

/// <summary>
/// Bootstrap dropdown selector control object.
/// Wraps elements with the <c>.dropdown</c> CSS class that contain a <c>.dropdown-toggle</c> button and <c>.dropdown-item</c> elements.
/// </summary>
public class DropdownSelector : ControlObject, IDropdownSelector
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => By.CssSelector(".dropdown");

    /// <inheritdoc />
    public async Task<string> GetSelectedValueAsync()
        => (await Locator.Locator("button.dropdown-toggle").TextContentAsync())?.Trim() ?? string.Empty;

    /// <inheritdoc />
    public async Task SelectAsync(string value)
    {
        await Locator.Locator("button.dropdown-toggle").ClickAsync();
        await Locator.Locator(".dropdown-item").Filter(new() { HasTextString = value }).ClickAsync();
    }

    /// <inheritdoc />
    public async Task WaitForSelectedValueAsync(string expected, TimeSpan? timeout = null)
    {
        var ms = (float)(timeout ?? TimeSpan.FromSeconds(5)).TotalMilliseconds;
        await Expect(Locator.Locator("button.dropdown-toggle")).ToHaveTextAsync(expected, new() { Timeout = ms });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetOptionsAsync()
    {
        await Locator.Locator("button.dropdown-toggle").ClickAsync();
        var items = await Locator.Locator(".dropdown-item").AllTextContentsAsync();
        // Close dropdown by clicking toggle again
        await Locator.Locator("button.dropdown-toggle").ClickAsync();
        return items;
    }
}
