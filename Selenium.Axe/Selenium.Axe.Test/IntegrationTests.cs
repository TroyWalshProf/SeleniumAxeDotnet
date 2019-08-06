using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;

namespace Selenium.Axe.Test
{
    [TestClass]
    [DeploymentItem("integration-test-target.html")]
    [DeploymentItem("chromedriver.exe")]
    [DeploymentItem("geckodriver.exe")]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private static readonly string IntegrationTestTargetFile = Path.GetFullPath(@"integration-test-target.html");
        private static readonly string IntegrationTestTargetUrl = new Uri(IntegrationTestTargetFile).AbsoluteUri;

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver?.Quit();
            _webDriver?.Dispose();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void TestAnalyzeTarget(string browser)
        {
            this.InitDriver(browser);
            _webDriver.Navigate().GoToUrl(IntegrationTestTargetUrl);
            var mainElement = _wait.Until(drv => drv.FindElement(By.TagName("main")));
            AxeResult results = _webDriver.Analyze(mainElement);
            results.Violations.Should().HaveCount(3);
        }

        private void InitDriver(string browser)
        {
            switch (browser.ToUpper())
            {
                case "CHROME":
                    var chromeDriverDirectory = Environment.GetEnvironmentVariable("ChromeWebDriver") ?? Environment.CurrentDirectory;
                    ChromeOptions options = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    options.AddArgument("no-sandbox");
                    options.AddArgument("--log-level=3");
                    options.AddArgument("--silent");

                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromeDriverDirectory);
                    service.SuppressInitialDiagnosticInformation = true;
                    _webDriver = new ChromeDriver(chromeDriverDirectory, options);
                    
                    break;

                case "FIREFOX":
                    var geckoDriverDirectory = Environment.GetEnvironmentVariable("GeckoWebDriver") ?? Environment.CurrentDirectory;
                    _webDriver = new FirefoxDriver(geckoDriverDirectory);
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            _wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(20));
            _webDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(20);
            _webDriver.Manage().Window.Maximize();
        }
    }
}
