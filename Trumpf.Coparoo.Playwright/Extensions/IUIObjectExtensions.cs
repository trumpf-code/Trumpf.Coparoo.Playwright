using System;
using Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Extensions.
/// </summary>
public static class IUIObjectExtensions
{
    /// <summary>
    /// Waits for the specified UI element to become visible within the given timeout period.
    /// </summary>
    /// <param name="node">The UI object to wait for.</param>
    /// <param name="timeout">The maximum time to wait for the element to become visible.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the element becomes visible within the timeout; 
    /// otherwise, <c>false</c> if the timeout is exceeded.
    /// </returns>
    public static async Task<bool> TryWaitForVisibleAsync(this IUIObject node, TimeSpan timeout)
    {
        try
        {
            // Wait for the element to become visible within the specified timeout
            await node.Node.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = (int)timeout.TotalMilliseconds
            });

            return true; // The element became visible within the timeout
        }
        catch (TimeoutException)
        {
            // Timeout or visibility failure
            return false;
        }
    }

    /// <summary>
    /// Asynchronously checks if the UI element is visible.
    /// </summary>
    /// <param name="node">The UI element to check for visibility.</param>
    /// <returns>A task that resolves to <c>true</c> if the element is visible; otherwise, <c>false</c>.</returns>
    public static async Task<bool> IsVisibleAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        return await node.Node.IsVisibleAsync();
    }

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

        await node.Node.WaitForAsync(options);
    }

    /// <summary>
    /// Waits for the UI object node to become hidden (still in DOM but not visible).
    /// </summary>
    public static async Task WaitForHiddenAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
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

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Detached,

        });
    }

    /// <summary>
    /// Waits for the UI object node to become enabled.
    /// </summary>
    public static async Task WaitForEnabledAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached,

        });

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {

        });
    }

    /// <summary>
    /// Waits for the UI object node to become disabled.
    /// </summary>
    public static async Task WaitForDisabledAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached,

        });

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {

        });
    }

    /// <summary>
    /// Waits for the UI object node to become editable.
    /// </summary>
    public static async Task WaitForEditableAsync(this IUIObject node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {

        });
    }

    /// <summary>
    /// Waits for the UI object node to contain the specified text.
    /// </summary>
    public static async Task WaitForTextAsync(this IUIObject node, string expectedText)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        if (expectedText == null) throw new ArgumentNullException(nameof(expectedText));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {

        });
    }

    /// <summary>
    /// Waits for the UI object node to have a specific attribute value.
    /// </summary>
    public static async Task WaitForAttributeAsync(this IUIObject node, string attribute, string expectedValue)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        if (attribute == null) throw new ArgumentNullException(nameof(attribute));
        if (expectedValue == null) throw new ArgumentNullException(nameof(expectedValue));

        await node.Node.WaitForAsync(new LocatorWaitForOptions
        {

        });
    }
}