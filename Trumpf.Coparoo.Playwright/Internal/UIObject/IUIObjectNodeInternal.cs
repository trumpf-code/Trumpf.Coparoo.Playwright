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

namespace Trumpf.Coparoo.Playwright.Internal;



/// <summary>
/// The UI node interface.
/// </summary>
internal interface IUIObjectNodeInternal
{
    /// <summary>
    /// Gets the root.
    /// </summary>
    ILocator Locator();

    /// <summary>
    /// Gets the process node.
    /// </summary>
    ITabObjectNode RootNode { get; }

    /// <summary>
    /// Sets the 0-based control index.
    /// </summary>
    int Index { get; set; }

    /// <summary>
    /// Gets the search patter used to locate the node starting from the root.
    /// </summary>
    By SearchPattern { get; }

    /// <summary>
    /// Initialize this object.
    /// The parent node is used to search nodes without.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <returns>This object.</returns>
    IUIObjectNode Init(IUIObjectNode parent);
}