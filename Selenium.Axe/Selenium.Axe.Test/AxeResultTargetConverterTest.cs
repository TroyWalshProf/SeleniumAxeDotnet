using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeResultTargetConverterTest
    {
        [TestMethod]
        public void ShouldReadNull()
        {
            const string json = @"{{""target"":null}}";

            var axeResultNode = DeserializeAxeNode(json);
            
            Assert.AreEqual(axeResultNode?.Target, null);
        }
        
        [TestMethod]
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
        
        [TestMethod]
        public void ShouldReadArrayOfSelectors()
        {
            var testObject = new AxeResultTarget
            {
                Selectors = { "a", "b"}
            };
            var json = $@"{{""target"":[[""{testObject.Selectors.First()}"", ""{testObject.Selectors.Last()}""]]}}";

            var axeResultTarget = DeserializeJsonAndReturnFirstTarget(json);
            
            Assert.AreEqual(axeResultTarget?.Selectors.First(), testObject.Selectors.First());
            Assert.AreEqual(axeResultTarget?.Selectors.Last(), testObject.Selectors.Last());
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
