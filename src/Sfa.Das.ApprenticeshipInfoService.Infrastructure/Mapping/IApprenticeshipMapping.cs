using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public interface IApprenticeshipMapping
    {
        ApprenticeshipSummary MapToApprenticeshipSummary(StandardSummary document);
        ApprenticeshipSummary MapToApprenticeshipSummary(FrameworkSummary document);
    }
}
