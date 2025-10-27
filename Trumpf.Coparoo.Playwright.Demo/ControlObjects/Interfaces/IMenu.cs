namespace Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Navigation menu control interface.
/// </summary>
public interface IMenu : IControlObject
{
    Task NavigateToAsync(IPageObject pageObject);
}
