using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Net;

namespace Globant.Selenium.Axe
{
    /// <summary>
    /// Fluent style builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
    /// exclude(), and options() methods before calling analyze() to run.
    /// </summary>
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly IncludeExcludeManager _includeExcludeManager = new IncludeExcludeManager();

        private static readonly AxeBuilderOptions DefaultOptions = new AxeBuilderOptions {ScriptProvider = new EmbeddedResourceAxeProvider()};

    	 public string Options { get; set; } = "null";

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        public AxeBuilder(IWebDriver webDriver): this(webDriver, DefaultOptions)
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        /// <param name="options">Builder options</param>
        public AxeBuilder(IWebDriver webDriver, AxeBuilderOptions options)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _webDriver = webDriver;
            _webDriver.Inject(options.ScriptProvider);
        }

        /// <summary>
        /// Execute the script into the target.
        /// </summary>
        /// <param name="command">Script to execute.</param>
        /// <param name="args"></param>
        private AxeResult Execute(string command, params object[] args)
        {
            object response = ((IJavaScriptExecutor)_webDriver).ExecuteAsyncScript(command, args);
            var jObject = JObject.FromObject(response);

            if (jObject.SelectToken("violations") == null)
                throw new InvalidOperationException("The response from browser is not valid.");

            if (jObject.SelectToken("passes") == null)
                throw new InvalidOperationException("The response from browser is not valid.");

            JToken violationsToken = jObject.SelectToken("violations");
            JToken passesToken = jObject.SelectToken("passes");

            return new AxeResult(violationsToken, passesToken);   
        }

        /// <summary>
        /// Selectors to include in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Include(params string[] selectors)
        {
            _includeExcludeManager.Include(selectors);
            return this;
        }

        /// <summary>
        /// Exclude selectors
        /// Selectors to exclude in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Exclude(params string[] selectors)
        {
            _includeExcludeManager.Exclude(selectors);
            return this;
        }

        /// <summary>
        /// Run aXe against a specific WebElement.
        /// </summary>
        /// <param name="context"> A WebElement to test</param>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze(IWebElement context)
        {
            string command = string.Format("axe.a11yCheck(arguments[0], {0}, arguments[arguments.length - 1]);", Options);
            return Execute(command, context);
        }

        /// <summary>
        /// Run aXe against the page.
        /// </summary>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze()
        {
            string command;

            if (_includeExcludeManager.HasMoreThanOneSelectorsToIncludeOrSomeToExclude())
            {
                command = $"axe.a11yCheck({_includeExcludeManager.ToJson()}, {Options}, arguments[arguments.length - 1]);";
            }
            else if (_includeExcludeManager.HasOneItemToInclude())
            {
                string itemToInclude = _includeExcludeManager.GetFirstItemToInclude().Replace("'", "");
                command = $"axe.a11yCheck('{itemToInclude}', {Options}, arguments[arguments.length - 1]);";
            }
            else
            {
                command = $"axe.a11yCheck(document, {Options}, arguments[arguments.length - 1]);";
            }

            return Execute(command);
        }
    }
}