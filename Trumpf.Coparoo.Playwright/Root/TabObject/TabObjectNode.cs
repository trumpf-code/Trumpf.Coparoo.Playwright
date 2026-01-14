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
    /// Gets or creates the page instance using the provided factory method.
    /// </summary>
    /// <param name="factory">The factory method that creates the page instance.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the IPage instance.</returns>
    /// <remarks>
    /// This method implements lazy initialization - the factory is only called once on the first access.
    /// Subsequent calls return the cached page instance.
    /// </remarks>
    public async Task<IPage> GetOrCreatePageAsync(Func<Task<IPage>> factory)
    {
        return page ??= await factory();
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
    /// Sets the page instance directly (primarily for testing scenarios).
    /// </summary>
    /// <param name="page">The IPage instance to set.</param>
    public void SetPage(IPage page)
    {
        this.page = page;
    }

    /// <summary>
    /// Open the web page.
    /// </summary>
    /// <param name="url">The url to open.</param>
    public async Task Open(string url) 
        => await page.GotoAsync(url);
    
    /// <summary>
    /// Quit the browser.
    /// </summary>
    public async Task Quit() 
        => await page.Context.Browser.CloseAsync();
}