using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Selenium.Axe.Test
{
    [TestFixture]
    public class AxeRunContextTest
    {
        [Test]
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

        [Test]
        public void ShouldNotIncludeNullPropertiesOnSerializing()
        {
            var context = new AxeRunContext();

            var expectedContent = "{}";

            JsonConvert.SerializeObject(context).Should().Be(expectedContent);
        }
    }
}
