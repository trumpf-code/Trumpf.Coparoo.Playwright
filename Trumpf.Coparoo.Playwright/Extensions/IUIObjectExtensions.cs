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

    /// <summary>
    /// Asynchronously checks if the UI element is enabled.
    /// </summary>
    /// <param name="source">The UI element to check for being enabled.</param>
    /// <returns>A task that resolves to <c>true</c> if the element is enable; otherwise, <c>false</c>.</returns>
    public static async Task<bool> Enabled(this IUIObject source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var locator = await source.Locator;
        return await locator.IsEnabledAsync();
    }
}