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

using System.Collections.Generic;

namespace Trumpf.Coparoo.Playwright.Controls.Interfaces;

/// <summary>
/// Interface for generic table segment control object.
/// </summary>
public interface ISegment : IControlObject
{
    /// <summary>
    /// Gets the enumeration of row control objects.
    /// </summary>
    IAsyncEnumerable<IRow> Rows { get; }

    /// <summary>
    /// Returns the row element at a specified index in the row sequence.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Row sequence is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Index is less than 0 or greater than or equal to the number of row elements in the row sequence.</exception>
    /// <param name="index">The zero-based index of the row to retrieve.</param>
    /// <returns>The row element at the specified position in the row sequence.</returns>
    Task<IRow> RowAt(int index);
}
