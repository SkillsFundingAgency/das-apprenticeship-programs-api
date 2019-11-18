using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types.V4;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers.V4
{
    [TestFixture]
    public class ProvidersV4ControllerTests
    {
        private ProvidersV4Controller _sut;
        private Mock<IGetProviderApprenticeshipLocationsV4> _mockProvidersService;

        [SetUp]
        public void Init()
        {
            _mockProvidersService = new Mock<IGetProviderApprenticeshipLocationsV4>();

            _sut = new ProvidersV4Controller(
                _mockProvidersService.Object,
                new ControllerHelper(),
                Mock.Of<ILogger<ProvidersV4Controller>>());
        }

        [Test]
        public void ProviderGroupedSearch_ReturnsListOfSearchResultItemResponses()
        {
            _mockProvidersService.Setup(x => x.SearchStandardProviderLocations(It.IsAny<int>(), It.IsAny<Coordinate>(), 1, 10, false, false))
                .Returns(new UniqueProviderApprenticeshipLocationSearchResult 
                {
                    Results = new List<UniqueProviderApprenticeshipLocationSearchResultItem>
                    {
                        new UniqueProviderApprenticeshipLocationSearchResultItem()
                    }
                });

            var response = _sut.GetByApprenticeshipIdAndLatLon("12", 10.1, 12.2, 1, 10, false, false);
            
            response.Value.Should().BeOfType<UniqueProviderApprenticeshipLocationSearchResult>();
            var results = response.Value;
            results.Results.Should().HaveCount(1);
            _mockProvidersService.Verify();
        }

        [Test]
        public void ProviderGroupedSearch_SearchesForFrameworkLocationsIfFrameworkIdPassed()
        {
            const string FrameworkId = "420-2-1";

            _mockProvidersService.Setup(x => x.SearchFrameworkProvidersLocations(FrameworkId, It.IsAny<Coordinate>(), 1, 10, false, false))
                .Verifiable();

            _sut.GetByApprenticeshipIdAndLatLon(FrameworkId, 10.1, 12.2, 1, 10, false, false);

            _mockProvidersService.Verify();
        }

        [Test]
        public void ProviderGroupedSearch_SearchesForStandardLocationsIfStandardIdPassed()
        {
            const int StandardId = 30;
            _mockProvidersService.Setup(x => x.SearchStandardProviderLocations(StandardId, It.IsAny<Coordinate>(), 1, 10, false, false))
                .Verifiable();

             _sut.GetByApprenticeshipIdAndLatLon(StandardId.ToString(), 10.1, 12.2, 1, 10, false, false);

            _mockProvidersService.Verify();
        }


        [Test]
        public void ProviderApprenticeshipLocations_ReturnsListOfLocationResponses()
        {
            _mockProvidersService.Setup(x => x.GetClosestLocationsForStandard(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<Coordinate>(), 1, 10, false))
                .Returns(new ProviderLocationsSearchResults 
                {
                    Results = new List<ProviderLocationsSearchResultsItem>
                    {
                        new ProviderLocationsSearchResultsItem(),
                        new ProviderLocationsSearchResultsItem()
                    }
                });

            var response = _sut.GetClosestProviderLocationsThatCoverPointForApprenticeship(12345678, "10", 10.1, 12.2, false, 1, 10);

            response.Value.Should().BeOfType<ProviderLocationsSearchResults>();
            var results = response.Value;

            results.Results.Should().HaveCount(2);
            _mockProvidersService.Verify();
        }

        [Test]
        public void ProviderApprenticeshipLocations_SearchesForFrameworkLocationsIfFrameworkIdPassed()
        {
            const string FrameworkId = "420-2-1";

            _mockProvidersService.Setup(x => x.GetClosestLocationsForFramework(It.IsAny<long>(), FrameworkId, It.IsAny<Coordinate>(), 1, 10, false))
                .Verifiable();

            _sut.GetClosestProviderLocationsThatCoverPointForApprenticeship(12345678, FrameworkId, 10.1, 12.2, false, 1, 10);

            _mockProvidersService.Verify();
        }

        [Test]
        public void ProviderApprenticeshipLocations_SearchesForStandardLocationsIfStandardIdPassed()
        {
            const int StandardId = 30;
            _mockProvidersService.Setup(x => x.GetClosestLocationsForStandard(It.IsAny<long>(), StandardId, It.IsAny<Coordinate>(), 1, 10, false))
                .Verifiable();

             _sut.GetClosestProviderLocationsThatCoverPointForApprenticeship(12345678, StandardId.ToString(), 10.1, 12.2, false, 1, 10);

            _mockProvidersService.Verify();
        }
    }
}
