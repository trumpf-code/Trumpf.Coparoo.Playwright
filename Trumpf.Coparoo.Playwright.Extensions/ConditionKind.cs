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

namespace Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Identifies the boolean UI state an <see cref="ElementCondition"/> represents.
/// </summary>
internal enum ConditionKind
{
    /// <summary>The element is enabled (not disabled).</summary>
    Enabled,

    /// <summary>The checkbox or radio button is checked.</summary>
    Checked,

    /// <summary>The element is editable (an input/textarea that is not read-only or disabled).</summary>
    Editable,

    /// <summary>The element is visible (rendered, with a non-empty bounding box and not <c>visibility:hidden</c>).</summary>
    Visible,

    /// <summary>The element is attached to the DOM.</summary>
    Attached,
}
