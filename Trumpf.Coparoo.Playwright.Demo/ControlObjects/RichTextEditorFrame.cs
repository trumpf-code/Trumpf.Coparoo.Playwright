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

namespace Trumpf.Coparoo.Playwright.Demo.ControlObjects;

using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Rich text editor frame control object.
/// Demonstrates FrameControlObject usage for embedded WYSIWYG editor iframes.
/// </summary>
public sealed class RichTextEditorFrame : FrameControlObject, IRichTextEditorFrame
{
    /// <summary>
    /// Gets the search pattern for locating this frame.
    /// </summary>
    protected override By SearchPattern => By.TestId("editor-frame");

    /// <summary>
    /// Gets the bold button.
    /// </summary>
    public Button BoldButton => Find<Button>(By.TestId("bold-btn"));

    /// <summary>
    /// Gets the italic button.
    /// </summary>
    public Button ItalicButton => Find<Button>(By.TestId("italic-btn"));

    /// <summary>
    /// Gets the save button.
    /// </summary>
    public Button SaveButton => Find<Button>(By.TestId("save-btn"));

    /// <summary>
    /// Gets the content area text input.
    /// </summary>
    public TextInput ContentArea => Find<TextInput>(By.TestId("editor-content"));

    /// <summary>
    /// Gets the editor status display locator.
    /// </summary>
    public ILocator EditorStatus => Locator.GetByTestId("editor-status");
}
