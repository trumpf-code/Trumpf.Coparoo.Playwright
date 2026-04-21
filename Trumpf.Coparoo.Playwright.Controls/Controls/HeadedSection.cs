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
/// Section control object with heading access.
/// Wraps <c>&lt;section&gt;</c> elements that contain a heading (<c>h1</c>–<c>h6</c>).
/// </summary>
public class HeadedSection : Section, IHeadedSection
{
    /// <inheritdoc />
    public async Task<string> GetHeadingTextAsync()
        => await Locator.Locator("h1, h2, h3, h4, h5, h6").First.TextContentAsync() ?? string.Empty;
}
