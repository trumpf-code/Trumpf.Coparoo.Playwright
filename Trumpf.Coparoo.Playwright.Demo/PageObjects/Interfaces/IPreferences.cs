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

namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Interface for the preferences page containing action buttons.
/// This interface is designed to be implemented by teams working on preferences functionality
/// without requiring knowledge of the main application structure.
/// </summary>
public interface IPreferences : IPageObject
{
    /// <summary>
    /// Gets the button for saving user preferences.
    /// </summary>
    Button SavePreferences { get; }

    /// <summary>
    /// Gets the button for resetting preferences to defaults.
    /// </summary>
    Button ResetToDefaults { get; }

    /// <summary>
    /// Gets the button for exporting settings.
    /// </summary>
    Button ExportSettings { get; }
}
