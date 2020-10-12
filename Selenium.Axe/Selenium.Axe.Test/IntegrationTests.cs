using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;

namespace Selenium.Axe.Test
{
    [TestClass]
    [DeploymentItem("integration-test-simple.html")]
    [DeploymentItem("integration-test-target-complex.html")]
    [DeploymentItem("SampleResults.json")]
    [DeploymentItem("chromedriver.exe")]
    [DeploymentItem("geckodriver.exe")]
    [TestCategory("Integration")]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private static readonly string IntegrationTestTargetSimpleFile = @"integration-test-simple.html";
        private static readonly string IntegrationTestTargetComplexTargetsFile = @"integration-test-target-complex.html";
        private static readonly string IntegrationTestJsonResultFile = Path.GetFullPath(@"SampleResults.json");

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
            InitDriver(browser);
            LoadSimpleTestPage();

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
            InitDriver(browser);
            LoadSimpleTestPage();

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
            InitDriver(browser);
            LoadSimpleTestPage();

            var mainElement = _wait.Until(drv => drv.FindElement(By.TagName("main")));
            _webDriver.CreateAxeHtmlReport(path);

            ValidateReport(path, 4, 28, 0, 55);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportOnElement(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();

            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));
            _webDriver.CreateAxeHtmlReport(mainElement, path);

            ValidateReport(path, 3, 16, 0, 61);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportOnElementEventFiring(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();
            
            _webDriver = new EventFiringWebDriver(_webDriver);
            _wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(20));

            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));
            _webDriver.CreateAxeHtmlReport(mainElement, path);

            ValidateReport(path, 3, 16, 0, 61);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportRespectRules(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();
            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));

            var builder = new AxeBuilder(_webDriver).DisableRules("color-contrast");
            _webDriver.CreateAxeHtmlReport(builder.Analyze(), path);

            ValidateReport(path, 3, 23, 0, 55);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportSampleResults(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();
            var mainElement = _wait.Until(drv => drv.FindElement(By.CssSelector("main")));
            JObject jResult = JObject.Parse(File.ReadAllText(IntegrationTestJsonResultFile));
            var results = new AxeResult(jResult);
            _webDriver.CreateAxeHtmlReport(results, path);

            ValidateReport(path, 3, 5, 2, 4);

            string text = File.ReadAllText(path);
            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            var errorMessage = doc.DocumentNode.SelectSingleNode(".//*[@id=\"ErrorMessage\"]").InnerText;
            Assert.AreEqual("AutomationError", errorMessage);

            var reportContext = doc.DocumentNode.SelectSingleNode(".//*[@id=\"reportContext\"]").InnerText;
            Assert.IsTrue(reportContext.Contains($"Url: {results.Url}"));
            Assert.IsTrue(reportContext.Contains($"Orientation: {results.TestEnvironment.OrientationType}"));
            Assert.IsTrue(reportContext.Contains($"Size: {results.TestEnvironment.WindowWidth} x {results.TestEnvironment.WindowHeight}"));
            Assert.IsTrue(reportContext.Contains($"Time: {results.Timestamp}"));
            Assert.IsTrue(reportContext.Contains($"User agent: {results.TestEnvironment.UserAgent}"));
            Assert.IsTrue(reportContext.Contains($"Using: {results.TestEngineName} ({results.TestEngineVersion}"));
        }

        [TestMethod]
        [DataRow("Chrome")]
        public void RunSiteThatReturnsMultipleTargets(string browser)
        {
            var filename = new Uri(Path.GetFullPath(IntegrationTestTargetComplexTargetsFile)).AbsolutePath;
            InitDriver(browser);
            _webDriver.Navigate().GoToUrl(filename);
            new AxeBuilder(_webDriver)
                .WithOutputFile(@".\raw-axe-results.json")
                .Analyze();
        }

        private string CreateReportPath()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(Path.GetDirectoryName(path), Guid.NewGuid() + ".html");
        }

        private void ValidateReport(string path, int violationCount, int passCount, int incompleteCount = 0, int inapplicableCount = 0)
        {
            string text = File.ReadAllText(path);
            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            // Check violations 
            string xpath = ".//*[@id=\"ViolationsSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            HtmlNodeCollection liNodes = doc.DocumentNode.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
            Assert.AreEqual(violationCount, liNodes.Count, $"Expected {violationCount} violations");

            // Check passes
            xpath = ".//*[@id=\"PassesSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            liNodes = doc.DocumentNode.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
            Assert.AreEqual(passCount, liNodes.Count, $"Expected {passCount} passess");

            // Check inapplicables
            xpath = ".//*[@id=\"InapplicableSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"findings\")]";
            liNodes = doc.DocumentNode.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
            Assert.AreEqual(inapplicableCount, liNodes.Count, $"Expected {inapplicableCount} inapplicables");

            // Check incompletes
            xpath = ".//*[@id=\"IncompleteSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            liNodes = doc.DocumentNode.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
            Assert.AreEqual(incompleteCount, liNodes.Count, $"Expected {incompleteCount} incompletes");

            // Check header data
            Assert.IsTrue(text.Contains("Using: axe-core"), "Expected to find 'Using: axe-core'");
            Assert.IsTrue(text.Contains($"Violation: {violationCount}"), $"Expected to find 'Violation: {violationCount}'");
            Assert.IsTrue(text.Contains($"Incomplete: {incompleteCount}"), $"Expected to find 'Incomplete: {incompleteCount}'");
            Assert.IsTrue(text.Contains($"Pass: {passCount}"), $"Expected to find 'Pass: {passCount}'");
            Assert.IsTrue(text.Contains($"Inapplicable: {inapplicableCount}"), $"Expected to find 'Inapplicable: {inapplicableCount}'");
        }

        private void LoadSimpleTestPage()
        {
            var filename = new Uri(Path.GetFullPath(IntegrationTestTargetSimpleFile)).AbsoluteUri;
            _webDriver.Navigate().GoToUrl(filename);

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
