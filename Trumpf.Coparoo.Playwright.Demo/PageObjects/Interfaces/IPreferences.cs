namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Preferences page interface.
/// </summary>
public interface IPreferences : IPageObject
{
    Button SavePreferences { get; }
    Button ResetToDefaults { get; }
    Button ExportSettings { get; }
}
