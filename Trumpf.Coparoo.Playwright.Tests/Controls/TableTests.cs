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

namespace Trumpf.Coparoo.Tests;

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Controls;

[TestClass]
public class TableTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenATableIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var tab = await Tab.CreateAsync("<table><thead><tr><th>1</th><th>2</th></tr></thead><tfoot><tr><td>3</td><td>4</td></tr></tfoot><tbody><tr><td>5</td><td>6</td></tr><tr><td>7</td><td>8</td></tr></tbody></table>");

        // Act
        var table = tab.Find<Table>();
        var headerRows = await table.Header.Rows.CountAsync();
        var contentRows = await table.Content.Rows.CountAsync();
        var footerRows = await table.Footer.Rows.CountAsync(); ;
        var allRows = await table.AllRows().CountAsync();

        // Check
        headerRows.Should().Be(1);
        contentRows.Should().Be(2);
        footerRows.Should().Be(1);
        allRows.Should().Be(4);
    }
}