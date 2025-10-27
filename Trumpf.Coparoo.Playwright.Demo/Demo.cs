namespace Trumpf.Coparoo.Playwright.Demo;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.TabObjects;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Demonstration tests showcasing dynamic page composition and convention-based navigation.
/// </summary>
[TestClass]
public sealed class Demo
{
    /// <summary>
    /// Demonstrates navigation and interaction in headless mode (CI-friendly).
    /// </summary>
    [TestMethod]
    public async Task DemonstrateModularPageComposition_Headless()
    {
        var tab = new DemoTab(headless: true);

        try
        {
            await tab.Open();

            var settingsPage = tab.On<ISettings>();

            await settingsPage.EnableNotifications.Check();
            (await settingsPage.EnableNotifications.IsChecked).Should().BeTrue();

            await settingsPage.EnableAutoSave.Check();
            (await settingsPage.EnableAutoSave.IsChecked).Should().BeTrue();

            await settingsPage.EnableDarkMode.Check();
            (await settingsPage.EnableDarkMode.IsChecked).Should().BeTrue();

            await settingsPage.EnableAutoSave.Uncheck();
            (await settingsPage.EnableAutoSave.IsChecked).Should().BeFalse();

            var preferencesPage = tab.Goto<IPreferences>();

            await preferencesPage.SavePreferences.ClickAsync();
            await preferencesPage.ResetToDefaults.ClickAsync();
            await preferencesPage.ExportSettings.ClickAsync();

            tab.Goto<ISettings>();
        }
        finally
        {
            await tab.Close();
        }
    }

    /// <summary>
    /// Demonstrates navigation and interaction in headed mode (visible browser for debugging).
    /// </summary>
    [TestMethod]
    [TestCategory("VisualTest")]
    public async Task DemonstrateModularPageComposition_Headed()
    {
        var tab = new DemoTab(headless: false);

        try
        {
            await tab.Open();

            var settingsPage = tab.Goto<ISettings>();
            await Task.Delay(500);

            await settingsPage.EnableNotifications.Check();
            await Task.Delay(400);

            await settingsPage.EnableAutoSave.Check();
            await Task.Delay(400);

            await settingsPage.EnableDarkMode.Check();
            await Task.Delay(400);

            await settingsPage.EnableAutoSave.Uncheck();
            await Task.Delay(400);

            var preferencesPage = tab.Goto<IPreferences>();
            await Task.Delay(500);

            await preferencesPage.SavePreferences.ClickAsync();
            await Task.Delay(600);

            await preferencesPage.ResetToDefaults.ClickAsync();
            await Task.Delay(600);

            await preferencesPage.ExportSettings.ClickAsync();
            await Task.Delay(600);

            var settingsPageAgain = tab.Goto<ISettings>();
            await Task.Delay(500);

            await Task.Delay(1000);
        }
        finally
        {
            await tab.Close();
        }
    }
}
