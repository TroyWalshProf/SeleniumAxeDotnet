using System.IO;
using System.Text;

namespace Selenium.Axe
{
    internal class EmbeddedResourceAxeProvider : IAxeScriptProvider
    {
        public string GetScript() => GetEmbeddedResourceFileContents("axe.min.js");

        private string GetEmbeddedResourceFileContents(string fileName) {
            var assembly = typeof(EmbeddedResourceAxeProvider).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"Selenium.Axe.Resources.{fileName}");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
    }
}
