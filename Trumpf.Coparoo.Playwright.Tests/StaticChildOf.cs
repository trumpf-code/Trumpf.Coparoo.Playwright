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

#if DEBUG 
namespace Trumpf.Coparoo.Tests;

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Exceptions;
using Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Test class.
/// </summary>
[TestClass]
public class PageObjectLocatorStatic
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheRootObjectIsFreshh_ThenTheCacheIsEmpty()
    {
        // Act
        var root = new A();

        // Check
        (root as ITabObjectInternal).PageObjectLocator.CacheObjectCount.Should().Be(0);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenThePageObjectsAreAccessed_ThenTheyAreCached()
    {
        // Act
        var root = new A();
        var a = root.On<A>();
        var b = root.On<B>();

        // Check
        (root as ITabObjectInternal).PageObjectLocator.CacheObjectCount.Should().Be(2);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenThePageObjectCacheIsCleared_ThenItIsEmpty()
    {
        // Act
        var root = new A() as ITabObjectInternal;
        var a = root.On<A>();
        var b = root.On<B>();
        root.PageObjectLocator.Clear();

        // Check
        root.PageObjectLocator.CacheObjectCount.Should().Be(0);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenAPageObjectIsUnregistered_ThenIsDisappearsFromTheCache()
    {
        // Act
        var root = new A() as ITabObjectInternal;
        var b = root.On<B>();
        root.PageObjectLocator.Unregister<B>();

        // Check
        root.PageObjectLocator.CacheObjectCount.Should().Be(0);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenAPageObjectIsUnregistedBeforeItWasCached_ThenNoExceptionIsThrown()
    {
        // Act
        var a = new A() as ITabObjectInternal;
        a.PageObjectLocator.Unregister<B>();

        // Check
        a.PageObjectLocator.CacheObjectCount.Should().Be(0);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheSamePageObjectIsAccessedOnDifferentPath_ThenBothAreResultsAreCached()
    {
        // Act
        var a = new A() as ITabObjectInternal;
        var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
        var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

        // Check
        (a as ITabObjectInternal).PageObjectLocator.CacheObjectCount.Should().Be(2);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheSamePageObjectIsAccessedOnDifferentPath_ThenExplicitOnConditionIsEffective()
    {
        // Act
        var a = new A() as ITabObjectInternal;
        var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
        var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

        // Check
        gWithParentA.Parent.GetType().Should().Be(typeof(A));
        gWithParentB.Parent.GetType().Should().Be(typeof(B));
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheSamePageObjectIsAccessedOnDifferentPathMultipleTime_ThenDuplicatesDoNotFurtherIncreaseTheCacheCount()
    {
        // Act
        var a = new A() as ITabObjectInternal;
        var gWithParentA = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
        var gWithParentB = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));
        var gWithParentA2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(A)));
        var gWithParentB2 = a.On<G>(e => e.Parent.GetType().Equals(typeof(B)));

        // Check
        a.PageObjectLocator.CacheObjectCount.Should().Be(2);
        gWithParentA.Parent.GetType().Should().Be(typeof(A));
        gWithParentB.Parent.GetType().Should().Be(typeof(B));
        gWithParentA2.Parent.GetType().Should().Be(typeof(A));
        gWithParentB2.Parent.GetType().Should().Be(typeof(B));
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheDotTreeIsGenerated_ThenIsContainsTheExpectedNodeAndEdgeCount()
    {
        // Act
        var tree = ((ITreeObject)new A()).Tree;

        // Check
        tree.EdgeCount.Should().Be(4);
        tree.NodeCount.Should().Be(4);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    //[TestMethod]
    public void WhenTheDotTreeIsWrittenToDisk_ThenAFileIsCreated()
    {
        string file = null;
        try
        {
            // Act
            var root = new A();
            file = root.WriteTree();

            // Check
            (File.Exists(file)).Should().BeTrue();
        }
        finally
        {
            // Clean up
            if (file != null && File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalled_ThenStaticallyAddedChildRelationAreEffective()
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
    public void WhenMultipleRootObjectAreCreated_ThenTheirCachesAreSeperated()
    {
        // Act
        var root1 = new A() as ITabObjectInternal;
        var root2 = new A() as ITabObjectInternal;
        B b = root1.On<B>(); // increase the counter by one

        // Check
        root1.PageObjectLocator.CacheTypeCount.Should().Be(1);
        root2.PageObjectLocator.CacheTypeCount.Should().Be(0);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalledManyTimes_ThenTheCacheCountIncreases()
    {
        // Act
        var root = new A() as ITabObjectInternal; // create the root object, initially the cache count shall be zero
        root.On<A>(); // add all new relation, which shall increase the counter once per step
        root.On<B>();
        root.On<C<object>>();
        root.On<C<int>>();
        root.On<D<object>>();
        root.On<D<int>>();

        // Check
        root.PageObjectLocator.CacheTypeCount.Should().Be(6);
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalledWithKnownObject_ThenTheCacheDoesNotIncreases()
    {
        // Act
        var root = new A() as ITabObjectInternal; // create the root object, initially the cache count shall be zero
        foreach (var i in Enumerable.Range(0, 3))
        {
            root.On<A>();
            root.On<B>();
            root.On<C<object>>();
            root.On<C<int>>();
            root.On<D<object>>();
            root.On<D<int>>();
        }

        // Check
        root.PageObjectLocator.CacheTypeCount.Should().Be(6);
    }

    /// <summary>
    /// Test method.
    /// Generic page object children of generic page objects are not supported
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalledWithAGenericChildOfAGenericChild_ThenAnExceptionIsThrown()
    {
        // Act
        try
        {
            new A().On<E<object>>(); // E<T> is child of D<T>
        }
        catch (PageObjectNotFoundException<E<object>>)
        {
            return;
        }

        false.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// Generic page object children of generic page objects are not supported
    /// </summary>
    [TestMethod]
    public void WhenTheOnMethodIsCalledWithAGenericChildOfAGenericChildThatHasMultipleTypeParameters_ThenAnExceptionIsThrown()
    {
        try
        {
            // Act
            new A().On<F<object, object>>(); // F<T, TT> is child of D<T>
        }
        catch (PageObjectNotFoundException<F<object, object>>)
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
    private class C<T> : PageObject, IChildOf<A>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class D<T> : PageObject, IChildOf<A>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class E<T> : PageObject, IChildOf<D<T>>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class F<T, TT> : PageObject, IChildOf<D<T>>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class G : PageObject, IChildOf<A>, IChildOf<B>
    {
        protected override By SearchPattern => null;
    }

    /// <summary>
    /// Helper class.
    /// </summary>
    private class B2 : B
    {
    }
}
#endif