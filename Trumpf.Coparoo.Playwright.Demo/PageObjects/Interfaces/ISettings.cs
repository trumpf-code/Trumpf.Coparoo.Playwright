namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Settings page interface.
/// </summary>
public interface ISettings : IPageObject
{
    Checkbox EnableNotifications { get; }
    Checkbox EnableAutoSave { get; }
    Checkbox EnableDarkMode { get; }
}
