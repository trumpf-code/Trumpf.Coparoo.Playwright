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
using System.Linq;
using Trumpf.Coparoo.Playwright.Controls;

[TestClass]
public class SelectTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenASelectHas3Options_ThenTheOptionEnumerationHas3Items()
    {
        // Prepare
        var tab = await TestTab.CreateAsync(HtmlContents);
        var select = tab.Find<Select>();
        var options = await select.Options().ToArrayAsync();

        // Act
        var visible = await (await select.Locator).IsVisibleAsync();
        var count = options.Length;
        var values = options.Select(e => e.Value).ToList();
        var selected = options.Select(e => e.IsSelected()).ToList();

        // Check
        visible.Should().Be(true);
        count.Should().Be(3);
        (await values[0]).Should().Be("a");
        (await values[1]).Should().Be("b");
        (await values[2]).Should().Be("c");
        (await selected[0]).Should().Be(true);
        (await selected[1]).Should().Be(false);
        (await selected[2]).Should().Be(false);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenTheSecondOptionIsSelected_ThenTheSecondElementReturnIsSelectedTrue()
    {
        // Prepare
        var tab = await TestTab.CreateAsync(HtmlContents);
        var select = tab.Find<Select>();

        // Act
        await (await select.Options().ElementAtAsync(1)).Select();


        // Check
        var options = await select.Options().ToArrayAsync();
        var first = await options.ElementAt(0).IsSelected();
        var second = await options.ElementAt(1).IsSelected();
        var last = await options.ElementAt(2).IsSelected();

        first.Should().BeFalse();
        second.Should().BeTrue();
        last.Should().BeFalse();
    }

    private string HtmlContents
        => $"<select><option value=\"a\">A</option><option value=\"b\">B</option><option value=\"c\">C</option></select>";
}