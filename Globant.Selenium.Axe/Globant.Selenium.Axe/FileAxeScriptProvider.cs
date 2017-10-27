using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globant.Selenium.Axe
{
    public class FileAxeScriptProvider : IAxeScriptProvider
    {
        private readonly string _filePath;

        internal FileAxeScriptProvider(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exists", nameof(filePath));

            _filePath = filePath;
        }

        public string GetScript()
        {
            if (!File.Exists(_filePath))
                throw new InvalidOperationException($"File '{_filePath}' does not exist");

            return File.ReadAllText(_filePath);
        }
    }
}
