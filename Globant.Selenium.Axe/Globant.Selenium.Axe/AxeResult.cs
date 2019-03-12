using Newtonsoft.Json.Linq;
using System;

namespace Globant.Selenium.Axe
{
    public class AxeResult
    {
        public AxeResultItem[] Violations { get; }
        public AxeResultItem[] Passes { get; }
        public AxeResultItem[] Inapplicable { get; }
        public AxeResultItem[] Incomplete { get; }
        public DateTimeOffset? Timestamp { get; private set; }
        public string Url { get; private set; }

        public AxeResult(JObject results)
        {
            JToken violationsToken = results.SelectToken("violations");
            JToken passesToken = results.SelectToken("passes");
            JToken inapplicableToken = results.SelectToken("inapplicable");
            JToken incompleteToken = results.SelectToken("incomplete");
            JToken timestampToken = results.SelectToken("timestamp");
            JToken urlToken = results.SelectToken("url");

            Violations = violationsToken?.ToObject<AxeResultItem[]>();
            Passes = passesToken?.ToObject<AxeResultItem[]>();
            Inapplicable = inapplicableToken?.ToObject<AxeResultItem[]>();
            Incomplete = incompleteToken?.ToObject<AxeResultItem[]>();
            Timestamp = timestampToken?.ToObject<DateTimeOffset>();
            Url = urlToken?.ToObject<string>();
        }
    }
}
