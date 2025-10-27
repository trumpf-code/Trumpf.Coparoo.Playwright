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

namespace Trumpf.Coparoo.Playwright.Demo.ControlObjects;

using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Navigation menu control that provides convention-based navigation between pages.
/// This control demonstrates how to implement navigation logic that doesn't require
/// explicit knowledge of all possible target pages, enabling modular page object design.
/// </summary>
public sealed class Menu : ControlObject, IMenu
{
    /// <summary>
    /// Gets the search pattern for locating the navigation menu in the DOM.
    /// </summary>
    protected override By SearchPattern => By.CssSelector("[data-testid='main-menu']");

    /// <summary>
    /// Navigates to a page by clicking the menu item that corresponds to the page type.
    ///
    /// Convention: The page type name (e.g., "Settings") must match the value
    /// of the data-page attribute on the corresponding menu button element.
    ///
    /// Example: For ISettings interface implemented by Settings class,
    /// the menu button should have data-page="Settings".
    /// </summary>
    /// <param name="pageObject">The page object to navigate to.</param>
    /// <returns>A task representing the asynchronous click operation.</returns>
    public async Task NavigateToAsync(IPageObject pageObject)
    {
        var pageName = pageObject.GetType().Name.TrimStart('I'); // Remove 'I' prefix from interface names
        var menuItemLocator = Locator.Locator($"[data-page='{pageName}']");
        await menuItemLocator.ClickAsync();
    }
}
