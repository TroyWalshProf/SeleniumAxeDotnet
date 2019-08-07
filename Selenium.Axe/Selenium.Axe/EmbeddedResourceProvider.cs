using System.IO;
using System.Text;

namespace Selenium.Axe
{
    internal static class EmbeddedResourceProvider
    {
        public static string ReadEmbeddedFile(string fileName) {
            var assembly = typeof(EmbeddedResourceProvider).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"Selenium.Axe.Resources.{fileName}");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
    }
}