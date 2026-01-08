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

using Trumpf.Coparoo.Playwright.Demo.PageObjects;

/// <summary>
/// Demo tab object using launched browser instead of CDP.
/// This simulates a CDP connection for testing purposes by launching a browser directly.
/// </summary>
/// <remarks>
/// In a real WPF/CefSharp scenario, you would use CdpTabObject with a real CDP endpoint.
/// This demo uses LaunchedBrowser to make it testable without requiring CefSharp.
/// </remarks>
public sealed class LaunchedBrowserDemoTab : TabObject
{
    private readonly bool headless;
    private readonly string pageIdentifier;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public LaunchedBrowserDemoTab(bool headless = true, string pageIdentifier = "default")
    {
        this.headless = headless;
        this.pageIdentifier = pageIdentifier;

        // Register page object relationships dynamically
        ChildOf<Shell, LaunchedBrowserDemoTab>();
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();
    }

    protected override string Url
    {
        get
        {
            var htmlPath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "demo.html");
            return $"file:///{htmlPath.Replace("\\", "/")}";
        }
    }

    protected override async Task<IPage> Creator()
    {
        // Store references for cleanup
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = headless ? 0 : 100
        });

        var page = await _browser.NewPageAsync();
        await page.GotoAsync(Url);
        return page;
    }

    /// <summary>
    /// Closes the tab and cleans up browser and playwright resources.
    /// </summary>
    public new async Task Close()
    {
        await base.Close();
        
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }
        
        if (_playwright != null)
        {
            _playwright.Dispose();
            _playwright = null;
        }
    }
}
