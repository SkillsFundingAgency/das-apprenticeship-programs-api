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
        [TestCase(null, null, false, "No dates, so this should be false")]
        [TestCase("2017-11-26", null, true, "Effective from in past, so should be true")]
        [TestCase("2017-11-24", "2017-11-26", false, "Effective from in past and effective to in past, so should be false")]
        [TestCase("2040-01-01", null, false, "Effective from in future, so should be false")]
        [TestCase(null, "2040-01-01", false, "Effective from  absent and effective to in future, so should be false")]

        public void ShouldCheckActiveFrameworkScenarios(DateTime? effectiveFrom, DateTime? effectiveTo, bool expectedResult, string message)
        {
            var activeFrameworkChecker = new ActiveApprenticeshipChecker();
            var res = activeFrameworkChecker.CheckActiveFramework(effectiveFrom, effectiveTo);

            Assert.AreEqual(expectedResult, res, message);
        }

        [TestCase(null, null, false, "No dates, so this should be false")]
        [TestCase("2017-11-26", null, true, "Effective from in past, so should be true")]
        [TestCase("2017-11-24", "2017-11-26", false, "Effective from in past and effective to in past, so should be false")]
        [TestCase("2040-01-01", null, false, "Effective from in future, so should be false")]
        [TestCase(null, "2040-01-01", false, "Effective from absent and effective to in future, so should be false")]

        public void ShouldCheckActiveStandardScenarios(DateTime? effectiveFrom, DateTime? effectiveTo, bool expectedResult, string message)
        {
            var activeFrameworkChecker = new ActiveApprenticeshipChecker();
            var res = activeFrameworkChecker.CheckActiveStandard(effectiveFrom, effectiveTo);

            Assert.AreEqual(expectedResult, res, message);
        }
    }
}
