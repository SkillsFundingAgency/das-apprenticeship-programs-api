﻿using System.Configuration;
using System.Linq;

namespace Sfa.Das.ApprenticeshipInfoService.Health
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Elasticsearch;

    using Models;

    public class HealthService : IHealthService
    {
        private readonly IElasticsearchHealthService _elasticsearchHealthService;
        private readonly IAngleSharpService _angleSharpService;

        private readonly IHealthSettings _healthSettings;

        private readonly IHttpServer _httpServer;
        private readonly ISqlService _sqlService;

        public HealthService(
            IElasticsearchHealthService elasticsearchHealthService,
            IAngleSharpService angleSharpService,
            IHealthSettings healthSettings,
            IHttpServer httpServer,
            ISqlService sqlService)
        {
            _elasticsearchHealthService = elasticsearchHealthService;
            _angleSharpService = angleSharpService;
            _healthSettings = healthSettings;
            _httpServer = httpServer;
            _sqlService = sqlService;
        }

        public HealthModel CreateModel()
        {
            var timer = Stopwatch.StartNew();
            var elasticsearchModel = _elasticsearchHealthService.GetElasticHealth(_healthSettings.ElasticsearchUrls, _healthSettings.RequiredIndexAliases, _healthSettings.Environment);
            var elasticErrorLogs = _elasticsearchHealthService.GetErrorLogs(
                _healthSettings.ElasticsearchUrls,
                _healthSettings.Environment);

            var larsDownloadPageUrl = string.Concat(_healthSettings.LarsSiteRootUrl,
                _healthSettings.LarsSiteDownloadsPageUrl);

            var larsDownloadPageStatus = _httpServer.ResponseCode(larsDownloadPageUrl);
            var courseDirectoryStatus = _httpServer.ResponseCode(_healthSettings.CourseDirectoryUrl);

            var ukrlpStatus = _httpServer.ResponseCode(_healthSettings.UkrlpUrl);
            var fechoicesStatus = _sqlService.TestConnection(ConfigurationManager.AppSettings["AchievementRateDataBaseConnectionString"]);


            var model = new HealthModel
            {
                Status = Status.Green,
                Errors = new List<string>(),
                ElasticSearchAliases = elasticsearchModel.ElasticsearchAliases,
                CoreAliasesExistStatus = elasticsearchModel.RequiredAliasesExist ? Status.Green : Status.Red,
                ElasticsearchLog = elasticErrorLogs,
                LarsFilePageStatus = larsDownloadPageStatus,
                CourseDirectoryStatus = courseDirectoryStatus,
                FEChoices = fechoicesStatus,
                UkrlpStatus = ukrlpStatus
            };

            if (elasticsearchModel.Exception != null)
            {
                model.Status = Status.Red;
                model.Errors.Add(elasticsearchModel.Exception.Message);
            }

            if (elasticsearchModel.ElasticsearchAliases.Count < 2)
            {
                model.Status = Status.Red;
                model.Errors.Add($"Missing aliases / indices. Should be 2 but found {elasticsearchModel.ElasticsearchAliases.Count}");
            }

            if (model.LarsFilePageStatus != Status.Green)
            {
                model.Status = Status.Red;
                model.LarsZipFileStatus = Status.Red;
                model.Errors.Add("Cant access hub.imservices.org.uk (LARS)");
            }
            else
            {
                var links = _angleSharpService.GetLinks(larsDownloadPageUrl, "li a", "LARS CSV");
                var linkEndpoint = links?.FirstOrDefault();
                model.LarsZipFileStatus = _httpServer.ResponseCode(string.Concat(_healthSettings.LarsSiteRootUrl, linkEndpoint));
            }

            if (model.CourseDirectoryStatus != Status.Green)
            {
                model.Status = Status.Red;
                model.Errors.Add("Cant access Course Directory");
            }

            if (model.FEChoices != Status.Green)
            {
                model.Status = Status.Red;
                model.Errors.Add("Can't access FEChoices");
            }

            timer.Stop();
            model.Took = timer.ElapsedMilliseconds;

            return model;
        }
    }
}
