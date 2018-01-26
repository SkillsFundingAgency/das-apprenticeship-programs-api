using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Repositories
{
    [TestFixture]
    public class ProviderRepositoryTests
    {
        private Mock<IElasticsearchCustomClient> _elasticClient;

        private Mock<ILog> _log;

        private Mock<IQueryHelper> _queryHelper;

        [SetUp]
        public void Setup()
        {
            _elasticClient = new Mock<IElasticsearchCustomClient>();
            _log = new Mock<ILog>();
            _log.Setup(x => x.Warn(It.IsAny<string>())).Verifiable();
            _queryHelper = new Mock<IQueryHelper>();
            _queryHelper.Setup(x => x.GetProvidersByFrameworkTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersByStandardTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersTotalAmount()).Returns(1);
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
                _queryHelper.Object);

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
                _queryHelper.Object);

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
                _queryHelper.Object);

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
                _queryHelper.Object);

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

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipTrainingMaximum).Returns(2);
            apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

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

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipTrainingMaximum).Returns(10);
            apiCallForFrameworkDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworkDtos.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworkDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForFrameworks.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFrameworkDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworkDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderFramework>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForFrameworks.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

             var result = repo.GetFrameworksByProviderUkprn(ukprn);
              Assert.AreEqual(result.Count(), providerFrameworks.Count );
             _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Never);

        }

        //[Test]
        //public void GetProviderStandardsByUkprnShouldLogWhenInvalidStatusCode()
        //{
        //    var ukprn = 1L;
        //    var searchResponse = new Mock<ISearchResponse<ProviderStandardDto>>();
        //    var apiCall = new Mock<IApiCallDetails>();
        //    apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
        //    searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

        //    _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
        //    var repo = new ProviderRepository(
        //        _elasticClient.Object,
        //        _log.Object,
        //        Mock.Of<IConfigurationSettings>(),
        //        Mock.Of<IProviderLocationSearchProvider>(),
        //        Mock.Of<IProviderMapping>(),
        //        _queryHelper.Object);

        //    Assert.Throws<ApplicationException>(() => repo.GetStandardsByProviderUkprn(ukprn));

        //    _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider standards for ukprn [{ukprn}]"), Times.Once);

        //}

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

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipTrainingMaximum).Returns(2);
            apiCallForStandardDtos.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponseForStandardsDtos.SetupGet(x => x.ApiCall).Returns(apiCallForStandardDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponseForStandards.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForStandardsDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForStandards.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetStandardsByProviderUkprn(ukprn));
            _log.Verify(x => x.Warn($"httpStatusCode was {(int)HttpStatusCode.Ambiguous} when querying provider standards apprenticeship details for ukprn [{ukprn}]"), Times.Once);
        }

        [Test]
        public void GetProviderStandardsByUkprn()
        {
            var ukprn = 1L;
            var searchResponseForDtos = new Mock<ISearchResponse<ProviderStandardDto>>();
            var apiCallForDtos = new Mock<IApiCallDetails>();
            var searchResponse = new Mock<ISearchResponse<ProviderStandard>>();
            var apiCallForFrameworks = new Mock<IApiCallDetails>();

            searchResponseForDtos.Setup(x => x.Documents)
                .Returns(new List<ProviderStandardDto> {new ProviderStandardDto()});

            var providerStandards = new List<ProviderStandard>
            {
                new ProviderStandard { StandardId = 1 },
                new ProviderStandard { StandardId = 2 },
                new ProviderStandard { StandardId = 3 },
                new ProviderStandard { StandardId = 4 }
            };
            searchResponse.Setup(x => x.Documents).Returns(providerStandards);

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipTrainingMaximum).Returns(10);
            apiCallForDtos.SetupGet(x => x.HttpStatusCode).Returns((int) HttpStatusCode.OK);
            searchResponseForDtos.SetupGet(x => x.ApiCall).Returns(apiCallForDtos.Object);

            apiCallForFrameworks.SetupGet(x => x.HttpStatusCode).Returns((int) HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCallForFrameworks.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandardDto>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponseForDtos.Object);
            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<ProviderStandard>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

            var result = repo.GetStandardsByProviderUkprn(ukprn);
            Assert.AreEqual(result.Count(), providerStandards.Count);
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
                _queryHelper.Object);

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
                _queryHelper.Object);

            Assert.Throws<ApplicationException>(() => repo.GetProvidersByFrameworkId(string.Empty));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }
    }
}