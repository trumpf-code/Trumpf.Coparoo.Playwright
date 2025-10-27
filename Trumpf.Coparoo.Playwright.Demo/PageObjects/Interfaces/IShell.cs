namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Shell page interface.
/// </summary>
public interface IShell : IPageObject
{
    IMenu Menu { get; }
}
