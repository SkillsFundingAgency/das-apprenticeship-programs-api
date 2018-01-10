using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers
{
    [TestFixture]
    public class ActiveApprenticeshipCheckerTests
    {
        [TestCase("123", null, null, "x", false, "No dates and no special case, so this should be false")]
        [TestCase("123", null, null, "123", true, "No dates and special case, so this should be true")]
        [TestCase("123", "2017-11-26", null, "x", true, "Effective from in past, so should be true")]
        [TestCase("123", "2017-11-24", "2017-11-26", "x", false, "Effective from in past and effective to in past, so should be false")]
        [TestCase("123", "2017-11-24", "2017-11-26", "123", true, "Effective from in past and effective to in past + special case, so should be true")]
        [TestCase("123", "2040-01-01", null, null, false, "Effective from in future and no special case, so should be false")]
        [TestCase("123", "2040-01-01", null, "123", true, "Effective from in future and special case, so should be true")]
        [TestCase("123", null, "2040-01-01", null, false, "Effective from  absent and effective to in future and no special case, so should be false")]

        public void ShouldCheckActiveFrameworkScenarios(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo, string frameworkExpiredRequired, bool expectedResult, string message)
        {
            var mockConfigSettings = new Mock<IConfigurationSettings>();
            mockConfigSettings.Setup(x => x.FrameworksExpiredRequired).Returns(new List<string> { frameworkExpiredRequired });

            var activeFrameworkChecker = new ActiveApprenticeshipChecker(mockConfigSettings.Object);
            var res = activeFrameworkChecker.CheckActiveFramework(frameworkId, effectiveFrom, effectiveTo);

            Assert.AreEqual(expectedResult, res, message);
        }

        [TestCase("123", null, null, "x", false, "No dates and no special case, so this should be false")]
        [TestCase("123", null, null, "123", true, "No dates and special case, so this should be true")]
        [TestCase("123", "2017-11-26", null, "x", true, "Effective from in past, so should be true")]
        [TestCase("123", "2017-11-24", "2017-11-26", "x", false, "Effective from in past and effective to in past, so should be false")]
        [TestCase("123", "2017-11-24", "2017-11-26", "123", true, "Effective from in past and effective to in past + special case, so should be true")]
        [TestCase("123", "2040-01-01", null, null, false, "Effective from in future and no special case, so should be false")]
        [TestCase("123", "2040-01-01", null, "123", true, "Effective from in future and special case, so should be true")]
        [TestCase("123", null, "2040-01-01", null, false, "Effective from  absent and effective to in future and no special case, so should be false")]

        public void ShouldCheckActiveStandardScenarios(string standardId, DateTime? effectiveFrom, DateTime? effectiveTo, string standardExpiredRequired, bool expectedResult, string message)
        {
            var mockConfigSettings = new Mock<IConfigurationSettings>();
            mockConfigSettings.Setup(x => x.StandardsExpiredRequired).Returns(new List<string> { standardExpiredRequired });

            var activeFrameworkChecker = new ActiveApprenticeshipChecker(mockConfigSettings.Object);
            var res = activeFrameworkChecker.CheckActiveStandard(standardId, effectiveFrom, effectiveTo);

            Assert.AreEqual(expectedResult, res, message);
        }
    }
}
