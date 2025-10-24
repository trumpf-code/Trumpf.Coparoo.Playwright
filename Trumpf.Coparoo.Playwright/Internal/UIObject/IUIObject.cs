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
using System.Collections.Generic;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// DOM object interface.
/// </summary>
public interface IUIObject
{
    /// <summary>
    /// Sets the 0-based control index.
    /// </summary>
    int Index { set; get; }

    /// <summary>
    /// Gets the locator for this UI object.
    /// </summary>
    ILocator Locator { get; }

    /// <summary>
    /// Get the specific page object.
    /// </summary>
    /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
    /// <typeparam name="TPageObject">Type of the page object.</typeparam>
    /// <returns>Type of page object.</returns>
    TPageObject On<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject;

    /// <summary>
    /// Goto the page object.
    /// Throws if the page object cannot be navigated to.
    /// </summary>
    /// <param name="condition">The condition that must evaluate true for target page object.</param>
    /// <typeparam name="TPageObject">The target page object type.</typeparam>
    /// <returns>The target page object.</returns>
    TPageObject Goto<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject;

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="pattern">The search pattern to locate the control.</param>
    /// <returns>The control object.</returns>
    TControl Find<TControl>(By pattern = null) where TControl : IControlObject;

    /// <summary>
    /// Gets all matching controls.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="pattern">The search pattern to locate the control.</param>
    /// <returns>The control enumeration.</returns>
    IAsyncEnumerable<TControl> FindAll<TControl>(By pattern = null) where TControl : IControlObject;
}