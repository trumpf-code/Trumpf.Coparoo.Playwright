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

using Trumpf.Coparoo.Playwright.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using Trumpf.Coparoo.Playwright.Internal;
using Trumpf.Coparoo.Playwright.Logging.Tree;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Tab object base class.
/// </summary>
public abstract class TabObject : PageObject, ITabObjectInternal, ITabObject
{
    private bool opened;
    private readonly PageObjectLocator pageObjectLocator;
    private readonly UIObjectInterfaceResolver objectInterfaceResolver;
    internal const string DEFAULT_FILE_PREFIX = "Coparoo";
    internal const string DEFAULT_DOT_PATH = @"C:\Program Files\Graphviz-12.0.0-win64\bin\dot.exe";
    private readonly Dictionary<Type, HashSet<Type>> dyanamicParentToChildMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TabObject"/> class.
    /// </summary>
    public TabObject()
    {
        TabObjectNode n = new();
        n.SetCreator(Creator());
        Init(null, n);
        pageObjectLocator = new PageObjectLocator(this);
        objectInterfaceResolver = new UIObjectInterfaceResolver();
    }

    /// <summary>
    /// Gets the node type of this UI object.
    /// </summary>
    internal override NodeType NodeType => NodeType.RootObject;

    /// <summary>
    /// Gets a fresh node object.
    /// </summary>
    internal override IUIObjectNode CreateNode => new TabObjectNode();

    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => throw new NotImplementedException();

    /// <summary>
    /// Gets the page instance for this tab.
    /// </summary>
    public Task<IPage> Page => ((TabObjectNode)Node).Page();

    /// <summary>
    /// Sets the page instance (primarily for testing scenarios).
    /// </summary>
    /// <param name="page">The IPage instance that represents the browser page to interact with.</param>
    /// <remarks>
    /// This method is typically used in test setups where you want to inject
    /// a pre-configured page instance instead of using the Creator() method.
    /// </remarks>
    public void SetPage(IPage page)
    {
        ((TabObjectNode)Node).SetPage(page);
    }

    /// <summary>
    /// Gets the page object locator.
    /// </summary>
    IPageObjectLocator ITabObjectInternal.PageObjectLocator 
        => pageObjectLocator;

    /// <summary>
    /// Gets the UI object interface resolver.
    /// </summary>
    IUIObjectInterfaceResolver ITabObjectInternal.UIObjectInterfaceResolver 
        => objectInterfaceResolver;

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public Configuration Configuration 
        => ((TabObjectNode)Node).Configuration;

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public Statistics Statistics 
        => ((TabObjectNode)Node).Statistics;

    /// <summary>
    /// Gets the URL.
    /// </summary>
    protected virtual string Url { get; } = null;

    /// <summary>
    /// Gets the page creator function.
    /// </summary>
    protected virtual Task<IPage> Creator()
        => null;

    /// <summary>
    /// Resolve the root object interface to a loaded process object in the current app domain.
    /// </summary>
    /// <typeparam name="TRootObject">The interface to resolve.</typeparam>
    /// <returns>The root object.</returns>
    public static TRootObject Resolve<TRootObject>() where TRootObject : ITabObject
    {
        var matches = Locate.Types.Where(e => !e.IsAbstract && e.GetConstructor(Type.EmptyTypes) != null && typeof(TRootObject).IsAssignableFrom(e)).ToArray();
        if (!matches.Any())
        {
            throw new TabObjectNotFoundException<TRootObject>();
        }
        else
        {
            var type = matches.First();
            return (TRootObject)Activator.CreateInstance(type);
        }
    }

    /// <summary>
    /// Register the page object type with the given object.
    /// </summary>
    /// <typeparam name="TChildPageObject">The child page object type.</typeparam>
    /// <typeparam name="TParentPageObject">The parent page object type.</typeparam>
    /// <returns>Whether the value was not yet registered.</returns>
    public bool ChildOf<TChildPageObject, TParentPageObject>()
    {
        if (typeof(TChildPageObject).IsAbstract || typeof(TChildPageObject).IsInterface)
        {
            throw new InvalidOperationException($"{typeof(TChildPageObject).FullName} must not be abstract nor an interface.");
        }

        if (typeof(TParentPageObject).IsAbstract || typeof(TParentPageObject).IsInterface)
        {
            throw new InvalidOperationException($"{typeof(TParentPageObject).FullName} must not be abstract nor an interface.");
        }

        if (dyanamicParentToChildMap.TryGetValue(typeof(TParentPageObject), out HashSet<Type> o))
        {
            return o.Add(typeof(TChildPageObject));
        }
        else
        {
            dyanamicParentToChildMap.Add(typeof(TParentPageObject), new HashSet<Type>() { typeof(TChildPageObject) });
            return true;
        }
    }

    /// <summary>
    /// Cast the tab object.
    /// </summary>
    /// <typeparam name="TTab">The type to cast to.</typeparam>
    /// <returns>The tab object with the new type.</returns>
    public TTab Cast<TTab>() where TTab : ITabObject
    {
        TTab result = Resolve<TTab>();
        result.SetPage(((TabObjectNode)Node).page);
        return result;
    }

    /// <summary>
    /// Gets the dynamically registered children of a page object.
    /// </summary>
    /// <param name="pageObjectType">The parent page object type.</param>
    /// <returns>The registered children types.</returns>
    IEnumerable<Type> ITabObjectInternal.DynamicChildren(Type pageObjectType) => dyanamicParentToChildMap.TryGetValue(pageObjectType, out HashSet<Type> value) ? value : new HashSet<Type>();

    /// <summary>
    /// Write the page object tree in the PDF format.
    /// </summary>
    /// <param name="filename">The filename to write to with extension.</param>
    /// <param name="dotBinaryPath">The hint path to the dot.exe binary file.</param>
    /// <returns>The absolute file name that was written.</returns>
    public string WriteTree(string filename = DEFAULT_FILE_PREFIX, string dotBinaryPath = DEFAULT_DOT_PATH) => ((ITreeObject)this).Tree.WriteGraph(filename, dotBinaryPath);

    /// <summary>
    /// Opens the tab if this has not yet been done.
    /// </summary>
    public override async Task Goto()
    {
        if (!opened)
        {
            await Open();
        }
    }

    /// <summary>
    /// Open the web page in a new tab (will open a new browser).
    /// </summary>
    public async Task Open()
    {
        await ((TabObjectNode)Node).Open(Url);
        opened = true;
    }

    /// <summary>
    /// Close the tab.
    /// </summary>
    public async Task Close()
    {
        await (await Locator).Page.CloseAsync();
        opened = false;
    }
}
