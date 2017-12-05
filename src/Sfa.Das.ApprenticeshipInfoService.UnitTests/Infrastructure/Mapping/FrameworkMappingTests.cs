using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Api.Controllers;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Mapping
{
	using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class FrameworkMappingTests
    {
        private FrameworkMapping _sut;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;

        [SetUp]
        public void Init()
        {
			_mockConfigurationSettings = new Mock<IConfigurationSettings>();

            _sut = new FrameworkMapping(_mockConfigurationSettings.Object);
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