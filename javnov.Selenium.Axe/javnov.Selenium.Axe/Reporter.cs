using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    public static class Reporter
    {

        /// <summary>
        /// Writes a readable report based on the found violations.
        /// </summary>
        /// <param name="violations">JSONArray of violations</param>
        /// <returns>readable report of accessibility violations found</returns>
        public static string Report(JArray violations)
        {
            throw new NotImplementedException("Implemented me Perverse lord");
        }

        private static void AppendFixes(StringBuilder sb, JArray arr, string heading)
        {
            throw new NotImplementedException("Implemented me Perverse lord");
        }

        /// <summary>
        ///Writes a raw object out to a JSON file with the specified name. 
        /// </summary>
        /// <param name="name">Desired filename, sans extension</param>
        /// <param name="output">Object to write. Most useful if you pass in either the Builder.analyze() response or the violations array it contains.</param>
        /// @author <a href="mailto:jdmesalosada@gmail.com">Julian Mesa</a>
        public static void WriteResults(string name, object output)
        {
            using (StreamWriter writer = new StreamWriter(name + ".json")) { 
                writer.WriteLine(output.ToString());
            }
        }
    }
}
