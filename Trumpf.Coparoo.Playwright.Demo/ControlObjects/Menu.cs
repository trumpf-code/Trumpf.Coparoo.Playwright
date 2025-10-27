namespace Trumpf.Coparoo.Playwright.Demo.ControlObjects;

using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Navigation menu control with convention-based page navigation.
/// </summary>
public sealed class Menu : ControlObject, IMenu
{
    protected override By SearchPattern => By.TestId("main-menu");

    /// <summary>
    /// Navigates to a page by clicking the menu item matching the page type name.
    /// </summary>
    public async Task NavigateToAsync(IPageObject pageObject)
    {
        var pageName = pageObject.GetType().Name.TrimStart('I');
        var menuItemLocator = Locator.Locator($"[data-page='{pageName}']");
        await menuItemLocator.ClickAsync();
    }
}
