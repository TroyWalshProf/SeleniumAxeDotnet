using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;

namespace Globant.Selenium.Axe.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private const string TargetTestUrl = "https://www.facebook.com/";

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void TestAnalyzeTarget(string browser)
        {
            this.InitDriver(browser);
            _webDriver.Navigate().GoToUrl(TargetTestUrl);
            AxeResult results = _webDriver.Analyze();
            results.Should().NotBeNull(nameof(results));
        }

        private void InitDriver(string browser)
        {
            switch (browser.ToUpper())
            {
                case "CHROME":
                    _webDriver = new ChromeDriver();
                    break;

                case "FIREFOX":
                    _webDriver = new FirefoxDriver();
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            _webDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(3);
            _webDriver.Manage().Window.Maximize();
        }
    }
}
