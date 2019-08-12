[![Build Status](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_apis/build/status/SeleniumAxeDotnet?branchName=master)](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_build/latest?definitionId=4&branchName=master)

# Selenium.Axe

Automated web accessibility testing with .NET, C#, and Selenium. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Selenium.WebDriver](https://www.seleniumhq.org/) browser automation framework.

Inspired by [axe-selenium-java](https://github.com/dequelabs/axe-selenium-java) and [axe-webdriverjs](https://github.com/dequelabs/axe-webdriverjs).
Forked from [Globant.Selenium.Axe](https://github.com/javnov/axe-selenium-csharp).

## Getting Started

Install via Nuget: 
```powershell
PM> Install-Package Selenium.Axe
```

Import this namespace:
```csharp
using Selenium.Axe;
```

To run axe scan with default configuration, call the extension method ```Analyze``` from your WebDriver object
```csharp
IWebDriver webDriver = new ChromeDriver();

AxeResult results = webDriver.Analyze();

```

To configure scanning, use the AxeBuilder class chainable apis.
-   AxeBuilder.Include - To scope the scanning to dom elements identified by the given selectors. Note that the selectors array uniquely identifies one element in the page. This cannot be used with AxeBuilder.Analyze(element) api.
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .Include("#parent-iframe1", "#element-inside-iframe") // to select #element-inside-iframe under #parent-iframe1
                    .Include("#element-inside-main-frame1") // to select #element-inside-main-frame1 under the main frame 
                    .Analyze();
    ``` 
-   AxeBuilder.Exclude - To exclude dom elements identified by the given selectors from scanning. Note that the selectors array uniquely identifies one element in the page. This cannot be used with AxeBuilder.Analyze(element) api.
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .Exclude("#parent-iframe1", "#element-inside-iframe") // to exclude #element-inside-iframe under #parent-iframe1
                    .Exclude("#element-inside-main-frame1") // to exclude #element-inside-main-frame1 under the main frame 
                    .Analyze();
    ``` 
- AxeBuilder.Analyze(element) - To run scan on the specified dom element. This cannot be used with Include/Exclude apis.
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .Analyze(webDriver.FindElement(By.Id("nav-bar"))); // Runs scan on the dom element that has id nav-bar.
    ``` 

-   AxeBuilder.WithTags - To limit analysis to rules that have the mentioned tags. Refer https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter to get the complete list of available tags. This api cannot be used with  WithRules / Options api
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .WithTags("wcag2aa", "best-practice")
                    .Analyze();
    ``` 
-   AxeBuilder.WithRules - To limit analysis to specified rules. Refer https://www.deque.com/axe/axe-for-web/documentation/api-documentation/#api-name-axegetrules to get the complete list of available rule IDs. This api cannot be used with  WithTags / Options apis.
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .WithRules("color-contrast", "duplicate-id")
                    .Analyze();
    ``` 
-   AxeBuilder.DisableRules - To exclude rules from scanning. Refer https://www.deque.com/axe/axe-for-web/documentation/api-documentation/#api-name-axegetrules to get the complete list of available rule IDs. This api cannot be used with Options api.
    ```csharp
    var results = new AxeBuilder(webDriver)
                    .DisableRules("color-contrast")
                    .Analyze();
    ``` 
-   AxeBuilder.Options - Run options that is to be passed to axe scan api. This should be a json string of format - https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter. This cannot be used with WithRules /WithTags /DisableRules apis.
    ```csharp
    var axeBuilder = new AxeBuilder(webDriver);
    axeBuilder.Options = "{\"runOnly\": {\"type\": \"tag\", \"values\": [\"wcag2a\"]}, \"restoreScroll\": true}"
    var results = axeBuilder.Analyze();
    ``` 
- AxeBuilder.AxeBuilder(webDriver, axeBuilderOptions) - This api allows you to run scanning on axe version that is not packaged with this library.
    ```csharp
    var axeBuilderOptions = new AxeBuilderOptions
    {
        ScriptProvider = new FileAxeScriptProvider(".\\axe.min.js")
    };
    var results = new AxeBuilder(webDriver, axeBuilderOptions)
                        .Analyze();
    ``` 

Validating scan results:
-   If you start with no accessibility issues in your page, you can stay clean by validating that the Violations list is empty.
```csharp
IWebDriver webDriver = new ChromeDriver();

AxeResult results = webDriver.Analyze();

results.Violations.Should().BeEmpty();
```
- If you already have some accessibility issues & you want to make sure that you do not introduce any more new issues till you get to a clean state, you can do snapshot testing / validate by issues count.
```csharp
IWebDriver webDriver = new ChromeDriver();

AxeResult results = webDriver.Analyze();

results.Violations.Should().HaveCount(2);
```


## Contributing

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
