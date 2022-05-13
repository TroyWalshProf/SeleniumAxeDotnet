using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Selenium.Axe.Test
{
    [TestFixture]
    [NonParallelizable]
    public class AxeBuilderTest
    {
        private static Mock<IWebDriver> webDriverMock = new Mock<IWebDriver>();
        private static Mock<IJavaScriptExecutor> jsExecutorMock = webDriverMock.As<IJavaScriptExecutor>();
        private static Mock<ITargetLocator> targetLocatorMock = new Mock<ITargetLocator>();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly object testAxeResult = new
        {
            violations = new object[] { },
            passes = new object[] { },
            inapplicable = new object[] { },
            incomplete = new object[] { },
            timestamp = DateTimeOffset.Now,
            url = "www.test.com"
        };

        [Test]
        public void ThrowWhenDriverIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                //arrange / act /assert
                var axeBuilder = new AxeBuilder(null);
                axeBuilder.Should().NotBeNull();
            });
        }

        [Test]
        public void ThrowWhenOptionsAreNull()
        {
            //arrange
            var driver = new Mock<IWebDriver>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                // act / assert
                var axeBuilder = new AxeBuilder(driver.Object, null);
                axeBuilder.Should().NotBeNull();
            });
        }

        [Test]
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

        [Test]
        public void ShouldPassContextIfIncludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Include = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Include("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
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
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Include(includeSelector).Exclude(excludeSelector);

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [Test]
        public void ShouldPassContextIfExcludeSet()
        {
            var expectedContext = SerializeObject(new AxeRunContext()
            {
                Exclude = new List<string[]>(new string[][] { new string[] { "#div1" } })
            });

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(expectedContext, "{}");

            var builder = new AxeBuilder(webDriverMock.Object).Exclude("#div1");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }


        [Test]
        public void ShouldPassRunOptionsIfDeprecatedOptionsSet()
        {
            var expectedOptions = "deprecated run options";

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object);
#pragma warning disable CS0618
            builder.Options = expectedOptions;
#pragma warning restore CS0618

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassRunOptionsIfDeprecatedOptionsSetWithContextElement()
        {
            var expectedOptions = "deprecated run options";
            var expectedContext = new Mock<IWebElement>();

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanElementCall(expectedContext.Object, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object);
#pragma warning disable CS0618
            builder.Options = expectedOptions;
#pragma warning restore CS0618

            var result = builder.Analyze(expectedContext.Object);

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
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
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object)
                .DisableRules("excludeRule1", "excludeRule2")
                .WithRules("rule1", "rule2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
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
            SetupVerifiableScanCall(null, expectedOptions);

            var builder = new AxeBuilder(webDriverMock.Object)
                .WithTags("tag1", "tag2");

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
        public void ShouldPassRunOptions()
        {
            var runOptions = new AxeRunOptions()
            {
                Iframes = true,
                Rules = new Dictionary<string, RuleOptions>() { { "rule1", new RuleOptions() { Enabled = false } } }
            };

            var expectedRunOptions = SerializeObject(runOptions);

            SetupVerifiableAxeInjectionCall();
            SetupVerifiableScanCall(null, expectedRunOptions);

            var builder = new AxeBuilder(webDriverMock.Object)
                .WithOptions(runOptions);

            var result = builder.Analyze();

            VerifyAxeResult(result);

            webDriverMock.VerifyAll();
            targetLocatorMock.VerifyAll();
            jsExecutorMock.VerifyAll();
        }

        [Test]
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
            VerifyExceptionThrown<ArgumentNullException>(() => builder.WithOptions(null));
        }

        [Test]
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

        [Test]
        public void ShouldThrowIfDeprecatedOptionsIsUsedWithNewOptionsApis()
        {
            SetupVerifiableAxeInjectionCall();

            var builder = new AxeBuilder(webDriverMock.Object);
#pragma warning disable CS0618
            builder.Options = "{xpath:true}";
#pragma warning restore CS0618

            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithRules("rule-1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.DisableRules("rule-1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithTags("tag1"));
            VerifyExceptionThrown<InvalidOperationException>(() => builder.WithOptions(new AxeRunOptions() { Iframes = true }));
        }

        private void VerifyExceptionThrown<T>(Action action) where T : Exception
        {
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

        private static void SetupVerifiableAxeInjectionCall()
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

        private void SetupVerifiableScanElementCall(IWebElement elementContext, string serialzedOptions)
        {
            jsExecutorMock.Setup(js => js.ExecuteAsyncScript(
                EmbeddedResourceProvider.ReadEmbeddedFile("scan.js"),
                elementContext,
                It.Is<string>(options => options == serialzedOptions))).Returns(testAxeResult).Verifiable();
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

    }
}
