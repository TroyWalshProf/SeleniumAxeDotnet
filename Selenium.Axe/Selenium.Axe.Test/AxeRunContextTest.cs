using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeRunContextTest
    {
        [TestMethod]
        public void ShouldSerializeObject()
        {
            var context = new AxeRunContext()
            {
                Include = new List<string[]> { new string[] { "#if1", "#idiv1" } },
                Exclude = new List<string[]> { new string[] { "#ef1", "#ediv1" } }
            };

            var expectedContent = "{\"include\":[[\"#if1\",\"#idiv1\"]],\"exclude\":[[\"#ef1\",\"#ediv1\"]]}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }

        [TestMethod]
        public void ShouldNotIncludeNullPropertiesOnSerializing()
        {
            var context = new AxeRunContext();

            var expectedContent = "{}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }
    }
}
