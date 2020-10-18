using System.Collections.Generic;
using Newtonsoft.Json;

namespace Selenium.Axe
{
    public class AxeResultNode
    {
        [JsonProperty("target", ItemConverterType = typeof(AxeResultTargetConverter), NullValueHandling = NullValueHandling.Ignore)]
        public List<AxeResultTarget> Target { get; set; }
        public List<string> XPath { get; set; }
        public string Html { get; set; }
        public string Impact { get; set; }
        public AxeResultCheck[] Any { get; set; }
        public AxeResultCheck[] All { get; set; }
        public AxeResultCheck[] None { get; set; }
    }
}
