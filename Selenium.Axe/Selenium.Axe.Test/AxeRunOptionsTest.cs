using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using FluentAssertions;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeRunOptionsTest
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
        };

        [TestMethod]
        public void ShouldSerializeRunOnlyOption()
        {
            var options = new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = new List<string> { "tag1", "tag2" }
                }
            };

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);
            var expectedObject = "{\"runOnly\":{\"type\":\"tag\",\"values\":[\"tag1\",\"tag2\"]}}";

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }

        [TestMethod]
        public void ShouldSerializeRuleOptions()
        {
            var options = new AxeRunOptions()
            {
                Rules = new Dictionary<string, RuleOptions>
                {
                    {"enabledRule", new RuleOptions(){ Enabled = true} },
                    {"disabledRule", new RuleOptions(){ Enabled = false} },
                    {"rule3WithoutOptionsData", new RuleOptions(){ } },
                }
            };
            var expectedObject = "{\"rules\":{\"enabledRule\":{\"enabled\":true},\"disabledRule\":{\"enabled\":false},\"rule3WithoutOptionsData\":{}}}";

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }

        [TestMethod]
        public void ShouldSerializeLiteralTypes()
        {
            var options = new AxeRunOptions()
            {
                AbsolutePaths = true,
                FrameWaitTimeInMilliseconds = 10,
                Iframes = true,
                RestoreScroll = true,
            };
            var expectedObject = "{\"absolutePaths\":true,\"iframes\":true,\"restoreScroll\":true,\"frameWaitTime\":10}";

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);

        }

        [TestMethod]
        public void ShouldSerializeResultTypes()
        {
            var options = new AxeRunOptions()
            {
                ResultTypes = new HashSet<ResultType>() { ResultType.Inapplicable, ResultType.Incomplete, ResultType.Passes, ResultType.Violations }
            };

            var serializedObject = JsonConvert.SerializeObject(options, serializerSettings);

            var expectedObject = "{\"resultTypes\":[\"inapplicable\",\"incomplete\",\"passes\",\"violations\"]}";

            serializedObject.Should().Be(expectedObject);
            JsonConvert.DeserializeObject<AxeRunOptions>(expectedObject).Should().BeEquivalentTo(options);
        }
    }
}
