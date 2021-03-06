﻿using System;
using System.Net;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Nest;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Repositories
{
    [TestFixture]
    public class ApprenticeshipProviderRepositoryTests
    {
        private Mock<IQueryHelper> _queryHelper;

        private Mock<ILogger<ApprenticeshipProviderRepository>> _log;

        private Mock<IElasticsearchCustomClient> _elasticClient;

        [SetUp]
        public void Setup()
        {
            _elasticClient = new Mock<IElasticsearchCustomClient>();
            _log = new Mock<ILogger<ApprenticeshipProviderRepository>>();
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
            _log.Verify(x => x.Log(Microsoft.Extensions.Logging.LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

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

            _log.Verify(x => x.Log(Microsoft.Extensions.Logging.LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);

        }
    }
}