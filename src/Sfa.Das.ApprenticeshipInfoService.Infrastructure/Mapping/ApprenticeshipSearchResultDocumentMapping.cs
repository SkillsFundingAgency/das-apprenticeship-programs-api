using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ApprenticeshipSearchResultDocumentMapping : IApprenticeshipSearchResultDocumentMapping
    {
        public ApprenticeshipSearchResultsItem MapToApprenticeshipSearchResultsItem(ApprenticeshipSearchResultsDocument document)
        {
            return new ApprenticeshipSearchResultsItem
            {
                Duration = document.Duration,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                FrameworkId = document.FrameworkId,
                FrameworkName = document.FrameworkName,
                JobRoleItems = document.JobRoleItems,
                JobRoles = document.JobRoles,
                Keywords = document.Keywords,
                LastDateForNewStarts = document.LastDateForNewStarts,
                Level = document.Level,
                PathwayName = document.PathwayName,
                Published = document.Published,
                StandardId = document.StandardId?.ToString(),
                Title = document.Title,
                TitleKeyword = document.TitleKeyword
            };
        }
    }
}
