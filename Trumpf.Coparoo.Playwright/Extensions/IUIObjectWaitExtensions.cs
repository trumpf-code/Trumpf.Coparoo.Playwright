// Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extension methods for waiting on various states of <see cref="IUIObject"/>.
/// </summary>
public static class IUIObjectWaitExtensions
{
    /// <summary>
    /// Waits for the element to be attached to the DOM.
    /// </summary>
    /// <param name="source">The UI object to wait for.</param>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default timeout.</param>
    /// <returns>A task that completes when the element is attached.</returns>
    /// <exception cref="TimeoutException">Thrown when the timeout is exceeded.</exception>
    public static Task WaitForAttachedAsync(this IUIObject source, TimeSpan? timeout = null)
        => WaitForStateAsync(source, WaitForSelectorState.Attached, timeout);

    /// <summary>
    /// Waits for the element to be detached from the DOM.
    /// </summary>
    /// <param name="source">The UI object to wait for.</param>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default timeout.</param>
    /// <returns>A task that completes when the element is detached.</returns>
    /// <exception cref="TimeoutException">Thrown when the timeout is exceeded.</exception>
    public static Task WaitForDetachedAsync(this IUIObject source, TimeSpan? timeout = null)
        => WaitForStateAsync(source, WaitForSelectorState.Detached, timeout);

    /// <summary>
    /// Waits for the element to be visible.
    /// </summary>
    /// <param name="source">The UI object to wait for.</param>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default timeout.</param>
    /// <returns>A task that completes when the element is visible.</returns>
    /// <exception cref="TimeoutException">Thrown when the timeout is exceeded.</exception>
    public static Task WaitForVisibleAsync(this IUIObject source, TimeSpan? timeout = null)
        => WaitForStateAsync(source, WaitForSelectorState.Visible, timeout);

    /// <summary>
    /// Waits for the element to be hidden (not visible).
    /// </summary>
    /// <param name="source">The UI object to wait for.</param>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default timeout.</param>
    /// <returns>A task that completes when the element is hidden.</returns>
    /// <exception cref="TimeoutException">Thrown when the timeout is exceeded.</exception>
    public static Task WaitForHiddenAsync(this IUIObject source, TimeSpan? timeout = null)
        => WaitForStateAsync(source, WaitForSelectorState.Hidden, timeout);

    /// <summary>
    /// Helper method to wait for a specific selector state.
    /// </summary>
    /// <param name="source">The UI object to wait for.</param>
    /// <param name="state">The desired selector state.</param>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default timeout.</param>
    /// <returns>A task that completes when the element reaches the desired state.</returns>
    private static async Task WaitForStateAsync(IUIObject source, WaitForSelectorState state, TimeSpan? timeout)
    {
        var locator = await source.Locator;
        var options = new LocatorWaitForOptions { State = state };

        if (timeout.HasValue)
        {
            options.Timeout = (float)timeout.Value.TotalMilliseconds;
        }

        await locator.WaitForAsync(options);
    }
}
