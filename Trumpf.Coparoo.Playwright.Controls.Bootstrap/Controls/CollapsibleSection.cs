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
/// A collapsible section control (fieldset with expand/collapse behavior).
/// Override <see cref="HeaderSelector"/> and <see cref="BodySelector"/> to adapt to your CSS.
/// </summary>
public class CollapsibleSection : ControlObject, ICollapsibleSection
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => By.CssSelector("fieldset");

    /// <summary>
    /// Gets the CSS selector for the clickable header element. Override to customize.
    /// </summary>
    protected virtual string HeaderSelector => "legend";

    /// <summary>
    /// Gets the CSS selector for the collapsible body element. Override to customize.
    /// </summary>
    protected virtual string BodySelector => ".collapse";

    /// <inheritdoc />
    public Task ToggleAsync()
        => Locator.Locator(HeaderSelector).ClickAsync();

    /// <inheritdoc />
    public Task<bool> IsExpandedAsync()
        => Locator.Locator(BodySelector).IsVisibleAsync();
}
