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

namespace Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Checkbox control object.
/// Expects an input html element with attribute type="checkbox".
/// </summary>
public class Checkbox : ControlObject, ICheckbox
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => By.CssSelector("input[type='checkbox']");

    /// <summary>
    /// Gets the value.
    /// </summary>
    public Task<string> Value
        => GetValueAsync();

    private async Task<string> GetValueAsync()
        => await (await Locator).GetAttributeAsync("value");

    /// <summary>
    /// Gets the name.
    /// </summary>
    public Task<string> Name
        => GetNameAsync();

    private async Task<string> GetNameAsync()
        => await (await Locator).GetAttributeAsync("name");

    /// <summary>
    /// Gets or sets a value indicating whether the checkbox is checked.
    /// </summary>
    public Task<bool> IsChecked
        => GetIsCheckedAsync();

    private async Task<bool> GetIsCheckedAsync()
        => await (await Locator).IsCheckedAsync();

    /// <summary>
    /// Ensures that the element is checked. 
    /// If the element is not already checked, this method will check it.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Check()
    {
        if (!await IsChecked)
            await (await Locator).CheckAsync();
    }

    /// <summary>
    /// Ensures that the element is unchecked. 
    /// If the element is already checked, this method will uncheck it.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Uncheck()
    {
        if (await IsChecked)
            await (await Locator).UncheckAsync();
    }
}