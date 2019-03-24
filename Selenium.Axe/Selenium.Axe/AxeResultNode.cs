using System.Collections.Generic;


namespace Selenium.Axe
{
    public class AxeResultNode
    {
        public List<string> Target { get; set; }
        public string Html { get; set; }
        public string Impact { get; set; }
        public AxeResultCheck[] Any { get; set; }
        public AxeResultCheck[] All { get; set; }
        public AxeResultCheck[] None { get; set; }
    }
}
