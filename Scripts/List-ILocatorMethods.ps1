# Copyright 2016 - 2025 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

<#
.SYNOPSIS
    Lists all methods from the ILocator interface in Microsoft.Playwright.

.DESCRIPTION
    This script loads the Microsoft.Playwright assembly and extracts all methods
    from the ILocator interface. This is useful for tracking changes in Playwright
    versions and ensuring the IControlObjectActionExtensions class stays up to date.

.PARAMETER PlaywrightDllPath
    Optional path to the Microsoft.Playwright.dll. If not provided, the script will
    search for it in the bin directory of the main project.

.PARAMETER OutputFormat
    Format for the output: 'Summary' (default), 'Detailed', or 'Markdown'.
    - Summary: Lists method names and return types
    - Detailed: Includes full signatures with parameters
    - Markdown: Formatted as a markdown table

.EXAMPLE
    .\List-ILocatorMethods.ps1
    Lists all ILocator methods in summary format.

.EXAMPLE
    .\List-ILocatorMethods.ps1 -OutputFormat Detailed
    Lists all ILocator methods with full parameter details.

.EXAMPLE
    .\List-ILocatorMethods.ps1 -OutputFormat Markdown > ILocatorMethods.md
    Generates a markdown table and saves it to a file.
#>

param(
    [string]$PlaywrightDllPath,
    [ValidateSet('Summary', 'Detailed', 'Markdown')]
    [string]$OutputFormat = 'Summary'
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Function to find the Playwright DLL
function Find-PlaywrightDll {
    $scriptDir = Split-Path -Parent $PSScriptRoot
    $searchPaths = @(
        "$scriptDir\Trumpf.Coparoo.Playwright\bin\Debug\netstandard2.0",
        "$scriptDir\Trumpf.Coparoo.Playwright\bin\Release\netstandard2.0",
        "$scriptDir\Trumpf.Coparoo.Playwright.Tests\bin\Debug\net8.0",
        "$scriptDir\Trumpf.Coparoo.Playwright.Tests\bin\Release\net8.0"
    )

    foreach ($path in $searchPaths) {
        $dllPath = Join-Path $path "Microsoft.Playwright.dll"
        if (Test-Path $dllPath) {
            return $dllPath
        }
    }

    throw "Could not find Microsoft.Playwright.dll. Please build the solution first or specify the path using -PlaywrightDllPath parameter."
}

# Get the DLL path
if ([string]::IsNullOrEmpty($PlaywrightDllPath)) {
    $PlaywrightDllPath = Find-PlaywrightDll
    Write-Host "Found Playwright DLL: $PlaywrightDllPath" -ForegroundColor Green
} elseif (-not (Test-Path $PlaywrightDllPath)) {
    throw "Specified Playwright DLL not found: $PlaywrightDllPath"
}

# Load the assembly
Write-Host "Loading assembly..." -ForegroundColor Cyan
$assembly = [System.Reflection.Assembly]::LoadFrom($PlaywrightDllPath)

# Get the ILocator interface
$iLocatorType = $assembly.GetType("Microsoft.Playwright.ILocator")
if ($null -eq $iLocatorType) {
    throw "Could not find ILocator interface in the assembly."
}

# Get all methods
$methods = $iLocatorType.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance)

# Filter out inherited methods from Object and group by name
$locatorMethods = $methods | Where-Object {
    $_.DeclaringType.Name -eq 'ILocator'
} | Sort-Object Name

# Get unique method names (excluding overloads for count)
$uniqueMethodNames = $locatorMethods | Select-Object -ExpandProperty Name -Unique

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Summary: ILocator Interface Methods" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Total methods (including overloads): $($locatorMethods.Count)" -ForegroundColor White
Write-Host "Unique method names: $($uniqueMethodNames.Count)" -ForegroundColor White
Write-Host ""

# Output based on format
switch ($OutputFormat) {
    'Summary' {
        Write-Host "Method List:" -ForegroundColor Cyan
        Write-Host "----------------------------------------" -ForegroundColor Gray

        $grouped = $locatorMethods | Group-Object Name | Sort-Object Name
        foreach ($group in $grouped) {
            $overloadCount = $group.Count
            $firstMethod = $group.Group[0]
            $returnType = $firstMethod.ReturnType.Name

            if ($overloadCount -eq 1) {
                Write-Host "  $($group.Name)() -> $returnType" -ForegroundColor White
            } else {
                Write-Host "  $($group.Name)() -> $returnType [$overloadCount overloads]" -ForegroundColor White
            }
        }
    }

    'Detailed' {
        Write-Host "Detailed Method Signatures:" -ForegroundColor Cyan
        Write-Host "----------------------------------------" -ForegroundColor Gray

        foreach ($method in $locatorMethods) {
            $params = $method.GetParameters() | ForEach-Object {
                $paramType = if ($_.ParameterType.IsGenericType) {
                    $_.ParameterType.Name -replace '`.*$', "<...>"
                } else {
                    $_.ParameterType.Name
                }
                "$paramType $($_.Name)"
            }
            $paramString = if ($params) { $params -join ', ' } else { '' }

            Write-Host ""
            Write-Host "  $($method.ReturnType.Name) $($method.Name)($paramString)" -ForegroundColor White
        }
    }

    'Markdown' {
        Write-Host "| Method Name | Return Type | Overloads | Description |"
        Write-Host "|-------------|-------------|-----------|-------------|"

        $grouped = $locatorMethods | Group-Object Name | Sort-Object Name
        foreach ($group in $grouped) {
            $overloadCount = $group.Count
            $firstMethod = $group.Group[0]
            $returnType = $firstMethod.ReturnType.Name

            # Get parameter info for description
            $params = $firstMethod.GetParameters()
            $paramInfo = if ($params.Count -gt 0) {
                "($($params.Count) param" + $(if ($params.Count -gt 1) { "s" } else { "" }) + ")"
            } else {
                "(no params)"
            }

            Write-Host "| ``$($group.Name)`` | ``$returnType`` | $overloadCount | $paramInfo |"
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Action Methods (for IControlObjectActionExtensions):" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

# List of action methods that should be in IControlObjectActionExtensions
$actionMethods = @(
    'ClickAsync', 'DblClickAsync', 'TapAsync', 'FillAsync', 'TypeAsync',
    'PressAsync', 'PressSequentiallyAsync', 'CheckAsync', 'UncheckAsync',
    'SetCheckedAsync', 'SelectOptionAsync', 'SetInputFilesAsync',
    'HoverAsync', 'FocusAsync', 'DragToAsync', 'ScrollIntoViewIfNeededAsync',
    'DispatchEventAsync', 'HighlightAsync', 'SelectTextAsync'
)

$foundActions = $uniqueMethodNames | Where-Object { $actionMethods -contains $_ }
$missingActions = $actionMethods | Where-Object { $uniqueMethodNames -notcontains $_ }

Write-Host "Found action methods: $($foundActions.Count)/$($actionMethods.Count)" -ForegroundColor Green
foreach ($action in $foundActions) {
    $overloadCount = ($locatorMethods | Where-Object Name -eq $action).Count
    Write-Host "  ✓ $action" -ForegroundColor Green -NoNewline
    if ($overloadCount -gt 1) {
        Write-Host " ($overloadCount overloads)" -ForegroundColor Gray
    } else {
        Write-Host ""
    }
}

if ($missingActions.Count -gt 0) {
    Write-Host ""
    Write-Host "Missing action methods: $($missingActions.Count)" -ForegroundColor Red
    foreach ($action in $missingActions) {
        Write-Host "  ✗ $action" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Script completed successfully!" -ForegroundColor Green
