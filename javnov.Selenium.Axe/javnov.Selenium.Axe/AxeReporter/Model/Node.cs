using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe.AxeReporter.Model
{
    public class Node
    {
        public List<object> all { get; set; }
        public string impact { get; set; }
        public string html { get; set; }
        public List<object> none { get; set; }
        public List<Any> any { get; set; }
        public List<string> target { get; set; }
    }
}
