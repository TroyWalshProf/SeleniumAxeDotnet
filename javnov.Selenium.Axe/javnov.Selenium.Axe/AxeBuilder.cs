using javnov.Selenium.Axe.Properties;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly string _scriptContent;

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


    }
}
