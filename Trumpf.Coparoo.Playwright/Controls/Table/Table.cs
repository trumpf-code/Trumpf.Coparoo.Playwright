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

using System;
using System.Collections.Generic;
using System.Linq;



using Trumpf.Coparoo.Playwright.Controls.Interfaces;

/// <summary>
/// Table control object.
/// </summary>
public class Table : ControlObject, ITable
{
    /// <summary>
    /// Gets the search pattern.
    /// </summary>
    protected override By SearchPattern => "table";

    /// <summary>
    /// Gets the table head control object.
    /// </summary>
    public IHead Header => Find<IHead>();

    /// <summary>
    /// Gets the table body control object.
    /// </summary>
    public IBody Content => Find<IBody>();

    /// <summary>
    /// Gets the table foot control object.
    /// </summary>
    public IFoot Footer => Find<IFoot>();

    /// <summary>
    /// Gets a sorted enumeration of all row control objects (including header, body and footer rows).
    /// </summary>
    public IAsyncEnumerable<IRow> AllRows()
    {
        var headerRows = Header.Rows;
        var contentRows = Content.Rows;
        var footerRows = Footer.Rows;
        var all = headerRows.Concat(contentRows).Concat(footerRows);
        return all;
    }
}
