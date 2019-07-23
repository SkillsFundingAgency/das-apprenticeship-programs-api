using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public interface IProviderNameSearchProviderQuery
    {
        ISearchResponse<ProviderNameSearchResult> GetResults(string term, int take, int skip);
        long GetTotalMatches(string term);
    }
}