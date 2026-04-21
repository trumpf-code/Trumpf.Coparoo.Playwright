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

using System.Globalization;
using System.Text.RegularExpressions;
using Trumpf.Coparoo.Playwright.Extensions;

namespace Trumpf.Coparoo.Playwright.Controls.Bootstrap;

/// <summary>
/// Bootstrap progress bar segment control object.
/// Wraps elements with the <c>.progress-bar</c> CSS class.
/// </summary>
public class ProgressBarSegment : ControlObject, IProgressBarSegment
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => By.CssSelector(".progress-bar");

    /// <inheritdoc />
    public async Task<string> GetCssClassesAsync()
        => await this.GetAttributeAsync("class") ?? "";

    /// <inheritdoc />
    public async Task<double?> GetWidthPercentAsync()
    {
        var style = await this.GetAttributeAsync("style") ?? "";
        var match = Regex.Match(style, @"width:\s*([\d.]+)%");
        return match.Success
            ? double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)
            : null;
    }

    /// <inheritdoc />
    public async Task<string> GetComputedBackgroundColorAsync()
        => await Locator.EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor");
}
