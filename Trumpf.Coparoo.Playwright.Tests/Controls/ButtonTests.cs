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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Trumpf.Coparoo.Playwright.Controls;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class ButtonTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        //Prepare
        var expectedButtonText = "button text";
        Tab tab = await Tab.CreateAsync($"<button type=\"button\">{expectedButtonText}</button>");
        Button button = tab.Find<Button>();

        // Act
        string actualButtonText = await button.Text();
        var isVisible = button.IsVisibleAsync();

        // Log
        Trace.WriteLine($"buttonText: {actualButtonText}");

        // Check
        actualButtonText.Should().Be(expectedButtonText);
    }
}