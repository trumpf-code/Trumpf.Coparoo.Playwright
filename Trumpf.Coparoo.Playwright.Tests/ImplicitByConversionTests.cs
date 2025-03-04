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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trumpf.Coparoo.Playwright;

[TestClass]
public class ImplicitByConversionTests
{
    [TestMethod]
    public void ImplicitConversion_FromStringToBy_ShouldSetSelectorCorrectly()
    {
        // Arrange
        string selector = "div.container";

        // Act
        By byObject = selector;

        // Assert
        byObject.Should().NotBeNull();
        byObject.ToLocator().Should().Be("div.container");
    }

    [TestMethod]
    public void ImplicitConversion_FromByToString_ShouldReturnCorrectSelector()
    {
        // Arrange
        By byObject = "button.submit";

        // Act
        string selectorString = byObject;

        // Assert
        selectorString.Should().Be("button.submit");
    }

    [TestMethod]
    public void ImplicitConversion_FromNullByToString_ShouldThrowException()
    {
        // Arrange
        By byObject = null;

        // Act
        Action act = () => { string selectorString = byObject; };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}