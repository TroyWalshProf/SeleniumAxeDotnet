using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using FluentAssertions;

namespace Globant.Selenium.Axe.Test
{
    [TestClass]
    [Ignore]
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
