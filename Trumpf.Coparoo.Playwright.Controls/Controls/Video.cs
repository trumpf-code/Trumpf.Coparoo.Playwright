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

using System;

namespace Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// HTML5 video control object.
/// Wraps <c>&lt;video&gt;</c> elements and provides attribute access, metadata queries, and playback control.
/// </summary>
public class Video : ControlObject, IVideo
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => "video";

    // ── Attributes ──────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<string> GetPosterAsync()
        => await Locator.GetAttributeAsync("poster") ?? string.Empty;

    /// <inheritdoc />
    public async Task<string> GetSourceUrlAsync()
        => await Locator.Locator("source").GetAttributeAsync("src") ?? string.Empty;

    // ── Metadata ────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task WaitForMetadataLoadedAsync(TimeSpan timeout)
    {
        var ms = (int)timeout.TotalMilliseconds;
        await Locator.EvaluateAsync(@"(el, ms) => new Promise((resolve, reject) => {
            if (el.readyState >= 1) { resolve(); return; }
            el.addEventListener('loadedmetadata', () => resolve(), { once: true });
            el.addEventListener('error', () => reject(new Error(el.error?.message ?? 'Video load error')), { once: true });
            setTimeout(() => reject(new Error('Timeout waiting for video metadata')), ms);
        })", ms);
    }

    /// <inheritdoc />
    public async Task<VideoDimensions> GetDimensionsAsync()
    {
        var width = await Locator.EvaluateAsync<int>("el => el.videoWidth");
        var height = await Locator.EvaluateAsync<int>("el => el.videoHeight");
        return new VideoDimensions(width, height);
    }

    /// <inheritdoc />
    public async Task<double> GetDurationAsync()
        => await Locator.EvaluateAsync<double>("el => el.duration");

    /// <inheritdoc />
    public async Task<string> GetErrorAsync()
        => await Locator.EvaluateAsync<string>("el => el.error ? el.error.message : null");

    // ── Playback ────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<double> GetCurrentTimeAsync()
        => await Locator.EvaluateAsync<double>("el => el.currentTime");

    /// <inheritdoc />
    public async Task PlayAsync()
        => await Locator.EvaluateAsync("el => el.play()");

    /// <inheritdoc />
    public async Task PauseAsync()
        => await Locator.EvaluateAsync("el => el.pause()");

    /// <inheritdoc />
    public async Task WaitForPlaybackAdvancedAsync(TimeSpan timeout)
    {
        var ms = (int)timeout.TotalMilliseconds;
        await Locator.EvaluateAsync(@"(el, ms) => new Promise((resolve, reject) => {
            if (el.currentTime > 0.1) { resolve(); return; }
            el.addEventListener('timeupdate', function handler() {
                if (el.currentTime > 0.1) {
                    el.removeEventListener('timeupdate', handler);
                    resolve();
                }
            });
            setTimeout(() => reject(new Error('Timeout waiting for video playback')), ms);
        })", ms);
    }
}
