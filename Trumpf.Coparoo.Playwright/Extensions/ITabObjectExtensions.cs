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

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Extension methods for ITabObject.
/// </summary>
public static class ITabObjectExtensions
{
    /// <summary>
    /// Initializes the tab with an existing page instance (fluent API).
    /// </summary>
    /// <typeparam name="TTab">The tab object type.</typeparam>
    /// <param name="tab">The tab object to configure.</param>
    /// <param name="page">The IPage instance to use.</param>
    /// <returns>The same tab object for method chaining.</returns>
    /// <remarks>
    /// This extension method is primarily used in test setups where you want to inject
    /// a pre-configured page instance instead of using the Creator() method.
    /// </remarks>
    /// <example>
    /// <code>
    /// var page = await browser.NewPageAsync();
    /// var tab = new MyTab().WithPage(page);
    /// </code>
    /// </example>
    public static TTab WithPage<TTab>(this TTab tab, IPage page) where TTab : ITabObject
    {
        tab.SetPage(page);
        return tab;
    }
}
