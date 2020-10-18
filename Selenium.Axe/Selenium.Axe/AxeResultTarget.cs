using System.Collections.Generic;
using System.Text;

namespace Selenium.Axe
{
    public class AxeResultTarget
    {
        public string Selector { get; set; }
        public List<string> Selectors { get; set; } = new List<string>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Selector != null)
            {
                sb.Append(Selector);
            }

            if (Selectors != null)
            {
                sb.Append(string.Join(",", Selectors));
            }

            return sb.ToString();
        }
    }
}
