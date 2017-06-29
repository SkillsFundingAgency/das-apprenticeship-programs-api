using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;

    public interface IFrameworkMapping
    {
        Framework MapToFramework(FrameworkSearchResultsItem document);

        FrameworkSummary MapToFrameworkSummary(FrameworkSearchResultsItem document);

        FrameworkResume MapToFrameworkResume(FrameworkSearchResultsItem document);

        FrameworkResume MapToFrameworkResume(FrameworkSummary frameworkSummary);
    }
}
