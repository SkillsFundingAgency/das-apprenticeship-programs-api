using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Helpers
{
    public interface IFundingCapCalculator
    {
        int CalculateCurrentFundingBand(StandardSearchResultsItem standard);

        int CalculateCurrentFundingBand(FrameworkSearchResultsItem framework);
    }
}