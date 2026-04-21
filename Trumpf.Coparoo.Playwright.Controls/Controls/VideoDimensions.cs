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

namespace Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Represents the intrinsic dimensions of a <c>&lt;video&gt;</c> element.
/// </summary>
public readonly struct VideoDimensions
{
    /// <summary>
    /// Gets the intrinsic width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the intrinsic height in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Initializes a new <see cref="VideoDimensions"/> instance.
    /// </summary>
    /// <param name="width">The intrinsic width in pixels.</param>
    /// <param name="height">The intrinsic height in pixels.</param>
    public VideoDimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
