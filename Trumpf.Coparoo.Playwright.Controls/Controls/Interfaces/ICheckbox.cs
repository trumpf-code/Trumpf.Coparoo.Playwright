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
/// Checkbox control object interface.
/// Expects an input html element with attribute type="checkbox".
/// </summary>
public interface ICheckbox : IControlObject
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    Task<string> Value { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    Task<string> Name { get; }

    /// <summary>
    /// Gets a value indicating whether the element is currently checked.
    /// </summary>
    /// <returns>A task that resolves to <c>true</c> if the element is checked; otherwise, <c>false</c>.</returns>
    Task<bool> IsChecked { get; }

    /// <summary>
    /// Ensures that the element is checked. 
    /// If the element is not already checked, this method will check it.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Check();

    /// <summary>
    /// Ensures that the element is unchecked. 
    /// If the element is already checked, this method will uncheck it.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Uncheck();
}