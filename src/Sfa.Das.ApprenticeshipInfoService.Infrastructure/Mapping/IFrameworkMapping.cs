using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;

    public interface IFrameworkMapping
    {
        Framework MapToFramework(FrameworkSearchResultsItem document);

        FrameworkSummary MapToFrameworkSummary(FrameworkSearchResultsItem document);

        FrameworkCodeSummary MapToFrameworkCodeSummary(FrameworkSearchResultsItem document);

        FrameworkCodeSummary MapToFrameworkCodeSummary(FrameworkSummary frameworkSummary);

	    FrameworkCodeSummary MapToFrameworkCodeSummaryFromList(List<FrameworkSearchResultsItem> documents);

		FrameworkCodeSummary MapToFrameworkCodeSummaryFromList(List<FrameworkSummary> documents);
    }
}
