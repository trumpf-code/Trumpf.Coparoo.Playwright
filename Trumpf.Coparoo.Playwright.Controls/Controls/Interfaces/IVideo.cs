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
/// HTML5 video control object interface.
/// Wraps <c>&lt;video&gt;</c> elements and provides attribute access, metadata queries, and playback control.
/// </summary>
public interface IVideo : IControlObject
{
    // ── Attributes ──────────────────────────────────────────────

    /// <summary>
    /// Gets the <c>poster</c> attribute value.
    /// </summary>
    Task<string> GetPosterAsync();

    /// <summary>
    /// Gets the <c>src</c> attribute of the first <c>&lt;source&gt;</c> child element.
    /// </summary>
    Task<string> GetSourceUrlAsync();

    // ── Metadata ────────────────────────────────────────────────

    /// <summary>
    /// Waits until the video metadata (duration, dimensions) is loaded.
    /// </summary>
    /// <param name="timeout">Maximum time to wait.</param>
    Task WaitForMetadataLoadedAsync(TimeSpan timeout);

    /// <summary>
    /// Gets the intrinsic video dimensions.
    /// </summary>
    Task<VideoDimensions> GetDimensionsAsync();

    /// <summary>
    /// Gets the video duration in seconds.
    /// </summary>
    Task<double> GetDurationAsync();

    /// <summary>
    /// Gets the error message if the video failed to load; otherwise <c>null</c>.
    /// </summary>
    Task<string> GetErrorAsync();

    // ── Playback ────────────────────────────────────────────────

    /// <summary>
    /// Gets the current playback position in seconds.
    /// </summary>
    Task<double> GetCurrentTimeAsync();

    /// <summary>
    /// Starts video playback.
    /// </summary>
    Task PlayAsync();

    /// <summary>
    /// Pauses video playback.
    /// </summary>
    Task PauseAsync();

    /// <summary>
    /// Waits until the playback position advances beyond zero.
    /// </summary>
    /// <param name="timeout">Maximum time to wait.</param>
    Task WaitForPlaybackAdvancedAsync(TimeSpan timeout);
}
