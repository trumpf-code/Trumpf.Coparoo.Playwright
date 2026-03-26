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
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Extensions;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class SectionTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task SectionWithNonExistentLocator_ShouldNotExist()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync("<div>no section here</div>");
        var section = tab.Find<Section>();

        // Act
        var count = await section.CountAsync();
        var isVisible = await section.IsVisibleAsync();

        // Check
        count.Should().Be(0);
        isVisible.Should().BeFalse();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenASectionExists_ThenItCanBeFound()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync("<section><h2>My Section</h2><p>Content</p></section>");
        var section = tab.Find<Section>();

        // Act
        var count = await section.CountAsync();
        var isVisible = await section.IsVisibleAsync();

        // Check
        count.Should().Be(1);
        isVisible.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenASectionIsFoundByTestId_ThenItCanBeFound()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync("<section data-testid=\"hero\">Hero content</section><section data-testid=\"about\">About content</section>");
        var section = tab.Find<Section>(By.TestId("about"));

        // Act
        var isVisible = await section.IsVisibleAsync();

        // Check
        isVisible.Should().BeTrue();
    }
}
