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

using Trumpf.Coparoo.Playwright.Demo.ControlObjects;
using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.TabObjects;

/// <summary>
/// Page object representing the main application shell that contains the navigation menu.
/// This page object is explicitly defined as a child of <see cref="DemoTab"/>.
/// 
/// The Shell serves as the container for the entire application UI and provides
/// access to the navigation menu. However, it does NOT have explicit knowledge of specific
/// page implementations (Settings, Preferences) - these relationships are registered
/// dynamically in the TabObject constructor.
/// 
/// This separation enables teams to work independently on different sections of the application.
/// </summary>
public sealed class Shell : PageObject, IShell, IChildOf<DemoTab>
{
    /// <summary>
    /// Gets the search pattern for locating the application shell container.
    /// Uses the main container div that wraps the entire application.
    /// </summary>
    protected override By SearchPattern => By.CssSelector(".container");

    /// <summary>
    /// Gets the navigation menu control for switching between pages.
    /// </summary>
    public IMenu Menu => Find<Menu>();
}
