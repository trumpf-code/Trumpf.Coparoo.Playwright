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

namespace Trumpf.Coparoo.Playwright.Demo;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.TabObjects;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Demonstration test class showcasing the Coparoo.Playwright framework's capabilities.
///
/// This test demonstrates:
/// 1. Dynamic page object composition without explicit parent-child declarations
/// 2. Convention-based navigation using type names
/// 3. Team-independent page object development
/// 4. Both headless (CI-friendly) and headed (visual debugging) execution modes
///
/// Key Architectural Pattern:
/// The Settings and Preferences classes do NOT declare themselves as children
/// of any parent page object. Instead, their relationship to ApplicationShell is registered
/// dynamically in the DemoTabObject constructor using the ChildOf&lt;TChild, TParent&gt;() method.
///
/// This pattern enables:
/// - Team A to develop ApplicationShell independently
/// - Team B to develop Settings independently
/// - Team C to develop Preferences independently
/// - All teams can work in parallel without code conflicts
/// - New pages can be added without modifying existing page objects
/// - Pages can be distributed as separate modules/packages
///
/// Convention-Based Navigation:
/// The navigation menu uses a naming convention where the page type name (without 'I' prefix)
/// must match the data-page attribute in the HTML:
/// - ISettings → data-page="Settings"
/// - IPreferences → data-page="Preferences"
/// </summary>
[TestClass]
public sealed class DynamicCompositionDemo
{
    /// <summary>
    /// Demonstrates the complete workflow in headless mode (suitable for CI/CD pipelines).
    ///
    /// This test:
    /// 1. Opens the application in headless mode
    /// 2. Navigates to Settings page using Goto&lt;T&gt; pattern
    /// 3. Interacts with checkboxes (checking/unchecking)
    /// 4. Navigates to Preferences page
    /// 5. Clicks action buttons
    /// 6. Cleans up browser resources
    ///
    /// The headless mode is ideal for:
    /// - Continuous Integration pipelines
    /// - Automated regression testing
    /// - Fast execution without GUI overhead
    /// </summary>
    [TestMethod]
    public async Task DemonstrateModularPageComposition_Headless()
    {
        // Arrange: Create tab object in headless mode
        var tab = new DemoTabObject(headless: true);

        try
        {
            // Act: Open the application
            await tab.Open();

            // Get reference to Settings page (it's already active/visible by default in HTML)
            // Note: Settings doesn't explicitly know about its parent relationship
            var settingsPage = tab.On<ISettings>();

            // Interact with checkboxes on Settings page
            await settingsPage.EnableNotifications.Check();
            (await settingsPage.EnableNotifications.IsChecked).Should().BeTrue();

            await settingsPage.EnableAutoSave.Check();
            (await settingsPage.EnableAutoSave.IsChecked).Should().BeTrue();

            await settingsPage.EnableDarkMode.Check();
            (await settingsPage.EnableDarkMode.IsChecked).Should().BeTrue();

            // Uncheck one to demonstrate state change
            await settingsPage.EnableAutoSave.Uncheck();
            (await settingsPage.EnableAutoSave.IsChecked).Should().BeFalse();

            // Navigate to Preferences page using Goto (which triggers navigation via menu)
            // Again, Preferences doesn't explicitly know about its parent
            var preferencesPage = tab.Goto<IPreferences>();

            // Interact with buttons on Preferences page
            await preferencesPage.SavePreferences.ClickAsync();
            await preferencesPage.ResetToDefaults.ClickAsync();
            await preferencesPage.ExportSettings.ClickAsync();

            // Navigate back to Settings - demonstrates clean navigation
            tab.Goto<ISettings>();
        }
        finally
        {
            // Cleanup
            await tab.Close();
        }
    }

    /// <summary>
    /// Demonstrates the same workflow in headed mode (visible browser for debugging).
    ///
    /// This test is identical in functionality to the headless version but runs with
    /// a visible browser window, making it useful for:
    /// - Visual debugging during development
    /// - Demonstrating functionality to stakeholders
    /// - Troubleshooting test failures
    /// - Understanding timing and interaction issues
    ///
    /// Note: This test is excluded from CI pipelines using TestCategory attribute.
    /// </summary>
    [TestMethod]
    [TestCategory("VisualTest")] // Exclude from CI - only run manually
    public async Task DemonstrateModularPageComposition_Headed()
    {
        // Arrange: Create tab object in headed mode (visible browser)
        var tab = new DemoTabObject(headless: false);

        try
        {
            // Act: Open the application
            await tab.Open();

            // Navigate to Settings page
            var settingsPage = tab.Goto<ISettings>();
            await Task.Delay(500); // Pause for visual observation

            // Interact with checkboxes - slower for visibility
            await settingsPage.EnableNotifications.Check();
            await Task.Delay(400);

            await settingsPage.EnableAutoSave.Check();
            await Task.Delay(400);

            await settingsPage.EnableDarkMode.Check();
            await Task.Delay(400);

            await settingsPage.EnableAutoSave.Uncheck();
            await Task.Delay(400);

            // Navigate to Preferences page
            var preferencesPage = tab.Goto<IPreferences>();
            await Task.Delay(500);

            // Interact with buttons - with pauses for visual feedback
            await preferencesPage.SavePreferences.ClickAsync();
            await Task.Delay(600);

            await preferencesPage.ResetToDefaults.ClickAsync();
            await Task.Delay(600);

            await preferencesPage.ExportSettings.ClickAsync();
            await Task.Delay(600);

            // Navigate back to Settings
            var settingsPageAgain = tab.Goto<ISettings>();
            await Task.Delay(500);

            // Keep browser open briefly for final observation
            await Task.Delay(1000);
        }
        finally
        {
            // Cleanup
            await tab.Close();
        }
    }
}