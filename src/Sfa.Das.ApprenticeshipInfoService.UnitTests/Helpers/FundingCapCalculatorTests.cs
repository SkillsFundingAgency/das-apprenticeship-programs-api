using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers
{
    [TestFixture]
    public class FundingCapCalculatorTests
    {
        private Mock<IActiveApprenticeshipChecker> _mockApprenticeshipChecker;

        [SetUp]
        public void Init()
        {
            _mockApprenticeshipChecker = new Mock<IActiveApprenticeshipChecker>();

            _mockApprenticeshipChecker
                .Setup(x => x.IsActiveStandard(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);
            _mockApprenticeshipChecker
                .Setup(x => x.IsActiveFramework(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(true);
        }

        [Test]
        public void ShouldReturnZeroIfFundingPeriodsAreNull()
        {
            var sut = new FundingCapCalculator(_mockApprenticeshipChecker.Object);

            var standardResult = sut.CalculateCurrentFundingBand(new StandardSearchResultsItem
            {
                StandardId = "1",
                FundingPeriods = null
            });

            var frameworkResult = sut.CalculateCurrentFundingBand(new FrameworkSearchResultsItem
            {
                FrameworkId = "1",
                FundingPeriods = null
            });

            Assert.AreEqual(0, standardResult);
            Assert.AreEqual(0, frameworkResult);
        }

        [Test]
        public void ShouldReturnZeroIfApprenticeshipIsNotActive()
        {
            _mockApprenticeshipChecker
                .Setup(x => x.IsActiveStandard(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(false);
            _mockApprenticeshipChecker
                .Setup(x => x.IsActiveFramework(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(false);

            var sut = new FundingCapCalculator(_mockApprenticeshipChecker.Object);

            var standardResult = sut.CalculateCurrentFundingBand(new StandardSearchResultsItem
            {
                StandardId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1)
            });

            var frameworkResult = sut.CalculateCurrentFundingBand(new FrameworkSearchResultsItem()
            {
                FrameworkId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1)
            });

            Assert.AreEqual(0, standardResult);
            Assert.AreEqual(0, frameworkResult);
        }

        [Test]
        public void ShouldReturnCurrentFundingBand()
        {
            var sut = new FundingCapCalculator(_mockApprenticeshipChecker.Object);

            var standardResult = sut.CalculateCurrentFundingBand(new StandardSearchResultsItem
            {
                StandardId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-3),
                        EffectiveTo = DateTime.Today.AddDays(-2),
                        FundingCap = 1000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today,
                        EffectiveTo = DateTime.Today.AddDays(1),
                        FundingCap = 2000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(2),
                        EffectiveTo = DateTime.Today.AddDays(3),
                        FundingCap = 3000
                    }
                }
            });

            var frameworkResult = sut.CalculateCurrentFundingBand(new FrameworkSearchResultsItem()
            {
                FrameworkId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-3),
                        EffectiveTo = DateTime.Today.AddDays(-2),
                        FundingCap = 1000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today,
                        EffectiveTo = DateTime.Today.AddDays(1),
                        FundingCap = 2000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(2),
                        EffectiveTo = DateTime.Today.AddDays(3),
                        FundingCap = 3000
                    }
                }
            });

            Assert.AreEqual(2000, standardResult);
            Assert.AreEqual(2000, frameworkResult);
        }

        [Test]
        public void ShouldReturnLastCurrentBandIfTheRestAreExpired()
        {
            var sut = new FundingCapCalculator(_mockApprenticeshipChecker.Object);

            var standardResult = sut.CalculateCurrentFundingBand(new StandardSearchResultsItem
            {
                StandardId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-10),
                        EffectiveTo = DateTime.Today.AddDays(-9),
                        FundingCap = 1000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-7),
                        EffectiveTo = DateTime.Today.AddDays(-6),
                        FundingCap = 2000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-5),
                        EffectiveTo = DateTime.Today.AddDays(-4),
                        FundingCap = 3000
                    }
                }
            });

            var frameworkResult = sut.CalculateCurrentFundingBand(new FrameworkSearchResultsItem()
            {
                FrameworkId = "1",
                EffectiveFrom = DateTime.Today,
                EffectiveTo = DateTime.Today.AddDays(1),
                FundingPeriods = new List<FundingPeriod>
                {
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-10),
                        EffectiveTo = DateTime.Today.AddDays(-9),
                        FundingCap = 1000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-7),
                        EffectiveTo = DateTime.Today.AddDays(-6),
                        FundingCap = 2000
                    },
                    new FundingPeriod
                    {
                        EffectiveFrom = DateTime.Today.AddDays(-5),
                        EffectiveTo = DateTime.Today.AddDays(-4),
                        FundingCap = 3000
                    }
                }
            });

            Assert.AreEqual(3000, standardResult);
            Assert.AreEqual(3000, frameworkResult);
        }
    }
}
