using Nest;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.Querys
{
    public interface IProviderNameSearchProviderQuery
    {
        ISearchResponse<ProviderNameSearchResult> GetResults(string term, int take, int skip);
        long GetTotalMatches(string term);
    }
}