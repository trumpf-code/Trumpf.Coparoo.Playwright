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

namespace Trumpf.Coparoo.Playwright.Demo.TabObjects;

using Trumpf.Coparoo.Playwright.Demo.PageObjects;

/// <summary>
/// Demo CDP tab object for testing CDP connection pooling.
/// This would connect to a real CefSharp CDP endpoint in production.
/// For demo purposes, it simulates CDP behavior by launching a browser.
/// </summary>
/// <remarks>
/// In a real WPF application with CefSharp:
/// <code>
/// protected override string CdpEndpoint => "http://localhost:12345";
/// </code>
/// The CefSharp browser would expose a CDP endpoint on this port.
/// </remarks>
public sealed class CdpDemoTab : CdpTabObject
{
    private readonly string identifier;

    public CdpDemoTab(string identifier = "default")
    {
        this.identifier = identifier;

        // Register page object relationships dynamically
        ChildOf<Shell, CdpDemoTab>();
        ChildOf<Settings, Shell>();
        ChildOf<Preferences, Shell>();
    }

    /// <summary>
    /// In production, this would be the actual CDP endpoint from CefSharp.
    /// For demo, we use a mock endpoint that triggers browser launch via pool.
    /// </summary>
    protected override string CdpEndpoint => "demo://localhost";

    protected override string PageIdentifier => identifier;

    protected override string Url
    {
        get
        {
            var htmlPath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "demo.html");
            return $"file:///{htmlPath.Replace("\\", "/")}";
        }
    }

    /// <summary>
    /// Note: This Creator() is sealed in CdpTabObject and uses SmartPlaywrightConnectionPool.
    /// We cannot override it. The pool will call this indirectly.
    /// </summary>
}
