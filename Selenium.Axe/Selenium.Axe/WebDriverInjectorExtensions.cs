using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selenium.Axe
{
    internal static class WebDriverInjectorExtensions
    {
        /// <summary>
        /// Injects Axe script into frames.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="scriptProvider">Provider that get the aXe script to inject.</param>
        /// <param name="runOptions">Axe run options</param>
        internal static void Inject(this IWebDriver driver, IAxeScriptProvider scriptProvider, AxeRunOptions runOptions)
        {
            if (scriptProvider == null)
                throw new ArgumentNullException(nameof(scriptProvider));

            string script = scriptProvider.GetScript();
            IList<IWebElement> parents = new List<IWebElement>();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Skip if value is set to false
            if (runOptions.Iframes != false)
            {
                InjectIntoFrames(driver, script, parents);
            }

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
