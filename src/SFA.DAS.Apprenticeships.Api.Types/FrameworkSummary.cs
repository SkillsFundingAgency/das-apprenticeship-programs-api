using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public sealed class FrameworkSummary
    {
        /// <summary>
        /// A composite framework Id {framework-code}-{program-type}-{pathway-code}
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// a link to the framework details
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// a unique title for framework and pathway
        /// </summary>
        public string Title { get; set; }

        public string FrameworkName { get; set; }

        public string PathwayName { get; set; }

        public int ProgType { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

        public int Level { get; set; }

        [Obsolete("Use 'Duration' Instead.")]
        public TypicalLength TypicalLength { get; set; }

        public int Duration { get; set; }

        public int MaxFunding { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public bool IsActiveFramework { get; set; }

	    public List<FundingPeriod> FundingPeriods { get; set; }
    }
}
