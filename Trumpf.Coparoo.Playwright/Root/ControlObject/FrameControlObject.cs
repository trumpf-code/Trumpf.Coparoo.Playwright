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
using Trumpf.Coparoo.Playwright.Internal;

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Frame control object base class for controls rendered inside iframes or frames.
/// The <see cref="ControlObject.SearchPattern"/> should point to the iframe/frame element itself.
/// </summary>
/// <remarks>
/// Use this class when you need to interact with transient UI elements (like dialogs or editors)
/// that appear inside an iframe or frame element. The search pattern identifies the frame element
/// in the parent context, and all child controls will be located within that frame's document context.
/// </remarks>
public abstract class FrameControlObject : ControlObject, IFrameControlObject
{
    /// <summary>
    /// Gets the iframe/frame element itself (in the parent document context).
    /// Use this to check if the frame element exists or is visible.
    /// </summary>
    /// <remarks>
    /// This locator refers to the frame element in the parent DOM, not the content inside it.
    /// Use <see cref="UIObject.Locator"/> to interact with elements inside the frame.
    /// </remarks>
    public ILocator FrameElement => Parent.Locator.Locator(SearchPattern.ToLocator());

    /// <summary>
    /// Gets a fresh node object for frame-based control objects.
    /// </summary>
    internal override IUIObjectNode CreateNode => new FrameUIObjectNode();
}
