# Deprecation Notice

### Why is this project being deprecated?
-	Deque is officially taking on the .Net bindings  
	-	https://github.com/dequelabs/axe-core-nuget  
	-	https://www.nuget.org/packages/Deque.AxeCore.Selenium/  
### Is this a good thing?
- Yes
  - Selenium.Axe actually uses Deque’s Axe core under the hood
  - This should help keep the .Net bindings at feature parity with the other official supported binding, such as Java and JavaScript
### What will be happening?
- Upgrade instructions will be provided early in 2023
- This project will be archived
- The NuGet package will be deprecated and https://www.nuget.org/packages/Deque.AxeCore.Selenium/ will be set as the alternate package
### Anything special we should know?
- The new Deque project will support both Selenium and Playwright 
- A new project (https://github.com/microsoft/html-reporter-for-axe-core-dotnet) will be used for creating HTML reports.  This should provide a much better user experience than the admittedly primitive reporting currently included in Selenium.Axe

# Selenium.Axe for .NET
[![Selenium.Axe NuGet package](https://img.shields.io/nuget/v/Selenium.Axe)](https://www.nuget.org/packages/Selenium.Axe) 
[![NuGet package download counter](https://img.shields.io/nuget/dt/Selenium.Axe)](https://www.nuget.org/packages/Selenium.Axe/) 
[![Main pipeline](https://github.com/TroyWalshProf/SeleniumAxeDotnet/actions/workflows/mainPipeline.yml/badge.svg?branch=main)](https://github.com/TroyWalshProf/SeleniumAxeDotnet/actions/workflows/mainPipeline.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TroyWalshProf_SeleniumAxeDotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=TroyWalshProf_SeleniumAxeDotnet)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=TroyWalshProf_SeleniumAxeDotnet&metric=coverage)](https://sonarcloud.io/dashboard?id=TroyWalshProf_SeleniumAxeDotnet)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=TroyWalshProf_SeleniumAxeDotnet&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=TroyWalshProf_SeleniumAxeDotnet)


Automated web accessibility testing with .NET, C#, and Selenium. Wraps the [axe-core](https://github.com/dequelabs/axe-core) accessibility scanning engine and the [Selenium.WebDriver](https://www.seleniumhq.org/) browser automation framework.

Compatible with .NET Standard 2.0+, .NET Framework 4.7.1+, and .NET Core 2.0+. 

Inspired by [axe-selenium-java](https://github.com/dequelabs/axe-selenium-java) and [axe-webdriverjs](https://github.com/dequelabs/axe-webdriverjs). Forked from [Globant.Selenium.Axe](https://github.com/javnov/axe-selenium-csharp).

## Documentation

* [Getting Started](https://troywalshprof.github.io/SeleniumAxeDotnet/#/?id=getting-started)
* [`AxeBuilder` Reference](https://troywalshprof.github.io/SeleniumAxeDotnet/#/?id=axebuilder-reference)
* [Working with `AxeResult` objects](https://troywalshprof.github.io/SeleniumAxeDotnet/#/?id=working-with-axeresult-objects)
* [Creating a HTML report](https://troywalshprof.github.io/SeleniumAxeDotnet/#/?id=creating-a-html-report)
* [Contributing](https://troywalshprof.github.io/SeleniumAxeDotnet/#/?id=contributing)

Patron
------
This project would not exist in its current form without the generous support of

[![Magenic](docs/magenic-logo.png)](https://magenic.com)
