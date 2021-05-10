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
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

// Setup parallelization
[assembly: Parallelize(Workers = 5, Scope = ExecutionScope.MethodLevel)]

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
        public TestContext TestContext { get; set; }
        private readonly ConcurrentDictionary<string, IWebDriver> localDriver = new ConcurrentDictionary<string, IWebDriver>();
        private readonly ConcurrentDictionary<string, WebDriverWait> localWaitDriver = new ConcurrentDictionary<string, WebDriverWait>();

        public IWebDriver WebDriver
        {
            get
            {
                return localDriver[TestContext.FullyQualifiedTestClassName];
            }

            set
            {
                localDriver.AddOrUpdate(TestContext.FullyQualifiedTestClassName, value, (oldkey, oldvalue) => value);
            }
        }

        public WebDriverWait Wait
        {
            get
            {
                return localWaitDriver[TestContext.FullyQualifiedTestClassName];
            }

            set
            {
                localWaitDriver.AddOrUpdate(TestContext.FullyQualifiedTestClassName, value, (oldkey, oldvalue) => value);
            }
        }

        private static readonly string IntegrationTestTargetSimpleFile = @"integration-test-simple.html";
        private static readonly string IntegrationTestTargetComplexTargetsFile = @"integration-test-target-complex.html";
        private static readonly string IntegrationTestJsonResultFile = Path.GetFullPath(@"SampleResults.json");

        private const string mainElementSelector = "main";

        [TestCleanup]
        public virtual void TearDown()
        {
            
            WebDriver?.Quit();
            WebDriver?.Dispose();
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void RunScanOnPage(string browser)
        {
            InitDriver(browser);
            LoadSimpleTestPage();

            var timeBeforeScan = DateTime.Now;

            var builder = new AxeBuilder(WebDriver)
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

            var mainElement = Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));

            AxeResult results = WebDriver.Analyze(mainElement);
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

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));

            WebDriver.CreateAxeHtmlReport(path);

            ValidateReport(path, 4, 28, 0, 63);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("FireFox")]
        public void ReportFullPageViolationsOnly(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));

            WebDriver.CreateAxeHtmlReport(path, ReportTypes.Violations);

            ValidateReport(path, 4, 0);
            ValidateResultNotWritten(path, ReportTypes.Passes | ReportTypes.Incomplete | ReportTypes.Inapplicable);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("FireFox")]
        public void ReportFullPagePassesInapplicableViolationsOnly(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));
            WebDriver.CreateAxeHtmlReport(path, ReportTypes.Passes | ReportTypes.Inapplicable | ReportTypes.Violations);

            ValidateReport(path, 4, 28, 0, 63);
            ValidateResultNotWritten(path, ReportTypes.Incomplete);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportOnElement(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();

            var mainElement = Wait.Until(drv => drv.FindElement(By.CssSelector(mainElementSelector)));
            WebDriver.CreateAxeHtmlReport(mainElement, path);

            ValidateReport(path, 3, 16, 0, 69);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportOnElementEventFiring(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();

            WebDriver = new EventFiringWebDriver(WebDriver);
            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(20));

            var mainElement = Wait.Until(drv => drv.FindElement(By.CssSelector(mainElementSelector)));
            WebDriver.CreateAxeHtmlReport(mainElement, path);

            ValidateReport(path, 3, 16, 0, 69);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportRespectRules(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();
            Wait.Until(drv => drv.FindElement(By.CssSelector(mainElementSelector)));

            var builder = new AxeBuilder(WebDriver).DisableRules("color-contrast");
            WebDriver.CreateAxeHtmlReport(builder.Analyze(), path);

            ValidateReport(path, 3, 23, 0, 63);
        }

        [TestMethod]
        [DataRow("Chrome")]
        [DataRow("Firefox")]
        public void ReportSampleResults(string browser)
        {
            string path = CreateReportPath();
            InitDriver(browser);
            LoadSimpleTestPage();
            Wait.Until(drv => drv.FindElement(By.CssSelector(mainElementSelector)));
            JObject jResult = JObject.Parse(File.ReadAllText(IntegrationTestJsonResultFile));
            var results = new AxeResult(jResult);
            WebDriver.CreateAxeHtmlReport(results, path);

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
            WebDriver.Navigate().GoToUrl(filename);
            var axeResult = new AxeBuilder(WebDriver)
                .WithOutputFile(@".\raw-axe-results.json")
                .Analyze();

            var colorContrast = axeResult
                .Violations
                .FirstOrDefault(x => x.Id == "color-contrast");

            Assert.IsNotNull(colorContrast);
            var complexTargetNode = colorContrast
                .Nodes
                .Where(x => x.Target.Any(node => node.Selectors.Any()))
                .Select(x => x.Target.Last())
                .First();
            Assert.IsNotNull(complexTargetNode);
            Assert.IsTrue(complexTargetNode.Selectors.Count == 2);
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
            ValidateElementCount(doc, violationCount, xpath, ResultType.Violations);

            // Check passes
            xpath = ".//*[@id=\"PassesSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            ValidateElementCount(doc, passCount, xpath, ResultType.Passes);

            // Check inapplicables
            xpath = ".//*[@id=\"InapplicableSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"findings\")]";
            ValidateElementCount(doc, inapplicableCount, xpath, ResultType.Inapplicable);

            // Check incompletes
            xpath = ".//*[@id=\"IncompleteSection\"]//*[contains(concat(\" \",normalize-space(@class),\" \"),\"htmlTable\")]";
            ValidateElementCount(doc, incompleteCount, xpath, ResultType.Incomplete);

            // Check header data
            Assert.IsTrue(text.Contains("Using: axe-core"), "Expected to find 'Using: axe-core'");

            if (!violationCount.Equals(0))
            {
                ValidateResultCount(text, violationCount, ResultType.Violations);
            }

            if (!passCount.Equals(0))
            {
                ValidateResultCount(text, passCount, ResultType.Passes);
            }

            if (!inapplicableCount.Equals(0))
            {
                ValidateResultCount(text, inapplicableCount, ResultType.Inapplicable);
            }

            if (!incompleteCount.Equals(0))
            {
                ValidateResultCount(text, incompleteCount, ResultType.Incomplete);
            }
        }

        private void ValidateElementCount(HtmlDocument doc, int count, string xpath, ResultType resultType)
        {
            HtmlNodeCollection liNodes = doc.DocumentNode.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
            Assert.AreEqual(liNodes.Count, count, $"Expected {count} {resultType}");
        }

        private void ValidateResultCount(string text, int count, ResultType resultType)
        {
            Assert.IsTrue(text.Contains($"{resultType}: {count}"), $"Expected to find '{resultType}: {count}'");
        }

        private void ValidateResultNotWritten(string path, ReportTypes ReportType)
        {
            string text = File.ReadAllText(path);
 
            foreach (string resultType in ReportType.ToString().Split(','))
            {
                Assert.IsFalse(text.Contains($"{resultType}: "), $"Expected to not find '{resultType}: '");
            }
        }

        private void LoadSimpleTestPage()
        {
            var filename = new Uri(Path.GetFullPath(IntegrationTestTargetSimpleFile)).AbsoluteUri;
            WebDriver.Navigate().GoToUrl(filename);

            Wait.Until(drv => drv.FindElement(By.TagName(mainElementSelector)));
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
                    WebDriver = new ChromeDriver(chromeDriverDirectory, options);

                    break;

                case "FIREFOX":
                    var geckoDriverDirectory = Environment.GetEnvironmentVariable("GeckoWebDriver") ?? Environment.CurrentDirectory;
                    WebDriver = new FirefoxDriver(geckoDriverDirectory);
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(20));
            WebDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(20);
            WebDriver.Manage().Window.Maximize();
        }
    }
}