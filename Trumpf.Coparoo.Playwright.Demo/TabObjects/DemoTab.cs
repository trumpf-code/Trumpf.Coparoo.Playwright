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
/// Root tab object for the Coparoo Playwright Demo application.
///
/// This tab object demonstrates how to:
/// 1. Configure browser instances (headless vs. headed mode)
/// 2. Load a local HTML file for testing
/// 3. Register page objects dynamically using conventions
///
/// The dynamic registration pattern shown here is particularly useful in scenarios where:
/// - Multiple teams work on different parts of the application independently
/// - Page objects are distributed across different modules/packages
/// - The main application doesn't need to know about all possible child pages
/// </summary>
public sealed class DemoTab : TabObject
{
    private readonly bool headless;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoTab"/> class.
    /// </summary>
    /// <param name="headless">
    /// Determines whether the browser runs in headless mode.
    /// Set to false to see the browser interactions during test execution.
    /// </param>
    public DemoTab(bool headless = true)
    {
        this.headless = headless;

        // Register page object relationships dynamically.
        // This demonstrates the convention-based composition pattern where:
        // - Shell is a child of DemoTab
        // - Settings is a child of Shell (but defined independently)
        // - Preferences is a child of Shell (but defined independently)
        //
        // In a real-world scenario with multiple teams:
        // - Team A maintains DemoTab and Shell
        // - Team B maintains Settings independently
        // - Team C maintains Preferences independently
        // - Teams B and C don't need to modify Team A's code to integrate their pages
        ChildOf<Shell, DemoTab>();
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();
    }

    /// <summary>
    /// Gets the URL of the demo HTML page.
    /// Resolves to the local file in the wwwroot directory.
    /// </summary>
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

    /// <summary>
    /// Creates and configures a new browser page instance.
    /// </summary>
    /// <returns>A configured IPage instance for browser automation.</returns>
    protected override async Task<IPage> Creator()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = headless ? 0 : 100 // Slow down actions in headed mode for visibility
        });

        return await browser.NewPageAsync();
    }
}
