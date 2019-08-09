using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeBuilderTest
    {
        private Mock<IWebDriver> webDriverMock;
        private Mock<IJavaScriptExecutor> jsExecutorMock;
        private Mock<ITargetLocator> targetLocatorMock;
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly string testAxeResult = JsonConvert.SerializeObject(new
        {
            results = new
            {
                violations = new object[] { },
                passes = new object[] { },
                inapplicable = new object[] { },
                incomplete = new object[] { },
                timestamp = DateTimeOffset.Now,
                url = "www.test.com",
            }
        });

        [TestInitialize]
        public void TestInitialize()
        {
            webDriverMock = new Mock<IWebDriver>();
            jsExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
            targetLocatorMock = new Mock<ITargetLocator>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowWhenDriverIsNull()
        {
            //arrange / act /assert
            var axeBuilder = new AxeBuilder(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowWhenOptionsAreNull()
        {
            //arrange
            var driver = new Mock<IWebDriver>();

            // act / assert
            var axeBuilder = new AxeBuilder(driver.Object, null);
        }

        [TestMethod]
        public void ShouldHandleIfOptionsAndContextNotSet()
        {

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, "{}");

            var builder = new AxeBuilder(webDriverMock.Object);
            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();

        }

        [TestMethod]
        public void ShouldPassContextIfIncludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext.ToString(), "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Include("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldPassContextIfIncludeAndExcludeSet()
        {
            var includeSelector = "#div1";
            var excludeSelector = "#div2";
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { includeSelector } }),
                Exclude = new List<string[]>(new string[][] { new string[] { excludeSelector } })
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext.ToString(), "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Include(includeSelector).Exclude(excludeSelector);

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [TestMethod]
        public void ShouldPassContextIfExcludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Exclude = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext.ToString(), "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Exclude("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [TestMethod]
        public void ShouldPassRunOptionsIfDeprecatedOptionsSet()
        {
            var expectedOptions = "deprecated run options";

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions.ToString());

            var builder = new AxeBuilder(webDriverMock.Object);
            builder.Options = expectedOptions;

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldPassRuleConfig()
        {
            var expectedRules = new List<string> { "rule1", "rule2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "rule",
                    Values = expectedRules
                },
                Rules = new Dictionary<string, RuleOptions>()
                {
                   { "excludeRule1", new RuleOptions(){ Enabled = false} },
                   { "excludeRule2", new RuleOptions(){ Enabled = false } }
                }

            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions.ToString());

            var builder = new AxeBuilder(webDriverMock.Object)
                .DisableRules("excludeRule1", "excludeRule2")
                .WithRules("rule1", "rule2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldPassRunOptionsWithTagConfig()
        {
            var expectedTags = new List<string> { "tag1", "tag2" };

            var expectedOptions = SerializeObject(new AxeRunOptions()
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values = expectedTags
                },
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions.ToString());

            var builder = new AxeBuilder(webDriverMock.Object)
                .WithTags("tag1", "tag2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldThrowIfNullParameterPassed()
        {
            SetupVerifiableAxeInjectionCall();

            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(webDriverMock.Object, null));
            VerifyExceptionThrown<ArgumentNullException>(() => new AxeBuilder(null));

            var builder = new AxeBuilder(webDriverMock.Object);

            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.DisableRules(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithTags(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Include(null));
            VerifyExceptionThrown<ArgumentNullException>(() => builder.Exclude(null));
        }

        [TestMethod]
        public void ShouldThrowIfEmptyParameterPassed()
        {
            var values = new string[] { "val1", "" };

            SetupVerifiableAxeInjectionCall();

            var builder = new AxeBuilder(webDriverMock.Object);

            VerifyExceptionThrown<ArgumentException>(() => builder.WithRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.DisableRules(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.WithTags(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.Include(values));
            VerifyExceptionThrown<ArgumentException>(() => builder.Exclude(values));
        }

        private void VerifyExceptionThrown<T>(Action action) where T : Exception {
            action.Should().Throw<T>();
        }

        private void VerifyAxeResult(AxeResult result)
        {
            result.Should().NotBeNull();
            result.Inapplicable.Should().NotBeNull();
            result.Incomplete.Should().NotBeNull();
            result.Passes.Should().NotBeNull();
            result.Violations.Should().NotBeNull();

            result.Inapplicable.Length.Should().Be(0);
            result.Incomplete.Length.Should().Be(0);
            result.Passes.Length.Should().Be(0);
            result.Violations.Length.Should().Be(0);
        }
        private void SetupVerifiableAxeInjectionCall()
        {
            webDriverMock
          .Setup(d => d.FindElements(It.IsAny<By>()))
          .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0)));

            webDriverMock.Setup(d => d.SwitchTo()).Returns(targetLocatorMock.Object);

            jsExecutorMock
                .Setup(js => js.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("axe.min.js"))).Verifiable();

        }

        private void SetupVerifiableScanCall(string serializedContext, string serialzedOptions)
        {
            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                EmbeddedResourceProvider.ReadEmbeddedFile("scan.js"),
                It.Is<string>(context => context == serializedContext),
                It.Is<string>(options => options == serialzedOptions))).Returns(testAxeResult).Verifiable();
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

    }
}
