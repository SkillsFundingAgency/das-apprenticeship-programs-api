namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    using System;
    using System.Collections.Generic;
    using SFA.DAS.Apprenticeships.Api.Types;
    
    public sealed class FrameworkSearchResultsItem
    {
        public string FrameworkId { get; set; }

        public string Title { get; set; }

        public string FrameworkName { get; set; }

        public string PathwayName { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

        public int ProgType { get; set; }

        public int Level { get; set; }

        public string CompletionQualifications { get; set; }

        public string FrameworkOverview { get; set; }

        public string EntryRequirements { get; set; }

        public string ProfessionalRegistration { get; set; }
        
        public int Duration { get; set; }

        public List<FundingPeriod> FundingPeriods { get; set; }

        public int FundingCap { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public IEnumerable<JobRoleItem> JobRoleItems { get; set; }

        public IEnumerable<string> CompetencyQualification { get; set; }

        public IEnumerable<string> KnowledgeQualification { get; set; }

        public IEnumerable<string> CombinedQualification { get; set; }

        public double SectorSubjectAreaTier1 { get; set; }

        public double SectorSubjectAreaTier2 { get; set; }

        public string FrameworkIdKeyword { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
