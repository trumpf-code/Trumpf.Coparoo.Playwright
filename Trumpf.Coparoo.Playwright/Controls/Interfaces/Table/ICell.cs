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

namespace Trumpf.Coparoo.Playwright.Controls.Interfaces;

/// <summary>
/// Interface for table data cell object.
/// </summary>
public interface ICell : IControlObject
{
    /// <summary>
    /// Searches for a control object like provided and returns it.
    /// </summary>
    /// <typeparam name="T">Control object type.</typeparam>
    /// <returns>Cell object as a control object like provided.</returns>
    T As<T>() where T : IControlObject;

    /// <summary>
    /// Returns true if node element is header cell, otherwise false.
    /// </summary>
    Task<bool> IsHeaderCell();

    /// <summary>
    /// Returns true if node element is data cell, otherwise false.
    /// </summary>
    Task<bool> IsDataCell();
}