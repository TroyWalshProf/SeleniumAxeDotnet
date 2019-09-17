# Selenium.Axe for .NET

[![Selenium.Axe NuGet package](https://img.shields.io/nuget/v/Selenium.Axe)](https://www.nuget.org/packages/Selenium.Axe) [![NuGet package download counter](https://img.shields.io/nuget/dt/Selenium.Axe)](https://www.nuget.org/packages/Selenium.Axe/)  [![Build Status](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_apis/build/status/SeleniumAxeDotnet?branchName=master)](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_build/latest?definitionId=4&branchName=master) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TroyWalshProf_SeleniumAxeDotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=TroyWalshProf_SeleniumAxeDotnet)

Automated web accessibility testing with .NET, C#, and Selenium. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Selenium.WebDriver](https://www.seleniumhq.org/) browser automation framework.

Compatible with .NET Standard 2.0+, .NET Framework 4.5+, and .NET Core 2.0+.

Inspired by [axe-selenium-java](https://github.com/dequelabs/axe-selenium-java) and [axe-webdriverjs](https://github.com/dequelabs/axe-webdriverjs). Forked from [Globant.Selenium.Axe](https://github.com/javnov/axe-selenium-csharp).

## Table of Contents

* [Getting Started](#Getting_Started)
* [`AxeBuilder` Reference](#AxeBuilder_Reference)
* [Working with `AxeResult` objects](#Working_with_AxeResult_objects)
* [Contributing](#Contributing)

## Getting Started

Install via NuGet:

```powershell
PM> Install-Package Selenium.Axe
# or, use the Visual Studio "Manage NuGet Packages" UI
```

Import this namespace:

```csharp
using Selenium.Axe;
```

To run an axe accessibility scan of a web page with the default configuration, create a new `AxeBuilder` using your Selenium `IWebDriver` object and call `Analyze`:

```csharp
IWebDriver webDriver = new ChromeDriver();
AxeResult axeResult = new AxeBuilder(webDriver).Analyze();
```

To cause a test to pass or fail based on the scan, use the `Violations` property of the `AxeResult`:

```csharp
// We recommend FluentAssertions to get great error messages out of the box
using FluentAssertions;

axeResult.Violations.Should().BeEmpty();
```

To configure different scan options, use the chainable methods of the `AxeBuilder` ([reference docs](#AxeBuilder_Reference)):

```csharp
AxeResult axeResult = new AxeBuilder(webDriver)
    .Exclude(".css-class-of-element-with-known-failures")
    .WithTags("wcag2a")
    .Analyze();
```

For a complete working sample project that uses this library, see [the C# sample in microsoft/axe-pipelines-samples](https://github.com/microsoft/axe-pipelines-samples/tree/master/csharp-selenium-webdriver-sample).

## AxeBuilder Reference

### `AxeBuilder.Analyze()`

```csharp
AxeResult axeResult = new AxeBuilder(webDriver).Analyze();
```

Runs an axe accessibility scan of the entire page using all previously chained options and returns an `AxeResult` representing the scan results.

### `AxeBuilder.Analyze(IWebElement element)`

```csharp
var elementToTest = webDriver.FindElement(By.Id("nav-bar"));
AxeResult axeResult = new AxeBuilder(webDriver)
    .Analyze(elementToTest);
```

Runs an axe accessibility scan scoped using all previously chained options to the given Selenium `IWebElement`. Returns an `AxeResult` representing the scan results.

Not compatible with `AxeBuilder.Include` or `AxeBuilder.Exclude`; the element passed to `Analyze` will take precedence and the `Include`/`Exclude` calls will be ignored.

### `AxeBuilder.Include(params string[] cssSelectorPath)`

```csharp
var results = new AxeBuilder(webDriver)
    .Include(".class-of-element-under-test")
    .Analyze();
```

Scopes future `Analyze()` calls to include *only* the element(s) matching the given CSS selector.

`Include` may be chained multiple times to include multiple selectors in a scan.

`Include` may be combined with `Exclude` to scan a tree of elements but omit some children of that tree. For example:

```csharp
var results = new AxeBuilder(webDriver)
    .Include("#element-under-test")
    .Exclude("#element-under-test div.child-class-with-known-issues")
    .Analyze();
```

`Include` is not compatible with `Analyze(IWebElement)` - the `Analyze` argument will take precedence and `Include` will be ignored.

If you pass multiple CSS selectors to a single invocation of `Include`, this will be interpreted as being a path through different `<iframe>`s on the page under test, **not** as multiple elements:

```csharp
// This is correct
var results = new AxeBuilder(webDriver)
    .Include("#id-of-iframe", "#id-of-child-element-inside-iframe")
    .Analyze();

// This is wrong!
var results = new AxeBuilder(webDriver)
    .Include("#first-element", "#second-element")
    .Analyze();

// If you want to include multiple elements, use multiple .Include() calls instead
var results = new AxeBuilder(webDriver)
    .Include("#first-element")
    .Include("#second-element")
    .Analyze();
```

### `AxeBuilder.Exclude(params string[] cssSelectorPath)`

```csharp
var results = new AxeBuilder(webDriver)
    .Exclude(".class-of-element-with-known-issues")
    .Analyze();
```

Scopes future `Analyze()` calls to exclude the element(s) matching the given CSS selector.

`Exclude` may be chained multiple times to exclude multiple selectors in a scan.

`Exclude` may be combined with `Include` to scan a tree of elements but omit some children of that tree. For example:

```csharp
var results = new AxeBuilder(webDriver)
    .Include("#element-under-test")
    .Exclude("#element-under-test div.child-class-with-known-issues")
    .Analyze();
```

`Exclude` is not compatible with `Analyze(IWebElement)` - the `Analyze` argument will take precedence and `Exclude` will be ignored.

If you pass multiple CSS selectors to a single invocation of `Exclude`, this will be interpreted as being a path through different `<iframe>`s on the page under test, **not** as multiple elements:

```csharp
// This is correct
var results = new AxeBuilder(webDriver)
    .Exclude("#id-of-iframe", "#id-of-child-element-inside-iframe")
    .Analyze();

// This is wrong!
var results = new AxeBuilder(webDriver)
    .Exclude("#first-element", "#second-element")
    .Analyze();

// If you want to exclude multiple elements, use multiple .Exclude() calls instead
var results = new AxeBuilder(webDriver)
    .Exclude("#first-element")
    .Exclude("#second-element")
    .Analyze();
```

### `AxeBuilder.WithRules(params string[] axeRuleIds)`

```csharp
var results = new AxeBuilder(webDriver)
    .WithRules("color-contrast", "duplicate-id")
    .Analyze();
```

Causes future calls to `Analyze` to only run the specified axe rules.

For a list of the available axe rules, see https://dequeuniversity.com/rules/axe/3.3. The "Rule ID" at the top of each individual rule's page is the ID you would want to pass to this method.

`WithRules` is not compatible with `WithTags` or `DisableRules`; whichever one you specify last will take precedence.

`WithRules` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.DisableRules(params string[] axeRuleIds)`

```csharp
var results = new AxeBuilder(webDriver)
    .DisableRules("color-contrast", "duplicate-id")
    .Analyze();
```

Causes future calls to `Analyze` to omit the specified axe rules.

For a list of the available axe rules, see https://dequeuniversity.com/rules/axe/3.3. The "Rule ID" at the top of each individual rule's page is the ID you would want to pass to this method.

`DisableRules` is compatible with `WithTags`; you can use this to run all-but-some of the rules for a given set of tags. For example, to run all WCAG 2.0 A rules except for `color-contrast`:

```csharp
var results = new AxeBuilder(webDriver)
    .WithTags("wcag2a")
    .DisableRules("color-contrast")
    .Analyze();
```

`DisableRules` is not compatible with `WithRules`; whichever one you specify second will take precedence.

`DisableRules` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.WithTags(params string[] axeRuleTags)`

Causes future calls to `Analyze` to only run axe rules that match at least one of the specified tags.

A "tag" is a string that may be associated with a given axe rule. See the [axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for a complete list of available tags.

`WithTags` is compatible with `DisableRules`; you can use this to run all-but-some of the rules for a given set of tags. For example, to run all WCAG 2.0 A rules except for `color-contrast`:

```csharp
var results = new AxeBuilder(webDriver)
    .WithTags("wcag2a")
    .DisableRules("color-contrast")
    .Analyze();
```

`WithTags` is not compatible with `WithRules`; whichever one you specify second will take precedence.

`WithTags` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.WithOptions(AxeRunOptions options)`

*Note: in most cases, the simpler `WithRules`, `WithTags`, and `DisableRules` can be used instead.*

```csharp
var results = new AxeBuilder(webDriver)
    .WithOptions(new AxeRunOptions()
    {
        RunOnly = new RunOnlyOptions
        {
            Type = "rule",
            Values = new List<string> { "duplicate-id", "color-contrast" }
        },
        RestoreScroll = true
    })
    .Analyze();
```

Causes future calls to `Analyze` to use the specified options when calling `axe.run` in axe-core. See [the axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for descriptions of the different properties of `AxeRunOptions`.

`WithOptions` will override any values previously set by `WithRules`, `WithTags`, and `DisableRules`.

`WithOptions` is not compatible with the deprecated raw `Options` property.

### `AxeBuilder.WithOutputFile(string filePath)`

```csharp
var results = new AxeBuilder(webDriver)
    .WithOutputFile(@"./path/to/axe-results.json")
    .Analyze();
```

Causes future calls to `Analyze` to export their results to a JSON file, *in addition* to being returned as an `AxeResult` object as usual.

The output format is exactly the same as axe-core would have produced natively, and is compatible with other tools that read axe result JSON, like [axe-sarif-converter](https://github.com/microsoft/axe-sarif-converter).

### `AxeBuilder.AxeBuilder(webDriver, axeBuilderOptions)`

This constructor overload enables certain advanced options not required by most `Selenium.Axe` users. Currently, its only use is to allow you to use a custom `axe-core` implementation, rather than the one that is packaged with this library.

```csharp
var axeBuilderOptions = new AxeBuilderOptions
{
    ScriptProvider = new FileAxeScriptProvider(".\\axe.min.js")
};
var results = new AxeBuilder(webDriver, axeBuilderOptions).Analyze();
```

### *Deprecated*: `AxeBuilder.Options`

*This property is deprecated; instead, use `WithOptions`, `WithRules`, `WithTags`, and `DisableRules`*

```csharp
var axeBuilder = new AxeBuilder(webDriver);
axeBuilder.Options = "{\"runOnly\": {\"type\": \"tag\", \"values\": [\"wcag2a\"]}, \"restoreScroll\": true}"
var results = axeBuilder.Analyze();
```

Sets a JSON string that will be passed as-is to the axe.run `options` parameter.

See the [axe-core API documentation](https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter) for the format for the JSON string.

`Options` is not compatible with `WithRules`, `WithTags`, `DisableRules`, or `WithOptions`

## Working with AxeResult objects

In most cases, you would run an axe scan from within a test method in a suite of end to end tests, and you would want to use a test assertion to verify that there are no unexpected accessibility violations in a page or component.

We strongly recommend [FluentAssertions](https://fluentassertions.com/), a NuGet package that does a good job of producing actionable error messages based on the `AxeResult` output from a scan. That said, if you prefer a different assertion library, you can still use this library; it does not require any particular test or assertion framework.

If you start with no accessibility issues in your page, you can stay clean by validating that the Violations list is empty:

```csharp
IWebDriver webDriver = new ChromeDriver();
AxeResult results = webDriver.Analyze();

results.Violations.Should().BeEmpty();
```

If you already have some accessibility issues & you want to make sure that you do not introduce any more new issues till you get to a clean state, you can use `Exclude` to remove problematic elements from a broad scan, and a combination of `Include` and `DisableRules` to perform a more scoped scan of the element with known issues:

```csharp
// Suppose #element-with-contrast-issue has known violations of the color-contrast rule.

// You could scan that element with the color-contrast rule disabled...
new AxeBuilder(webDriver)
    .Include("#element-with-contrast-issue")
    .DisableRules("color-contrast")
    .Analyze()
    .Violations.Should().BeEmpty();

// ...and then also scan the rest of the page with all rules enabled.
new AxeBuilder(webDriver)
    .Exclude("#element-with-contrast-issue")
    .Analyze()
    .Violations.Should().BeEmpty();
```

## Contributing

*Please note that this project is released with a [Contributor Code of Conduct](./CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.*

This project builds against a combination of .NET Standard, .NET Core, and .NET Framework targets.

Prerequisites to build:

* Install the [.NET Core SDK](https://dotnet.microsoft.com/download)
* Install [Node.js](https://nodejs.org/en/)
* Install either Visual Studio 2017+ *OR* the [.NET Framework Dev Pack](https://dotnet.microsoft.com/download)

Prerequisites to run integration tests:

* Install [Chrome](https://www.google.com/chrome/) (the version should match the version of the `Selenium.WebDriver.ChromeDriver` PackageReference in [Selenium.Axe.Test.csproj](./Selenium.Axe/Selenium.Axe.Test/Selenium.Axe.Test.csproj))
* Install [Firefox](https://www.mozilla.org/firefox/download) (the version should be compatible with the version of the `Selenium.WebDriver.GeckoDriver` PackageReference in [Selenium.Axe.Test.csproj](./Selenium.Axe/Selenium.Axe.Test/Selenium.Axe.Test.csproj))

To build and run all tests:

```sh
cd ./Selenium.Axe
dotnet restore
dotnet build
dotnet test
```
