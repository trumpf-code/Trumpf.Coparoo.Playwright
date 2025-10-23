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

using Trumpf.Coparoo.Playwright.Demo.ControlObjects.Interfaces;

/// <summary>
/// Interface for the main application page that contains the navigation menu.
/// This interface allows tests to interact with the application shell independently
/// of specific page implementations.
/// </summary>
public interface IShell : IPageObject
{
    /// <summary>
    /// Gets the navigation menu control for navigating between pages.
    /// </summary>
    IMenu Menu { get; }
}
