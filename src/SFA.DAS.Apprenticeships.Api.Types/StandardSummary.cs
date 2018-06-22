using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public class StandardSummary
    {
        /// <summary>
        /// The standard identifier from LARS
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// a link to the standard details
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The standard title
        /// </summary>
        public string Title { get; set; }

        [Obsolete("Use 'Duration' instead.")]
        public TypicalLength TypicalLength { get; set; }

        public int Duration { get; set; }

	    public int CurrentFundingCap { get; set; }

	    [Obsolete("Use 'CurrentFundingCap' instead.")]
		public int MaxFunding => CurrentFundingCap;

        public int Level { get; set; }

        public bool IsPublished { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime? LastDateForNewStarts { get; set; }

        public bool IsActiveStandard { get; set; }

        public int StandardSectorCode { get; set; }

	    public List<FundingPeriod> FundingPeriods { get; set; }
    }
}
