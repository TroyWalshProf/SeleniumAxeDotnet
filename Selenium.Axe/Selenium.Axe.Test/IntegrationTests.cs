using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;

namespace Selenium.Axe.Test
{
    [TestClass]
    [DeploymentItem("integration-test-target.html")]
    [DeploymentItem("chromedriver.exe")]
    [DeploymentItem("geckodriver.exe")]
    [TestCategory("Integration")]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private static readonly string IntegrationTestTargetFile = Path.GetFullPath(@"integration-test-target.html");
        private static readonly string IntegrationTestTargetUrl = new Uri(IntegrationTestTargetFile).AbsoluteUri;

        private const string mainElementSelector = "main";

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver?.Quit();
            _webDriver?.Dispose();
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void RunScanOnPage(string browser)
        {
            this.InitDriver(browser);
            LoadTestPage();

            var timeBeforeScan = DateTime.Now;

            var builder = new AxeBuilder(_webDriver)
                .WithOptions(new AxeRunOptions() { XPath = true })
                .WithTags("wcag2a", "wcag2aa")
                .DisableRules("color-contrast")
                .WithOutputFile(@"./raw-axe-results.json");

            var results = builder.Analyze();
            results.Violations.FirstOrDefault(v => v.Id == "color-contrast").Should().BeNull();
            results.Violations.FirstOrDefault(v => !v.Tags.Contains("wcag2a") && !v.Tags.Contains("wcag2aa")).Should().BeNull();
            results.Violations.Should().HaveCount(2);
            results.Violations.First().Nodes.First().XPath.Should().NotBeNullOrEmpty();

            File.GetLastWriteTime(@"./raw-axe-results.json").Should().BeOnOrAfter(timeBeforeScan);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void RunScanOnGivenElement(string browser)
        {
            this.InitDriver(browser);
            LoadTestPage();

            var mainElement = _wait.Until(drv => drv.FindElement(By.TagName("main")));

            AxeResult results = _webDriver.Analyze(mainElement);
            results.Violations.Should().HaveCount(3);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportFullPage(string browser)
        {
            string path = CreateReportPath();
            this.InitDriver(browser);
            LoadTestPage();

            var mainElement = _wait.Until(drv => drv.FindElement(By.TagName("main")));
            _webDriver.CreateReport(path);

            ValidateReport(path, 4, 23);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportOffElement(string browser)
        {
            string path = CreateReportPath();
            this.InitDriver(browser);
            LoadTestPage();

            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));
            _webDriver.CreateReport(mainElement, path);

            ValidateReport(path, 3, 15);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportRespectRules(string browser)
        {
            string path = CreateReportPath();
            this.InitDriver(browser);
            LoadTestPage();
            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));

            var builder = new AxeBuilder(_webDriver).DisableRules("color-contrast");
            _webDriver.CreateReport(builder.Analyze(), path);

            ValidateReport(path, 3, 18);
        }

        private string CreateReportPath()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(Path.GetDirectoryName(path), Guid.NewGuid() + ".html");
        }

        private void ValidateReport(string path, int violationCount, int passCount)
        {
            string text = File.ReadAllText(path);
            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            // Check violations 
            string xpath = ".//*[@id=\"ViolationsSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            HtmlNodeCollection liNodes = doc.DocumentNode.SelectNodes(xpath);
            Assert.AreEqual(violationCount, liNodes.Count, $"Expected {violationCount} violations");

            // Check passes
            xpath = ".//*[@id=\"PassesSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            liNodes = doc.DocumentNode.SelectNodes(xpath);
            Assert.AreEqual(passCount, liNodes.Count, $"Expected {passCount} passess");

            // Check header data
            Assert.IsTrue(text.Contains("Using: axe-core"), "Expected to find 'Using: axe-core'");
            Assert.IsTrue(text.Contains($"Violation: {violationCount}"), $"Expected to find 'Violation: {violationCount}'");
            Assert.IsTrue(text.Contains("Incomplete: 0"), "Expected to find 'Incomplete: 0'");
            Assert.IsTrue(text.Contains($"Pass: {passCount}"), $"Expected to find 'Pass: {passCount}'");
            Assert.IsTrue(text.Contains("Inapplicable: 0"), "Expected to find 'Inapplicable: 0'");
        }

        private void LoadTestPage()
        {
            _webDriver.Navigate().GoToUrl(IntegrationTestTargetUrl);

            _wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));
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
