﻿// Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
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

using System.ComponentModel;

namespace Trumpf.Coparoo.Playwright.Logging.Tree;

/// <summary>
/// The colors.
/// </summary>
internal enum Color
{
    /// <summary>
    /// The color is white.
    /// </summary>
    [Description("white")]
    White,

    /// <summary>
    /// The color is gray.
    /// </summary>
    [Description("gray")]
    Gray,

    /// <summary>
    /// The color is palegreen3.
    /// </summary>
    [Description("palegreen3")]
    Palegreen3,

    /// <summary>
    /// The color is orange.
    /// </summary>
    [Description("orange")]
    Orange,

    /// <summary>
    /// The color is green.
    /// </summary>
    [Description("green")]
    Green,

    /// <summary>
    /// The color is red.
    /// </summary>
    [Description("red")]
    Red,

    /// <summary>
    /// The color is sienna2.
    /// </summary>
    [Description("sienna2")]
    Sienna2
}
