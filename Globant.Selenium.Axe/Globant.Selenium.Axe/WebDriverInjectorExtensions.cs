using Globant.Selenium.Axe.Properties;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Globant.Selenium.Axe
{
    internal static class WebDriverInjectorExtensions
    {
        /// <summary>
        /// Recursively injects aXe into all iframes and the top level document, using the embeded axe script
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        internal static void Inject(this IWebDriver driver)
        {
            Inject(driver, Resources.axe_min);
        }

        /// <summary>
        /// Recursively injects aXe into all iframes and the top level document.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="resourceUrl">Script resource Url.</param>
        internal static void Inject(this IWebDriver driver, Uri resourceUrl, IContentDownloader contentDownloader)
        {
            string script = contentDownloader.GetContent(resourceUrl);
            Inject(driver, script);
        }

        /// <summary>
        /// Injects Axe script into frames.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="script">Script to inject.</param>
        private static void Inject(IWebDriver driver, string script)
        {

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
        private static void InjectIntoFrames(IWebDriver driver, string script, IList<IWebElement> parents)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IList<IWebElement> frames = driver.FindElements(By.TagName("iframe"));

            foreach (var frame in frames)
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
