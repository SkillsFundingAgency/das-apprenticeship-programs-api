using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Mapping
{
    [TestFixture]
    public class FrameworkMappingTests
    {
        private FrameworkMapping _sut;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;
        private ActiveApprenticeshipChecker _activeApprenticeshipChecker;
	    private Mock<IFundingCapCalculator> _mockFundingCapCalculator;

	    [SetUp]
        public void Init()
        {
			_mockConfigurationSettings = new Mock<IConfigurationSettings>();
			_mockFundingCapCalculator = new Mock<IFundingCapCalculator>();
	        _mockFundingCapCalculator.Setup(x => x.CalculateCurrentFundingBand(It.IsAny<StandardSearchResultsItem>()))
		        .Returns(0);
			_mockFundingCapCalculator.Setup(x => x.CalculateCurrentFundingBand(It.IsAny<FrameworkSearchResultsItem>()))
		        .Returns(0);
            _activeApprenticeshipChecker = new ActiveApprenticeshipChecker(_mockConfigurationSettings.Object);

            _sut = new FrameworkMapping(_activeApprenticeshipChecker, _mockFundingCapCalculator.Object);
		}

	    [Test]
	    public void ShouldReturnEarlierAndLatestDates()
	    {
		    var documents = new List<FrameworkSearchResultsItem>
		    {
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today.AddDays(-1),
				    EffectiveTo = DateTime.Today.AddDays(3)
			    },
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today.AddDays(-7),
				    EffectiveTo = DateTime.Today.AddDays(2)
			    },
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today.AddDays(-14),
				    EffectiveTo = DateTime.Today
			    }
		    };
		    var result = _sut.MapToFrameworkCodeSummaryFromList(documents);

		    result.EffectiveFrom.Should().Be(DateTime.Today.AddDays(-14));
		    result.EffectiveTo.Should().Be(DateTime.Today.AddDays(3));
		}

		[Test]
		public void ShouldReturnEarlierDateEvenIfThereIsANull()
		{
			var documents = new List<FrameworkSearchResultsItem>
			{
				new FrameworkSearchResultsItem
				{
					EffectiveFrom = DateTime.Today.AddDays(-1),
					EffectiveTo = DateTime.Today.AddDays(3)
				},
				new FrameworkSearchResultsItem
				{
					EffectiveFrom = null,
					EffectiveTo = DateTime.Today.AddDays(2)
				},
				new FrameworkSearchResultsItem
				{
					EffectiveFrom = DateTime.Today.AddDays(-14),
					EffectiveTo = DateTime.Today
				}
			};
			var result = _sut.MapToFrameworkCodeSummaryFromList(documents);

			result.EffectiveFrom.Should().Be(DateTime.Today.AddDays(-14));
			result.EffectiveTo.Should().Be(DateTime.Today.AddDays(3));
		}

	    [Test]
	    public void ShouldReturnNullIfOneLatestDateHasNoValue()
	    {
		    var documents = new List<FrameworkSearchResultsItem>
		    {
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today.AddDays(-1),
				    EffectiveTo = null
			    },
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today,
				    EffectiveTo = DateTime.Today.AddDays(2)
			    },
			    new FrameworkSearchResultsItem
			    {
				    EffectiveFrom = DateTime.Today.AddDays(-14),
				    EffectiveTo = DateTime.Today
			    }
		    };
		    var result = _sut.MapToFrameworkCodeSummaryFromList(documents);

		    result.EffectiveFrom.Should().Be(DateTime.Today.AddDays(-14));
		    result.EffectiveTo.Should().Be(null);
	    }
	}
}