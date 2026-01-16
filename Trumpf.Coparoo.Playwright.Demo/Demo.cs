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

namespace Trumpf.Coparoo.Playwright.Demo;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;
using Trumpf.Coparoo.Playwright.Demo.TabObjects;
using Trumpf.Coparoo.Playwright.Extensions;

/// <summary>
/// Demonstration tests showing interaction with Settings/Preferences and IFrame pages in one session.
/// </summary>
[TestClass]
public sealed class Demo
{
    /// <summary>
    /// Headless demo run suitable for CI.
    /// </summary>
    [TestMethod]
    public async Task DemonstrateModularPageComposition_Headless()
    {
        var tab = new DemoTab(headless: true);

        try
        {
            await tab.Open();

            // Settings & Preferences (from demo)
            var settings = await tab.Goto<ISettings>();
            await settings.EnableNotifications.Check();
            (await settings.EnableNotifications.IsChecked).Should().BeTrue();
            await settings.EnableAutoSave.Check();
            await settings.EnableDarkMode.Check();

            var preferences = await tab.Goto<IPreferences>();
            await preferences.SavePreferences.ClickAsync();
            await preferences.ResetToDefaults.ClickAsync();
            await preferences.ExportSettings.ClickAsync();

            // IFrame demo: main page
            await tab.Goto<IIFrameDemoPage>();

            // Payment frame
            var payment = await tab.Goto<IPaymentFrame>();
            await payment.CardNumber.FillAsync("4242424242424242");
            await payment.CVV.FillAsync("123");
            await payment.SubmitButton.ClickAsync();
        }
        finally
        {
            await tab.Close();
        }
    }

    /// <summary>
    /// Headed demo run for visual debugging.
    /// </summary>
    [TestMethod]
    [TestCategory("VisualTest")]
    public async Task DemonstrateModularPageComposition_Headed()
    {
        var tab = new DemoTab(headless: false);

        try
        {
            await tab.Open();

            var settings = await tab.Goto<ISettings>();
            await settings.EnableNotifications.Check();
            await Task.Delay(300);
            await settings.EnableAutoSave.Check();
            await Task.Delay(300);
            await settings.EnableDarkMode.Check();
            await Task.Delay(300);

            var preferences = await tab.Goto<IPreferences>();
            await preferences.SavePreferences.ClickAsync();
            await Task.Delay(400);
            await preferences.ResetToDefaults.ClickAsync();
            await Task.Delay(400);
            await preferences.ExportSettings.ClickAsync();
            await Task.Delay(400);

            await tab.Goto<IIFrameDemoPage>();
            await Task.Delay(400);

            var payment = await tab.Goto<IPaymentFrame>();
            await payment.CardNumber.FillAsync("4242424242424242");
            await Task.Delay(300);
            await payment.CVV.FillAsync("123");
            await Task.Delay(300);
            await payment.SubmitButton.ClickAsync();
            await Task.Delay(800);
        }
        finally
        {
            await tab.Close();
        }
    }
}
