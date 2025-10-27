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

namespace Trumpf.Coparoo.Playwright.Demo.PageObjects;

using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Page object for the Preferences page.
///
/// IMPORTANT: This page object is intentionally NOT declared as IChildOf&lt;Shell&gt;.
/// Instead, the parent-child relationship is registered dynamically in the TabObject constructor.
///
/// This design pattern demonstrates a realistic scenario where:
/// - This page object could be maintained by a different team (Team Preferences)
/// - The team doesn't need to know about or depend on the Shell implementation
/// - The page can be developed, tested, and distributed as an independent module
/// - Integration happens through convention-based registration at runtime
///
/// In practice, this enables:
/// - Multiple teams working on different features independently
/// - Pages distributed across different NuGet packages
/// - Plugin-style architecture where pages can be added without modifying core code
///
/// Note: This page demonstrates different control types (Button) compared to Settings (Checkbox),
/// showcasing the framework's flexibility in handling various UI elements.
/// </summary>
public sealed class Preferences : PageObject, IPreferences
{
    /// <summary>
    /// Gets the search pattern for locating the preferences page in the DOM.
    /// </summary>
    protected override By SearchPattern => By.CssSelector("[data-testid='preferences-page']");

    /// <summary>
    /// Gets the button control for saving user preferences.
    /// </summary>
    public Button SavePreferences => Find<Button>(By.Id("save-preferences"));

    /// <summary>
    /// Gets the button control for resetting preferences to default values.
    /// </summary>
    public Button ResetToDefaults => Find<Button>(By.Id("reset-preferences"));

    /// <summary>
    /// Gets the button control for exporting user settings.
    /// </summary>
    public Button ExportSettings => Find<Button>(By.Id("export-preferences"));

    /// <summary>
    /// Navigates to the preferences page by clicking the corresponding menu item.
    /// This override implements the convention-based navigation pattern.
    /// Waits for the page to become visible after navigation.
    /// </summary>
    public override async Task Goto()
    {
        if (!await this.IsVisibleAsync())
        {
            await On<IShell>().Menu.NavigateToAsync(this);
            await this.WaitForVisibleAsync();
        }
    }
}
