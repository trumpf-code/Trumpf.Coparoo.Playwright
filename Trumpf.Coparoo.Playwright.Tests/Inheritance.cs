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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;

/// <summary>
/// Test class.
/// </summary>
[TestClass]
public class Inheritance
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenADerivedPageObjectIsPassedToTheOnMethod_ThenTheResultHasTheSameTypeAsTheInputType()
    {
        // Act
        var c = new A().On<C>();
        var b = new A().On<B>();

        // Check
        c.GetType().Should().Be(typeof(C));
        b.GetType().Should().Be(typeof(B));
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class A : TabObject
    {
        protected override Task<IPage> CreatePageAsync() => Task.FromResult<IPage>(null);
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
    private class C : B
    {
    }
}