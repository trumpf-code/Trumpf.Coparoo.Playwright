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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Controls;
using FluentAssertions;
using System.Diagnostics;

namespace Trumpf.Coparoo.Tests;

[TestClass]
public class LabelTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenALabelIsAccessed_ThenItCanBeFoundAndThePropertiesFit()
    {
        // Prepare
        var expectedLabelText = $"hellp";
        var tab = await TestTab.CreateAsync($"<label>{expectedLabelText}</label>");
        var label = tab.Find<Label>();

        // Act
        var labelText = await label.Text;

        // Log
        Trace.WriteLine(labelText);

        // Check
        labelText.Should().Be(expectedLabelText);
    }
}