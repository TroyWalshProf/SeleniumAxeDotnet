using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class AxeBuilderTest
    {
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
        public void ShouldExecuteAxeScript()
        {
            //arrange
            var driver = new Mock<IWebDriver>();
            var jsExecutor =driver.As<IJavaScriptExecutor>();
            var targetLocator = new Mock<ITargetLocator>();

            driver
                .Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0)));

            driver.Setup(d => d.SwitchTo()).Returns(targetLocator.Object);
            targetLocator.Setup(t => t.DefaultContent()).Returns(driver.Object);

            jsExecutor
                .Setup(js => js.ExecuteAsyncScript(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(@"
                {
                    results: {
                        violations: [],
                        passes: [],
                        inapplicable: [],
                        incomplete: [],
                        timestamp: ""2019-04-11T00:59:07.749Z"",
                        url: ""https://www.test.com""
                    }
                }");

            var builder = new AxeBuilder(driver.Object);
            var result = builder.Analyze();

            result.Should().NotBeNull();
            result.Inapplicable.Should().NotBeNull();
            result.Incomplete.Should().NotBeNull();
            result.Passes.Should().NotBeNull();
            result.Violations.Should().NotBeNull();

            result.Inapplicable.Length.Should().Be(0);
            result.Incomplete.Length.Should().Be(0);
            result.Passes.Length.Should().Be(0);
            result.Violations.Length.Should().Be(0);

            driver.VerifyAll();
            targetLocator.VerifyAll();
            jsExecutor.VerifyAll();
        }
    }
}
