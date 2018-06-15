using System;
using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    using System.Linq;
    using Core.Models;

    public class FrameworkMapping : IFrameworkMapping
    {
         private readonly IActiveApprenticeshipChecker _activeApprenticeshipChecker;
	    private readonly IFundingCapCalculator _fundingCapCalculator;

	    public FrameworkMapping(IActiveApprenticeshipChecker activeApprenticeshipChecker, IFundingCapCalculator fundingCapCalculator)
	    {
		    _activeApprenticeshipChecker = activeApprenticeshipChecker;
		    _fundingCapCalculator = fundingCapCalculator;
	    }

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
				FundingPeriods = document.FundingPeriods,
                MaxFunding = _fundingCapCalculator.CalculateCurrentFundingBand(document),
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
                IsActiveFramework = _activeApprenticeshipChecker.CheckActiveFramework(document.FrameworkId, document.EffectiveFrom, document.EffectiveTo)
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
				FundingPeriods = document.FundingPeriods,
                MaxFunding = _fundingCapCalculator.CalculateCurrentFundingBand(document),
                Ssa1 = document.SectorSubjectAreaTier1,
                Ssa2 = document.SectorSubjectAreaTier2,
                TypicalLength = new TypicalLength { From = document.Duration, To = document.Duration, Unit = "m" },
                EffectiveFrom = document.EffectiveFrom,
                EffectiveTo = document.EffectiveTo,
                IsActiveFramework = _activeApprenticeshipChecker.CheckActiveFramework(document.FrameworkId, document.EffectiveFrom, document.EffectiveTo)
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
				Title = document.FrameworkName,
				EffectiveTo = document.EffectiveTo
			};
		}

	    public FrameworkCodeSummary MapToFrameworkCodeSummaryFromList(List<FrameworkSearchResultsItem> documents)
	    {
		    var earliestDate = GetEarliestDate(documents);
		    var latestDate = GetLatestDate(documents);

		    return new FrameworkCodeSummary
		    {
			    FrameworkCode = documents.First().FrameworkCode,
			    Ssa1 = documents.First().SectorSubjectAreaTier1,
			    Ssa2 = documents.First().SectorSubjectAreaTier2,
			    Title = documents.First().FrameworkName,
			    EffectiveFrom = earliestDate,
			    EffectiveTo = latestDate
		    };
	    }

		public FrameworkCodeSummary MapToFrameworkCodeSummary(FrameworkSummary frameworkSummary)
        {
            return new FrameworkCodeSummary
            {
                FrameworkCode = frameworkSummary.FrameworkCode,
                Ssa1 = frameworkSummary.Ssa1,
                Ssa2 = frameworkSummary.Ssa2,
                Title = frameworkSummary.FrameworkName,
                EffectiveTo = frameworkSummary.EffectiveTo
            };
        }
	    public FrameworkCodeSummary MapToFrameworkCodeSummaryFromList(List<FrameworkSummary> documents)
	    {
		    var earliestDate = GetEarliestDate(documents);
		    var latestDate = GetLatestDate(documents);

		    return new FrameworkCodeSummary
		    {
			    FrameworkCode = documents.First().FrameworkCode,
			    Ssa1 = documents.First().Ssa1,
			    Ssa2 = documents.First().Ssa2,
			    Title = documents.First().FrameworkName,
			    EffectiveFrom = earliestDate,
			    EffectiveTo = latestDate
		    };
	    }

		private DateTime? GetLatestDate(List<FrameworkSummary> documents)
		{
			if (documents.Any(x => x.EffectiveTo == null))
			{
				return null;
			}

			return documents.Max(item => item.EffectiveTo);
		}

		private DateTime? GetEarliestDate(List<FrameworkSummary> documents)
		{
			return documents.Min(item => item.EffectiveFrom);
		}
		
	    private DateTime? GetLatestDate(List<FrameworkSearchResultsItem> documents)
	    {
		    if (documents.Any(x => x.EffectiveTo == null))
		    {
			    return null;
		    }

			return documents.Max(item => item.EffectiveTo);
		}

	    private DateTime? GetEarliestDate(List<FrameworkSearchResultsItem> documents)
	    {
			return documents.Min(item => item.EffectiveFrom);
		}

    }
}
