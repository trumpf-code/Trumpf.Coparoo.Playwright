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

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Internal interface for root page objects.
/// </summary>
internal interface ITabObjectInternal : ITabObject
{
    /// <summary>
    /// Gets the page object locator.
    /// </summary>
    IPageObjectLocator PageObjectLocator { get; }

    /// <summary>
    /// Gets the UI object interface resolver.
    /// </summary>
    IUIObjectInterfaceResolver UIObjectInterfaceResolver { get; }

    /// <summary>
    /// Gets the dynamically registered children of a page object.
    /// </summary>
    /// <param name="pageObjectType">The parent page object type.</param>
    /// <returns>The registered children types.</returns>
    IEnumerable<Type> DynamicChildren(Type pageObjectType);
}