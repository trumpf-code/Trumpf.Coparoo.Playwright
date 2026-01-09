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
using Trumpf.Coparoo.Playwright.Exceptions;
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Root page object node class.
/// </summary>
internal class TabObjectNode : UIObjectNode, ITabObjectNode
{
    internal IPage page;

    /// <summary>
    /// Gets the node representing this tree node in the UI, or null if not found
    /// It's the same as the root process.
    /// </summary>
    public override ILocator Locator()
    {
        if (page == null)
        {
            throw new TabObjectNotInitializedException("The TabObject has not been initialized with a page. Make sure to set the creator and initialize the TabObject before accessing the Locator.");
        }

        var x = page;
        var result = x.Locator("html");
        return result;
    }

    /// <summary>
    /// Gets the search patter used to locate the node starting from the root.
    /// </summary>
    public override By SearchPattern => throw new NotImplementedException();

    /// <summary>
    /// Gets the page creator.
    /// </summary>
    private Func<Task<IPage>> creator;
    public void SetCreator(Func<Task<IPage>> c)
    {
        creator = c;
    }

    public Task<IPage> Creator()
    {
        if (creator == null)
        {
            throw new TabObjectNotInitializedException("The TabObject has not been initialized with a creator function. Make sure to set the creator before accessing the Page.");
        }

        return creator();
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public Configuration Configuration { get; } = new Configuration();

    /// <summary>
    /// Gets the statistics.
    /// /// </summary>
    public Statistics Statistics { get; } = new Statistics();

    /// <summary>
    /// Gets or sets the page.
    /// </summary>
    public async Task<IPage> Page()
    {
        return page ??= await Creator();
    }

    public void SetPage(IPage page)
    {
        this.page = page;
    }

    /// <summary>
    /// Open the web page.
    /// </summary>
    /// <param name="url">The url to open.</param>
    public async Task Open(string url) 
        => await (await Page()).GotoAsync(url);
    
    /// <summary>
    /// Quit the browser.
    /// </summary>
    public async Task Quit() 
        => await (await Page()).Context.Browser.CloseAsync();
}