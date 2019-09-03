using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.HealthChecks
{
    public class ElasticsearchHealthCheck : IHealthCheck
    {
        private readonly IGetStandards _repository;

        public ElasticsearchHealthCheck(IGetStandards standardRepository)
        {
            _repository = standardRepository;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            try
            {
                var result = _repository.GetAllStandards();
            }
            catch
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Elasticsearch responded as expected."));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Elasticsearch didn't respond as expected."));
        }
    }
}