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

namespace Trumpf.Coparoo.Playwright.Controls.Bootstrap;

/// <summary>
/// Bootstrap dropdown selector control object interface.
/// Wraps elements with the <c>.dropdown</c> CSS class.
/// </summary>
public interface IDropdownSelector : IControlObject
{
    /// <summary>
    /// Gets the currently displayed value from the dropdown toggle button.
    /// </summary>
    Task<string> GetSelectedValueAsync();

    /// <summary>
    /// Opens the dropdown and clicks the item with the given text.
    /// </summary>
    /// <param name="value">The display text of the item to select.</param>
    Task SelectAsync(string value);

    /// <summary>
    /// Waits until the dropdown toggle displays the expected value.
    /// </summary>
    /// <param name="expected">The expected display text.</param>
    /// <param name="timeout">Optional timeout (defaults to 5 seconds).</param>
    Task WaitForSelectedValueAsync(string expected, TimeSpan? timeout = null);

    /// <summary>
    /// Gets all available dropdown item texts.
    /// </summary>
    Task<IReadOnlyList<string>> GetOptionsAsync();
}
