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
        public AxeTestEnvironment TestEnvironment { get; private set; }

        public string Url { get; private set; }

        public string Error { get; private set; }

        public string TestEngineName { get; private set; }

        public string TestEngineVersion { get; private set; }

        public AxeResult(JObject result)
        {
            JToken violationsToken = result.SelectToken("violations");
            JToken passesToken = result.SelectToken("passes");
            JToken inapplicableToken = result.SelectToken("inapplicable");
            JToken incompleteToken = result.SelectToken("incomplete");
            JToken timestampToken = result.SelectToken("timestamp");
            JToken urlToken = result.SelectToken("url");
            JToken testEnvironment = result.SelectToken("testEnvironment");
            JToken testEngine = result.SelectToken("testEngine");
            JToken testEngineName = testEngine?.SelectToken("name");
            JToken testEngineVersion = testEngine?.SelectToken("version");
            JToken error = result.SelectToken("error");

            Violations = violationsToken?.ToObject<AxeResultItem[]>();
            Passes = passesToken?.ToObject<AxeResultItem[]>();
            Inapplicable = inapplicableToken?.ToObject<AxeResultItem[]>();
            Incomplete = incompleteToken?.ToObject<AxeResultItem[]>();
            Timestamp = timestampToken?.ToObject<DateTimeOffset>();
            TestEnvironment = testEnvironment?.ToObject<AxeTestEnvironment>();
            Url = urlToken?.ToObject<string>();
            Error = error?.ToObject<string>();
            TestEngineName = testEngineName?.ToObject<string>();
            TestEngineVersion = testEngineVersion?.ToObject<string>();
        }
    }
}
