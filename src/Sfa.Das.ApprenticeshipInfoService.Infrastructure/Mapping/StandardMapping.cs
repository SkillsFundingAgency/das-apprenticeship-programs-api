using System;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Core.Models;

    public class StandardMapping : IStandardMapping
    {
        private readonly IActiveApprenticceshipChecker _activeApprenticceshipChecker;

        public StandardMapping(IActiveApprenticceshipChecker activeApprenticceshipChecker)
        {
            _activeApprenticceshipChecker = activeApprenticceshipChecker;
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
                MaxFunding = document.FundingCap,
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
                IsActiveStandard = _activeApprenticceshipChecker.CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
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
                MaxFunding = document.FundingCap,
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveStandard = _activeApprenticceshipChecker.CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
            };
        }
    }
}
