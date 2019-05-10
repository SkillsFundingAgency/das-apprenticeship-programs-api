using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IApprenticeshipSearchServiceV3
    {
        ApprenticeshipSearchResults SearchApprenticeships(string keywords, int pageNumber, int pageSize = 20, int order = 0, IEnumerable<int> selectedLevels = null);
    }
}
