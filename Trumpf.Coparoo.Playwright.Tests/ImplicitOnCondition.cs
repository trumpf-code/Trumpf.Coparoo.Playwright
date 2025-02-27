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
public class ConditionalOn
{
    /// <summary>
    /// Gets the process object with default depths.
    /// </summary>
    private A P => new();

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenAccessingAPage_ThenTheOnConditionIsEvalutated()
    {
        // Act
        var typeOfB = typeof(B);
        C.ExpectedParentType = typeOfB; // set the expected type; this has a side-effect on the succeeding call to On
        var d = P.On<D>();

        // Check
        d.Parent.Parent.GetType().Should().Be(typeOfB);
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
    private class C : PageObject, IChildOf<A>, IChildOf<B>
    {
        /// <summary>
        /// Gets or sets the expected type.
        /// </summary>
        public static Type ExpectedParentType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the expected type is the parent.
        /// </summary>
        public Task<bool> IsDisplayed => Task.FromResult(Parent.GetType().Equals(ExpectedParentType));

        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class D : PageObject, IChildOf<C>
    {
        /// <summary>
        /// Gets a value indicating whether the expected type is the parent.
        /// </summary>
        protected override bool OnCondition =>  IsDisplayed.Result;

        /// <summary>
        /// Gets a value indicating whether the expected type is the parent.
        /// </summary>
        public Task<bool> IsDisplayed => (Parent as C).IsDisplayed;

        protected override By SearchPattern => null;
    }
}