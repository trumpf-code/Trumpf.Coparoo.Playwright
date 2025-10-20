using System;
using Trumpf.Coparoo.Playwright.Internal;
using Trumpf.Coparoo.Playwright.Logging.Tree;

/// <summary>
/// Extensions.
/// </summary>
public static class IUIObjectExtensions
{
    /// <summary>
    /// Checks if the source exists by verifying if the count of matching elements is greater than zero.
    /// </summary>
    /// <returns>
    /// A boolean indicating whether the source exists (true if count is greater than zero, false otherwise).
    /// </returns>
    public static async Task<bool> Exists(this IUIObject source)
    {
        var locator = await source.Locator;
        return await locator.CountAsync() > 0;
    }
}