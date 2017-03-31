using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using javnov.Selenium.Axe;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using FluentAssertions;
using RazorEngine;
using RazorEngine.Templating;

namespace AxeCsharpSeleniumTest
{
    [TestClass]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private const string targetTestUrl = "https://www.facebook.com/";

        [TestInitialize]
        public void Initialize()
        {
            _webDriver = new FirefoxDriver();
            _webDriver.Manage().Window.Maximize();
        }

        [TestCleanup()]
        public virtual void TearDown()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [TestMethod]
        public void TestAnalyzeTarget()
        {
            _webDriver.Navigate().GoToUrl(targetTestUrl);
            AxeBuilder axeBuilder = new AxeBuilder(_webDriver);
            var results = axeBuilder.Analyze();
            results.Should().NotBeNull(nameof(results));
        }

    }
}
