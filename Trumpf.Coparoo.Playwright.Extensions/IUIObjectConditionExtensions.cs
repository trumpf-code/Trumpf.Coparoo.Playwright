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
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extension accessors that expose a single boolean UI state of an <see cref="IUIObject"/> — enabled,
/// checked, editable, visible, or attached — as a composable <see cref="ElementCondition"/> handle
/// (web-first wait / wait-not). Prefer these over a one-shot <c>IsXxxAsync()</c> read when asserting a
/// state a UI action produces.
/// </summary>
public static class IUIObjectConditionExtensions
{
    /// <summary>
    /// Returns a handle to the element's <b>enabled</b> state.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>A condition handle for the enabled state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static ElementCondition Enabled(this IUIObject source)
        => Create(source, ConditionKind.Enabled);

    /// <summary>
    /// Returns a handle to the element's <b>checked</b> state (checkbox/radio).
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>A condition handle for the checked state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static ElementCondition Checked(this IUIObject source)
        => Create(source, ConditionKind.Checked);

    /// <summary>
    /// Returns a handle to the element's <b>editable</b> state.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>A condition handle for the editable state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static ElementCondition Editable(this IUIObject source)
        => Create(source, ConditionKind.Editable);

    /// <summary>
    /// Returns a handle to the element's <b>visible</b> state; <c>WaitForNotAsync()</c> waits until hidden.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>A condition handle for the visible state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static ElementCondition Visible(this IUIObject source)
        => Create(source, ConditionKind.Visible);

    /// <summary>
    /// Returns a handle to the element's <b>attached</b> (present in the DOM) state;
    /// <c>WaitForNotAsync()</c> waits until detached.
    /// </summary>
    /// <param name="source">The UI object.</param>
    /// <returns>A condition handle for the attached state.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static ElementCondition Attached(this IUIObject source)
        => Create(source, ConditionKind.Attached);

    private static ElementCondition Create(IUIObject source, ConditionKind kind)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new ElementCondition(source.Locator, kind);
    }
}
