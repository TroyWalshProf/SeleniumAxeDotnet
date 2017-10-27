using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Globant.Selenium.Axe
{
    public class ExternalAxeScriptProvider : IAxeScriptProvider
    {
        private readonly Uri _scriptUri;
        private readonly IContentDownloader _contentDownloader;

        public ExternalAxeScriptProvider(WebClient webClient, Uri scriptUri)
        {
            if (webClient == null)
                throw new ArgumentNullException(nameof(webClient));

            if (scriptUri == null)
                throw new ArgumentNullException(nameof(scriptUri));

            _scriptUri = scriptUri;
            _contentDownloader = new CachedContentDownloader(webClient);
        }

        public string GetScript() => _contentDownloader.GetContent(_scriptUri);
    }
}
