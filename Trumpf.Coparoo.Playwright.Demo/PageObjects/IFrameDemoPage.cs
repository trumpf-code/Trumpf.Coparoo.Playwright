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

using System.Threading.Tasks;
using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.ControlObjects;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// IFrame demo main page object.
/// Demonstrates page with embedded iframe controls.
/// </summary>
public sealed class IFrameDemoPage : PageObject, IIFrameDemoPage
{
    /// <summary>
    /// Gets the search pattern for locating this page.
    /// </summary>
    protected override By SearchPattern => By.TestId("demo-title");

    /// <summary>
    /// Gets the main page button control.
    /// </summary>
    public Button MainPageButton => Find<Button>(By.TestId("main-button"));

    /// <summary>
    /// Gets the result display locator.
    /// </summary>
    public ILocator Result => Locator.GetByTestId("result");

    /// <summary>
    /// Gets the rich text editor frame control.
    /// </summary>
    public RichTextEditorFrame RichTextEditor => Find<RichTextEditorFrame>();

    /// <summary>
    /// Navigates to this page using the shell menu if not already visible.
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
