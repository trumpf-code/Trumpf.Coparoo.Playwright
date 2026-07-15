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

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// A composable, web-first handle to a single boolean UI state — <b>enabled</b>, <b>checked</b>,
/// <b>editable</b>, <b>visible</b>, or <b>attached</b> — of an element. Obtain one via a state accessor
/// on <see cref="IUIObjectConditionExtensions"/> (e.g. <c>button.Enabled()</c> or <c>panel.Visible()</c>),
/// then wait for the state to hold (<see cref="WaitForAsync"/>) or to clear (<see cref="WaitForNotAsync"/>).
/// The negation reads naturally: <c>Visible().WaitForNotAsync()</c> waits until hidden and
/// <c>Attached().WaitForNotAsync()</c> until detached. The waits retry until the state (or its negation)
/// lands, so they are robust against asynchronously applied UI changes where a one-shot read would race
/// the update. For one-shot reads use the <c>IsXxxAsync()</c> query extensions instead.
/// </summary>
public readonly struct ElementCondition
{
    private readonly ILocator locator;
    private readonly ConditionKind kind;

    internal ElementCondition(ILocator locator, ConditionKind kind)
    {
        this.locator = locator;
        this.kind = kind;
    }

    /// <summary>
    /// Waits (retrying) until the state holds — e.g. <c>await button.Enabled().WaitForAsync()</c>.
    /// </summary>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default assertion timeout.</param>
    /// <returns>A task that completes when the state holds.</returns>
    /// <exception cref="PlaywrightException">Thrown when the state does not hold within the timeout.</exception>
    public Task WaitForAsync(TimeSpan? timeout = null)
    {
        float? ms = timeout.HasValue ? (float)timeout.Value.TotalMilliseconds : (float?)null;
        var expect = Assertions.Expect(locator);
        return kind switch
        {
            ConditionKind.Enabled => expect.ToBeEnabledAsync(new LocatorAssertionsToBeEnabledOptions { Timeout = ms }),
            ConditionKind.Checked => expect.ToBeCheckedAsync(new LocatorAssertionsToBeCheckedOptions { Timeout = ms }),
            ConditionKind.Editable => expect.ToBeEditableAsync(new LocatorAssertionsToBeEditableOptions { Timeout = ms }),
            ConditionKind.Visible => expect.ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = ms }),
            ConditionKind.Attached => expect.ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions { Timeout = ms }),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    /// <summary>
    /// Waits (retrying) until the state does NOT hold — e.g. <c>await button.Enabled().WaitForNotAsync()</c>
    /// waits until the button becomes disabled.
    /// </summary>
    /// <param name="timeout">Optional timeout. If not specified, uses Playwright's default assertion timeout.</param>
    /// <returns>A task that completes when the state no longer holds.</returns>
    /// <exception cref="PlaywrightException">Thrown when the state still holds after the timeout.</exception>
    public Task WaitForNotAsync(TimeSpan? timeout = null)
    {
        float? ms = timeout.HasValue ? (float)timeout.Value.TotalMilliseconds : (float?)null;
        var expect = Assertions.Expect(locator).Not;
        return kind switch
        {
            ConditionKind.Enabled => expect.ToBeEnabledAsync(new LocatorAssertionsToBeEnabledOptions { Timeout = ms }),
            ConditionKind.Checked => expect.ToBeCheckedAsync(new LocatorAssertionsToBeCheckedOptions { Timeout = ms }),
            ConditionKind.Editable => expect.ToBeEditableAsync(new LocatorAssertionsToBeEditableOptions { Timeout = ms }),
            ConditionKind.Visible => expect.ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = ms }),
            ConditionKind.Attached => expect.ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions { Timeout = ms }),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
