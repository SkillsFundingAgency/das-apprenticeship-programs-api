using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IProviderNameSearchServiceV3
    {
        ProviderSearchResults SearchProviderNameAndAliases(string searchTerm, int page, int take);
    }
}