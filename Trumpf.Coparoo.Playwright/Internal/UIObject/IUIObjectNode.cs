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
using Microsoft.Playwright;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// The UI node interface.
/// </summary>
public interface IUIObjectNode
{
    /// <summary>
    /// Gets the root locator for this UI object node.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the root locator.</returns>
    Task<ILocator> Root();

    /// <summary>
    /// Initializes the control object with the specified search pattern.
    /// </summary>
    /// <param name="pattern">The search pattern used to locate the control.</param>
    void Init(By pattern);
}