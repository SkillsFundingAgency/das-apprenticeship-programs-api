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
        public void GetProviderFrameworksByUkprnShouldLogWhenInvalidStatusCode()
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

            Assert.Throws<ApplicationException>(() => repo.GetFrameworksByProviderUkprn(1L));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProviderFrameworksByUkprnShouldWarnWhenMultipleProvidersAndReturnFirstProviderFrameworks()
        {
            var searchResponse = new Mock<ISearchResponse<Provider>>();
            var apiCall = new Mock<IApiCallDetails>();
            var providerFrameworks = new List<ProviderFramework>
            {
                new ProviderFramework
                {
                    EffectiveFrom = DateTime.Today,
                    FrameworkId = "321-3-1",
                    FworkCode = 321,
                    Level = 2,
                    PathwayName = "Test name",
                    ProgType = 3,
                    PwayCode = 1
                },
                new ProviderFramework
                {
                    EffectiveFrom = DateTime.Today,
                    FrameworkId = "322-3-1",
                    FworkCode = 322,
                    Level = 2,
                    PathwayName = "Test name 2",
                    ProgType = 3,
                    PwayCode = 1
                }
            };

            var provider = new Provider {Frameworks = providerFrameworks};
            var provider2 = new Provider();
            searchResponse.Setup(x => x.Documents).Returns(new List<Provider> { provider, provider2 });

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipsMaximum).Returns(2);
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<Provider>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

            var res = repo.GetFrameworksByProviderUkprn(1L);

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(providerFrameworks, res);
        }

        [Test]
        public void GetProviderStandardsByUkprnShouldLogWhenInvalidStatusCode()
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

            Assert.Throws<ApplicationException>(() => repo.GetStandardsByProviderUkprn(1L));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetProviderStandardsByUkprnShouldWarnWhenMultipleProvidersAndReturnFirstProviderStandards()
        {
            var searchResponse = new Mock<ISearchResponse<Provider>>();
            var apiCall = new Mock<IApiCallDetails>();
            var providerStandards = new List<ProviderStandard>
            {
                new ProviderStandard
                {
                    EffectiveFrom = DateTime.Today,
                    Level = 2,
                    Title = "Test name",
                    StandardId = 3
                },
                new ProviderStandard
                {
                    EffectiveFrom = DateTime.Today.AddDays(-1),
                    Level = 2,
                    Title = "Test name 2",
                    StandardId = 3
                }
            };

            var provider = new Provider { Standards = providerStandards };
            var provider2 = new Provider();
            searchResponse.Setup(x => x.Documents).Returns(new List<Provider> { provider, provider2 });

            var configurationSettings = new Mock<IConfigurationSettings>();
            configurationSettings.Setup(x => x.ProviderApprenticeshipsMaximum).Returns(2);
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.OK);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<Provider>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);
            var repo = new ProviderRepository(
                _elasticClient.Object,
                _log.Object,
                configurationSettings.Object,
                Mock.Of<IProviderLocationSearchProvider>(),
                Mock.Of<IProviderMapping>(),
                _queryHelper.Object);

            var res = repo.GetStandardsByProviderUkprn(1L);

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(providerStandards, res);
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