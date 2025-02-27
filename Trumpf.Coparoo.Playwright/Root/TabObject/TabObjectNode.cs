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
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Root page object node class.
/// </summary>
internal class TabObjectNode : UIObjectNode, ITabObjectNode
{
    internal IPage driver;

    /// <summary>
    /// Gets the node representing this tree node in the UI, or null if not found
    /// It's the same as the root process.
    /// </summary>
    public async override Task<ILocator> Root()
    {
        var x = await Driver();
        var result = x.Locator("html");
        return result;
    }

    /// <summary>
    /// Gets the root node.
    /// </summary>
    protected override Task<ILocator> Parent 
        => throw new InvalidOperationException("A root has no parent");

    /// <summary>
    /// Gets the search patter used to locate the node starting from the root.
    /// </summary>
    public override By SearchPattern => throw new NotImplementedException();

    /// <summary>
    /// Gets the driver generator.
    /// </summary>
    private Task<IPage> creator;
    public void SetCreator(Task<IPage> c)
    {
        creator = c;
    }

    public Task<IPage> Creator()
    {
        return creator;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public Configuration Configuration { get; } = new Configuration();

    /// <summary>
    /// Gets the statistics.
    /// </summary>
    public Statistics Statistics { get; } = new Statistics();

    /// <summary>
    /// Gets or sets the driver.
    /// </summary>
    public async Task<IPage> Driver()
    {
        return driver ??= await Creator();
    }

    public void SetDriver(IPage driver)
    {
        this.driver = driver;
    }

    /// <summary>
    /// Open the web page.
    /// </summary>
    /// <param name="url">The url to open.</param>
    public void Open(string url) => Driver().Result.GotoAsync(url);
    
    public void Quit() => Driver().Result.Context.Browser.CloseAsync();
}