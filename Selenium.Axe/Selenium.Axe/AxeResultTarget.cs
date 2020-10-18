using System.Collections.Generic;
using System.Text;

namespace Selenium.Axe
{
    /// <summary>
    /// With AxeResultTarget,  we will have either a <see cref="Selector"/> or <see cref="Selectors"/> for each target.
    /// An example of where <see cref="Selectors"/> is set, is when the issue is found inside of a ShadowRoot.
    /// </summary>
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
