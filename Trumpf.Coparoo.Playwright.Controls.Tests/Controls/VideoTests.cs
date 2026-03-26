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
public class VideoTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task VideoWithNonExistentLocator_ShouldNotExist()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync("<div>no video here</div>");
        var video = tab.Find<Video>();

        // Act
        var count = await video.CountAsync();
        var isVisible = await video.IsVisibleAsync();

        // Check
        count.Should().Be(0);
        isVisible.Should().BeFalse();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenAVideoExists_ThenItCanBeFound()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync(HtmlContents("https://example.com/poster.jpg"));
        var video = tab.Find<Video>();

        // Act
        var count = await video.CountAsync();
        var isVisible = await video.IsVisibleAsync();

        // Check
        count.Should().Be(1);
        isVisible.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenAVideoHasAPoster_ThenThePosterCanBeRead()
    {
        // Prepare
        var expectedPoster = "https://example.com/poster.jpg";
        var tab = await TestTab.CreateTestPageAsync(HtmlContents(expectedPoster));
        var video = tab.Find<Video>();

        // Act
        var poster = await video.GetPosterAsync();

        // Check
        poster.Should().Be(expectedPoster);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenAVideoIsFoundByTestId_ThenItCanBeFound()
    {
        // Prepare
        var tab = await TestTab.CreateTestPageAsync("<video data-testid=\"hero-video\" poster=\"p.jpg\"></video>");
        var video = tab.Find<Video>(By.TestId("hero-video"));

        // Act
        var isVisible = await video.IsVisibleAsync();
        var poster = await video.GetPosterAsync();

        // Check
        isVisible.Should().BeTrue();
        poster.Should().Be("p.jpg");
    }

    private static string HtmlContents(string posterUrl)
    {
        return $"<video poster=\"{posterUrl}\"></video>";
    }
}
