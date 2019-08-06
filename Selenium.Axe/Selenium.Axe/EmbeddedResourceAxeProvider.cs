using System.IO;
using System.Reflection;
using System.Text;

namespace Selenium.Axe
{
    internal class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() {
            var assembly = Assembly.GetAssembly(typeof(EmbeddedResourceAxeProvider));
            var resourceStream = assembly.GetManifestResourceStream("Selenium.Axe.Resources.axe.min.js");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
    }
}
