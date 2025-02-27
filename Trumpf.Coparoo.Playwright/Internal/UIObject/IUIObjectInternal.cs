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

using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Internal interface for the UI object base class.
/// </summary>
internal interface IUIObjectInternal : IUIObject
{
    /// <summary>
    /// Gets the root page object.
    /// </summary>
    ITabObject Root();

    /// <summary>
    /// Initialize this object.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    /// <returns>The initialized page object.</returns>
    IUIObject Init(IUIObject parent);
}