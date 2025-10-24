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

namespace Trumpf.Coparoo.Tests.Search;

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;

/// <summary>
/// Test class for By selector combinations.
/// </summary>
[TestClass]
public class ByTests
{
    #region Tag Selector Position Tests

    /// <summary>
    /// Test that tag selectors are always placed first in combined selectors.
    /// </summary>
    [TestMethod]
    public void WhenCombiningTagWithClass_ThenTagComesFirst()
    {
        // Act
        var result = By.ClassName("primary").And(By.TagName("button"));

        // Check
        result.ToLocator().Should().Be("button.primary");
    }

    /// <summary>
    /// Test that tag selectors are always placed first when combined with ID.
    /// </summary>
    [TestMethod]
    public void WhenCombiningTagWithId_ThenTagComesFirst()
    {
        // Act
        var result = By.Id("submit").And(By.TagName("button"));

        // Check
        result.ToLocator().Should().Be("button#submit");
    }

    /// <summary>
    /// Test that tag selectors are placed first with multiple other selectors.
    /// </summary>
    [TestMethod]
    public void WhenCombiningTagWithIdAndClass_ThenTagComesFirst()
    {
        // Act
        var result = By.ClassName("primary")
            .And(By.Id("submit"))
            .And(By.TagName("button"));

        // Check
        result.ToLocator().Should().Be("button#submit.primary");
    }

    /// <summary>
    /// Test correct order: tag, ID, class, attribute, pseudo.
    /// </summary>
    [TestMethod]
    public void WhenCombiningAllSelectorTypes_ThenCorrectOrderIsApplied()
    {
        // Act
        var result = By.ClassName("primary")
            .And(By.CssSelector(":hover"))
            .And(By.TestId("my-test"))
            .And(By.Id("submit"))
            .And(By.TagName("button"));

        // Check
        var locator = result.ToLocator();
        locator.Should().StartWith("button");
        locator.Should().Contain("#submit");
        locator.Should().Contain(".primary");
        locator.Should().Contain("[data-testid=\"my-test\"]");
        locator.Should().Contain(":hover");
    }

    #endregion

    #region Multiple Tag Selectors Tests

    /// <summary>
    /// Test that combining two tag selectors throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningTwoTagSelectors_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.TagName("div").And(By.TagName("span"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*multiple tag selectors*");
    }

    /// <summary>
    /// Test that combining three tag selectors throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningThreeTagSelectors_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.TagName("div")
            .And(By.TagName("span"))
            .And(By.TagName("button"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*multiple tag selectors*");
    }

    #endregion

    #region Multiple ID Selectors Tests

    /// <summary>
    /// Test that combining two ID selectors throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningTwoIdSelectors_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.Id("main").And(By.Id("secondary"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*multiple ID selectors*");
    }

    /// <summary>
    /// Test that combining three ID selectors throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningThreeIdSelectors_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.Id("first")
            .And(By.Id("second"))
            .And(By.Id("third"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*multiple ID selectors*");
    }

    #endregion

    #region Multiple Class Selectors Tests (Should Work)

    /// <summary>
    /// Test that combining multiple class selectors is allowed.
    /// </summary>
    [TestMethod]
    public void WhenCombiningMultipleClassSelectors_ThenNoExceptionIsThrown()
    {
        // Act
        var result = By.ClassName("primary").And(By.ClassName("large"));

        // Check
        result.ToLocator().Should().Be(".primary.large");
    }

    /// <summary>
    /// Test that combining three class selectors is allowed.
    /// </summary>
    [TestMethod]
    public void WhenCombiningThreeClassSelectors_ThenNoExceptionIsThrown()
    {
        // Act
        var result = By.ClassName("btn")
            .And(By.ClassName("btn-primary"))
            .And(By.ClassName("btn-lg"));

        // Check
        result.ToLocator().Should().Be(".btn.btn-primary.btn-lg");
    }

    #endregion

    #region Duplicate Attribute Selectors Tests

    /// <summary>
    /// Test that combining duplicate TestId selectors throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningDuplicateTestIdSelectors_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.TestId("first").And(By.TestId("second"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*duplicate attribute selectors*data-testid*");
    }

    /// <summary>
    /// Test that combining different attribute selectors is allowed.
    /// </summary>
    [TestMethod]
    public void WhenCombiningDifferentAttributeSelectors_ThenNoExceptionIsThrown()
    {
        // Act
        var result = By.TestId("my-test")
            .And(By.CssSelector("[disabled]"));

        // Check
        result.ToLocator().Should().Contain("[data-testid=\"my-test\"]");
        result.ToLocator().Should().Contain("[disabled]");
    }

    /// <summary>
    /// Test that combining the same custom attribute throws an exception.
    /// </summary>
    [TestMethod]
    public void WhenCombiningSameCustomAttribute_ThenExceptionIsThrown()
    {
        // Act
        Action act = () => By.CssSelector("[type='button']")
            .And(By.CssSelector("[type='submit']"));

        // Check
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*duplicate attribute selectors*type*");
    }

    #endregion

    #region Complex Combination Tests

    /// <summary>
    /// Test a valid complex combination.
    /// </summary>
    [TestMethod]
    public void WhenCombiningValidComplexSelectors_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.TagName("button")
            .And(By.Id("submit"))
            .And(By.ClassName("primary"))
            .And(By.ClassName("large"))
            .And(By.TestId("submit-btn"))
            .And(By.CssSelector("[disabled]"))
            .And(By.CssSelector(":hover"));

        // Check
        var locator = result.ToLocator();
        locator.Should().StartWith("button");
        locator.Should().Contain("#submit");
        locator.Should().Contain(".primary");
        locator.Should().Contain(".large");
        locator.Should().Contain("[data-testid=\"submit-btn\"]");
        locator.Should().Contain("[disabled]");
        locator.Should().Contain(":hover");
        
        // Verify order
        var buttonIndex = locator.IndexOf("button");
        var idIndex = locator.IndexOf("#submit");
        var classIndex = locator.IndexOf(".primary");
        
        buttonIndex.Should().BeLessThan(idIndex);
        idIndex.Should().BeLessThan(classIndex);
    }

    /// <summary>
    /// Test combining in reverse order still produces correct output.
    /// </summary>
    [TestMethod]
    public void WhenCombiningInReverseOrder_ThenCorrectOrderIsApplied()
    {
        // Act
        var result = By.CssSelector(":hover")
            .And(By.ClassName("primary"))
            .And(By.Id("submit"))
            .And(By.TagName("button"));

        // Check
        var locator = result.ToLocator();
        locator.Should().StartWith("button");
        
        var buttonIndex = locator.IndexOf("button");
        var idIndex = locator.IndexOf("#submit");
        var classIndex = locator.IndexOf(".primary");
        var hoverIndex = locator.IndexOf(":hover");
        
        buttonIndex.Should().BeLessThan(idIndex);
        idIndex.Should().BeLessThan(classIndex);
        classIndex.Should().BeLessThan(hoverIndex);
    }

    #endregion

    #region Single Selector Tests

    /// <summary>
    /// Test that a single tag selector works correctly.
    /// </summary>
    [TestMethod]
    public void WhenUsingSingleTagSelector_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.TagName("button");

        // Check
        result.ToLocator().Should().Be("button");
    }

    /// <summary>
    /// Test that a single ID selector works correctly.
    /// </summary>
    [TestMethod]
    public void WhenUsingSingleIdSelector_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.Id("submit");

        // Check
        result.ToLocator().Should().Be("#submit");
    }

    /// <summary>
    /// Test that a single class selector works correctly.
    /// </summary>
    [TestMethod]
    public void WhenUsingSingleClassSelector_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.ClassName("primary");

        // Check
        result.ToLocator().Should().Be(".primary");
    }

    /// <summary>
    /// Test that a single TestId selector works correctly.
    /// </summary>
    [TestMethod]
    public void WhenUsingSingleTestIdSelector_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.TestId("my-test");

        // Check
        result.ToLocator().Should().Be("[data-testid=\"my-test\"]");
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// Test that special characters in IDs are properly escaped.
    /// </summary>
    [TestMethod]
    public void WhenUsingIdWithSpecialCharacters_ThenCharactersAreEscaped()
    {
        // Act
        var result = By.Id("my.special#id");

        // Check
        result.ToLocator().Should().Contain("\\.");
        result.ToLocator().Should().Contain("\\#");
    }

    /// <summary>
    /// Test that special characters in class names are properly escaped.
    /// </summary>
    [TestMethod]
    public void WhenUsingClassWithSpecialCharacters_ThenCharactersAreEscaped()
    {
        // Act
        var result = By.ClassName("my.special:class");

        // Check
        result.ToLocator().Should().Contain("\\.");
        result.ToLocator().Should().Contain("\\:");
    }

    /// <summary>
    /// Test combining tag with only classes (no ID).
    /// </summary>
    [TestMethod]
    public void WhenCombiningTagWithOnlyClasses_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.TagName("button")
            .And(By.ClassName("primary"))
            .And(By.ClassName("large"));

        // Check
        result.ToLocator().Should().Be("button.primary.large");
    }

    /// <summary>
    /// Test combining only ID and classes (no tag).
    /// </summary>
    [TestMethod]
    public void WhenCombiningIdWithOnlyClasses_ThenCorrectLocatorIsReturned()
    {
        // Act
        var result = By.Id("submit")
            .And(By.ClassName("primary"))
            .And(By.ClassName("large"));

        // Check
        result.ToLocator().Should().Be("#submit.primary.large");
    }

    #endregion
}
