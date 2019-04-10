using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Api.Controllers.V3;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types.V3;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers.V3
{
    [TestFixture]
    public class ProvidersV3ControllerTests
    {
        private ProvidersV3Controller _sut;
        private Mock<IGetProviderApprenticeshipLocationsV3> _mockProvidersService;

        [SetUp]
        public void Init()
        {
            _mockProvidersService = new Mock<IGetProviderApprenticeshipLocationsV3>();
            _mockProvidersService.Setup(x => x.SearchStandardProviders(12, It.Is<Coordinate>(a => a.Lat == 0.5 && a.Lon == 50), 1, 20, false, false, null)).Returns(GetStubResults());
            _mockProvidersService.Setup(x => x.SearchFrameworkProviders("420-2-1", It.Is<Coordinate>(a => a.Lat == 0.5 && a.Lon == 50), 1, 20, false, false, null)).Returns(GetStubResults());

            _sut = new ProvidersV3Controller(
                _mockProvidersService.Object,
                new ControllerHelper(),
                Mock.Of<ILog>());
        }

        [Test]
        public void StandardSearch_ReturnsListOfStandardSearchResultItemResponses()
        {
            var response = _sut.GetByStandardIdAndLocation(12, 0.5, 50);

            response.Should().BeOfType<OkNegotiatedContentResult<ProviderApprenticeshipLocationSearchResult>>();
            var results = (response as OkNegotiatedContentResult<ProviderApprenticeshipLocationSearchResult>).Content;

            results.Results.Should().HaveCount(2);
            _mockProvidersService.Verify();
        }

        [Test]
        public void StandardSearch_CorrectlyParsesListOfDeliveryModes()
        {
            List<DeliveryMode> passedDeliveryModes = null;

            _mockProvidersService.Setup(x => x.SearchStandardProviders(12, It.IsAny<Coordinate>(), 1, 20, false, false, It.IsAny<List<DeliveryMode>>()))
            .Callback<int, Coordinate, int, int, bool, bool, List<DeliveryMode>>((a, b, c, d, e, f, g) =>
            {
                passedDeliveryModes = g;
            });

            _sut.GetByStandardIdAndLocation(12, 0.5, 50, deliveryModes: "0, 2");

            passedDeliveryModes.Should().Contain(DeliveryMode.DayRelease);
            passedDeliveryModes.Should().Contain(DeliveryMode.HundredPercentEmployer);
        }

        [TestCase("abc")]
        [TestCase("a,b,c")]
        [TestCase("1,3,a,4")]
        [TestCase(",3,4")]
        public void StandardSearch_ReturnsBadRequestIfDeliveryModesAreInvalid(string deliveryModes)
        {
            List<DeliveryMode> passedDeliveryModes = null;

            _mockProvidersService.Setup(x => x.SearchStandardProviders(12, It.IsAny<Coordinate>(), 1, 20, false, false, It.IsAny<List<DeliveryMode>>()))
                .Callback<int, Coordinate, int, int, bool, bool, List<DeliveryMode>>((a, b, c, d, e, f, g) =>
                {
                    passedDeliveryModes = g;
                });

            var response = _sut.GetByStandardIdAndLocation(12, 0.5, 50, deliveryModes: deliveryModes);

            response.Should().BeOfType<BadRequestResult>();
        }

        [Test]
        public void FrameworkSearch_ReturnsListOfFrameworkSearchSearchResultItemResponses()
        {
            var response = _sut.GetByFrameworkIdAndLocation("420-2-1", 0.5, 50);

            response.Should().BeOfType<OkNegotiatedContentResult<ProviderApprenticeshipLocationSearchResult>>();
            var results = (response as OkNegotiatedContentResult<ProviderApprenticeshipLocationSearchResult>).Content;

            results.Results.Should().HaveCount(2);
            _mockProvidersService.Verify();
        }

        [Test]
        public void FrameworkSearch_CorrectlyParsesListOfDeliveryModes()
        {
            List<DeliveryMode> passedDeliveryModes = null;

            _mockProvidersService.Setup(x => x.SearchFrameworkProviders("420-2-1", It.IsAny<Coordinate>(), 1, 20, false, false, It.IsAny<List<DeliveryMode>>()))
            .Callback<string, Coordinate, int, int, bool, bool, List<DeliveryMode>>((a, b, c, d, e, f, g) =>
            {
                passedDeliveryModes = g;
            });

            _sut.GetByFrameworkIdAndLocation("420-2-1", 0.5, 50, deliveryModes: "0, 2");

            passedDeliveryModes.Should().Contain(DeliveryMode.DayRelease);
            passedDeliveryModes.Should().Contain(DeliveryMode.HundredPercentEmployer);
        }

        [TestCase("abc")]
        [TestCase("a,b,c")]
        [TestCase("1,3,a,4")]
        [TestCase(",3,4")]
        public void FrameworkSearch_ReturnsBadRequestIfDeliveryModesAreInvalid(string deliveryModes)
        {
            List<DeliveryMode> passedDeliveryModes = null;

            _mockProvidersService.Setup(x => x.SearchFrameworkProviders("420-2-1", It.IsAny<Coordinate>(), 1, 20, false, false, It.IsAny<List<DeliveryMode>>()))
                .Callback<string, Coordinate, int, int, bool, bool, List<DeliveryMode>>((a, b, c, d, e, f, g) =>
                {
                    passedDeliveryModes = g;
                });

            var response = _sut.GetByFrameworkIdAndLocation("420-2-1", 0.5, 50, deliveryModes: deliveryModes);

            response.Should().BeOfType<BadRequestResult>();
        }

        private ProviderApprenticeshipLocationSearchResult GetStubResults()
        {
            return new ProviderApprenticeshipLocationSearchResult
            {
                Results = new List<ProviderSearchResultItem>
                {
                    new ProviderSearchResultItem(),
                    new ProviderSearchResultItem()
                }
            };
        }
    }
}
