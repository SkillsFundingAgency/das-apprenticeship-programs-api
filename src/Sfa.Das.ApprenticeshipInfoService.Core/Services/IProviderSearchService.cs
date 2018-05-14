using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IProviderSearchService
    {
        List<ProviderSearchResultsItem> SearchProviders(string keywords, int page);
    }
}
