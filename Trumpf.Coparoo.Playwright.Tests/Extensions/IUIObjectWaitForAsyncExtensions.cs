using System;
using Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Extensions.
/// </summary>
public static class IUIObjectWaitForAsyncExtensions
{
    /// <summary>
    /// Waits for the UI object node to become visible.
    /// </summary>
    public static async Task WaitForVisibleAsync(this IUIObject node, TimeSpan timeout = default)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        var options = new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        };
        if (timeout != default)
            options.Timeout = timeout.Milliseconds;

        await (await node.Node.Root()).WaitForAsync(options);
    }
}
