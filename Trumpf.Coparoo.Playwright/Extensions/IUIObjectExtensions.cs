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
using Trumpf.Coparoo.Playwright.Logging.Tree;

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Extensions.
/// </summary>
public static class IUIObjectExtensions
{
    /// <summary>
    /// Checks if the source exists by verifying if the count of matching elements is greater than zero.
    /// </summary>
    /// <returns>
    /// A boolean indicating whether the source exists (true if count is greater than zero, false otherwise).
    /// </returns>
    public static async Task<bool> Exists(this IUIObject source)
    {
        var locator = await source.Locator;
        return await locator.CountAsync() > 0;
    }
}