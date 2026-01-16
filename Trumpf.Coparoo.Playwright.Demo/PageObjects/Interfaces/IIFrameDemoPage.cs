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

namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.ControlObjects;

/// <summary>
/// Interface for the IFrame demo main page.
/// </summary>
public interface IIFrameDemoPage : IPageObject 
{
    /// <summary>
    /// Gets the main page button control.
    /// </summary>
    Button MainPageButton { get; }

    /// <summary>
    /// Gets the result display locator.
    /// </summary>
    ILocator Result { get; }

    /// <summary>
    /// Gets the rich text editor frame control.
    /// </summary>
    RichTextEditorFrame RichTextEditor { get; }
}
