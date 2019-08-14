using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResultType
    {
        [EnumMember(Value = "violations")]
        Violations,
        [EnumMember(Value = "incomplete")]
        Incomplete,
        [EnumMember(Value = "inapplicable")]
        Inapplicable,
        [EnumMember(Value = "passes")]
        Passes
    }

    /// <summary>
    /// Run configuration data that is passed to axe for scanning the web page.
    /// Refer https://github.com/dequelabs/axe-core/blob/develop/doc/API.md#options-parameter for more information
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

        /// <summary>
        /// Limit which result types are processed and aggregated. An approach you can take to reducing the time is use the resultTypes option. 
        /// For eg, when set to [ResultTypes.Violations], scan results will only have the full details of the violations array and 
        /// will only have one instance of each of the inapplicable, incomplete and pass arrays for each rule that has at least one of those entries. 
        /// This will reduce the amount of computation that axe-core does for the unique selectors.
        /// </summary>
        [JsonProperty("resultTypes")]
        public HashSet<ResultType> ResultTypes { get; set; }

        /// <summary>
        /// Returns xpath selectors for elements
        /// </summary>
        [JsonProperty("xpath")]
        public bool? XPath { get; set; }

        /// <summary>
        /// Use absolute paths when creating element selectors
        /// </summary>
        [JsonProperty("absolutePaths")]
        public bool? AbsolutePaths { get; set; }

        /// <summary>
        /// Tell axe to run inside iframes
        /// </summary>
        [JsonProperty("iframes")]
        public bool? Iframes { get; set; }

        /// <summary>
        /// Scrolls elements back to the state before scan started
        /// </summary>
        [JsonProperty("restoreScroll")]
        public bool? RestoreScroll { get; set; }

        /// <summary>
        /// How long (in milliseconds) axe waits for a response from embedded frames before timing out
        /// </summary>
        [JsonProperty("frameWaitTime")]
        public int? FrameWaitTimeInMilliSec { get; set; }
    }
}
