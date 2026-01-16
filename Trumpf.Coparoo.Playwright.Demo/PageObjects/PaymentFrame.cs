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

namespace Trumpf.Coparoo.Playwright.Demo.PageObjects;

using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

/// <summary>
/// Payment frame page object.
/// Demonstrates FramePageObject usage for embedded payment provider iframes.
/// </summary>
public sealed class PaymentFrame : FramePageObject, IPaymentFrame
{
    /// <summary>
    /// Gets the search pattern for locating this frame.
    /// </summary>
    protected override By SearchPattern => By.TestId("payment-frame");

    /// <summary>
    /// Gets the card number input field.
    /// </summary>
    public TextInput CardNumber => Find<TextInput>(By.TestId("card-number"));

    /// <summary>
    /// Gets the CVV input field.
    /// </summary>
    public TextInput CVV => Find<TextInput>(By.TestId("cvv"));

    /// <summary>
    /// Gets the submit button.
    /// </summary>
    public Button SubmitButton => Find<Button>(By.TestId("submit-btn"));

    /// <summary>
    /// Gets the cancel button.
    /// </summary>
    public Button CancelButton => Find<Button>(By.TestId("cancel-btn"));

    /// <summary>
    /// Gets the status display locator.
    /// </summary>
    public ILocator Status => Locator.GetByTestId("status");
}
