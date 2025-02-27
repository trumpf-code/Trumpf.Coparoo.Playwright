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

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Controls;

[TestClass]
public class Root
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenRootIsCalledOnRoot_ThenTheRootIsReturned()
    {
        // Act
        var root = new A();
        var rootOfRoot = (root as IUIObjectInternal).Root();

        // Check
        rootOfRoot.Should().Be(root);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenWeGetTheRootOfTheChildOfTheRoot_ThenTheRootIsReturned()
    {
        // Act
        var root = new A();
        var child = root.On<B>();
        var rootOfTheRoot = (root as IUIObjectInternal).Root();
        var rootOfTheChild = (child as IUIObjectInternal).Root();

        // Check
        rootOfTheChild.Should().Be(rootOfTheRoot);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenWeGetTheRootOfTheChildOfChildOfTheRoot_ThenTheRootIsReturned()
    {
        // Act
        var root = new A();
        var grandchild = root.On<C>();
        var rootOfTheRoot = (root as IUIObjectInternal).Root();
        var rootOfTheGrandchild = (grandchild as IUIObjectInternal).Root();

        // Check
        rootOfTheGrandchild.Should().Be(rootOfTheRoot);
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class A : TabObject
    {
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class B : PageObject, IChildOf<A>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class C : PageObject, IChildOf<B>
    {
        protected override By SearchPattern => null;
    }
}