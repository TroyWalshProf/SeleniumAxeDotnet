using javnov.Selenium.Axe.Properties;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Net;

namespace javnov.Selenium.Axe
{

    /**
	 * Fluent style builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
	 * exclude(), and options() methods before calling analyze() to run.
	 */
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly string _scriptContent;
        private readonly List<string> _includes = new List<string>();
        private readonly List<string> _excludes = new List<string>();

        #region Properties

        public string Options { get; set; }

        #endregion Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        public AxeBuilder(IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver");

            _webDriver = webDriver;
            _scriptContent = Resources.axe_min;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="axeScriptUrl"></param>
        public AxeBuilder(IWebDriver webDriver, Uri axeScriptUrl) : this(webDriver, axeScriptUrl, new WebClient()) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="axeScriptUrl"></param>
        /// <param name="webClient"></param>
        public AxeBuilder(IWebDriver webDriver, Uri axeScriptUrl, WebClient webClient) : this(webDriver)
        {
            if (axeScriptUrl == null)
                throw new ArgumentNullException("axeScriptUrl");
            if (webClient == null)
                throw new ArgumentNullException("webClient");

            var contentDownloader = new ContentDownloader(webClient);
            _scriptContent = contentDownloader.GetContent(axeScriptUrl);
        }

        private JObject Execute(string command, params object[] args)
        {
            _webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
            object response = ((IJavaScriptExecutor)_webDriver).ExecuteAsyncScript(command, args);
            return new JObject(response);
        }

        public AxeBuilder Include(string selector)
        {
            _includes.Add(selector);
            return this;
        }

        public AxeBuilder Exclude(string selector)
        {
            _excludes.Add(selector);
            return this;
        }

        /// <summary>
        /// Run aXe against a specific WebElement.
        /// </summary>
        /// <param name="context"> A WebElement to test</param>
        /// <returns>An aXe results document</returns>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        public JObject Analyze(IWebElement context)
        {
            string command = string.Format("axe.a11yCheck(arguments[0], {0}, arguments[arguments.length - 1]);", Options);
            return Execute(command, context);
        }

        /// <summary>
        /// Run aXe against the page.
        /// </summary>
        /// <returns>An aXe results document</returns>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        public JObject Analyze()
        {
            string command;

            if (_includes.Count > 1 || _excludes.Count > 0)
            {
                command = string.Format("axe.a11yCheck({include: [{0}], exclude: [{1}]}, {2}, arguments[arguments.length - 1]);",
                        "['" + string.Join("'],['", _includes) + "']",
                        _excludes.Count == 0 ? "" : "['" + string.Join("'],['", _excludes) + "']",
                        Options);
            }

            else if (_includes.Count == 1)
            {
                command = string.Format("axe.a11yCheck('{0}', {1}, arguments[arguments.length - 1]);", _includes[0].Replace("'", ""), Options);
            }
            else
            {
                command = string.Format("axe.a11yCheck(document, {0}, arguments[arguments.length - 1]);", Options);
            }

            return Execute(command);
        }
    }
}