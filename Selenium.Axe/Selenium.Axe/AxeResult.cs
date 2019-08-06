using Newtonsoft.Json.Linq;
using System;

namespace Selenium.Axe
{
    public class AxeResult
    {
        public AxeResultItem[] Violations { get; }
        public AxeResultItem[] Passes { get; }
        public AxeResultItem[] Inapplicable { get; }
        public AxeResultItem[] Incomplete { get; }
        public DateTimeOffset? Timestamp { get; private set; }
        public string Url { get; private set; }

        public string Error { get; private set; }

        public AxeRunOptions ToolOptions { get; private set; }

        public AxeResult(JObject result)
        {
            JToken results = result.SelectToken("results");
            JToken violationsToken = results.SelectToken("violations");
            JToken passesToken = results.SelectToken("passes");
            JToken inapplicableToken = results.SelectToken("inapplicable");
            JToken incompleteToken = results.SelectToken("incomplete");
            JToken timestampToken = results.SelectToken("timestamp");
            JToken urlToken = results.SelectToken("url");
            JToken toolOptions = results.SelectToken("toolOptions");
            JToken error = result.SelectToken("error");

            Violations = violationsToken?.ToObject<AxeResultItem[]>();
            Passes = passesToken?.ToObject<AxeResultItem[]>();
            Inapplicable = inapplicableToken?.ToObject<AxeResultItem[]>();
            Incomplete = incompleteToken?.ToObject<AxeResultItem[]>();
            Timestamp = timestampToken?.ToObject<DateTimeOffset>();
            Url = urlToken?.ToObject<string>();
            Error = error?.ToObject<string>();
            ToolOptions = toolOptions?.ToObject<AxeRunOptions>();
        }
    }
}
