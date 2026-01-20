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

namespace Trumpf.Coparoo.Tests.Frame;

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trumpf.Coparoo.Playwright;
using Trumpf.Coparoo.Playwright.Controls;
using Trumpf.Coparoo.Playwright.Extensions;

[TestClass]
public class FrameTests
{
    internal static string HtmlFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "frame-test.html");
    
    /// <summary>
    /// Test that FramePageObject can locate and interact with elements inside an iframe.
    /// </summary>
    [TestMethod]
    public async Task FramePageObject_ShouldLocateElementsInsideIframe()
    {
        // Arrange
        var tab = new FrameTestTab(headless: true);
        
        try
        {
            await tab.Open();
            
            // Act - Navigate to payment frame
            var paymentFrame = tab.On<IPaymentFrame>();
            
            // Assert - Check frame element exists
            (await paymentFrame.FrameElement.IsVisibleAsync()).Should().BeTrue();
            
            // Assert - Interact with elements inside the frame
            await paymentFrame.CardNumber.FillAsync("4242424242424242");
            await paymentFrame.CVV.FillAsync("123");
            
            var cardValue = await paymentFrame.CardNumber.InputValueAsync();
            var cvvValue = await paymentFrame.CVV.InputValueAsync();
            
            cardValue.Should().Be("4242424242424242");
            cvvValue.Should().Be("123");
            
            // Submit and verify status change
            await paymentFrame.SubmitButton.ClickAsync();
            await Task.Delay(100); // Wait for click handler
            
            var status = await paymentFrame.Status.TextContentAsync();
            status.Should().Be("Payment submitted!");
        }
        finally
        {
            await tab.Close();
        }
    }
    
    /// <summary>
    /// Test that FrameControlObject can locate and interact with transient controls inside an iframe.
    /// </summary>
    [TestMethod]
    public async Task FrameControlObject_ShouldLocateElementsInsideIframe()
    {
        // Arrange
        var tab = new FrameTestTab(headless: true);
        
        try
        {
            await tab.Open();
            
            // Act - Access editor frame directly from tab (FramePageObject)
            var editor = tab.On<IEditorFrame>();
            
            // Assert - Interact with editor controls inside the frame
            await editor.BoldButton.ClickAsync();
            await Task.Delay(100);
            
            var status = await editor.EditorStatus.TextContentAsync();
            status.Should().Be("Bold applied");
            
            // Test other buttons
            await editor.ItalicButton.ClickAsync();
            await Task.Delay(100);
            status = await editor.EditorStatus.TextContentAsync();
            status.Should().Be("Italic applied");
            
            // Test content area
            await editor.ContentArea.FillAsync("Test content");
            var content = await editor.ContentArea.InputValueAsync();
            content.Should().Be("Test content");
        }
        finally
        {
            await tab.Close();
        }
    }
    
    /// <summary>
    /// Test that elements outside the frame remain accessible.
    /// </summary>
    [TestMethod]
    public async Task ShouldAccessBothFrameAndMainPageElements()
    {
        // Arrange
        var tab = new FrameTestTab(headless: true);
        
        try
        {
            await tab.Open();
            
            // Act & Assert - Main page elements
            var mainPage = tab.On<IFrameTestPage>();
            await mainPage.MainButton.ClickAsync();
            await Task.Delay(100);
            var result = await mainPage.Result.TextContentAsync();
            result.Should().Be("Main button clicked!");
            
            // Act & Assert - Frame elements (accessed directly from tab)
            var paymentFrame = tab.On<IPaymentFrame>();
            await paymentFrame.CardNumber.FillAsync("1234567890123456");
            var cardValue = await paymentFrame.CardNumber.InputValueAsync();
            cardValue.Should().Be("1234567890123456");
        }
        finally
        {
            await tab.Close();
        }
    }
    
    /// <summary>
    /// Test nested frames (frame within frame).
    /// </summary>
    [TestMethod]
    public async Task FramePageObject_ShouldHandleNestedFrames()
    {
        // Arrange
        var tab = new FrameTestTab(headless: true);
        
        try
        {
            await tab.Open();
            
            // Act - Access outer frame
            var outerFrame = tab.On<IOuterFrame>();
            (await outerFrame.FrameElement.IsVisibleAsync()).Should().BeTrue();
            
            var outerTitle = await outerFrame.OuterTitle.TextContentAsync();
            outerTitle.Should().Be("Outer Frame");
            
            await outerFrame.OuterButton.ClickAsync();
            
            // Act - Access inner frame (nested inside outer frame)
            var innerFrame = outerFrame.On<IInnerFrame>();
            (await innerFrame.FrameElement.IsVisibleAsync()).Should().BeTrue();
            
            var innerTitle = await innerFrame.InnerTitle.TextContentAsync();
            innerTitle.Should().Be("Inner Frame");
            
            await innerFrame.InnerButton.ClickAsync();
        }
        finally
        {
            await tab.Close();
        }
    }
}

// ========== Tab Object ==========

internal class FrameTestTab : TabObject
{
    private readonly bool headless;
    
    public FrameTestTab(bool headless = false)
    {
        this.headless = headless;
        
        // Register page relationships
        ChildOf<PaymentFrame, FrameTestTab>();
        ChildOf<FrameTestPage, FrameTestTab>();
        ChildOf<EditorFrame, FrameTestTab>();  // Editor is a direct child of Tab
        ChildOf<OuterFrame, FrameTestTab>();
        ChildOf<InnerFrame, OuterFrame>();
    }
    
    protected override string Url => "file://" + FrameTests.HtmlFilePath.Replace("\\", "/");
    
    protected override async Task<IPage> CreatePageAsync()
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = headless });
        return await browser.NewPageAsync();
    }
}

// ========== Main Page Object ==========

internal interface IFrameTestPage : IPageObject 
{
    Button MainButton { get; }
    ILocator Title { get; }
    ILocator Result { get; }
}

internal class FrameTestPage : PageObject, IFrameTestPage
{
    // Use body or a container that contains all elements
    protected override By SearchPattern => By.CssSelector("body");
    
    public ILocator Title => Locator.GetByTestId("page-title");
    public Button MainButton => Find<Button>(By.TestId("main-button"));
    public ILocator Result => Locator.GetByTestId("result");
}

// ========== Payment Frame (FramePageObject) ==========

internal interface IPaymentFrame : IFramePageObject 
{
    TextInput CardNumber { get; }
    TextInput CVV { get; }
    Button SubmitButton { get; }
    Button CancelButton { get; }
    ILocator Status { get; }
}

internal class PaymentFrame : FramePageObject, IPaymentFrame
{
    protected override By SearchPattern => By.TestId("payment-frame");
    
    public TextInput CardNumber => Find<TextInput>(By.TestId("card-number"));
    public TextInput CVV => Find<TextInput>(By.TestId("cvv"));
    public Button SubmitButton => Find<Button>(By.TestId("submit-btn"));
    public Button CancelButton => Find<Button>(By.TestId("cancel-btn"));
    public ILocator Status => Locator.GetByTestId("status");
}

// ========== Editor Frame (FramePageObject - treated as page for direct Tab access) ==========

internal interface IEditorFrame : IFramePageObject 
{
    Button BoldButton { get; }
    Button ItalicButton { get; }
    Button SaveButton { get; }
    TextInput ContentArea { get; }
    ILocator EditorStatus { get; }
}

internal class EditorFrame : FramePageObject, IEditorFrame
{
    protected override By SearchPattern => By.TestId("editor-frame");
    
    public Button BoldButton => Find<Button>(By.TestId("bold-btn"));
    public Button ItalicButton => Find<Button>(By.TestId("italic-btn"));
    public Button SaveButton => Find<Button>(By.TestId("save-btn"));
    public TextInput ContentArea => Find<TextInput>(By.TestId("editor-content"));
    public ILocator EditorStatus => Locator.GetByTestId("editor-status");
}

// ========== Nested Frames ==========

internal interface IOuterFrame : IFramePageObject 
{
    ILocator OuterTitle { get; }
    Button OuterButton { get; }
}

internal class OuterFrame : FramePageObject, IOuterFrame
{
    protected override By SearchPattern => By.TestId("outer-frame");
    
    public ILocator OuterTitle => Locator.GetByTestId("outer-title");
    public Button OuterButton => Find<Button>(By.TestId("outer-button"));
}

internal interface IInnerFrame : IFramePageObject 
{
    ILocator InnerTitle { get; }
    Button InnerButton { get; }
}

internal class InnerFrame : FramePageObject, IInnerFrame
{
    protected override By SearchPattern => By.TestId("inner-frame");
    
    public ILocator InnerTitle => Locator.GetByTestId("inner-title");
    public Button InnerButton => Find<Button>(By.TestId("inner-button"));
}
