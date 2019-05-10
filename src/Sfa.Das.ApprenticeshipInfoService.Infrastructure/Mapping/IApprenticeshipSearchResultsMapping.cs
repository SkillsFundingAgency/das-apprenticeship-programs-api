using V1ApprenticeshipSearchResultsItem = SFA.DAS.Apprenticeships.Api.Types.ApprenticeshipSearchResultsItem;
using V2ApprenticeshipSearchResultsItem = SFA.DAS.Apprenticeships.Api.Types.V3.ApprenticeshipSearchResultsItem;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IApprenticeshipSearchResultsMapping
    {
        V2ApprenticeshipSearchResultsItem MapToApprenticeshipSearchResult(V1ApprenticeshipSearchResultsItem document);
    }
}