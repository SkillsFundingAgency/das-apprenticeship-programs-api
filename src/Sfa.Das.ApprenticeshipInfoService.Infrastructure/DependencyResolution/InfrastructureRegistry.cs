using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.PostCodeIo;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.DependencyResolution
{
    using Core.Configuration;
    using Core.Helpers;
    using Core.Services;
    using Elasticsearch;
    using Helpers;
    using Mapping;
    using Settings;
    using StructureMap;

    public sealed class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, x.GetInstance<IRequestContext>(), GetProperties())).AlwaysUnique();
            For<IConfigurationSettings>().Use<ApplicationSettings>();
            For<IElasticsearchClientFactory>().Use<ElasticsearchClientFactory>();
            For<IApprenticeshipSearchService>().Use<ApprenticeshipSearchService>();
            For<IProviderSearchService>().Use<ProviderSearchService>();
            For<IGetStandards>().Use<StandardRepository>();
            For<IGetFrameworks>().Use<FrameworkRepository>();
            For<IGetProviders>().Use<ProviderRepository>();
            For<IGetAssessmentOrgs>().Use<AssessmentOrgsRepository>();
            For<IApprenticeshipProviderRepository>().Use<ApprenticeshipProviderRepository>();
            For<IApprenticeshipMapping>().Use<ApprenticeshipMapping>();
            For<IStandardMapping>().Use<StandardMapping>();
            For<IFrameworkMapping>().Use<FrameworkMapping>();
            For<IProviderMapping>().Use<ProviderMapping>();
            For<IAssessmentOrgsMapping>().Use<AssessmentOrgsMapping>();
            For<IProviderLocationSearchProvider>().Use<ElasticsearchProviderLocationSearchProvider>();
            For<IElasticsearchCustomClient>().Use<ElasticsearchCustomClient>();
            For<IControllerHelper>().Use<ControllerHelper>();
            For<IAnalyticsService>().Use<AnalyticsService>();
            For<IQueryHelper>().Use<QueryHelper>();
            For<IActiveApprenticeshipChecker>().Use<ActiveApprenticeshipChecker>();
            For<IFundingCapCalculator>().Use<FundingCapCalculator>();
            For<IPaginationHelper>().Use<PaginationHelper>();
            For<IGetIfaStandardsUrlService>().Use<GetIfaStandardsUrlService>();
            For<IHttpClient>().Use<HttpClient>();
            For<IPostCodeIoLocator>().Use<PostCodeIoLocator>();
        }

        private IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Version", GetVersion());
            return properties;
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}