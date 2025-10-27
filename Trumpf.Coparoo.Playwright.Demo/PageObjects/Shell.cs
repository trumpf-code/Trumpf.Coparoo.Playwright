namespace Trumpf.Coparoo.Playwright.Demo.PageObjects;

using Trumpf.Coparoo.Playwright.Demo.ControlObjects;
using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.TabObjects;

/// <summary>
/// Main application shell containing the navigation menu.
/// Child of <see cref="DemoTab"/>.
/// </summary>
public sealed class Shell : PageObject, IShell, IChildOf<DemoTab>
{
    protected override By SearchPattern => By.TestId("app-shell");

    public IMenu Menu => Find<Menu>();
}
