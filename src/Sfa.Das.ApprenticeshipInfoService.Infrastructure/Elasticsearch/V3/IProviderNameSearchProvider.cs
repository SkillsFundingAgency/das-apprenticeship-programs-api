using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch.V3
{
    public interface IProviderNameSearchProvider
    {
        Task<ProviderSearchResults> SearchByTerm(string searchTerm, int page, int take);
    }
}
