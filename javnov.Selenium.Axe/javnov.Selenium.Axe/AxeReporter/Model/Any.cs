using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe.AxeReporter.Model
{
    public class Any
    {
        public List<string> data { get; set; }
        public string impact { get; set; }
        public List<object> relatedNodes { get; set; }
        public string id { get; set; }
        public string message { get; set; }
    }
}
