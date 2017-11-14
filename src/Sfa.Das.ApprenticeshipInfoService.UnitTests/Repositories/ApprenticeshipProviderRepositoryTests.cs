using System;
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
    public class ApprenticeshipProviderRepositoryTests
    {
        private Mock<IQueryHelper> _queryHelper;

        private Mock<ILog> _log;

        private Mock<IElasticsearchCustomClient> _elasticClient;

        [SetUp]
        public void Setup()
        {
            System.Configuration.ConfigurationManager.AppSettings["FeatureToggle.RoatpProvidersFeature"] = "true";

            _elasticClient = new Mock<IElasticsearchCustomClient>();
            _log = new Mock<ILog>();
            _log.Setup(x => x.Warn(It.IsAny<string>())).Verifiable();
            _log.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>())).Verifiable();
            _queryHelper = new Mock<IQueryHelper>();
            _queryHelper.Setup(x => x.GetProvidersByFrameworkTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersByStandardTotalAmount(It.IsAny<string>())).Returns(1);
            _queryHelper.Setup(x => x.GetProvidersTotalAmount()).Returns(1);
        }

        [Test]
        public void GetCourseByStandardCodeLogsWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<StandardProviderSearchResultsItem>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<StandardProviderSearchResultsItem>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);

            var repo = new ApprenticeshipProviderRepository(
                _elasticClient.Object, 
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderMapping>());

            Assert.Throws<ApplicationException>(() => repo.GetCourseByStandardCode(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
            _log.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetCourseByFrameworkIdLogsWhenInvalidStatusCode()
        {
            var searchResponse = new Mock<ISearchResponse<FrameworkProviderSearchResultsItem>>();
            var apiCall = new Mock<IApiCallDetails>();
            apiCall.SetupGet(x => x.HttpStatusCode).Returns((int)HttpStatusCode.Ambiguous);
            searchResponse.SetupGet(x => x.ApiCall).Returns(apiCall.Object);

            _elasticClient.Setup(x => x.Search(It.IsAny<Func<SearchDescriptor<FrameworkProviderSearchResultsItem>, ISearchRequest>>(), It.IsAny<string>())).Returns(searchResponse.Object);

            var repo = new ApprenticeshipProviderRepository(
                _elasticClient.Object,
                _log.Object,
                Mock.Of<IConfigurationSettings>(),
                Mock.Of<IProviderMapping>());

            Assert.Throws<ApplicationException>(() => repo.GetCourseByFrameworkId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));

            _log.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
            _log.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}