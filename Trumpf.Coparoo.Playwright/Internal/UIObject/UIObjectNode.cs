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
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Page object node class wrapping a UI tree node.
/// </summary>
internal class UIObjectNode : IUIObjectNode, IUIObjectNodeInternal
{
    private By searchPattern;
    private IUIObjectNode mParent;
    private ITabObjectNode mRootNode = null;

    /// <summary>
    /// Gets the root to node search pattern.
    /// </summary>
    public virtual By SearchPattern
        => searchPattern;

    /// <summary>
    /// Sets the 0-based control index.
    /// </summary>
    public int Index { get; set; } = 0;

    /// <summary>
    /// Gets the process node.
    /// </summary>
    public ITabObjectNode RootNode
        => mRootNode ??= this is ITabObjectNode
            ? this as ITabObjectNode
            : ((IUIObjectNodeInternal)mParent).RootNode;

    /// <summary>
    /// Gets the root node.
    /// </summary>
    protected virtual Task<ILocator> Parent
        => (mParent as IUIObjectNodeInternal).Locator();

    /// <summary>
    /// Gets the node representing this tree node in the UI.
    /// </summary>
    public async virtual Task<ILocator> Locator()
        => (await Parent).Locator(SearchPattern.ToLocator()).Nth(Index);

    /// <summary>
    /// Initialize this object.
    /// The parent node is used to search nodes without, hence disabling any caching.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <returns>This object.</returns>
    public IUIObjectNode Init(IUIObjectNode parent)
    {
        mParent = parent;
        return this;
    }

    /// <summary>
    /// Initialize the control object.
    /// </summary>
    /// <param name="searchPattern">The search pattern used to locate the control.</param>
    public void Init(By searchPattern)
        => this.searchPattern = searchPattern;
}