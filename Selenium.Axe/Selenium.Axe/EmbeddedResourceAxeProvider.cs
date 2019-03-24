using Selenium.Axe.Properties;

namespace Selenium.Axe
{
    internal class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() => Resources.axe_min;
    }
}
