using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IApprenticeshipSearchServiceV1
    {
        List<ApprenticeshipSearchResultsItem> SearchApprenticeships(string keywords, int page);
    }
}
