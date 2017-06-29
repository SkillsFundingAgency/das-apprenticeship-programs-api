using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Health.Elasticsearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Elasticsearch.Net;

    using Nest;
    
    using Models;

    using Health.Models;

    public class ElasticsearchHealthHealthService : IElasticsearchHealthService
    {
        private readonly ILog _logger;

        public ElasticsearchHealthHealthService(ILog logger)
        {
            _logger = logger;
        }

        public ElasticsearchLog GetErrorLogs(IEnumerable<Uri> uriStrings, string environment)
        {
            using (var connectionSettings = new ConnectionSettings(new StaticConnectionPool(uriStrings)))
            {
                connectionSettings.DisableDirectStreaming();
                connectionSettings.InferMappingFor<ElasticsearchLogHit>(i => i
                    .Rename(p => p.Timestamp, "@timestamp")
                    );

                var elasticClient = new ElasticClient(connectionSettings);

                var oneDayAgo = DateMath.Now.Subtract(new Time(new TimeSpan(1, 0, 0, 0)));
                var result = elasticClient.Search<ElasticsearchLogHit>(
                    s => s
                        .Index("logstash-*")
                        .Type(Types.All)
                        .Query(q => q
                            .Bool(b => b
                                .Must(m => m
                                    .Match(ma => ma
                                        .Field("Application")
                                        .Query("search-index"))
                                    , bs2 => bs2
                                        .Match(ma => ma
                                            .Field(f => f.Level)
                                            .Query("error"))
                                    , bs2 => bs2
                                        .Match(ma => ma
                                            .Field("Environment")
                                            .Query(environment))
                                    , bs2 => bs2
                                        .DateRange(r => r.GreaterThan(oneDayAgo).Field("@timestamp")))
                            )
                        )
                    );
                return new ElasticsearchLog
                {
                    LogErrors = result.Hits.Select(m => $"{m.Source.Timestamp} - {m.Source.Message}"),
                    ErrorCount = result.Total
                };
            }
        }

        public ElasticsearchResponse GetElasticHealth(IEnumerable<Uri> uris, IEnumerable<string> requiredAliases, string environment)
        {
            try
            {
                using (var connectionSettings = new ConnectionSettings(new StaticConnectionPool(uris)))
                {
                    var elasticClient = new ElasticClient(connectionSettings);
                    var catAliases = elasticClient
                        .CatAliases()
                        .Records
                        .Where(m => m.Alias.StartsWith(environment)).ToList();

                    var rawAliases = catAliases.Select(al => al.Alias.Replace(environment, string.Empty));

                    var aliases = new List<ElasticsearchAlias>();

                    foreach (var catAlias in catAliases)
                    {
                        var index = elasticClient.CatIndices(m => m.Index(catAlias.Index));

                        aliases.Add(new ElasticsearchAlias
                        {
                            AliasName = catAlias.Alias,
                            IndexName = catAlias.Index,
                            Status = index.Records.FirstOrDefault()?.Status,
                            DocumentCount = index.Records.FirstOrDefault()?.DocsCount,
                            Health = index.Records.FirstOrDefault()?.Health
                        });
                    }
                    return new ElasticsearchResponse
                    {
                        RequiredAliasesExist = requiredAliases.All(al => rawAliases.Contains(al)),
                        ElasticsearchAliases = aliases
                    };
                }
            }
            catch (Exception exception)
            {
                _logger.Warn(exception, "Not able to get aliases");

                return new ElasticsearchResponse { ElasticsearchAliases = new List<ElasticsearchAlias>(), Exception = exception };
            }
        }
    }
}