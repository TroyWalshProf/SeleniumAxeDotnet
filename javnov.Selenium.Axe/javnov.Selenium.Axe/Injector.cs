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
    public class Injector
    {
        private ContentDownloader _contentDownloader;
        private readonly IWebDriver _webDriver;

        public Injector(IWebDriver webDriver, WebClient webClient)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver.");

            if (webClient == null)
                throw new ArgumentNullException("webClient.");

            _webDriver = webDriver;
            _contentDownloader = new ContentDownloader(webClient);
        }

        /// <summary>
        /// Recursively injects aXe into all iframes and the top level document.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        public void Inject(IWebDriver driver)
        {
            string script = Resources.axe_min;
            Inject(driver, script);
        }

        /// <summary>
        /// Recursively injects aXe into all iframes and the top level document.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="resourceUrl">Script resource Url.</param>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        public void Inject(IWebDriver driver, Uri resourceUrl)
        {
            string script = _contentDownloader.GetContent(resourceUrl);
            Inject(driver, script);
        }

        /// <summary>
        /// Injects Axe script into frames.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="script">Script to inject.</param>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        private void Inject(IWebDriver driver, string script) {

            IList<IWebElement> parents = new List<IWebElement>();
            InjectIntoFrames(driver, script, parents);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            driver.SwitchTo().DefaultContent();
            js.ExecuteScript(script);
        }

        /// <summary>
        ///  Recursively find frames and inject a script into them.
        /// </summary>
        /// <param name="driver">An initialized WebDriver</param>
        /// <param name="script">Script to inject</param>
        /// <param name="parents">A list of all toplevel frames</param>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        private void InjectIntoFrames(IWebDriver driver, string script, IList<IWebElement> parents)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IList<IWebElement > frames = driver.FindElements(By.TagName("iframe"));

            foreach(var frame in frames)
            {
                driver.SwitchTo().DefaultContent();

                if (parents != null)
                {
                    foreach (IWebElement parent in parents)
                    {
                        driver.SwitchTo().Frame(parent);
                    }
                }

                driver.SwitchTo().Frame(frame);
                js.ExecuteScript(script);

                 IList<IWebElement> localParents = parents.ToList();
                localParents.Add(frame);

                InjectIntoFrames(driver, script, localParents);
            }
        }

    }
}
