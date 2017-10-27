# axe-selenium-csharp
Tools for using aXe for web accessibility testing with C# and Selenium. Inspired on [axe-selenium-java](https://github.com/dequelabs/axe-selenium-java)

This project born as a need to have a clean .NET wrapper for aXe.

**Work in progress!! Stay tunned.**

## Getting Started

Install via Nuget: 
```powershell
PM> Install-Package Globant.Selenium.Axe
```

Import this namespace:
```csharp
using Globant.Selenium.Axe;
```

and call the extension method ```Analyze``` from your WebDriver object
```csharp
IWebDriver webDriver = new FirefoxDriver();
AxeResult results = webDriver.Analyze();
```

##Documentation
Work in progress!!

##Thanks
Specially thanks to @jdmesalosada to make this happen and to always improve our jobs.