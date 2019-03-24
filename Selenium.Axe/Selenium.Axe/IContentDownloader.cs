using System;

namespace Selenium.Axe
{
    internal interface IContentDownloader
    {
        /// <summary>
        /// Get the resource's content
        /// </summary>
        /// <param name="resourceUrl">Resource url</param>
        /// <returns>Content of the resource</returns>
        string GetContent(Uri resourceUrl);
    }
}