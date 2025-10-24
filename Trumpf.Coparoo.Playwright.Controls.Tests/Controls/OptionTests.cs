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
public class OptionTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task SelectElement_ShouldContainThreeOptions_WithExpectedValues()
    {
        // Prepare
        var tab = await TestTab.CreateAsync(HtmlContents);
        var select = tab.Find<Select>();

        // Act
        var options = await select.Options().ToArrayAsync();

        // Check
        options.Length.Should().Be(3);
        (await options[0].Value).Should().Be("a");
        (await options[1].Value).Should().Be("b");
        (await options[2].Value).Should().Be("c");
    }

    private string HtmlContents
        => $"<select id=\"selectID\"><option value=\"a\">A</option><option value=\"b\">B</option><option value=\"c\">C</option></select>";
}