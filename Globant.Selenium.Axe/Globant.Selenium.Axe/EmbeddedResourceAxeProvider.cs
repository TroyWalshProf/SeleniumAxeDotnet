using Globant.Selenium.Axe.Properties;

namespace Globant.Selenium.Axe
{
    internal class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() => Resources.axe_min;
    }
}
