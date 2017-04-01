using OpenQA.Selenium;
using System;

namespace Globant.Selenium.Axe
{
    public static class WebDriverExtensions
    {
        public static AxeResult Analyze(this IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            return axeBuilder.Analyze();
        }

        public static AxeResult Analyze(this IWebDriver webDriver, IWebElement context)
        {
            if (webDriver == null)
                throw new ArgumentNullException(nameof(webDriver));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            AxeBuilder axeBuilder = new AxeBuilder(webDriver);
            return axeBuilder.Analyze(context);
        }
    }
}
