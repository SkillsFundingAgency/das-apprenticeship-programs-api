using System;
using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ProviderSearchResultItemMapping : IProviderSearchResultItemMapping
    {
        public ProviderSearchResultItem MapToApprenticeshipSearchResult(StandardProviderSearchResultsItem source)
        {
            return new ProviderSearchResultItem
            {
                Ukprn = source.Ukprn,
                Location = new SFA.DAS.Apprenticeships.Api.Types.V3.TrainingLocation(),
                ProviderName = source.ProviderName,
                OverallAchievementRate = source.OverallAchievementRate,
                NationalProvider = source.NationalProvider,
                DeliveryModes = source.DeliveryModes,
                Distance = source.Distance,
                EmployerSatisfaction = source.EmployerSatisfaction,
                LearnerSatisfaction = source.LearnerSatisfaction,
                NationalOverallAchievementRate = source.NationalOverallAchievementRate,
                OverallCohort = source.OverallCohort,
                HasNonLevyContract = source.HasNonLevyContract,
                IsLevyPayerOnly = source.IsLevyPayerOnly,
                CurrentlyNotStartingNewApprentices = source.CurrentlyNotStartingNewApprentices,
            };
        }

        private Feedback MapFeedback(SFA.DAS.Apprenticeships.Api.Types.Providers.Feedback sourceFeedback)
        {
            if (sourceFeedback == null)
            {
                return null;
            }

            return new Feedback
            {
                Strengths = sourceFeedback.Strengths?.Select(x => new ProviderAttribute { Name = x.Name, Count = x.Count}).ToList(),
                Weaknesses = sourceFeedback.Weaknesses?.Select(x => new ProviderAttribute { Name = x.Name, Count = x.Count }).ToList(),
                ExcellentFeedbackCount = sourceFeedback.ExcellentFeedbackCount,
                GoodFeedbackCount = sourceFeedback.GoodFeedbackCount,
                PoorFeedbackCount = sourceFeedback.PoorFeedbackCount,
                VeryPoorFeedbackCount = sourceFeedback.VeryPoorFeedbackCount
            };
        }

        private IEnumerable<SFA.DAS.Apprenticeships.Api.Types.V3.TrainingLocation> MapTrainingLocations(IEnumerable<Core.Models.TrainingLocation> s)
        {
            if (s == null)
            {
                return null;
            }

            return s.Select(x => new SFA.DAS.Apprenticeships.Api.Types.V3.TrainingLocation
            {
                LocationId = x.LocationId,
                LocationName = x.LocationName,
                Address = new SFA.DAS.Apprenticeships.Api.Types.V3.Address
                {
                    Primary = x.Address.Address1

                }

            });
        }
    }
}
