using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure
{
    [TestFixture]
    public class ApplicationSettingsTests
    {
        [Test]
        public void TestFrameworkExpiredRequiredHandlesDetailsCorrectly()
        {
            ConfigurationManager.AppSettings["FrameworksExpiredRequired"] = "123 , 456 , 768   ";

            var frameworksExpired = new ApplicationSettings().FrameworksExpiredRequired;

            var expectedList = new List<string> { "123", "456", "768" };

            Assert.AreEqual(expectedList, frameworksExpired);
        }
    }
}
