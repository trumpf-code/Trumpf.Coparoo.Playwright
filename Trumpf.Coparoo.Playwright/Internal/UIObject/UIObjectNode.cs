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

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trumpf.Coparoo.Playwright.Internal;

/// <summary>
/// Page object node class wrapping a UI tree node.
/// </summary>
internal class UIObjectNode : IUIObjectNode, IUIObjectNodeInternal
{
    private By searchPattern;
    private IUIObjectNode mParent;
    private ITabObjectNode mRootNode = null;

    /// <summary>
    /// Gets the root to node search pattern.
    /// </summary>
    public virtual By SearchPattern
        => searchPattern;

    /// <summary>
    /// Sets the 0-based control index.
    /// </summary>
    public int Index { get; set; } = 0;

    /// <summary>
    /// Gets the process node.
    /// </summary>
    public ITabObjectNode RootNode
        => mRootNode ??= this is ITabObjectNode
            ? this as ITabObjectNode
            : ((IUIObjectNodeInternal)mParent).RootNode;

    /// <summary>
    /// Gets the root node.
    /// </summary>
    protected virtual Task<ILocator> Parent
        => (mParent as IUIObjectNodeInternal).Root();

    /// <summary>
    /// Gets the node representing this tree node in the UI.
    /// </summary>
    public async virtual Task<ILocator> Root()
        => (await Parent).Locator(SearchPattern.ToLocator()).Nth(Index);

    /// <summary>
    /// Initialize this object.
    /// The parent node is used to search nodes without, hence disabling any caching.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <returns>This object.</returns>
    public IUIObjectNode Init(IUIObjectNode parent)
    {
        mParent = parent;
        return this;
    }

    /// <summary>
    /// Initialize the control object.
    /// </summary>
    /// <param name="searchPattern">The search pattern used to locate the control.</param>
    public void Init(By searchPattern)
        => this.searchPattern = searchPattern;

    #region ILocator
    public async Task<IReadOnlyList<ILocator>> AllAsync()
        => await (await Root()).AllAsync();

    public async Task<IReadOnlyList<string>> AllInnerTextsAsync()
        => await (await Root()).AllInnerTextsAsync();

    public async Task<IReadOnlyList<string>> AllTextContentsAsync()
        => await (await Root()).AllTextContentsAsync();

    public ILocator And(ILocator locator)
        => Root().Result.And(locator);

    public async Task<string> AriaSnapshotAsync(LocatorAriaSnapshotOptions options = null)
        => await (await Root()).AriaSnapshotAsync(options);

    public async Task BlurAsync(LocatorBlurOptions options = null)
        => await (await Root()).BlurAsync(options);

    public async Task<LocatorBoundingBoxResult> BoundingBoxAsync(LocatorBoundingBoxOptions options = null)
        => await (await Root()).BoundingBoxAsync(options);

    public async Task CheckAsync(LocatorCheckOptions options = null)
        => await (await Root()).CheckAsync(options);

    public async Task ClearAsync(LocatorClearOptions options = null)
        => await (await Root()).ClearAsync(options);

    public async Task ClickAsync(LocatorClickOptions options = null)
        => await (await Root()).ClickAsync(options);

    public async Task<int> CountAsync()
        => await (await Root()).CountAsync();

    public async Task DblClickAsync(LocatorDblClickOptions options = null)
        => await (await Root()).DblClickAsync(options);

    public async Task DispatchEventAsync(string type, object eventInit = null, LocatorDispatchEventOptions options = null)
        => await (await Root()).DispatchEventAsync(type, eventInit, options);

    public async Task DragToAsync(ILocator target, LocatorDragToOptions options = null)
        => await (await Root()).DragToAsync(target, options);

    public async Task<IElementHandle> ElementHandleAsync(LocatorElementHandleOptions options = null)
        => await (await Root()).ElementHandleAsync(options);

    public async Task<IReadOnlyList<IElementHandle>> ElementHandlesAsync()
        => await (await Root()).ElementHandlesAsync();

    public async Task<T> EvaluateAsync<T>(string expression, object arg = null, LocatorEvaluateOptions options = null)
        => await (await Root()).EvaluateAsync<T>(expression, arg, options);

    public async Task<T> EvaluateAllAsync<T>(string expression, object arg = null)
        => await (await Root()).EvaluateAllAsync<T>(expression, arg);

    public async Task<IJSHandle> EvaluateHandleAsync(string expression, object arg = null, LocatorEvaluateHandleOptions options = null)
        => await (await Root()).EvaluateHandleAsync(expression, arg, options);

    public async Task FillAsync(string value, LocatorFillOptions options = null)
        => await (await Root()).FillAsync(value, options);

    public ILocator Filter(LocatorFilterOptions options = null)
        => Root().Result.Filter(options);

    public async Task FocusAsync(LocatorFocusOptions options = null)
        => await (await Root()).FocusAsync(options);

    public IFrameLocator FrameLocator(string selector)
        => Root().Result.FrameLocator(selector);

    public async Task<string> GetAttributeAsync(string name, LocatorGetAttributeOptions options = null)
        => await (await Root()).GetAttributeAsync(name, options);

    public async Task HighlightAsync()
        => await (await Root()).HighlightAsync();

    public async Task HoverAsync(LocatorHoverOptions options = null)
        => await (await Root()).HoverAsync(options);

    public async Task<string> InnerHTMLAsync(LocatorInnerHTMLOptions options = null)
        => await (await Root()).InnerHTMLAsync(options);

    public async Task<string> InnerTextAsync(LocatorInnerTextOptions options = null)
        => await (await Root()).InnerTextAsync(options);

    public async Task<string> InputValueAsync(LocatorInputValueOptions options = null)
        => await (await Root()).InputValueAsync(options);

    public async Task<byte[]> ScreenshotAsync(LocatorScreenshotOptions options = null)
        => await (await Root()).ScreenshotAsync(options);

    public async Task ScrollIntoViewIfNeededAsync(LocatorScrollIntoViewIfNeededOptions options = null)
        => await (await Root()).ScrollIntoViewIfNeededAsync(options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(string values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(IElementHandle values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<string> values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(SelectOptionValue values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<IElementHandle> values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task<IReadOnlyList<string>> SelectOptionAsync(IEnumerable<SelectOptionValue> values, LocatorSelectOptionOptions options = null)
        => await (await Root()).SelectOptionAsync(values, options);

    public async Task SelectTextAsync(LocatorSelectTextOptions options = null)
        => await (await Root()).SelectTextAsync(options);

    public async Task SetCheckedAsync(bool checkedState, LocatorSetCheckedOptions options = null)
        => await (await Root()).SetCheckedAsync(checkedState, options);

    public async Task SetInputFilesAsync(string files, LocatorSetInputFilesOptions options = null)
        => await (await Root()).SetInputFilesAsync(files, options);

    public async Task SetInputFilesAsync(IEnumerable<string> files, LocatorSetInputFilesOptions options = null)
        => await (await Root()).SetInputFilesAsync(files, options);

    public async Task SetInputFilesAsync(FilePayload files, LocatorSetInputFilesOptions options = null)
        => await (await Root()).SetInputFilesAsync(files, options);

    public async Task SetInputFilesAsync(IEnumerable<FilePayload> files, LocatorSetInputFilesOptions options = null)
        => await (await Root()).SetInputFilesAsync(files, options);

    public async Task TapAsync(LocatorTapOptions options = null)
        => await (await Root()).TapAsync(options);

    public async Task<string> TextContentAsync(LocatorTextContentOptions options = null)
        => await (await Root()).TextContentAsync(options);

    [Obsolete]
    public async Task TypeAsync(string text, LocatorTypeOptions options = null)
        => await (await Root()).TypeAsync(text, options);

    public async Task UncheckAsync(LocatorUncheckOptions options = null)
        => await (await Root()).UncheckAsync(options);

    public async Task WaitForAsync(LocatorWaitForOptions options = null)
        => await (await Root()).WaitForAsync(options);

    public async Task<JsonElement?> EvaluateAsync(string expression, object arg = null, LocatorEvaluateOptions options = null)
        => await (await Root()).EvaluateAsync(expression, arg, options);

    public async Task<ILocator> GetByAltTextAsync(string text, LocatorGetByAltTextOptions options = null)
        => (await Root()).GetByAltText(text, options);

    public async Task<ILocator> GetByAltTextAsync(Regex text, LocatorGetByAltTextOptions options = null)
        => (await Root()).GetByAltText(text, options);

    public async Task<ILocator> GetByLabelAsync(string text, LocatorGetByLabelOptions options = null)
        => (await Root()).GetByLabel(text, options);

    public async Task<ILocator> GetByLabelAsync(Regex text, LocatorGetByLabelOptions options = null)
        => (await Root()).GetByLabel(text, options);

    public async Task<ILocator> GetByPlaceholderAsync(string text, LocatorGetByPlaceholderOptions options = null)
        => (await Root()).GetByPlaceholder(text, options);

    public async Task<ILocator> GetByPlaceholderAsync(Regex text, LocatorGetByPlaceholderOptions options = null)
        => (await Root()).GetByPlaceholder(text, options);

    public async Task<ILocator> GetByRoleAsync(AriaRole role, LocatorGetByRoleOptions options = null)
        => (await Root()).GetByRole(role, options);

    public async Task<ILocator> GetByTestIdAsync(string testId)
        => (await Root()).GetByTestId(testId);

    public async Task<ILocator> GetByTestIdAsync(Regex testId)
        => (await Root()).GetByTestId(testId);

    public async Task<ILocator> GetByTextAsync(string text, LocatorGetByTextOptions options = null)
        => (await Root()).GetByText(text, options);

    public async Task<ILocator> GetByTextAsync(Regex text, LocatorGetByTextOptions options = null)
        => (await Root()).GetByText(text, options);

    public async Task<ILocator> GetByTitleAsync(string text, LocatorGetByTitleOptions options = null)
        => (await Root()).GetByTitle(text, options);

    public async Task<ILocator> GetByTitleAsync(Regex text, LocatorGetByTitleOptions options = null)
        => (await Root()).GetByTitle(text, options);

    public async Task<bool> IsCheckedAsync(LocatorIsCheckedOptions options = null)
        => await (await Root()).IsCheckedAsync(options);

    public async Task<bool> IsDisabledAsync(LocatorIsDisabledOptions options = null)
        => await (await Root()).IsDisabledAsync(options);

    public async Task<bool> IsEditableAsync(LocatorIsEditableOptions options = null)
        => await (await Root()).IsEditableAsync(options);

    public async Task<bool> IsEnabledAsync(LocatorIsEnabledOptions options = null)
        => await (await Root()).IsEnabledAsync(options);

    public async Task<bool> IsHiddenAsync(LocatorIsHiddenOptions options = null)
        => await (await Root()).IsHiddenAsync(options);

    public async Task<bool> IsVisibleAsync(LocatorIsVisibleOptions options = null)
        => await (await Root()).IsVisibleAsync(options);

    public async Task<ILocator> LocatorAsync(string selectorOrLocator, LocatorLocatorOptions options = null)
        => (await Root()).Locator(selectorOrLocator, options);

    public async Task<ILocator> LocatorAsync(ILocator selectorOrLocator, LocatorLocatorOptions options = null)
        => (await Root()).Locator(selectorOrLocator, options);

    public async Task<ILocator> NthAsync(int index)
        => (await Root()).Nth(index);

    public async Task<ILocator> OrAsync(ILocator locator)
        => (await Root()).Or(locator);

    public async Task PressAsync(string key, LocatorPressOptions options = null)
        => await (await Root()).PressAsync(key, options);

    public async Task PressSequentiallyAsync(string text, LocatorPressSequentiallyOptions options = null)
        => await (await Root()).PressSequentiallyAsync(text, options);

    public ILocator GetByAltText(string text, LocatorGetByAltTextOptions options = null)
        => Root().Result.GetByAltText(text, options);

    public ILocator GetByAltText(Regex text, LocatorGetByAltTextOptions options = null)
        => Root().Result.GetByAltText(text, options);

    public ILocator GetByLabel(string text, LocatorGetByLabelOptions options = null)
        => Root().Result.GetByLabel(text, options);

    public ILocator GetByLabel(Regex text, LocatorGetByLabelOptions options = null)
        => Root().Result.GetByLabel(text, options);

    public ILocator GetByPlaceholder(string text, LocatorGetByPlaceholderOptions options = null)
        => Root().Result.GetByPlaceholder(text, options);

    public ILocator GetByPlaceholder(Regex text, LocatorGetByPlaceholderOptions options = null)
        => Root().Result.GetByPlaceholder(text, options);

    public ILocator GetByRole(AriaRole role, LocatorGetByRoleOptions options = null)
        => Root().Result.GetByRole(role, options);

    public ILocator GetByTestId(string testId)
        => Root().Result.GetByTestId(testId);

    public ILocator GetByTestId(Regex testId)
        => Root().Result.GetByTestId(testId);

    public ILocator GetByText(string text, LocatorGetByTextOptions options = null)
        => Root().Result.GetByText(text, options);

    public ILocator GetByText(Regex text, LocatorGetByTextOptions options = null)
        => Root().Result.GetByText(text, options);

    public ILocator GetByTitle(string text, LocatorGetByTitleOptions options = null)
        => Root().Result.GetByTitle(text, options);

    public ILocator GetByTitle(Regex text, LocatorGetByTitleOptions options = null)
        => Root().Result.GetByTitle(text, options);

    public ILocator Locator(string selectorOrLocator, LocatorLocatorOptions options = null)
        => Root().Result.Locator(selectorOrLocator, options);

    public ILocator Locator(ILocator selectorOrLocator, LocatorLocatorOptions options = null)
        => Root().Result.Locator(selectorOrLocator);

    public ILocator Nth(int index)
        => Root().Result.Nth(index);

    public ILocator Or(ILocator locator)
        => Root().Result.Or(locator);

    public IFrameLocator ContentFrame
        => Root().Result.ContentFrame;

    public ILocator First
        => Root().Result.First;

    public ILocator Last
        => Root().Result.Last;

    public IPage Page
        => Root().Result.Page;
    #endregion
}