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

namespace Trumpf.Coparoo.Tests;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Trumpf.Coparoo.Playwright.Controls;

[TestClass]
public class TextInputTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenATextInputIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var expectedName = "input name";
        var expectedValue = "input value";
        var tab = await Tab.CreateAsync(HtmlContents(expectedName, expectedValue));

        // Act
        var textInput = tab.Find<TextInput>();
        var actualText = await textInput.GetValue();
        var actualName = await textInput.GetName();

        // Log
        Trace.WriteLine(actualText);
        Trace.WriteLine(actualName);

        // Check
        actualName.Should().Be(expectedName);
        actualText.Should().Be(expectedValue);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenATextInputIsSet_ThenTheNewValueCanAlsoBeRetrieved()
    {
        // Prepare
        var expectedName = "some name";
        var originalValue = "original value";
        var expectedValue = "expected value";
        var tab = await Tab.CreateAsync(HtmlContents(expectedName, originalValue));

        // Act
        var textInput = tab.Find<TextInput>();

        var oldValue = await textInput.GetValue();
        await Task.Delay(2000);
        await textInput.SetText(expectedValue);
        await Task.Delay(2000);
        var newValue = await textInput.GetValue();

        // Check
        oldValue.Should().Be(originalValue);
        newValue.Should().Be(expectedValue);
    }

    private string HtmlContents(string name, string value) => $"<input type=\"text\" name=\"{name}\" value=\"{value}\">";
}