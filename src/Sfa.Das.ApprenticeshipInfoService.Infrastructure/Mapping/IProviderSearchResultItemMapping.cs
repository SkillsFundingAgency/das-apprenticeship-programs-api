using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IProviderSearchResultItemMapping
    {
        ProviderSearchResultItem MapToApprenticeshipSearchResult(StandardProviderSearchResultsItem source);
    }
}
