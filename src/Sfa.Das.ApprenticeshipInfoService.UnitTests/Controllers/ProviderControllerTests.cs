using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using Api.Controllers;
    using Core.Helpers;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;
    using SFA.DAS.NLog.Logger;

    [TestFixture]
    public class ProviderControllerTests
    {
        private const int ProviderApprenticeshipsMaximum = 3;
        private ProvidersController _sut;
        private Mock<IGetProviders> _mockGetProviders;
        private Mock<IControllerHelper> _mockControllerHelper;
        private Mock<IApprenticeshipProviderRepository> _mockApprenticeshipProviderRepository;
        private Mock<ILog> _mockLogger;
        private Mock<IGetStandards> _mockGetStandards;
        private Mock<IGetFrameworks> _mockGetFrameworks;
        private Mock<IActiveFrameworkChecker> _mockActiveFrameworkChecker;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;
      
        [SetUp]
        public void Init()
        {
            _mockGetProviders = new Mock<IGetProviders>();
            _mockControllerHelper = new Mock<IControllerHelper>();
            _mockGetStandards = new Mock<IGetStandards>();
            _mockGetFrameworks = new Mock<IGetFrameworks>();
            _mockApprenticeshipProviderRepository = new Mock<IApprenticeshipProviderRepository>();
            _mockLogger = new Mock<ILog>();
            _mockActiveFrameworkChecker = new Mock<IActiveFrameworkChecker>();
            _mockConfigurationSettings = new Mock<IConfigurationSettings>();
            _mockConfigurationSettings.Setup(x => x.ProviderApprenticeshipsMaximum)
                .Returns(ProviderApprenticeshipsMaximum);

            _sut = new ProvidersController(
                _mockGetProviders.Object,
                _mockControllerHelper.Object,
                _mockGetStandards.Object,
                _mockGetFrameworks.Object,
                _mockApprenticeshipProviderRepository.Object,
                _mockActiveFrameworkChecker.Object,
                _mockConfigurationSettings.Object)
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/providers")
                },
                Configuration = new HttpConfiguration()
            };

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
            var ukprn = 12345678;
            var expected = new Provider() { Ukprn = ukprn };

            _mockGetProviders.Setup(
                x =>
                    x.GetProviderByUkprn(ukprn)).Returns(expected);

            var actual = _sut.Get(ukprn);

            actual.ShouldBeEquivalentTo(expected);
            actual.Uri.Should().Be($"http://localhost/providers/{ukprn}");
        }

        [Test]
        public void ShouldReturnProvidersNotFound()
        {
            var ex = Assert.Throws<HttpResponseException>(() => _sut.Get(12345679));
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void AnInvalidUkprnShouldReturnABadRequest()
        {
            var ex = Assert.Throws<HttpResponseException>(() => _sut.Get(123456));
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
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
        public void ShouldReturnListOfUniqueProvidersForAStandard()
        {
            var standardCode = 1;
            var data = new List<StandardProviderSearchResultsItem>
            {
                new StandardProviderSearchResultsItem { Ukprn = 10005214, StandardCode = standardCode },
                new StandardProviderSearchResultsItem { Ukprn = 10005214, StandardCode = standardCode },
                new StandardProviderSearchResultsItem { Ukprn = 10006214, StandardCode = standardCode }
            };

            _mockGetProviders.Setup(x => x.GetProvidersByStandardId(It.IsAny<string>())).Returns(data);
            _mockGetProviders.Setup(x => x.GetProviderByUkprnList(It.IsAny<List<long>>())).Returns(new List<Provider>());
            _mockGetStandards.Setup(x => x.GetStandardById(It.IsAny<string>())).Returns(new Standard());

            _sut.GetStandardProviders(standardCode.ToString());

            _mockGetProviders.Verify(x => x.GetProviderByUkprnList(new List<long> { 10005214L, 10006214L }), Times.Once);
        }

        [Test]
        public void ShouldReturnActiveListOfProviderApprenticeshipsForUkprnInExpectedOrder()
        {
            const long ukprn = 10005214L;
            var providerStandardArcheologistLev1 = new ProviderStandard { StandardId = 20, Title = "Archeologist", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3) };
            var providerStandardZebraWranglerShouldBeCutOffByProviderApprenticeshipsMaximum 
                = new ProviderStandard() {StandardId = 10, Title = "Zebra Wrangler", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3) };

            var providerStandardWithNoEffectiveFrom = new ProviderStandard { StandardId = 30, Title = "Absent because no effective from date", Level = 4, EffectiveFrom = null };


            var standards = new List<ProviderStandard>
            {
                providerStandardZebraWranglerShouldBeCutOffByProviderApprenticeshipsMaximum,
                providerStandardArcheologistLev1,
                providerStandardWithNoEffectiveFrom

            };

            var providerFrameworkAccountingLev3 = new ProviderFramework { FrameworkId = "321-1-1", PathwayName = "Accounting", Level = 3, EffectiveFrom = DateTime.Today.AddDays(-3) };
            var providerFrameworkAccountingLev2 = new ProviderFramework { FrameworkId = "321-2-1", PathwayName = "Accounting", Level = 2, EffectiveFrom = DateTime.Today.AddDays(-3), EffectiveTo = DateTime.Today.AddDays(2) };
            var providerFrameworkNoLongerActive = new ProviderFramework { FrameworkId = "234-3-2", PathwayName = "Active in the past", Level = 4, EffectiveFrom = DateTime.MinValue, EffectiveTo = DateTime.Today.AddDays(-2) };

            var frameworks = new List<ProviderFramework>
            {
                providerFrameworkAccountingLev3,
                providerFrameworkAccountingLev2,
                providerFrameworkNoLongerActive
            };

            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveFramework(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(true);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveFramework("234-3-2", It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(false);
            _mockGetProviders.Setup(x => x.GetStandardsByProviderUkprn(ukprn)).Returns(standards);
            _mockGetProviders.Setup(x => x.GetFrameworksByProviderUkprn(ukprn)).Returns(frameworks);

            var result = _sut.GetActiveApprenticeshipsByProvider(ukprn);
           var providerApprenticeships = result as ProviderApprenticeship[] ?? result.ToArray();
            Assert.AreEqual(ProviderApprenticeshipsMaximum, providerApprenticeships.Length);
            Assert.AreEqual(providerApprenticeships[0].Identifier, providerFrameworkAccountingLev2.FrameworkId, $"Expect first item to be Framework Id [{providerFrameworkAccountingLev2.FrameworkId}], but was [{providerApprenticeships[0].Identifier} ]");
            Assert.AreEqual(providerApprenticeships[1].Identifier, providerFrameworkAccountingLev3.FrameworkId);
            Assert.AreEqual(providerApprenticeships[2].Identifier, providerStandardArcheologistLev1.StandardId.ToString());
   }

        [Test]
        public void ShouldReturnListOfEmptyProvidersForAStandard()
        {
            var standardCode = 1;
            var data = new List<StandardProviderSearchResultsItem>();

            _mockGetProviders.Setup(x => x.GetProvidersByStandardId(It.IsAny<string>())).Returns(data);
            _mockGetProviders.Setup(x => x.GetProviderByUkprnList(It.IsAny<List<long>>())).Returns(new List<Provider>());
            _mockGetStandards.Setup(x => x.GetStandardById(It.IsAny<string>())).Returns(new Standard());

            var response = _sut.GetStandardProviders(standardCode.ToString());

            _mockGetProviders.Verify(x => x.GetProviderByUkprnList(new List<long>()), Times.Once);

            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Test]
        public void ShouldThrow404IfStandardIsMissing()
        {
            var standardCode = 1;

            TestDelegate action = () => _sut.GetStandardProviders(standardCode.ToString());

            var ex = Assert.Throws<HttpResponseException>(action);
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShouldReturnListOfUniqueProvidersForAFramework()
        {
            var frameworkId = 500;
            var data = new List<FrameworkProviderSearchResultsItem>
            {
                new FrameworkProviderSearchResultsItem { Ukprn = 10005214, FrameworkCode = frameworkId },
                new FrameworkProviderSearchResultsItem { Ukprn = 10005214, FrameworkCode = frameworkId },
                new FrameworkProviderSearchResultsItem { Ukprn = 10006214, FrameworkCode = frameworkId }
            };

            _mockGetProviders.Setup(x => x.GetProvidersByFrameworkId(It.IsAny<string>())).Returns(data);
            _mockGetProviders.Setup(x => x.GetProviderByUkprnList(It.IsAny<List<long>>())).Returns(new List<Provider>());
            _mockGetFrameworks.Setup(x => x.GetFrameworkById(frameworkId.ToString())).Returns(new Framework());

            _sut.GetFrameworkProviders(frameworkId.ToString());

            _mockGetProviders.Verify(x => x.GetProviderByUkprnList(new List<long> { 10005214L, 10006214L }), Times.Once);
        }

        [Test]
        public void ShouldReturnListOfEmptyProvidersForAFramework()
        {
            var frameworkId = "416-3-1";
            var data = new List<FrameworkProviderSearchResultsItem> { };

            _mockGetProviders.Setup(x => x.GetProvidersByFrameworkId(It.IsAny<string>())).Returns(data);
            _mockGetProviders.Setup(x => x.GetProviderByUkprnList(It.IsAny<List<long>>())).Returns(new List<Provider>());
            _mockGetFrameworks.Setup(x => x.GetFrameworkById(It.IsAny<string>())).Returns(new Framework());

            var response = _sut.GetFrameworkProviders(frameworkId);

            _mockGetProviders.Verify(x => x.GetProviderByUkprnList(new List<long> { }), Times.Once);

            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Test]
        public void ShouldThrow404IfFrameworkIsMissing()
        {
            var frameworkId = 500;

            TestDelegate action = () => _sut.GetFrameworkProviders(frameworkId.ToString());

            var ex = Assert.Throws<HttpResponseException>(action);
            Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        //[Test]
        //public void ShouldReturnListOfAllProvidersWithLocationForAStandard()
        //{
        //    var data = new List<StandardProviderSearchResultsItem>
        //    {
        //        new StandardProviderSearchResultsItem
        //        {
        //            Ukprn = 10004214, StandardCode = 1,
        //            TrainingLocations = new List<TrainingLocation>
        //            {
        //                new TrainingLocation { LocationId = 1234 },
        //                new TrainingLocation { LocationId = 2345 },
        //                new TrainingLocation { LocationId = 3456 }
        //            }
        //        },
        //        new StandardProviderSearchResultsItem
        //        {
        //            Ukprn = 10005214, StandardCode = 1,
        //            TrainingLocations = new List<TrainingLocation>
        //            {
        //                new TrainingLocation { LocationId = 4567 },
        //                new TrainingLocation { LocationId = 5678 }
        //            }
        //        },
        //        new StandardProviderSearchResultsItem
        //        {
        //            Ukprn = 10006214, StandardCode = 1,
        //            TrainingLocations = new List<TrainingLocation>
        //            {
        //                new TrainingLocation { LocationId = 6789 }
        //            }
        //        }
        //    };

        //    _mockGetProviders.Setup(x => x.GetProvidersByStandardId(It.IsAny<string>())).Returns(data);

        //    var response = _sut.GetStandardProviderLocations("1");

        //    response.Should().NotBeNull();
        //    response.Should().BeOfType<List<StandardProviderSearchResultsItem>>();
        //    response.Should().NotBeEmpty();
        //    response.Should().BeEquivalentTo(data);
        //    response.First().Should().NotBe(null);
        //    response.Count().Should().Be(3);
        //    response.FirstOrDefault(c => c.Ukprn == 10004214).TrainingLocations.Count().Should().Be(3);
        //}

        //[Test]
        //public void ShouldReturnListOfAllProvidersWithLocationForAFramework()
        //{
        //    var data = new List<FrameworkProviderSearchResultsItem>
        //    {
        //        new FrameworkProviderSearchResultsItem { Ukprn = 10005214, FrameworkCode = 500 },
        //        new FrameworkProviderSearchResultsItem { Ukprn = 10005214, FrameworkCode = 500 },
        //        new FrameworkProviderSearchResultsItem { Ukprn = 10006214, FrameworkCode = 500 }
        //    };

        //    _mockGetProviders.Setup(x => x.GetProvidersByFrameworkId(It.IsAny<string>())).Returns(data);

        //    var response = _sut.GetFrameworkProviderLocations("500");

        //    response.Should().NotBeNull();
        //    response.Should().BeOfType<List<FrameworkProviderSearchResultsItem>>();
        //    response.Should().NotBeEmpty();
        //    response.Should().BeEquivalentTo(data);
        //    response.First().Should().NotBe(null);
        //    response.Count().Should().Be(3);

        //}

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
        public void ShouldThrowExceptionWhenServiceisDown()
        {
            _mockGetProviders.Setup(
               x =>
                   x.GetAllProviders()).Throws<ApplicationException>();

            Assert.Throws<ApplicationException>(() => _sut.Head());
        }

        [Test]
        public void ShouldNotThrowExceptionWhenServiceisUp()
        {
            _mockGetProviders.Setup(
               x =>
                   x.GetAllProviders()).Returns(new List<ProviderSummary> { new ProviderSummary { Ukprn = 40120001 }, new ProviderSummary { Ukprn = 52140002 } });

            Assert.DoesNotThrow(() => _sut.Head());
        }
    }
}
