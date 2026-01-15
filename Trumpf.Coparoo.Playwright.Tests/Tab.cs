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

namespace Trumpf.Coparoo.Tests;

using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Test helper class for creating tab objects with HTML content.
/// </summary>
public class Tab : TabObject
{
    /// <summary>
    /// Creates a new tab object with the specified HTML content.
    /// </summary>
    /// <param name="htmlContent">The HTML content to load in the tab.</param>
    /// <returns>A new tab object instance.</returns>
    public static async Task<Tab> CreateAsync(string htmlContent)
    {
        var tab = new Tab();
        await tab.Open();
        var page = await tab.Page;
        await page.SetContentAsync(htmlContent);
        await tab.WaitForVisibleAsync();
        return tab;
    }

    protected override async Task<IPage> CreatePageAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        return await browser.NewPageAsync();
    }

    protected override string Url => "about:blank";
}
