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
/// Interface for the settings page containing configuration checkboxes.
/// This interface is designed to be implemented by teams working on settings functionality
/// without requiring knowledge of the main application structure.
/// </summary>
public interface ISettingsPage : IPageObject
{
    /// <summary>
    /// Gets the checkbox for enabling notifications.
    /// </summary>
    Checkbox EnableNotifications { get; }

    /// <summary>
    /// Gets the checkbox for enabling auto-save functionality.
    /// </summary>
    Checkbox EnableAutoSave { get; }

    /// <summary>
    /// Gets the checkbox for enabling dark mode.
    /// </summary>
    Checkbox EnableDarkMode { get; }

    /// <summary>
    /// Verifies that the settings page is currently active and visible.
    /// </summary>
    /// <returns>True if the page is active, false otherwise.</returns>
    Task<bool> IsActiveAsync();
}
