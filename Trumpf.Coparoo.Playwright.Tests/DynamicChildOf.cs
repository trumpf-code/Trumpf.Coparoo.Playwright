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

namespace Trumpf.Coparoo.Tests;

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Test class.
/// </summary>
[TestClass]
public class DynamicChildOf
{
    /// <summary>
    /// Interface for B.
    /// </summary>
    private interface IB : IPageObject
    {
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenChildRelationsAreAddedAtRuntime_ThenTheseShowUpInThePageObjectTree()
    {
        // Act
        var root = new A();
        var tree = ((ITreeObject)root).Tree;

        // Check
        tree.EdgeCount.Should().Be(19);
        tree.NodeCount.Should().Be(13);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalled_ThenDynamicallyAddedChildRelationAreEffective()
    {
        // Act
        var root = new A();
        var a = root.On<A>();
        var b = root.On<B>();
        var cobject = root.On<C<object>>();
        var cint = root.On<C<int>>();
        var dobject = root.On<D<object>>();
        var dint = root.On<D<int>>();

        // Check
        root.GetType().Should().Be(typeof(A));
        a.GetType().Should().Be(typeof(A));
        b.GetType().Should().Be(typeof(B));
        cobject.GetType().Should().Be(typeof(C<object>));
        cint.GetType().Should().Be(typeof(C<int>));
        dobject.GetType().Should().Be(typeof(D<object>));
        dint.GetType().Should().Be(typeof(D<int>));
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheSameRelationIsAddedMultipleTime_ThenThisIsRecognizedByTheReturnCode()
    {
        // Act
        var a = new A();

        // Check
        (a.ChildOf<B, A>()).Should().BeFalse(); // this relation was already added during construction
        (a.ChildOf<C<object>, A>()).Should().BeFalse(); // this relation was already added during construction
        (a.ChildOf<C<string>, A>()).Should().BeTrue(); // due to the string parameter, this relation is new
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenAnChildInterfaceIsRegisteredAtRuntime_ThenAnExceptionIsThrown()
    {
        try
        {
            // Act
            new A().ChildOf<IB, A>();
        }
        catch (InvalidOperationException)
        {
            return;
        }

        false.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenAParentInterfaceIsRegisteredAtRuntime_ThenAnExceptionIsThrown()
    {
        try
        {
            // Act
            new A().ChildOf<A, IB>();
        }
        catch (InvalidOperationException)
        {
            return;
        }

        false.Should().BeTrue();
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class A : TabObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="A"/> class.
        /// </summary>
        public A()
        {
            ChildOf<B, A>();
            ChildOf<C<object>, A>();
            ChildOf<C<int>, A>();
            ChildOf<D<object>, A>();
            ChildOf<D<int>, A>();
            ChildOf<E<object>, D<object>>();
            ChildOf<E<object>, D<int>>();
            ChildOf<E<int>, D<object>>();
            ChildOf<E<int>, D<int>>();
            ChildOf<F<object, object>, D<object>>();
            ChildOf<F<object, int>, D<object>>();
            ChildOf<F<int, object>, D<object>>();
            ChildOf<F<int, int>, D<object>>();
            ChildOf<F<object, object>, D<int>>();
            ChildOf<F<object, int>, D<int>>();
            ChildOf<F<int, object>, D<int>>();
            ChildOf<F<int, int>, D<int>>();
            ChildOf<G, A>();
            ChildOf<G, B>();
        }
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class B : PageObject, IB
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class C<T> : PageObject, IChildOf<A>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class D<T> : PageObject
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class E<T> : PageObject
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class F<T, TT> : PageObject
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class G : PageObject
    {
        protected override By SearchPattern => null;
    }
}