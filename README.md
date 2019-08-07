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

and call the extension method ```Analyze``` from your WebDriver object
```csharp
IWebDriver webDriver = new ChromeDriver();
AxeResult results = webDriver.Analyze();
```

## Contributing

This project builds against a combination of .NET Standard, .NET Core, and .NET Framework targets.

Prerequisites to build:

* Install the [.NET Core SDK](https://dotnet.microsoft.com/download)
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
