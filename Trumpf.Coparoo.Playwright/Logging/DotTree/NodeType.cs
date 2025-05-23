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
/// Node types.
/// </summary>
internal enum NodeType
{
    /// <summary>
    /// Node represents a root object.
    /// </summary>
    [Description("Root")]
    RootObject,

    /// <summary>
    /// Node represents a page object.
    /// </summary>
    [Description("Page")]
    PageObject,

    /// <summary>
    /// Node represents a control object.
    /// </summary>
    [Description("Control")]
    ControlObject
}