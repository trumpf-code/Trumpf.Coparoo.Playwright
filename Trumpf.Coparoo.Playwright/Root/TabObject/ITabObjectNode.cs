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
/// Interface for the process object node class.
/// </summary>
internal interface ITabObjectNode : IUIObjectNode
{
    /// <summary>
    /// Gets the node locator.
    /// </summary>
    Configuration Configuration { get; }

    /// <summary>
    /// Gets the statistics.
    /// </summary>
    Statistics Statistics { get; }

    /// <summary>
    /// Gets or creates the page instance using the provided factory method.
    /// </summary>
    /// <param name="factory">The factory method that creates the page instance.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the IPage instance.</returns>
    Task<IPage> GetOrCreatePageAsync(Func<Task<IPage>> factory);

    /// <summary>
    /// Open the web page.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    /// <param name="factory">The factory method that creates the page instance if not yet initialized.</param>
    Task Open(string url, Func<Task<IPage>> factory);

    /// <summary>
    /// Quit the browser.
    /// </summary>
    Task Quit();
}