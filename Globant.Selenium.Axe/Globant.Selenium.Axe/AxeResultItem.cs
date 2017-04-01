using System.Collections.Generic;

namespace Globant.Selenium.Axe
{
    public class AxeResultItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Help { get; set; }
        public string HelpUrl { get; set; }
        public string Impact { get; set; }
        public List<string> Tags { get; set; }
        public List<AxeResultNode> Nodes { get; set; }
    }
}
