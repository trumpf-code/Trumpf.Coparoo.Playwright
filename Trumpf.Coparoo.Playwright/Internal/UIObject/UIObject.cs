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
using Trumpf.Coparoo.Playwright.Logging.Tree;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// DOM object base class.
/// </summary>
public abstract class UIObject : IUIObjectInternal
{
    private IUIObjectNode node;

    /// <summary>
    /// Gets the internal root node.
    /// </summary>
    internal ITabObjectInternal RootInternal
        => Root() as ITabObjectInternal;

    /// <summary>
    /// Gets the node type of this UI object.
    /// </summary>
    internal abstract NodeType NodeType { get; }

    /// <summary>
    /// Gets the tab object.
    /// </summary>
    public ITabObject Root()
        => (this as IUIObjectInternal).Root() as ITabObject;

    /// <summary>
    /// Gets the root page object.
    /// </summary>
    /// <returns>The root page object.</returns>
    ITabObject IUIObjectInternal.Root()
        => this is ITabObject
            ? this as ITabObject
            : (Parent as IUIObjectInternal).Root();

    /// <summary>
    /// Sets the 0-based control index.
    /// </summary>
    int IUIObject.Index
    {
        set
        {
            NodeInternal.Index = value;
        }
        get
        {
            return NodeInternal.Index;
        }
    }

    /// <summary>
    /// Gets the root node.
    /// </summary>
    IUIObjectNode IUIObject.Node
        => node;

    /// <summary>
    /// Gets the parent of this page object.
    /// </summary>
    public IUIObject Parent
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the node in the UI tree associated with this object.
    /// </summary>
    public IUIObjectNode Node
        => node;

    /// <summary>
    /// Gets the locator for this UI object.
    /// </summary>
    public Task<ILocator> Locator =>
        Node.Root();

    /// <summary>
    /// Gets the typed root node of the page object.
    /// This is the object this page object accesses to interact with UI elements.
    /// </summary>
    internal IUIObjectNodeInternal NodeInternal
        => (IUIObjectNodeInternal)Node;

    /// <summary>
    /// Initializes this object.
    /// </summary>
    /// <param name="parent">Parent page object.</param>
    /// <param name="root">The root node.</param>
    /// <returns>The initialized object.</returns>
    internal virtual IUIObject Init(IUIObject parent, IUIObjectNode root)
    {
        Parent = parent;
        node = root;

        return this;
    }

    /// <summary>
    /// Get the page object.
    /// </summary>
    /// <param name="condition">The condition that must evaluate true for the resulting page object.</param>
    /// <typeparam name="TPageObject">The target page object type.</typeparam>
    /// <returns>The page object.</returns>
    public TPageObject On<TPageObject>(Predicate<TPageObject> condition = null)
        where TPageObject : IPageObject
        => RootInternal.PageObjectLocator.Find(condition ?? (_ => true));

    /// <summary>
    /// Goto the page object.
    /// If the current page object cannot directly navigate to the target, it may forward it to its child page objects.
    /// Throws if the page object cannot be navigated to.
    /// </summary>
    /// <param name="condition">The condition that must evaluate true for target page object.</param>
    /// <typeparam name="TPageObject">The target page object type.</typeparam>
    /// <returns>The target page object.</returns>
    public TPageObject Goto<TPageObject>(Predicate<TPageObject> condition = null) where TPageObject : IPageObject
    {
        TPageObject r = On(condition ?? (_ => true));
        r.Goto();
        return r;
    }

    /// <summary>
    /// Initialize this object.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    /// <returns>The initialized page object.</returns>
    IUIObject IUIObjectInternal.Init(IUIObject parent)
    {
        return Init(parent);
    }

    /// <summary>
    /// Initialize this object.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    /// <returns>The initialized page object.</returns>
    internal virtual IUIObject Init(IUIObject parent)
    {
        IUIObjectNode node = CreateNode;
        ((IUIObjectNodeInternal)node).Init(parent.Node);
        Init(parent, node);

        return this;
    }

    /// <summary>
    /// Gets a fresh node object.
    /// </summary>
    internal abstract IUIObjectNode CreateNode { get; }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
        => GetType().FullName.GetHashCode();

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="pattern">The search pattern to locate the control.</param>
    /// <returns>The control object.</returns>
    public virtual TControl Find<TControl>(By pattern = null)
        where TControl : IControlObject
    {
        var result = (TControl)Activator.CreateInstance(((ITabObjectInternal)Root()).UIObjectInterfaceResolver.Resolve<TControl>());
        (result as IUIObjectInternal).Init(this);
        (result as IControlObjectInternal).Init(pattern);
        return result;
    }

    /// <summary>
    /// Gets all matching controls.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="pattern">The search pattern to locate the control.</param>
    /// <returns>The control enumeration.</returns>
    public virtual async IAsyncEnumerable<TControl> FindAll<TControl>(By pattern = null)
        where TControl : IControlObject
    {
        int next = 0;
        while (true)
        {
            TControl result = Find<TControl>(pattern);
            (result as IUIObjectInternal).Index = next++;

            var count = await (await result.Node.Root()).CountAsync();
            if (count == 0)
                break;

            yield return result;
        }
    }
}