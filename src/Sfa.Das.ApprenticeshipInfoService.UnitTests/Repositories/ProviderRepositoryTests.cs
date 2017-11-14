using System;
using System.Collections.Generic;
using System.Net;
using Elasticsearch.Net;
using Moq;
using Nest;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
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
            System.Configuration.ConfigurationManager.AppSettings["FeatureToggle.RoatpProvidersFeature"] = "true";

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