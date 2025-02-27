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
/// Text input control object interface.
/// Expects an input html element with attribute type="text".
/// </summary>
public interface ITextInput : IControlObject
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    Task<string> GetName();

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    Task<string> GetValue();

    /// <summary>
    /// Sets the text content of the element to the specified value.
    /// </summary>
    /// <param name="value">The text value to set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetText(string value);
}