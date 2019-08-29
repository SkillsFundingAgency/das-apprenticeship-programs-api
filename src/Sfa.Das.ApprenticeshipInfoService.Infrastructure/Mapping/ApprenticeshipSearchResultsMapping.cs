using SFA.DAS.Apprenticeships.Api.Types;
using System;
using System.Linq;
using V1ApprenticeshipSearchResultsItem = SFA.DAS.Apprenticeships.Api.Types.ApprenticeshipSearchResultsItem;
using V2ApprenticeshipSearchResultsItem = SFA.DAS.Apprenticeships.Api.Types.V3.ApprenticeshipSearchResultsItem;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ApprenticeshipSearchResultsMapping : IApprenticeshipSearchResultsMapping
    {
        public V2ApprenticeshipSearchResultsItem MapToApprenticeshipSearchResult(V1ApprenticeshipSearchResultsItem document)
        {
            var isStandard = IsStandard(document.StandardId.ToString());

            if (isStandard)
            {
                return CreateStandardVersion(document);
            }

            return CreateFrameworkVersion(document);
        }

        private static V2ApprenticeshipSearchResultsItem CreateStandardVersion(V1ApprenticeshipSearchResultsItem document)
        {
            return new V2ApprenticeshipSearchResultsItem
            {
                Id = document.StandardId.ToString(),
                ProgrammeType = ApprenticeshipTrainingType.Standard,
                Title = document.Title,
                Level = document.Level,
                Duration = document.Duration,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                LastDateForNewStarts = document.LastDateForNewStarts,
                Published = document.Published,
                JobRoles = document.JobRoles,
                Keywords = document.Keywords
            };
        }

        private static V2ApprenticeshipSearchResultsItem CreateFrameworkVersion(V1ApprenticeshipSearchResultsItem document)
        {
            return new V2ApprenticeshipSearchResultsItem
            {
                Id = document.FrameworkId,
                ProgrammeType = ApprenticeshipTrainingType.Framework,
                Title = document.Title,
                Level = document.Level,
                Duration = document.Duration,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                LastDateForNewStarts = document.LastDateForNewStarts,
                Published = document.Published,
                JobRoles = document.JobRoleItems?.Select(x => x.Title).ToList(),
                Keywords = document.Keywords,
                FrameworkName = document.FrameworkName,
                PathwayName = document.PathwayName
            };
        }

        private bool IsStandard(string standardId)
        {
            return !string.IsNullOrWhiteSpace(standardId);
        }
    }
}
