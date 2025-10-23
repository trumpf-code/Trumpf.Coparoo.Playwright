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

namespace Trumpf.Coparoo.Tests.Extensions;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Tests for <see cref="IUIObjectWaitExtensions"/>.
/// </summary>
[TestClass]
public class IUIObjectWaitExtensionsTests
{
    /// <summary>
    /// Test method for WaitForAttachedAsync.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForAttached_ThenElementBecomesAttached()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"">Click me</button>
            </div>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.WaitForAttachedAsync();
        var exists = await button.ExistsAsync();
        exists.Should().BeTrue();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Test method for WaitForDetachedAsync.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForDetached_ThenElementBecomesDetached()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"">Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').remove();
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.WaitForDetachedAsync();
        var exists = await button.ExistsAsync();
        exists.Should().BeFalse();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Test method for WaitForVisibleAsync.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForVisible_ThenElementBecomesVisible()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"" style=""display: none;"">Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').style.display = 'block';
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.WaitForVisibleAsync();
        var locator = await button.Locator;
        var isVisible = await locator.IsVisibleAsync();
        isVisible.Should().BeTrue();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Test method for WaitForHiddenAsync.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForHidden_ThenElementBecomesHidden()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"">Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').style.display = 'none';
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.WaitForHiddenAsync();
        var locator = await button.Locator;
        var isVisible = await locator.IsVisibleAsync();
        isVisible.Should().BeFalse();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Helper page object for button tests.
    /// </summary>
    private class ButtonPage : PageObject, IChildOf<Tab>
    {
        protected override By SearchPattern => "#container";
    }

    /// <summary>
    /// Helper control object for button tests.
    /// </summary>
    private class Button : ControlObject
    {
        protected override By SearchPattern => "#testButton";
    }
}
