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

using Microsoft.Playwright;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Frame control object node class for wrapping iframe/frame elements.
/// </summary>
internal class FrameControlObjectNode : UIObjectNode
{
    /// <summary>
    /// Gets the locator for elements inside the frame.
    /// This uses Playwright's FrameLocator to cross the iframe boundary.
    /// </summary>
    /// <returns>A locator scoped to the frame's document context.</returns>
    public override ILocator Locator()
    {
        // Get the frame locator by using the search pattern (which points to the frame element)
        var frameLocator = Parent.FrameLocator(SearchPattern.ToLocator());
        
        // Return the root element inside the frame
        // Using ":root" or "body" to get a locator context within the frame
        return frameLocator.Locator(":root").Nth(Index);
    }
}
