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
/// Text input control object.
/// Expects an input html element with attribute type="text".
/// </summary>
public class TextInput : ControlObject, ITextInput
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => "input";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public async Task<string> GetName()
         => await Node.EvaluateAsync<string>("input => input.name");

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public async Task<string> GetValue()
         => await Node.EvaluateAsync<string>("input => input.value");

    /// <summary>
    /// Sets the text content of the element to the specified value.
    /// This method is typically used for editable elements like input, textarea, etc.
    /// </summary>
    /// <param name="value">The text value to set in the element.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetText(string value)
    {
        var text = await GetValue();
        if (text != value)
        {
            if (text != string.Empty)
            {
                await Node.ClearAsync();
            }

            await Node.FillAsync(value);
        }
    }
}