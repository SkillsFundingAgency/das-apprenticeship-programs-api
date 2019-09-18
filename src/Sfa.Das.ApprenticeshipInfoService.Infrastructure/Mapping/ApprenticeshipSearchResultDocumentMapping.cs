using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ApprenticeshipSearchResultDocumentMapping : IApprenticeshipSearchResultDocumentMapping
    {
        public ApprenticeshipSearchResultsItem MapToApprenticeshipSearchResultsItem(ApprenticeshipSearchResultsDocument d)
        {
            return new ApprenticeshipSearchResultsItem
            {
                Duration = d.Duration,
                EffectiveFrom = d.EffectiveFrom,
                EffectiveTo = d.EffectiveTo,
                FrameworkId = d.FrameworkId,
                FrameworkName = d.FrameworkName,
                JobRoleItems = d.JobRoleItems,
                JobRoles = d.JobRoles,
                Keywords = d.Keywords,
                LastDateForNewStarts = d.LastDateForNewStarts,
                Level = d.Level,
                PathwayName = d.PathwayName,
                Published = d.Published,
                StandardId = d.StandardId?.ToString(),
                Title = d.Title,
                TitleKeyword = d.TitleKeyword
            };
        }
    }
}
