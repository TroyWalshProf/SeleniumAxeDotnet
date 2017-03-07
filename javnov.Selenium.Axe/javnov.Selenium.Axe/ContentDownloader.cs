using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    internal sealed class ContentDownloader
    {
        private readonly WebClient _webClient;

        public ContentDownloader(WebClient webClient)
        {
            if (webClient == null)
                throw new ArgumentNullException("webClient");

            _webClient = webClient;
        }

        public string GetContent(Uri resourceUrl)
        {
            var contentString = _webClient.DownloadString(resourceUrl);
            return contentString;
        }
    }
}
