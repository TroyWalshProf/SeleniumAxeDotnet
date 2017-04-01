using System;
using System.Net;

namespace Globant.Selenium.Axe
{
    /// <summary>
    /// Get resources content from URLs
    /// </summary>
    internal class ContentDownloader : IContentDownloader
    {
        private readonly WebClient _webClient;

        /// <summary>
        /// Initialize an instace of <see cref="ContentDownloader"/>
        /// </summary>
        /// <param name="webClient">WebClient instace to use</param>
        public ContentDownloader(WebClient webClient)
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));

            _webClient = webClient;
        }

        /// <summary>
        /// Get the resource's content
        /// </summary>
        /// <param name="resourceUrl">Resource url</param>
        /// <returns>Content of the resource</returns>
        public string GetContent(Uri resourceUrl)
        {
            var contentString = _webClient.DownloadString(resourceUrl);
            return contentString;
        }
    }
}
