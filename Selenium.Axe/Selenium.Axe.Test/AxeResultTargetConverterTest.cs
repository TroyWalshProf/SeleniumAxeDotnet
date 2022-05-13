using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Selenium.Axe.Test
{
    [TestFixture]
    public class AxeResultTargetConverterTest
    {
        [Test]
        public void CanConvertPassedAxeResultTarget()
        {
            var instance = new AxeResultTargetConverter();
            var result = instance.CanConvert(typeof(AxeResultTarget));
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldReadSingleSelector()
        {
            var testObject = new AxeResultTarget
            {
                Selector = "test"
            };
            var json = $@"{{""target"":[""{testObject.Selector}""]}}";
            var axeResultTarget = DeserializeJsonAndReturnFirstTarget(json);

            Assert.AreEqual(axeResultTarget?.Selector, testObject.Selector);
        }

        [Test]
        public void ShouldReadArrayOfSelectors()
        {
            var testObject = new AxeResultTarget
            {
                Selectors = { "a", "b" }
            };
            var json = $@"{{""target"":[[""{testObject.Selectors.First()}"", ""{testObject.Selectors.Last()}""]]}}";

            var axeResultTarget = DeserializeJsonAndReturnFirstTarget(json);

            Assert.AreEqual(axeResultTarget?.Selectors.First(), testObject.Selectors.First());
            Assert.AreEqual(axeResultTarget?.Selectors.Last(), testObject.Selectors.Last());
        }

        [Test]
        public void Write()
        {
            var expectedResult = "\"test\"";
            var value = new AxeResultTarget
            {
                Selector = "test"
            };
            using (var writer = new StringWriter())
            {
                var jsonWriter = new JsonTextWriter(writer);
                var instance = new AxeResultTargetConverter();

                instance.WriteJson(jsonWriter, value, new JsonSerializer { Converters = { instance } });

                var result = writer.ToString();

                Assert.IsTrue(expectedResult.Equals(result));
            }
        }

        private static AxeResultTarget DeserializeJsonAndReturnFirstTarget(string json)
        {
            return DeserializeAxeNode(json)
                ?.Target.FirstOrDefault();
        }

        private static AxeResultNode DeserializeAxeNode(string json)
        {
            return JsonConvert
                .DeserializeObject<AxeResultNode>(json, new JsonSerializerSettings
                {
                    Converters = { new AxeResultTargetConverter() }
                });
        }
    }
}
