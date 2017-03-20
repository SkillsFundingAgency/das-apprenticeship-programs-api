﻿using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.DependencyResolution
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
    using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Core.Logging;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Logging;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
    using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings;
    using StructureMap;

    public sealed class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, x.GetInstance<IConfigurationSettings>(), x.GetInstance<IRequestContext>())).AlwaysUnique();
            For<IConfigurationSettings>().Use<ApplicationSettings>();
            For<IElasticsearchClientFactory>().Use<ElasticsearchClientFactory>();
            For<IGetStandards>().Use<StandardRepository>();
            For<IGetFrameworks>().Use<FrameworkRepository>();
            For<IGetProviders>().Use<ProviderRepository>();
            For<IGetAssessmentOrgs>().Use<AssessmentOrgsRepository>();
            For<IApprenticeshipProviderRepository>().Use<ApprenticeshipProviderRepository>();
            For<IStandardMapping>().Use<StandardMapping>();
            For<IFrameworkMapping>().Use<FrameworkMapping>();
            For<IProviderMapping>().Use<ProviderMapping>();
            For<IAssessmentOrgsMapping>().Use<AssessmentOrgsMapping>();
            For<IProviderLocationSearchProvider>().Use<ElasticsearchProviderLocationSearchProvider>();
            For<IElasticsearchCustomClient>().Use<ElasticsearchCustomClient>();
            For<IControllerHelper>().Use<ControllerHelper>();
            For<IAnalyticsService>().Use<AnalyticsService>();
        }
    }
}
