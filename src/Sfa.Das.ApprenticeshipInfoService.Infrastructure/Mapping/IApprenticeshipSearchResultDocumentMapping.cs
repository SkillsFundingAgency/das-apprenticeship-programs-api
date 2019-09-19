using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IApprenticeshipSearchResultDocumentMapping
    {
        ApprenticeshipSearchResultsItem MapToApprenticeshipSearchResultsItem(ApprenticeshipSearchResultsDocument document);
    }
}
