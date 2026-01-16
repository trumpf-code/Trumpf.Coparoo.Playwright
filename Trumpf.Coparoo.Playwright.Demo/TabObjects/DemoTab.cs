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

namespace Trumpf.Coparoo.Playwright.Demo.TabObjects;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Demo.PageObjects;
using Trumpf.Coparoo.Playwright.Demo;

/// <summary>
/// Root tab object for the demo application.
/// Demonstrates browser configuration and dynamic page object registration.
/// Hosts both the basic demo pages and the iframe demo on a single HTML.
/// </summary>
public sealed class DemoTab : TabObject
{
    private readonly bool headless;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoTab"/> class.
    /// </summary>
    /// <param name="headless">Whether to run in headless mode.</param>
    public DemoTab(bool headless = true)
    {
        this.headless = headless;

        // Register Demo relationships
        ChildOf<Shell, DemoTab>();
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();

        // Register IFrame demo relationships
        ChildOf<IFrameDemoPage, DemoTab>();
        ChildOf<PaymentFrame, DemoTab>();
    }

    /// <summary>
    /// Gets the file URL to the demo HTML.
    /// </summary>
    protected override string Url
    {
        get
        {
            var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "demo.html");
            return $"file:///{htmlPath.Replace("\\", "/")}";
        }
    }

    /// <summary>
    /// Creates the Playwright page instance.
    /// </summary>
    protected override async Task<IPage> CreatePageAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = headless ? 0 : 100
        });

        return await browser.NewPageAsync();
    }
}
