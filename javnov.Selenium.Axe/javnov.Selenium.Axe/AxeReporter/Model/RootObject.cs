using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe.AxeReporter.Model
{
    public class RootObject
    {
        public string help { get; set; }
        public List<Node> nodes { get; set; }
        public string impact { get; set; }
        public string description { get; set; }
        public string helpUrl { get; set; }
        public string id { get; set; }
        public List<string> tags { get; set; }
    }
}
