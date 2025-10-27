namespace Trumpf.Coparoo.Playwright.Demo.PageObjects;

using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Settings page object.
/// NOT declared as IChildOf&lt;Shell&gt; - relationship is registered dynamically in TabObject.
/// This demonstrates modular page objects that can be developed independently.
/// </summary>
public sealed class Settings : PageObject, ISettings
{
    protected override By SearchPattern => By.TestId("settings-page");

    public Checkbox EnableNotifications => Find<Checkbox>(By.TestId("enable-notifications"));
    public Checkbox EnableAutoSave => Find<Checkbox>(By.TestId("enable-autosave"));
    public Checkbox EnableDarkMode => Find<Checkbox>(By.TestId("enable-darkmode"));

    public override async Task Goto()
    {
        if (!await this.IsVisibleAsync())
        {
            await On<IShell>().Menu.NavigateToAsync(this);
            await this.WaitForVisibleAsync();
        }
    }
}
