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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Tests for <see cref="IUIObjectConditionExtensions"/> and <see cref="ElementCondition"/>. Each test
/// drives an element through a state transition and awaits the matching handle wait; the wait is itself
/// the web-first assertion (it throws if the state never lands), so no separate read-back is asserted.
/// </summary>
[TestClass]
public class IUIObjectConditionExtensionsTests
{
    /// <summary>
    /// Enabled().WaitForAsync waits until a disabled element becomes enabled.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForEnabled_ThenElementBecomesEnabled()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"" disabled>Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').disabled = false;
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.Enabled().WaitForAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Enabled().WaitForNotAsync waits until an enabled element becomes disabled.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForNotEnabled_ThenElementBecomesDisabled()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"">Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').disabled = true;
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.Enabled().WaitForNotAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Checked().WaitForAsync waits until a checkbox becomes checked.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForChecked_ThenElementBecomesChecked()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <input type=""checkbox"" id=""testButton"" />
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').checked = true;
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var checkbox = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await checkbox.Checked().WaitForAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Checked().WaitForNotAsync waits until a checked checkbox becomes unchecked.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForNotChecked_ThenElementBecomesUnchecked()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <input type=""checkbox"" id=""testButton"" checked />
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').checked = false;
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var checkbox = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await checkbox.Checked().WaitForNotAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Editable().WaitForAsync waits until a read-only input becomes editable.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForEditable_ThenElementBecomesEditable()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <input type=""text"" id=""testButton"" readonly />
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').removeAttribute('readonly');
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var input = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await input.Editable().WaitForAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// An explicit timeout is honoured by WaitForAsync.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForEnabledWithTimeout_ThenElementBecomesEnabled()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"" disabled>Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').disabled = false;
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.Enabled().WaitForAsync(TimeSpan.FromSeconds(5));

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Visible().WaitForAsync waits until a hidden element becomes visible.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForVisible_ThenElementBecomesVisible()
    {
        // Arrange
        var html = @"
            <div id=""container"">
                <button id=""testButton"" style=""display:none"">Click me</button>
            </div>
            <script>
                setTimeout(() => {
                    document.getElementById('testButton').style.display = 'block';
                }, 100);
            </script>";
        var tab = await Tab.CreateAsync(html);
        var button = tab.On<ButtonPage>().Find<Button>();

        // Act & Assert
        await button.Visible().WaitForAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Visible().WaitForNotAsync waits until a visible element becomes hidden.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForNotVisible_ThenElementBecomesHidden()
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
        await button.Visible().WaitForNotAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Attached().WaitForAsync waits until an element is present in the DOM.
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
        await button.Attached().WaitForAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Attached().WaitForNotAsync waits until an element is removed from the DOM.
    /// </summary>
    [TestMethod]
    public async Task WhenWaitingForNotAttached_ThenElementBecomesDetached()
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
        await button.Attached().WaitForNotAsync();

        // Cleanup
        await tab.Close();
    }

    /// <summary>
    /// Helper page object for condition tests.
    /// </summary>
    private class ButtonPage : PageObject, IChildOf<Tab>
    {
        protected override By SearchPattern => "#container";
    }

    /// <summary>
    /// Helper control object for condition tests.
    /// </summary>
    private class Button : ControlObject
    {
        protected override By SearchPattern => "#testButton";
    }
}
