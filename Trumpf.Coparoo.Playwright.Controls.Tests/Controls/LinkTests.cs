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
using FluentAssertions;
using System.Linq;
using System.Diagnostics;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class LinkTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenALinkIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var expectedLinkText = "my link";
        var expectedLinkUrl = $"http://link";
        var tab = await Tab.CreateAsync($"<a href=\"{expectedLinkUrl}\">{expectedLinkText}</a>");
        var link = tab.Find<Link>();

        // Act
        var linkText = await link.Text;
        var linkUrl = await link.URL;

        // Log
        Trace.WriteLine(linkText);
        Trace.WriteLine(linkUrl);

        // Check
        linkText.Should().Be(expectedLinkText);
        linkUrl.Should().Be(expectedLinkUrl);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenTwoLinksExist_ThenTheyCanBeRetrieved()
    {
        // Prepare
        var firstExpectedLinkText = "first";
        var firstExpectedLinkUrl = $"http://firstExpectedLinkUrl/";
        var secondExpectedLinkText = "second";
        var secondExpectedLinkUrl = $"http://secondExpectedLinkText/";
        var tab = await Tab.CreateAsync($"<a href=\"{firstExpectedLinkUrl}\">{firstExpectedLinkText}</a><a href=\"{secondExpectedLinkUrl}\">{secondExpectedLinkText}</a>");

        // Act
        var links = tab.FindAll<Link>();
        var linkArray = await links.ToArrayAsync();

        // Check
        linkArray.Length.Should().Be(2);
        var firstButton = linkArray.First();
        var secondButton = linkArray.Last();

        (await firstButton.Text).Should().Be(firstExpectedLinkText);
        (await firstButton.URL).Should().Be(firstExpectedLinkUrl);
        (await secondButton.Text).Should().Be(secondExpectedLinkText);
        (await secondButton.URL).Should().Be(secondExpectedLinkUrl);

        // Log
        await foreach (var item in links)
        {
            Trace.WriteLine(await item.Text);
            Trace.WriteLine(await item.URL);
        }
    }
}