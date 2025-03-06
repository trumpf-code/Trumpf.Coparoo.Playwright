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

using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Exceptions;

[TestClass]
public class TabObjectTests
{
    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenCastingTabObjects_ThenTheCorrectTabObjectsAreReturned()
    {
        //Prepare
        var tab = await Tab.CreateAsync($"<button type=\"button\">text</button>");

        // Act
        var t1 = tab.Cast<T1>();
        var b1 = t1.On<B1>();
        var e1 = await b1.Visible();

        var t2 = tab.Cast<T2>();
        var b2 = t2.On<B2>();
        var e2 = await b2.Visible();

        // Check
        e1.Should().BeTrue();
        e2.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public async Task WhenCastingATabObjectAndThenBack_ThenTheCorrectTabObjectIsReturned()
    {
        //Prepare
        var tab = await Tab.CreateAsync($"<button type=\"button\">text</button>");

        // Act
        var t1 = tab.Cast<T1>();
        var t2 = t1.Cast<T2>(); // cast
        var t3 = t2.Cast<T1>(); // cast back

        // Act
        var b1 = t3.On<B1>();
        var e1 = await b1.Visible();

        // Check
        e1.Should().BeTrue();
    }

    /// <summary>
    /// Test method.
    /// </summary>
    [TestMethod]
    public void WhenCastingATabObject_ThenOnlyPageObjectInTheCastedPageObjectTreeCanBeAccessed()
    {
        // Prepare
        ITabObject tab = new Tab();

        // Act
        var t1 = tab.Cast<T1>();
        var t2 = tab.Cast<T2>();

        // Check
        FluentActions.Invoking(() => t1.On<B2>()).Should().Throw<PageObjectNotFoundException<B2>>();
        FluentActions.Invoking(() => t2.On<B1>()).Should().Throw<PageObjectNotFoundException<B1>>();
    }

    protected class T1 : Tab
    {
    }

    protected class T2 : Tab
    {
    }

    protected class B1 : PageObject, IChildOf<T1>
    {
        protected override By SearchPattern => "button";
    }

    protected class B2 : PageObject, IChildOf<T2>
    {
        protected override By SearchPattern => "button";
    }
}
