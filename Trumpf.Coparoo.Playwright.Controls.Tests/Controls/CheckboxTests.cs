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
using Trumpf.Coparoo.Playwright.Extensions;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class CheckboxTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenACheckboxIsUnchecked_ThenItsCheckedPropertyReturnsFalse()
    {
        // Prepare
        var expectedName = "checkbox name";
        var expectedValue = "some value";
        var expectedIsChecked = false;

        var tab = await TestTab.CreateAsync(HtmlContents(expectedName, expectedValue, expectedIsChecked));
        var checkbox = tab.Find<Checkbox>();

        // Act
        bool exists = await checkbox.ExistsAsync();
        bool isVisible = await (await checkbox.Locator).IsVisibleAsync();
        var actualName = await checkbox.Name;
        var actualValue = await checkbox.Value;
        var actualIsChecked = await checkbox.IsChecked;

        // Check
        exists.Should().BeTrue();
        isVisible.Should().BeTrue();
        expectedName.Should().Be(actualName);
        expectedValue.Should().Be(actualValue);
        expectedIsChecked.Should().Be(actualIsChecked);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenACheckboxIsToggled_ThenTheCheckedPropertyTogglesAsWell()
    {
        // Prepare
        var expectedName = "checkbox name";
        var expectedValue = "some value";
        var expectedIsCheck = false;
        var tab = await TestTab.CreateAsync(HtmlContents(expectedName, expectedValue, expectedIsCheck));
        var checkbox = tab.Find<Checkbox>();

        // Act
        var c0 = await checkbox.IsChecked;
        await checkbox.Check();
        var c1 = await checkbox.IsChecked;
        await checkbox.Uncheck();
        var c2 = await checkbox.IsChecked;

        // Log
        Trace.WriteLine(c0);
        Trace.WriteLine(c1);
        Trace.WriteLine(c2);

        // Check
        c0.Should().Be(false);
        c1.Should().Be(true);
        c2.Should().Be(false);
    }

    private string HtmlContents(string name, string value, bool check) => $"<input type=\"checkbox\" name=\"{name}\" value=\"{value}\" {(check ? "checked" : "")}>";
}