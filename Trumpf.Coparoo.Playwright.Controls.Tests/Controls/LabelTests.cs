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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright;
using FluentAssertions;
using System.Diagnostics;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class LabelTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenALabelIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var expectedLabelText = $"hellp";
        var tab = await TestTab.CreateAsync($"<label>{expectedLabelText}</label>");
        var label = tab.Find<Label>();

        // Act
        var labelText = await label.Text;

        // Log
        Trace.WriteLine(labelText);

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test that a label can be found by its ID.
    /// </summary>
    [TestMethod]
    public async Task WhenALabelWithIdIsAccessed_ThenItCanBeFound()
    {
        // Prepare
        var expectedLabelText = "Hello World";
        var tab = await TestTab.CreateAsync($"<label id=\"myLabel\">{expectedLabelText}</label>");
        
        // Act
        var label = tab.Find<Label>(By.Id("myLabel"));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test that a label can be found by its data-testid attribute.
    /// </summary>
    [TestMethod]
    public async Task WhenALabelWithTestIdIsAccessed_ThenItCanBeFound()
    {
        // Prepare
        var expectedLabelText = "Test Label";
        var tab = await TestTab.CreateAsync($"<label data-testid=\"myLabel\">{expectedLabelText}</label>");
        
        // Act
        var label = tab.Find<Label>(By.TestId("myLabel"));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test that .And() combination of ID and class works correctly.
    /// </summary>
    [TestMethod]
    public async Task WhenALabelWithIdAndClassIsAccessed_ThenAndCombinationWorks()
    {
        // Prepare
        var expectedLabelText = "Combined Selector";
        var tab = await TestTab.CreateAsync($"<label id=\"myLabel\" class=\"primary\">{expectedLabelText}</label>");
        
        // Act - Use .And() to combine ID and class
        var label = tab.Find<Label>(By.Id("myLabel").And(By.ClassName("primary")));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test that .And() combination finds the correct element when multiple labels exist.
    /// </summary>
    [TestMethod]
    public async Task WhenMultipleLabelsExist_ThenAndCombinationFindsCorrectOne()
    {
        // Prepare
        var tab = await TestTab.CreateAsync(
            "<label class=\"primary\">Wrong Label 1</label>" +
            "<label id=\"myLabel\" class=\"primary\">Correct Label</label>" +
            "<label id=\"myLabel\">Wrong Label 2</label>");
        
        // Act - Combine ID and class to find the correct label
        var label = tab.Find<Label>(By.Id("myLabel").And(By.ClassName("primary")));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be("Correct Label");
    }

    /// <summary>
    /// Test that tag selector is placed first in combination.
    /// </summary>
    [TestMethod]
    public async Task WhenCombiningTagWithIdAndClass_ThenCorrectElementIsFound()
    {
        // Prepare
        var expectedLabelText = "Tagged Label";
        var tab = await TestTab.CreateAsync(
            $"<div id=\"myLabel\" class=\"primary\">Wrong Element</div>" +
            $"<label id=\"myLabel\" class=\"primary\">{expectedLabelText}</label>");
        
        // Act - Combine tag, ID and class
        var label = tab.Find<Label>(By.TagName("label").And(By.Id("myLabel")).And(By.ClassName("primary")));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test combining TestId with class selector.
    /// </summary>
    [TestMethod]
    public async Task WhenCombiningTestIdWithClass_ThenCorrectElementIsFound()
    {
        // Prepare
        var expectedLabelText = "Test with Class";
        var tab = await TestTab.CreateAsync(
            "<label data-testid=\"test-label\">Wrong</label>" +
            $"<label data-testid=\"test-label\" class=\"highlight\">{expectedLabelText}</label>");
        
        // Act
        var label = tab.Find<Label>(By.TestId("test-label").And(By.ClassName("highlight")));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }

    /// <summary>
    /// Test combining multiple class selectors.
    /// </summary>
    [TestMethod]
    public async Task WhenCombiningMultipleClasses_ThenCorrectElementIsFound()
    {
        // Prepare
        var expectedLabelText = "Multi Class";
        var tab = await TestTab.CreateAsync(
            "<label class=\"primary\">Wrong 1</label>" +
            "<label class=\"primary large\">Wrong 2</label>" +
            $"<label class=\"primary large highlight\">{expectedLabelText}</label>");
        
        // Act
        var label = tab.Find<Label>(
            By.ClassName("primary")
                .And(By.ClassName("large"))
                .And(By.ClassName("highlight")));
        var labelText = await label.Text;

        // Check
        labelText.Should().Be(expectedLabelText);
    }
}