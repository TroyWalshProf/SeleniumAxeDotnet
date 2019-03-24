using System.Collections.Generic;

namespace Selenium.Axe
{
    public class AxeResultItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Help { get; set; }
        public string HelpUrl { get; set; }
        public string Impact { get; set; }
        public string[] Tags { get; set; }
        public AxeResultNode[] Nodes { get; set; }
    }
}
