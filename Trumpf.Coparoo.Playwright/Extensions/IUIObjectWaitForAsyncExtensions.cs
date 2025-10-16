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

    /// <summary>
    /// Waits for the UI object node to become hidden (still in DOM but not visible).
    /// </summary>
    public static async Task WaitForHiddenAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await (await node.Node.Root()).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,

        });
    }

    /// <summary>
    /// Waits for the UI object node to be completely removed from the DOM.
    /// </summary>
    public static async Task WaitForDetachedAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await (await node.Node.Root()).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Detached,

        });
    }

    /// <summary>
    /// Waits for the UI object node to be completely attached to the DOM.
    /// </summary>
    public static async Task WaitForAttachedAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await (await node.Node.Root()).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached,

        });
    }
}
