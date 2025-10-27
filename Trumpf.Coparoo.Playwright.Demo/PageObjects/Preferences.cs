namespace Trumpf.Coparoo.Playwright.Demo.PageObjects;

using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Preferences page object.
/// NOT declared as IChildOf&lt;Shell&gt; - relationship is registered dynamically in TabObject.
/// </summary>
public sealed class Preferences : PageObject, IPreferences
{
    protected override By SearchPattern => By.TestId("preferences-page");

    public Button SavePreferences => Find<Button>(By.TestId("save-preferences"));
    public Button ResetToDefaults => Find<Button>(By.TestId("reset-preferences"));
    public Button ExportSettings => Find<Button>(By.TestId("export-preferences"));

    public override async Task Goto()
    {
        if (!await this.IsVisibleAsync())
        {
            await On<IShell>().Menu.NavigateToAsync(this);
            await this.WaitForVisibleAsync();
        }
    }
}
