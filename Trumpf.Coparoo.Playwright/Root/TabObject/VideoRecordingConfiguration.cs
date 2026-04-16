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

namespace Trumpf.Coparoo.Playwright;

/// <summary>
/// Defines video recording settings for a tab.
/// </summary>
public class VideoRecordingConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether video recording is enabled.
    /// </summary>
    /// <remarks>
    /// Default is <see langword="false"/> to keep behavior backward compatible.
    /// </remarks>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the target directory for recorded videos.
    /// </summary>
    public string DirectoryPath { get; set; }

    /// <summary>
    /// Gets or sets the video frame width.
    /// </summary>
    /// <remarks>
    /// Must be configured together with <see cref="Height"/>.
    /// </remarks>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the video frame height.
    /// </summary>
    /// <remarks>
    /// Must be configured together with <see cref="Width"/>.
    /// </remarks>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets an optional desired file name for the final video artifact.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the preferred file extension for the final video artifact.
    /// </summary>
    /// <remarks>
    /// Default is ".webm" which matches Playwright's native output format.
    /// </remarks>
    public string FileExtension { get; set; } = ".webm";
}
