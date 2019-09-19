using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Pagination;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using Api.Controllers;
    using Core.Helpers;
    using Core.Models;
    using Core.Models.Responses;
    using Core.Services;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Sfa.Das.ApprenticeshipInfoService.UnitTests.Helpers;
    using SFA.DAS.Apprenticeships.Api.Types.Providers;

    [TestFixture]
    public class ProviderControllerTests
    {
        private ProvidersController _sut;
        private Mock<IGetProviders> _mockGetProviders;
        private Mock<IControllerHelper> _mockControllerHelper;
        private Mock<IGetStandards> _mockGetStandards;
        private Mock<IGetFrameworks> _mockGetFrameworks;
        private Mock<IUrlHelper> _mockUrlHelper;

        [SetUp]
        public void Init()
        {
            _mockGetProviders = new Mock<IGetProviders>();
            _mockControllerHelper = new Mock<IControllerHelper>();
            _mockGetStandards = new Mock<IGetStandards>();
            _mockGetFrameworks = new Mock<IGetFrameworks>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockUrlHelper.Setup(x => x.Link("GetProviderByUkprn", It.IsAny<object>())).Returns<string, dynamic>((a, b) => { var o = DynamicObjectHelper.ToExpandoObject(b); return $"http://localhost/providers/{o.ukprn}"; });

            _sut = new ProvidersController(
                _mockGetProviders.Object,
                _mockControllerHelper.Object,
                _mockGetStandards.Object,
                _mockGetFrameworks.Object,
                Mock.Of<IApprenticeshipProviderRepository>()
                );

            _sut.Url = _mockUrlHelper.Object;
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

            actual.Value.Should().BeEquivalentTo(expected);
            actual.Value.Uri.Should().Be($"http://localhost/providers/{ukprn}");
        }

        [Test]
        public void ShouldReturnProvidersNotFound()
        {
            var result = _sut.Get(12345679);
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }

        [Test]
        public void AnInvalidUkprnShouldReturnABadRequest()
        {
            var result = _sut.Get(123456);
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
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

            response.Value.Should().NotBeNull();
            response.Value.Should().BeOfType<List<StandardProviderSearchResultsItemResponse>>();
            response.Value.Should().NotBeEmpty();
            response.Value.Should().BeEquivalentTo(expected);
            response.Value.First().Should().NotBe(null);
            response.Value.First().Should().Be(element);
            response.Value.First().ProviderName.Should().Be(element.ProviderName);
            response.Value.First().LegalName.Should().Be(element.LegalName);
            response.Value.First().ApprenticeshipInfoUrl.Should().Be(element.ApprenticeshipInfoUrl);
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

            response.Value.Should().NotBeNull();
            response.Value.Should().BeOfType<List<FrameworkProviderSearchResultsItemResponse>>();
            response.Value.Should().NotBeEmpty();
            response.Value.Should().BeEquivalentTo(expected);
            response.Value.First().Should().NotBe(null);
            response.Value.First().Should().Be(element);
            response.Value.First().ProviderName.Should().Be(element.ProviderName);
            response.Value.First().LegalName.Should().Be(element.LegalName);
            response.Value.First().ApprenticeshipInfoUrl.Should().Be(element.ApprenticeshipInfoUrl);
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
        public void ShouldReturnActiveListOfProviderApprenticeshipsForUkprn()
        {
            const long ukprn = 10005214L;

            const int totalCount = 400;
            var apprenticeshipTraining = new ApprenticeshipTraining
            {
                Identifier = "321-1-1",
                Name = "Archeologist",
                TrainingType = ApprenticeshipTrainingType.Framework,
                Level = 3,
                Type = "Framework"
            };

            var apprenticeshipTrainingList = new List<ApprenticeshipTraining>
            {
                apprenticeshipTraining
            };

            const int numberPerPage = 20;
            const int numberReturned = 1;
            const int page = 2;
            const int lastPage = 37;
            var paginationDetails = new PaginationDetails {NumberPerPage = 20, Page = page, TotalCount = totalCount, LastPage = lastPage};

        var expected = new ApprenticeshipTrainingSummary
            {
                ApprenticeshipTrainingItems = apprenticeshipTrainingList,
                PaginationDetails = paginationDetails,
                Ukprn = ukprn
            };

            _mockGetProviders.Setup(
                x =>
                    x.GetActiveApprenticeshipTrainingByProvider(ukprn, 1)).Returns(expected);

           var result = _sut.GetActiveApprenticeshipTrainingByProvider(ukprn);
           var providerApprenticeships = result.Value.ApprenticeshipTrainingItems.ToArray();
           Assert.AreEqual(apprenticeshipTrainingList.Count, providerApprenticeships.Length);
           Assert.AreEqual(totalCount, result.Value.PaginationDetails.TotalCount);
           Assert.AreEqual(numberPerPage, result.Value.PaginationDetails.NumberPerPage);
           Assert.AreEqual(numberReturned, result.Value.ApprenticeshipTrainingItems.Count());
           Assert.AreEqual(page, result.Value.PaginationDetails.Page);
           Assert.AreEqual(lastPage, result.Value.PaginationDetails.LastPage);
           Assert.AreEqual(providerApprenticeships[0].Identifier, apprenticeshipTraining.Identifier);
        }

        [Test]
        public void ShouldReturnBadRequestIfRequestingActiveApprenticeshipsWithBadlyFormedUkprn()
        {
            var result = _sut.GetActiveApprenticeshipTrainingByProvider(1);
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
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

            response.Value.Should().NotBeNull();
            response.Value.Should().BeEmpty();
        }

        [Test]
        public void ShouldThrow404IfStandardIsMissing()
        {
            var standardCode = 1;

            var result = _sut.GetStandardProviders(standardCode.ToString());

            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
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

            response.Value.Should().NotBeNull();
            response.Value.Should().BeEmpty();
        }

        [Test]
        public void ShouldThrow404IfFrameworkIsMissing()
        {
            var frameworkId = 500;

            var result = _sut.GetFrameworkProviders(frameworkId.ToString());

            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
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

            var result = _sut.GetByStandardIdAndLocation(1, null, null, 1);

            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Test]
        public void ShouldThrowExceptionIfLatLonIsNullSearchingByFrameworkId()
        {
            _mockControllerHelper.Setup(x => x.GetActualPage(It.IsAny<int>())).Returns(1);

            var result = _sut.GetByFrameworkIdAndLocation(1, null, null, 1);

            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
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
