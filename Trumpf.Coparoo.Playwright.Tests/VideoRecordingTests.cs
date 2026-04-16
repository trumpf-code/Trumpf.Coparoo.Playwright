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

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;

[TestClass]
[DoNotParallelize]
public class VideoRecordingTests
{
    public TestContext TestContext { get; set; }

    private string videoDirectory;

    [TestInitialize]
    public void TestInitialize()
    {
        videoDirectory = Path.Combine(
            Path.GetTempPath(),
            "Coparoo.VideoRecordingTests",
            TestContext.TestName,
            Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && Directory.Exists(videoDirectory))
        {
            Directory.Delete(videoDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WhenVideoRecordingIsEnabled_ThenVideoArtifactIsWrittenOnClose()
    {
        var tab = new RecordingTab();
        tab.Configuration.Video.Enabled = true;
        tab.Configuration.Video.DirectoryPath = videoDirectory;

        try
        {
            await tab.Open();
            var page = await tab.Page;
            await page.SetContentAsync("<html><body><button id='run' onclick=\"document.body.dataset.state='clicked'\">Run</button></body></html>");
            await page.ClickAsync("#run");
            await tab.Close();
        }
        catch (PlaywrightException ex) when (IsMissingBrowserExecutable(ex))
        {
            Assert.Inconclusive(ex.Message);
            return;
        }

        tab.LastRecordedVideoPath.Should().NotBeNullOrWhiteSpace();
        File.Exists(tab.LastRecordedVideoPath).Should().BeTrue();
    }

    [TestMethod]
    public async Task WhenCustomVideoFileNameIsConfigured_ThenFinalPathMatchesRequestedName()
    {
        var tab = new RecordingTab();
        tab.Configuration.Video.Enabled = true;
        tab.Configuration.Video.DirectoryPath = videoDirectory;
        tab.Configuration.Video.FileName = "custom-recording";
        tab.Configuration.Video.FileExtension = ".webm";

        try
        {
            await tab.Open();
            var page = await tab.Page;
            await page.SetContentAsync("<html><body><div>record me</div></body></html>");
            await tab.Close();
        }
        catch (PlaywrightException ex) when (IsMissingBrowserExecutable(ex))
        {
            Assert.Inconclusive(ex.Message);
            return;
        }

        var expectedPath = Path.Combine(videoDirectory, "custom-recording.webm");
        tab.LastRecordedVideoPath.Should().Be(expectedPath);
        File.Exists(expectedPath).Should().BeTrue();
    }

    private static bool IsMissingBrowserExecutable(PlaywrightException ex)
        => ex.Message.IndexOf("Executable doesn't exist", StringComparison.OrdinalIgnoreCase) >= 0;

    private sealed class RecordingTab : TabObject
    {
        protected override async Task<IPage> CreatePageAsync()
        {
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            return await CreateConfiguredPageAsync(browser);
        }

        protected override string Url => "about:blank";
    }
}
