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

namespace Trumpf.Coparoo.Playwright.Demo.PageObjects.Interfaces;

using Microsoft.Playwright;
using Trumpf.Coparoo.Playwright.Controls;

/// <summary>
/// Interface for the payment frame page object.
/// Represents an embedded payment provider iframe.
/// </summary>
public interface IPaymentFrame : IFramePageObject 
{
    /// <summary>
    /// Gets the card number input field.
    /// </summary>
    TextInput CardNumber { get; }

    /// <summary>
    /// Gets the CVV input field.
    /// </summary>
    TextInput CVV { get; }

    /// <summary>
    /// Gets the submit button.
    /// </summary>
    Button SubmitButton { get; }

    /// <summary>
    /// Gets the cancel button.
    /// </summary>
    Button CancelButton { get; }

    /// <summary>
    /// Gets the status display locator.
    /// </summary>
    ILocator Status { get; }
}
