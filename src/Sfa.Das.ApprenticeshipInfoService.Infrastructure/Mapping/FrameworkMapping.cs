using System;
using Nest;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using System.Linq;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;

    public class FrameworkMapping : IFrameworkMapping
    {
        public Framework MapToFramework(FrameworkSearchResultsItem document)
        {
            var framework = new Framework
            {
                Title = document.Title,
                Level = document.Level,
                FrameworkCode = document.FrameworkCode,
                FrameworkId = document.FrameworkId,
                FrameworkName = document.FrameworkName,
                PathwayCode = document.PathwayCode,
                PathwayName = document.PathwayName,
                ProgType = document.ProgType,
                Duration = document.Duration,
                MaxFunding = document.FundingCap,
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                TypicalLength = new TypicalLength {From = document.Duration, To = document.Duration, Unit = "m"},
                ExpiryDate = document.ExpiryDate,
                JobRoleItems = document.JobRoleItems,
                CompletionQualifications = document.CompletionQualifications,
                FrameworkOverview = document.FrameworkOverview,
                EntryRequirements = document.EntryRequirements,
                ProfessionalRegistration = document.ProfessionalRegistration,
                CompetencyQualification = document.CompetencyQualification?.OrderBy(x => x),
                KnowledgeQualification = document.KnowledgeQualification?.OrderBy(x => x),
                CombinedQualification = document.CombinedQualification?.OrderBy(x => x),
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveFramework = CheckActiveFramework(document.FrameworkId, document.EffectiveFrom, document.EffectiveTo)
            };

            return framework;
        }

        public FrameworkSummary MapToFrameworkSummary(FrameworkSearchResultsItem document)
        {
            var framework = new FrameworkSummary
            {
                Id = document.FrameworkId,
                Title = document.Title,
                Level = document.Level,
                FrameworkCode = document.FrameworkCode,
                FrameworkName = document.FrameworkName,
                PathwayCode = document.PathwayCode,
                PathwayName = document.PathwayName,
                ProgType = document.ProgType,
                Duration = document.Duration,
                MaxFunding = document.FundingCap,
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveFramework = CheckActiveFramework(document.FrameworkId, document.EffectiveFrom, document.EffectiveTo)
            };

            return framework;
        }

        public FrameworkCodeSummary MapToFrameworkCodeSummary(FrameworkSearchResultsItem document)
        {
            return new FrameworkCodeSummary
            {
                FrameworkCode = document.FrameworkCode,
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                Title = document.FrameworkName
            };
        }

        public FrameworkCodeSummary MapToFrameworkCodeSummary(FrameworkSummary frameworkSummary)
        {
            return new FrameworkCodeSummary
            {
                FrameworkCode = frameworkSummary.FrameworkCode,
                Ssa1 = frameworkSummary.Ssa1,
                Ssa2 = frameworkSummary.Ssa2,
                Title = frameworkSummary.FrameworkName
            };
        }

        private bool CheckActiveFramework(string frameworkId, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            return DateHelper.CheckEffectiveDates(effectiveFrom, effectiveTo) || false;
        }
    }
}
