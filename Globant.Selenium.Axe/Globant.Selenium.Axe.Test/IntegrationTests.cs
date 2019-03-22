using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using FluentAssertions;
using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Configuration;

namespace Globant.Selenium.Axe.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private const string TargetTestUrl = "https://www.google.ca/";

        [TestInitialize]
        public void Initialize()
        {
            ChromeOptions options = new ChromeOptions
            {
                UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
            };
            options.AddArgument("no-sandbox");
            options.AddArgument("--log-level=3");
            options.AddArgument("--silent");
            options.BinaryLocation = ConfigurationManager.AppSettings["ChromeLocation"].ToString();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
            service.SuppressInitialDiagnosticInformation = true;
            _webDriver = new ChromeDriver(Environment.CurrentDirectory, options);
            _wait = new WebDriverWait(_webDriver, TimeSpan.FromMinutes(4));
            _webDriver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TestAnalyzeTarget()
        {
            _webDriver.Navigate().GoToUrl(TargetTestUrl);
            // wait for email input box is found
            _wait.Until(drv => drv.FindElement(By.XPath("//input[@title='Search']")));
            AxeResult results = _webDriver.Analyze();
            results.Should().NotBeNull(nameof(results));
        }

    }
}
