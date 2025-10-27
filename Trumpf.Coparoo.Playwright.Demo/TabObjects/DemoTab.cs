namespace Trumpf.Coparoo.Playwright.Demo.TabObjects;

using Trumpf.Coparoo.Playwright.Demo.PageObjects;

/// <summary>
/// Root tab object for the demo application.
/// Demonstrates browser configuration and dynamic page object registration.
/// </summary>
public sealed class DemoTab : TabObject
{
    private readonly bool headless;

    public DemoTab(bool headless = true)
    {
        this.headless = headless;

        // Register page object relationships dynamically
        ChildOf<Shell, DemoTab>();
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();
    }

    protected override string Url
    {
        get
        {
            var htmlPath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "demo.html");
            return $"file:///{htmlPath.Replace("\\", "/")}";
        }
    }

    protected override async Task<IPage> Creator()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = headless ? 0 : 100
        });

        return await browser.NewPageAsync();
    }
}
