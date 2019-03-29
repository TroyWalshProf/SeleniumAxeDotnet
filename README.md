[![Build Status](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_apis/build/status/SeleniumAxeDotnet?branchName=master)](https://dev.azure.com/AxeDotNet/Axe-Selenium-DotNet/_build/latest?definitionId=4&branchName=master)

# axe-selenium-csharp
Tools for using aXe for web accessibility testing with C# and Selenium. 
Inspired on [axe-selenium-java](https://github.com/dequelabs/axe-selenium-java)
Forked from [Globant.Selenium.Axe](https://github.com/javnov/axe-selenium-csharp)

This project born as a need to have a clean .NET wrapper for aXe.

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

## Documentation
*Current build only offically supports Chrome  
Work in progress!!
