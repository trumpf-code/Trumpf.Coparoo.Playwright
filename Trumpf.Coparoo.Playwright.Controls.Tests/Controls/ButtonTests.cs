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
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Extensions;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class ButtonTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task ButtonWithNonExistentLocator_ShouldNotExist()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync(HtmlContents("other text"));
        var checkbox = tab.Find<Button>("non-existing locator");

        // Act
        var count = await checkbox.CountAsync();
        var isVisible = await checkbox.IsVisibleAsync();

        // Check
        count.Should().Be(0);
        isVisible.Should().BeFalse();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenAButtonIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        //Prepare
        var expectedButtonText = "button text";
        TestTab tab = await TestTab.CreateTestPageAsync(HtmlContents(expectedButtonText));
        Button button = tab.Find<Button>();

        // Act
        string actualButtonText = await button.Text();
        var isVisible = await button.IsVisibleAsync();

        // Log
        Trace.WriteLine($"buttonText: {actualButtonText}");

        // Check
        actualButtonText.Should().Be(expectedButtonText);
    }

    /// <summary>
    /// Test method for By.TestId().
    /// </summary>
    [TestMethod]
    public async Task WhenAButtonIsFoundByTestId_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var expectedTestId = "submit-button";
        var expectedButtonText = "Submit Form";
        var tab = await TestTab.CreateTestPageAsync(HtmlContentsWithTestId(expectedTestId, expectedButtonText));

        // Act
        var button = tab.Find<Button>(By.TestId(expectedTestId));
        var actualButtonText = await button.Text();
        var isVisible = await button.IsVisibleAsync();

        // Log
        Trace.WriteLine($"testId: {expectedTestId}");
        Trace.WriteLine($"buttonText: {actualButtonText}");

        // Check
        actualButtonText.Should().Be(expectedButtonText);
        isVisible.Should().BeTrue();
    }

    private static string HtmlContents(string expectedButtonText)
    {
        return $"<button type=\"button\">{expectedButtonText}</button>";
    }

    private static string HtmlContentsWithTestId(string testId, string buttonText)
    {
        return $"<button type=\"button\" data-testid=\"{testId}\">{buttonText}</button>";
    }
}