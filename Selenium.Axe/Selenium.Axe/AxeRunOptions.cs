using Newtonsoft.Json;
using System.Collections.Generic;

namespace Selenium.Axe
{
    /// <summary>
    /// Used as part of <see cref="AxeRunOptions" to configure rules/tags to be executed/>
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RunOnlyOptions
    {
        /// <summary>
        /// Specifies the context for runonly option. (can be "rule" or "tag")
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Has rules / tags that needs to be executed. (context is based on <see cref="Type"/>.)
        /// </summary>
        [JsonProperty("values")]
        public List<string> Values { get; set; }

    }

    /// <summary>
    /// Used as part of <see cref="AxeRunOptions" to configure rules/>
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RuleOptions
    {
        /// <summary>
        /// Denotes if the rule has to be enabled for scanning
        /// </summary>
        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }

    /// <summary>
    /// Run configuration data that is passed to axe for scanning the web page
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AxeRunOptions
    {
        /// <summary>
        ///  Limit which rules are executed, based on names or tags
        /// </summary>
        [JsonProperty("runOnly")]
        public RunOnlyOptions RunOnly { get; set; }

        /// <summary>
        /// Allow customizing a rule's properties (including { enable: false })
        /// </summary>
        [JsonProperty("rules")]
        public Dictionary<string, RuleOptions> Rules { get; set; }

    }
}
