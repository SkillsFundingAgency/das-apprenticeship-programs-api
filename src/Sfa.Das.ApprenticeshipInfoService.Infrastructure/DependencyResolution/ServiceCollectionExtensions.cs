using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.Querys;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.HealthChecks;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ApplicationSettings();
            configuration.GetSection("ApplicationSettings").Bind(settings);
            services.AddSingleton<IConfigurationSettings>(x => settings);
            
            services.AddSingleton<IElasticsearchClientFactory, ElasticsearchClientFactory>();
            services.AddScoped<IApprenticeshipSearchServiceV1, ApprenticeshipSearchServiceV1>();
            services.AddScoped<IApprenticeshipSearchServiceV3, ApprenticeshipSearchServiceV3>();
            services.AddScoped<IProviderSearchService, ProviderSearchService>();
            services.AddScoped<IGetStandards, StandardRepository>();
            services.AddScoped<IGetFrameworks, FrameworkRepository>();
            services.AddScoped<IGetProviders, ProviderRepository>();
            services.AddScoped<IGetAssessmentOrgs, AssessmentOrgsRepository>();
            services.AddScoped<IApprenticeshipProviderRepository, ApprenticeshipProviderRepository>();
            services.AddScoped<IApprenticeshipMapping, ApprenticeshipMapping>();
            services.AddScoped<IStandardMapping, StandardMapping>();
            services.AddScoped<IFrameworkMapping, FrameworkMapping>();
            services.AddScoped<IProviderMapping, ProviderMapping>();
            services.AddScoped<IAssessmentOrgsMapping, AssessmentOrgsMapping>();
            services.AddScoped<IProviderLocationSearchProvider, ElasticsearchProviderLocationSearchProvider>();
            services.AddScoped<IGetProviderApprenticeshipLocationsV3, ElasticsearchProviderLocationSearchProviderV3>();
            services.AddScoped<IElasticsearchCustomClient, ElasticsearchCustomClient>();
            services.AddScoped<IControllerHelper, ControllerHelper>();
            services.AddScoped<IQueryHelper, Elasticsearch.QueryHelper>();
            services.AddScoped<IActiveApprenticeshipChecker, ActiveApprenticeshipChecker>();
            services.AddScoped<IFundingCapCalculator, FundingCapCalculator>();
            services.AddScoped<IPaginationHelper, PaginationHelper>();
            services.AddScoped<IGetIfaStandardsUrlService, GetIfaStandardsUrlService>();
            services.AddScoped<IApprenticeshipSearchResultsMapping, ApprenticeshipSearchResultsMapping>();
            services.AddScoped<IProviderNameSearchServiceV3, ProviderNameSearchServiceV3>();
            services.AddScoped<IProviderNameSearchProviderQuery, ProviderNameSearchProviderQuery>();
            services.AddScoped<IProviderNameSearchMapping, ProviderNameSearchMapping>();

            services.AddHealthChecks()
                .AddRedis(configuration.GetConnectionString("Redis"), "redis-check")
                .AddElasticsearch(opt => 
                {
                    opt.UseServer(settings.ElasticServerUrls.First().ToString());
                    opt.UseBasicAuthentication(settings.ElasticsearchUsername, settings.ElasticsearchPassword);
                }, "elasticsearch-check")
                .AddCheck<ElasticsearchHealthCheck>("elasticsearch-query-check");
        }
    }
}