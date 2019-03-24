using System;
using System.Collections.Concurrent;
using System.Net;

namespace Selenium.Axe
{
    /// <summary>
    /// Cache downloaded extenal resources
    /// </summary>
    internal class CachedContentDownloader : IContentDownloader
    {
        private readonly IContentDownloader _contentDownloader;
        private static readonly ConcurrentDictionary<string, string> _resourcesCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initialize an instace of <see cref="CachedContentDownloader"/>
        /// </summary>
        /// <param name="webClient">WebClient instace to use</param>
        public CachedContentDownloader(WebClient webClient)
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));

            _contentDownloader = new CachedContentDownloader(webClient);
        }

        /// <summary>
        /// Get the content from the cache if exists, otherwise get ir from the resource url
        /// </summary>
        /// <param name="resourceUrl">Resource url</param>
        /// <returns>Content of the resource</returns>
        public string GetContent(Uri resourceUrl)
        {
            if (resourceUrl == null)
                throw new ArgumentNullException(nameof(resourceUrl));

            string content;
            string key = resourceUrl.ToString();
            if (_resourcesCache.TryGetValue(key, out content))
                return content;

            content = _contentDownloader.GetContent(resourceUrl);

            if (string.IsNullOrEmpty(content))
                return content;

            if (_resourcesCache.TryAdd(key, content))
                return content;

            return null;
        }
    }
}
