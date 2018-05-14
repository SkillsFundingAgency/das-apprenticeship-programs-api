using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Pagination;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Repositories
{
    [TestFixture]
    public class ProviderRepositoryTests
    {
        private const int PageSizeApprenticeshipSummary = 4;
        private Mock<IElasticsearchCustomClient> _elasticClient;

        private Mock<ILog> _log;

        private Mock<IQueryHelper> _queryHelper;
        private Mock<IActiveApprenticeshipChecker> _mockActiveFrameworkChecker;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;
        private Mock<IPaginationHelper> _mockPaginationHelper;

        [SetUp]
        public void Setup()
        {
            _elasticClient = new Mock<IElasticsearchCustomClient>();
            _log = new Mock<ILog>();
            _mockActiveFrameworkChecker = new Mock<IActiveApprenticeshipChecker>();
            _log.Setup(x => x.Warn(It.IsAny<string>())).Verifiable();
            _queryHelper = new Mock<IQueryHelper>();
            _queryHelper.Setup(x => x.GetProvidersByFrameworkTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersByStandardTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersTotalAmount()).Returns(1);
            _mockConfigurationSettings = new Mock<IConfigurationSettings>();

            _mockConfigurationSettings.Setup(x => x.PageSizeApprenticeshipSummary)
                .Returns(PageSizeApprenticeshipSummary);
            _mockPaginationHelper = new Mock<IPaginationHelper>();
        }

        [Test]
        public void GetAllProvidersShouldLogWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<Provider>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<Provider>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetAllProviders());

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProviderByUkprnShouldLogWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<Provider>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<Provider>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetProviderByUkprn(1L));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProviderByUkprnListShouldLogWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<Provider>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<Provider>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetProviderByUkprnList(new List<long> { 1L }));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProviderFrameworksByUkprnShouldLogWhenFrameworkDetailsNotAsExpected()
        {
            var ukprn = 1L;
            var searchResponse = new Mock<ISearchResponse<ProviderFrameworkDto>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetFrameworksByProviderUkprn(ukprn));

            _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider frameworks for ukprn [{ukprn}]"), Times.Once);
        }

        [Test]
        public void GetProviderFrameworksByUkprnShouldLogWhenApprenticeshipDetailsDoNotRespondAsExpected()
        {
            var ukprn = 1L;
            var searchResponseForFrameworkDtos = new Mock<ISearchResponse<ProviderFrameworkDto>>();
            var apiCallForFrameworkDtos = new Mock<IApiCallDetails>();
            var searchResponseForFrameworks = new Mock<ISearchResponse<ProviderFramework>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();

            var frameworkDto1 = new ProviderFrameworkDto {FrameworkId = "321-3-1", Ukprn = ukprn.ToString() };
            var frameworkDto2 = new ProviderFrameworkDto { FrameworkId = "322-3-1", Ukprn = ukprn.ToString() };

            searchResponseForFrameworkDtos.Setup(x => x.Documents).Returns(new List<ProviderFrameworkDto> { frameworkDto1, frameworkDto2 });

         apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetFrameworksByProviderUkprn(ukprn));
            _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider frameworks apprenticeship details for ukprn [{ukprn}]"), Times.Once);
        }

        [Test]
        public void GetProviderFrameworksByUkprn()
        {
            var ukprn = 1L;
            var searchResponseForFrameworkDtos = new Mock<ISearchResponse<ProviderFrameworkDto>>();
            var apiCallForFrameworkDtos = new Mock<IApiCallDetails>();
            var searchResponseForFrameworks = new Mock<ISearchResponse<ProviderFramework>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();

            searchResponseForFrameworkDtos.Setup(x => x.Documents).Returns(new List<ProviderFrameworkDto> {new ProviderFrameworkDto() });

            var providerFrameworks = new List<ProviderFramework>
            {
                new ProviderFramework {FrameworkId = "321-3-1" },
                new ProviderFramework { FrameworkId = "322-3-1" },
                new ProviderFramework { FrameworkId = "323-3-1" },
            };

            searchResponseForFrameworks.Setup(x => x.Documents).Returns(providerFrameworks);

            apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

             var result = repo.GetFrameworksByProviderUkprn(ukprn);
              Assert.AreEqual(result.Count(), providerFrameworks.Count );
             _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Never);

        }

        [Test]
        public void GetProviderStandardsByUkprnShouldLogWhenInvalidStatusCode()
        {
            var ukprn = 1L;
            var searchResponse = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetStandardsByProviderUkprn(ukprn));

            _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider standards for ukprn [{ukprn}]"), Times.Once);

        }
        
        [Test]
        public void GetProviderStandardsByUkprnShouldLogWhenApprenticeshipDetailsDoNotRespondAsExpected()
        {
            var ukprn = 1L;
            var searchResponseForStandardsDtos = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCallForStandardDtos = new Mock<IApiCallDetails>();
            var searchResponseForStandards = new Mock<ISearchResponse<ProviderStandard>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();

            var standardDto1 = new ProviderStandardDto { StandardCode = 1, Ukprn = ukprn.ToString() };
            var standardDto2 = new ProviderStandardDto { StandardCode = 2, Ukprn = ukprn.ToString() };

            searchResponseForStandardsDtos.Setup(x => x.Documents).Returns(new List<ProviderStandardDto> { standardDto1, standardDto2 });

            apiCallForStandardDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForStandardsDtos.SetupGet(x => x.ApiCall).Returns(apiCallForStandardDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponseForStandards.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForStandardsDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForStandards.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetStandardsByProviderUkprn(ukprn));
            _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider standards apprenticeship details for ukprn [{ukprn}]"), Times.Once);
        }

        [Test]
        public void GetProviderStandardsByUkprn()
        {
            var ukprn = 1L;
            var numberReturnedActiveAndPublished = 4;
            var searchResponseForDtos = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCallForDtos = new Mock<IApiCallDetails>();
            var searchResponse = new Mock<ISearchResponse<ProviderStandard>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();

            searchResponseForDtos.Setup(x => x.Documents)
                .Returns(new List<ProviderStandardDto> {new ProviderStandardDto()});

            var providerStandards = new List<ProviderStandard>
            {
                new ProviderStandard { StandardId = 1, Published = true},
                new ProviderStandard { StandardId = 2, Published = true},
                new ProviderStandard { StandardId = 3, Published = true},
                new ProviderStandard { StandardId = 4, Published = true},
                new ProviderStandard { StandardId = 5, Published = false}
            };
            searchResponse.Setup(x => x.Documents).Returns(providerStandards);

            apiCallForDtos.SetupGet(x => x.HttpStatusCode).Returns((int) HttpStatusCode.OK);
            searchResponseForDtos.SetupGet(x => x.ApiCall).Returns(apiCallForDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int) HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            var result = repo.GetStandardsByProviderUkprn(ukprn);

            Assert.AreEqual(numberReturnedActiveAndPublished, result.Count());
            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetProvidersByStandardIdShouldLogWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<StandardProviderSearchResultsItem>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<StandardProviderSearchResultsItem>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetProvidersByStandardId(string.Empty));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProvidersByFrameworkIdShouldLogWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<FrameworkProviderSearchResultsItem>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<FrameworkProviderSearchResultsItem>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                _mockPaginationHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetProvidersByFrameworkId(string.Empty));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ShouldReturnActiveListOfProviderApprenticeshipsForUkprnInExpectedOrder()
        {
            const long ukprn = 10005214L;
            var searchResponseForDtos = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCallForStandards = new Mock<IApiCallDetails>();
            var apiCallForDtos = new Mock<IApiCallDetails>();
            var searchResponse = new Mock<ISearchResponse<ProviderStandard>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();
            var searchResponseForFrameworkDtos = new Mock<ISearchResponse<ProviderFrameworkDto>>();
            var apiCallForFrameworkDtos = new Mock<IApiCallDetails>();
            var searchResponseForFrameworks = new Mock<ISearchResponse<ProviderFramework>>();    

            searchResponseForDtos.Setup(x => x.Documents).Returns(new List<ProviderStandardDto> { new ProviderStandardDto() });
            searchResponseForFrameworkDtos.Setup(x => x.Documents).Returns(new List<ProviderFrameworkDto>());

            apiCallForDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForDtos.SetupGet(x => x.ApiCall).Returns(apiCallForDtos.Object);
            apiCallForStandards.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCallForStandards.Object);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            var providerStandardArcheologistLev1 = new ProviderStandard { StandardId = 20, Title = "Archeologist", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3), Published = true};
            var providerStandardZebraWranglerShouldBeCutOffByProviderApprenticeshipTrainingMaximum
                = new ProviderStandard() { StandardId = 10, Title = "Zebra Wrangler", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3), Published = true};

            var providerStandardWithNoEffectiveFrom = new ProviderStandard { StandardId = 30, Title = "Absent because no effective from date", Level = 4, EffectiveFrom = null, Published = true };

            var providerStandardNotPublished = new ProviderStandard { StandardId = 31, Title = "Absent because not published", Level = 4, EffectiveFrom = DateTime.Today.AddDays(-3), Published = false };

            var standards = new List<ProviderStandard>
            {
                providerStandardZebraWranglerShouldBeCutOffByProviderApprenticeshipTrainingMaximum,
                providerStandardArcheologistLev1,
                providerStandardWithNoEffectiveFrom,
                providerStandardNotPublished

            };

            searchResponse.Setup(x => x.Documents).Returns(standards);

            var providerFrameworkAccountingLev3 = new ProviderFramework { FrameworkId = "321-1-1", PathwayName = "Accounting", Level = 3, EffectiveFrom = DateTime.Today.AddDays(-3) };
            var providerFrameworkAccountingLev2 = new ProviderFramework { FrameworkId = "321-2-1", PathwayName = "Accounting", Level = 2, EffectiveFrom = DateTime.Today.AddDays(-3), EffectiveTo = DateTime.Today.AddDays(2) };
            var providerFrameworkNoLongerActive = new ProviderFramework { FrameworkId = "234-3-2", PathwayName = "Active in the past", Level = 4, EffectiveFrom = DateTime.MinValue, EffectiveTo = DateTime.Today.AddDays(-2) };

            var frameworks = new List<ProviderFramework>
            {
                providerFrameworkAccountingLev3,
                providerFrameworkAccountingLev2,
                providerFrameworkNoLongerActive
            };
        searchResponseForFrameworks.Setup(x => x.Documents).Returns(frameworks);

            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveFramework(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(true);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveFramework("234-3-2", It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(false);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveStandard(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(true);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveStandard(It.IsAny<string>(), null, It.IsAny<DateTime?>()))
                .Returns(false);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);

            var paginationDetails = new PaginationDetails
            {
                LastPage = 1,
                NumberOfRecordsToSkip = 0,
                NumberPerPage = 20,
                Page = 0,
                TotalCount = 3
            };

            _mockPaginationHelper
                .Setup(x => x.GeneratePaginationDetails(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(paginationDetails);

            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                _mockConfigurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                new PaginationHelper());

            var result = repo.GetActiveApprenticeshipTrainingByProvider(ukprn, 1);

            var providerApprenticeships = result.ApprenticeshipTrainingItems.ToArray();
            Assert.AreEqual(PageSizeApprenticeshipSummary, providerApprenticeships.Length);
            Assert.AreEqual(4, result.PaginationDetails.TotalCount);
            Assert.AreEqual(providerApprenticeships[0].Identifier, providerFrameworkAccountingLev2.FrameworkId,
                $"Expect first item to be Framework Id [{providerFrameworkAccountingLev2.FrameworkId}], but was [{providerApprenticeships[0].Identifier} ]");
            Assert.AreEqual(providerApprenticeships[1].Identifier, providerFrameworkAccountingLev3.FrameworkId);
            Assert.AreEqual(providerApprenticeships[1].TrainingType, ApprenticeshipTrainingType.Framework);
            Assert.AreEqual(providerApprenticeships[1].Type, "Framework");
            Assert.AreEqual(providerApprenticeships[1].Level, 3);
            Assert.AreEqual(providerApprenticeships[1].Name, "Accounting");
            Assert.AreEqual(providerApprenticeships[2].Identifier, providerStandardArcheologistLev1.StandardId.ToString());
            Assert.AreEqual(providerApprenticeships[2].TrainingType, ApprenticeshipTrainingType.Standard);
            Assert.AreEqual(providerApprenticeships[2].Type, "Standard");
            Assert.AreEqual(providerApprenticeships[2].Level, 1);
            Assert.AreEqual(providerApprenticeships[2].Name, "Archeologist");

        }

        [Test]
        public void ShouldReturnActiveListOfProviderApprenticeshipsForUkprnInExpectedOrderSecondPage()
        {
            const long ukprn = 10005214L;
            var searchResponseForDtos = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCallForStandards = new Mock<IApiCallDetails>();
            var apiCallForDtos = new Mock<IApiCallDetails>();
            var searchResponse = new Mock<ISearchResponse<ProviderStandard>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();
            var searchResponseForFrameworkDtos = new Mock<ISearchResponse<ProviderFrameworkDto>>();
            var apiCallForFrameworkDtos = new Mock<IApiCallDetails>();
            var searchResponseForFrameworks = new Mock<ISearchResponse<ProviderFramework>>();

            searchResponseForDtos.Setup(x => x.Documents).Returns(new List<ProviderStandardDto> { new ProviderStandardDto() });
            searchResponseForFrameworkDtos.Setup(x => x.Documents).Returns(new List<ProviderFrameworkDto>());

            apiCallForDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForDtos.SetupGet(x => x.ApiCall).Returns(apiCallForDtos.Object);
            apiCallForStandards.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCallForStandards.Object);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            var providerStandardArcheologistEntry4 = new ProviderStandard { StandardId = 20, Title = "Archeologist", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3), Published = true };
            var providerStandardArcheologistEntry5 = new ProviderStandard { StandardId = 21, Title = "Archeologist", Level = 2, EffectiveFrom = DateTime.Today.AddDays(-3), Published = true };
            var providerStandardArcheologistEntry6 = new ProviderStandard { StandardId = 22, Title = "Archeologist", Level = 3, EffectiveFrom = DateTime.Today.AddDays(-3), Published = true };

            var standards = new List<ProviderStandard>  
            {
                providerStandardArcheologistEntry4,
                providerStandardArcheologistEntry5,
                providerStandardArcheologistEntry6

            };

            searchResponse.Setup(x => x.Documents).Returns(standards);

            var frameworkAccountingEntry2 = new ProviderFramework { FrameworkId = "321-1-1", PathwayName = "Accounting", Level = 3, EffectiveFrom = DateTime.Today.AddDays(-3) };
            var frameworkAccountingEntry1 = new ProviderFramework { FrameworkId = "321-2-1", PathwayName = "Accounting", Level = 2, EffectiveFrom = DateTime.Today.AddDays(-3), EffectiveTo = DateTime.Today.AddDays(2) };
            var frameworkAccountingEntry3 = new ProviderFramework { FrameworkId = "234-3-2", PathwayName = "Accounting", Level = 4, EffectiveFrom = DateTime.Today.AddDays(-3), EffectiveTo = DateTime.Today.AddDays(3) };
            var standardZebraWranglerEntry7 = new ProviderFramework() { FrameworkId = "235-4-1", PathwayName = "Zebra Wrangler", Level = 1, EffectiveFrom = DateTime.Today.AddDays(-3) };

            var frameworks = new List<ProviderFramework>
            {
                frameworkAccountingEntry2,
                frameworkAccountingEntry3,
                frameworkAccountingEntry1,
                standardZebraWranglerEntry7
            };
            searchResponseForFrameworks.Setup(x => x.Documents).Returns(frameworks);

            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveFramework(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(true);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveStandard(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(true);
            _mockActiveFrameworkChecker
                .Setup(x => x.CheckActiveStandard(It.IsAny<string>(), null, It.IsAny<DateTime?>()))
                .Returns(false);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);

            var paginationDetails = new PaginationDetails
            {
                LastPage = 1,
                NumberOfRecordsToSkip = 0,
                NumberPerPage = 20,
                Page = 0,
                TotalCount = 3
            };

            _mockPaginationHelper
                .Setup(x => x.GeneratePaginationDetails(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(paginationDetails);

            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                _mockConfigurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object,
                _mockActiveFrameworkChecker.Object,
                new PaginationHelper());

            var result = repo.GetActiveApprenticeshipTrainingByProvider(ukprn, 2);

            var providerApprenticeships = result.ApprenticeshipTrainingItems.ToArray();
            Assert.AreEqual(3, providerApprenticeships.Length);

            Assert.AreEqual(providerApprenticeships[0].Identifier, providerStandardArcheologistEntry5.StandardId.ToString(),
                $"Expect first item to be Standard Id [{providerStandardArcheologistEntry5.StandardId}], but was [{providerApprenticeships[0].Identifier}]");
            Assert.AreEqual(providerApprenticeships[0].TrainingType, ApprenticeshipTrainingType.Standard);
            Assert.AreEqual(providerApprenticeships[0].Type, "Standard");
            Assert.AreEqual(providerApprenticeships[0].Level, 2);
            Assert.AreEqual(providerApprenticeships[0].Name, "Archeologist");
            Assert.AreEqual(providerApprenticeships[1].Identifier, providerStandardArcheologistEntry6.StandardId.ToString(),
                $"Expect first item to be Standard Id [{providerStandardArcheologistEntry6.StandardId}], but was [{providerApprenticeships[1].Identifier}]");
            Assert.AreEqual(providerApprenticeships[2].Identifier, standardZebraWranglerEntry7.FrameworkId,
                $"Expect first item to be Framework Id [{standardZebraWranglerEntry7.FrameworkId}], but was [{providerApprenticeships[2].Identifier}]");
        }
    }
}