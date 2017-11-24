using System;
using FluentAssertions;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers
{
    [TestFixture]
    public class DateHelperTests
    {
        [TestCase("", "", false)]
        [TestCase("", "02/02/2015", false)]
        [TestCase("", "02/02/2040", false)]
        [TestCase("02/02/2015", "", true)]
        [TestCase("02/02/2015", "02/03/2015", false)]
        [TestCase("02/02/2015", "02/02/2040", true)]
        [TestCase("02/02/2039", "", false)]
        [TestCase("02/02/2039", "02/02/2015", false)]
        [TestCase("02/02/2039", "02/02/2040", false)]
        public void ShouldCheckEffectiveDates(string effectiveFrom, string effectiveTo, bool expected)
        {
            var effectiveFromDate = MapStringToDate(effectiveFrom);
            var effectiveToDate = MapStringToDate(effectiveTo);

            var result = DateHelper.CheckEffectiveDates(effectiveFromDate, effectiveToDate);
            result.Should().Be(expected);
        }

        private static DateTime? MapStringToDate(string date)
        {
            DateTime? response;
            if (!string.IsNullOrWhiteSpace(date))
            {
                response = DateTime.Parse(date);
            }
            else
            {
                response = null;
            }
            return response;
        }
    }
}
