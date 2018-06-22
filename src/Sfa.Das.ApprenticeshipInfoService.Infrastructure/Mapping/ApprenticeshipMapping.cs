using System;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;

    public class ApprenticeshipMapping : IApprenticeshipMapping
    {
        public ApprenticeshipSummary MapToApprenticeshipSummary(StandardSummary document)
        {
            return new ApprenticeshipSummary
            {
                Id = document.Id,
                Uri = document.Uri,
                Title = document.Title,
                Duration = document.Duration,
                Level = document.Level,
                Ssa1 = document.Ssa1,
                Ssa2 = document.Ssa2,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                CurrentFundingCap = document.CurrentFundingCap,
                FundingPeriods = document.FundingPeriods
            };
        }

        public ApprenticeshipSummary MapToApprenticeshipSummary(FrameworkSummary document)
        {
            return new ApprenticeshipSummary
            {
                Id = document.Id,
                Uri = document.Uri,
                Title = document.Title,
                Duration = document.Duration,
                Level = document.Level,
                Ssa1 = document.Ssa1,
                Ssa2 = document.Ssa2,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                CurrentFundingCap = document.CurrentFundingCap,
                FundingPeriods = document.FundingPeriods
            };
        }
    }
}
