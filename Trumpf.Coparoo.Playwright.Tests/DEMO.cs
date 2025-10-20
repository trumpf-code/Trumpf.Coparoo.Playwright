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

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Test class.
/// </summary>
//[TestClass]
public class DEMO
{
    public class MyTab : TabObject
    {
        protected override async Task<IPage> Creator()
        {
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            return await browser.NewPageAsync();
        }

        protected override string Url => "http://google.de";
    }

    //[TestMethod]
    public async Task OpenAndClose()
    {
        var tab = new MyTab();              // create the root page object
        await tab.Open();                   // open the web page in new tab
        await tab.WaitForVisibleAsync();    // wait until the tab exists
        await tab.Close();                  // close the tab and browser
    }

    public class Link : ControlObject
    {
        protected override By SearchPattern => "a";
        public Task<string> Text 
            => GetTextAsync();
        
        private async Task<string> GetTextAsync()
            => await (await Locator).TextContentAsync();
        
        public Task<string> URL 
            => GetURLAsync();
        
        private async Task<string> GetURLAsync()
            => await (await Locator).GetAttributeAsync("href");
    }

    //[TestMethod]
    public async Task PrintLinkTexts()
    {
        var tab = new MyTab();
        await tab.Open();
        await tab.WaitForVisibleAsync();
        await foreach (var link in tab.FindAll<Link>())
            Trace.WriteLine($"Text: {await link.Text}");
    }

    public class Menu : PageObject, IChildOf<MyTab>
    {
        protected override By SearchPattern => By.ClassName("menu-header");  // some unique search criteria for the menu
        public Link Events => Find<Link>("myId");// some menu link
    }

    //[TestMethod]
    public async Task ClickEventsOnMenu()
    {
        MyTab myTab = new MyTab();              // create the root page object
        await myTab.Open();                     // open a new tab browser with the address 
        await myTab.On<Menu>().WaitForVisibleAsync();   // wait until the menu is displayed
        await myTab.On<Menu>().Events.Click();        // on the menu click the events link
        await myTab.Close();                    // kill the browser
    }

    public class Header : PageObject, IChildOf<MyTab>
    {
        protected override By SearchPattern => ".header";
        public Button SignIn => Find<Button>();
    }

    public class OtherMenu : PageObject, IChildOf<MyTab>
    {
        protected override By SearchPattern => By.ClassName("menu-header");  // some unique search criteria for the menu
        public Link Events => Find<Link>("myId");// some menu link
        public override async Task Goto()
        {
            if (!await (await this.Locator).IsVisibleAsync())       // no actions if the page is already displayed
            {
                await Goto<Header>().SignIn.Click();      // click the sign in button
                await this.WaitForVisibleAsync();   // wait for the SignInForm to be displayed
            }
        }
    }
}
#endif