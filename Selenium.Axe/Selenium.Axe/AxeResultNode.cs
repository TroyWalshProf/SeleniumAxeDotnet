using Newtonsoft.Json;
using System.Collections.Generic;

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
