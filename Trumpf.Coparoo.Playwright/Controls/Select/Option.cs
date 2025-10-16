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
/// Option control object.
/// </summary>
public class Option : ControlObject, IOption
{
    /// <inheritdoc/>
    protected override By SearchPattern
        => "option";

    /// <summary>
    /// Gets the option value.
    /// </summary>
    public Task<string> Value
        => GetValueAsync();

    private async Task<string> GetValueAsync()
        => await (await Locator).GetAttributeAsync("value");

    /// <summary>
    /// Gets the option value.
    /// </summary>
    public Task<string> Text
        => GetTextAsync();

    private async Task<string> GetTextAsync()
        => await (await Locator).TextContentAsync();

    /// <summary>
    /// Gets a value indicating whether the option is selected.
    /// </summary>
    public async Task<bool> IsSelected()
        => await (await Locator).EvaluateAsync<bool>("option => option.selected");

    /// <summary>
    /// Select this option.
    /// </summary>
    public async Task Select()
    {
        if (!await IsSelected())
        {
            string value = await (await Locator).GetAttributeAsync("value");
            await (await Parent.Locator).SelectOptionAsync(value);
        }
    }
}