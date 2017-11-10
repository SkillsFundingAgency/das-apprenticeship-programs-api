using System;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;

    public class StandardMapping : IStandardMapping
    {
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
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveStandard = CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
            };
        }

        public StandardSummary MapToStandardSummary(StandardSearchResultsItem document)
        {
            return new StandardSummary
            {
                Id = document.StandardId,
                Title = document.Title,
                Level = document.Level,
                IsPublished = document.Published,
                Duration = document.Duration,
                MaxFunding = document.FundingCap,
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveStandard = CheckActiveStandard(document.StandardId, document.EffectiveFrom, document.EffectiveTo)
            };
        }

        private bool CheckActiveStandard(string documentStandardId, DateTime? documentEffectiveFrom, DateTime? documentEffectiveTo)
        {
            return DateHelper.CheckEffectiveDates(documentEffectiveFrom, documentEffectiveTo) || false;
        }
    }
}
