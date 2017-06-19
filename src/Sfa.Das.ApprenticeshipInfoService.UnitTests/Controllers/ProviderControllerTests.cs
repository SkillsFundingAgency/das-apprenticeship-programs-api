using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using FluentAssertions;
using NUnit.Framework.Constraints;

using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using Sfa.Das.ApprenticeshipInfoService.Api.Controllers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;

    [TestFixture]
    public class ProviderControllerTests
    {
        private ProvidersController _sut;
        private Mock<IGetProviders> _mockGetProviders;
        private Mock<IControllerHelper> _mockControllerHelper;
        private Mock<IApprenticeshipProviderRepository> _mockApprenticeshipProviderRepository;
        private Mock<ILog> _mockLogger;

        [SetUp]
        public void Init()
        {
            _mockGetProviders = new Mock<IGetProviders>();
            _mockControllerHelper = new Mock<IControllerHelper>();
            _mockApprenticeshipProviderRepository = new Mock<IApprenticeshipProviderRepository>();
            _mockLogger = new Mock<ILog>();

            _sut = new ProvidersController(
                _mockGetProviders.Object,
                _mockControllerHelper.Object,
                _mockApprenticeshipProviderRepository.Object);
            _sut.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/providers")
            };
            _sut.Configuration = new HttpConfiguration();
            _sut.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            _sut.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "providers" } });

        }

        [Test]
        public void ShouldReturnProvider()
        {
            var expected = new Provider() { Ukprn = 1 };

            _mockGetProviders.Setup(
                x =>
                    x.GetProviderByUkprn(1)).Returns(expected);

            var actual = _sut.Get(1);

            actual.ShouldBeEquivalentTo(expected);
            actual.Uri.Should().Be("http://localhost/providers/1");
        }

        [Test]
        public void ShouldReturnProvidersNotFound()
        {
            var expected = new Provider() { Ukprn = 1 };

            _mockGetProviders.Setup(
                x =>
                    x.GetProviderByUkprn(1)).Returns(expected);

            ActualValueDelegate<object> test = () => _sut.Get(-2);

            Assert.That(test, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ShouldReturnValidListOfStandardProviders()
        {
            var element = new StandardProviderSearchResultsItemResponse
            {
                ProviderName = "Test provider name",
                ApprenticeshipInfoUrl = "http://www.abba.co.uk",
                LegalName = "Test Legal Name"
            };
            var expected = new List<StandardProviderSearchResultsItemResponse> { element };

            _mockControllerHelper.Setup(x => x.GetActualPage(It.IsAny<int>())).Returns(1);
            _mockGetProviders.Setup(
                x =>
                    x.GetByStandardIdAndLocation(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).Returns(expected);

            var response = _sut.GetByStandardIdAndLocation(1, 2, 3, 1);

            response.Should().NotBeNull();
            response.Should().BeOfType<List<StandardProviderSearchResultsItemResponse>>();
            response.Should().NotBeEmpty();
            response.Should().BeEquivalentTo(expected);
            response.First().Should().NotBe(null);
            response.First().Should().Be(element);
            response.First().ProviderName.Should().Be(element.ProviderName);
            response.First().LegalName.Should().Be(element.LegalName);
            response.First().ApprenticeshipInfoUrl.Should().Be(element.ApprenticeshipInfoUrl);
        }

        [Test]
        public void ShouldReturnValidListOfFrameworkProviders()
        {
            var element = new FrameworkProviderSearchResultsItemResponse
            {
                ProviderName = "Test provider name",
                ApprenticeshipInfoUrl = "http://www.abba.co.uk",
                LegalName = "Test Legal Name"
            };
            var expected = new List<FrameworkProviderSearchResultsItemResponse> { element };

            _mockControllerHelper.Setup(x => x.GetActualPage(It.IsAny<int>())).Returns(1);
            _mockGetProviders.Setup(
                x =>
                    x.GetByFrameworkIdAndLocation(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>())).Returns(expected);

            var response = _sut.GetByFrameworkIdAndLocation(1, 2, 3, 1);

            response.Should().NotBeNull();
            response.Should().BeOfType<List<FrameworkProviderSearchResultsItemResponse>>();
            response.Should().NotBeEmpty();
            response.Should().BeEquivalentTo(expected);
            response.First().Should().NotBe(null);
            response.First().Should().Be(element);
            response.First().ProviderName.Should().Be(element.ProviderName);
            response.First().LegalName.Should().Be(element.LegalName);
            response.First().ApprenticeshipInfoUrl.Should().Be(element.ApprenticeshipInfoUrl);
        }

        [Test]
        public void ShouldThrowExceptionIfLatLonIsNullSearchingByStandardId()
        {
            _mockControllerHelper.Setup(x => x.GetActualPage(It.IsAny<int>())).Returns(1);

            ActualValueDelegate<object> test = () => _sut.GetByStandardIdAndLocation(1, null, null, 1);

            Assert.That(test, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ShouldThrowExceptionIfLatLonIsNullSearchingByFrameworkId()
        {
            _mockControllerHelper.Setup(x => x.GetActualPage(It.IsAny<int>())).Returns(1);

            ActualValueDelegate<object> test = () => _sut.GetByFrameworkIdAndLocation(1, null, null, 1);

            Assert.That(test, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ShouldthrowExceptionWhenServiceisDown()
        {
            _mockGetProviders.Setup(
               x =>
                   x.GetAllProviders()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotthrowExceptionWhenServiceisUp()
        {
            _mockGetProviders.Setup(
               x =>
                   x.GetAllProviders()).Returns(new List<ProviderSummary> { new ProviderSummary { Ukprn = 40120001 }, new ProviderSummary { Ukprn = 52140002 } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}
