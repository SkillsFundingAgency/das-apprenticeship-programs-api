using System;
using System.Collections.Generic;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Core.Models;

    public class StandardMapping : IStandardMapping
    {
        private readonly IActiveApprenticeshipChecker _activeApprenticeshipChecker;
        private readonly IFundingCapCalculator _fundingCapCalculator;

        public StandardMapping(IActiveApprenticeshipChecker activeApprenticeshipChecker, IFundingCapCalculator fundingCapCalculator)
        {
            _activeApprenticeshipChecker = activeApprenticeshipChecker;
            _fundingCapCalculator = fundingCapCalculator;
        }

        public Standard MapToStandard(StandardSearchResultsItem document)
        {
            return new Standard
            {
                StandardId = document.StandardId,
                Title = document.Title,
                StandardPdf = document.StandardPdf,
                AssessmentPlanPdf = document.AssessmentPlanPdf,
                Level = document.Level,
                IsPublished = document.Published,
                JobRoles = document.JobRoles,
                Keywords = document.Keywords,
                Duration = document.Duration,
                FundingPeriods = document.FundingPeriods,
                CurrentFundingBand = _fundingCapCalculator.CalculateCurrentFundingBand(document),
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                IntroductoryText = document.IntroductoryText,
                EntryRequirements = document.EntryRequirements,
                WhatApprenticesWillLearn = document.WhatApprenticesWillLearn,
                Qualifications = document.Qualifications,
                ProfessionalRegistration = document.ProfessionalRegistration,
                OverviewOfRole = document.OverviewOfRole,
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                StandardSectorCode = document.StandardSectorCode,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveStandard = _activeApprenticeshipChecker.CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
            };
        }

        public StandardSummary MapToStandardSummary(StandardSearchResultsItem document)
        {
            return new StandardSummary
            {
                Id = document.StandardId,
                Title = document.Title,
                Level = document.Level,
                StandardSectorCode = document.StandardSectorCode,
                IsPublished = document.Published,
                Duration = document.Duration,
                FundingPeriods = document.FundingPeriods,
                MaxFunding = _fundingCapCalculator.CalculateCurrentFundingBand(document),
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveStandard = _activeApprenticeshipChecker.CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
            };
        }
    }
}
