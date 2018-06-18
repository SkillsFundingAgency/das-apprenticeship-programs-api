using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public sealed class Standard
    {
        /// <summary>
        /// The standard identifier from LARS
        /// </summary>
        public string StandardId { get; set; }

        /// <summary>
        /// a link to the standard details
        /// </summary>
        public string Uri { get; set; }

        public string Title { get; set; }

        public int Level { get; set; }

        public bool IsPublished { get; set; }

        public string StandardPdf { get; set; }

        public string AssessmentPlanPdf { get; set; }

        public IEnumerable<string> JobRoles { get; set; }

        public IEnumerable<string> Keywords { get; set; }

        [Obsolete("Use 'Duration' instead.")]
        public TypicalLength TypicalLength { get; set; }

        public int Duration { get; set; }

	    [Obsolete("Use 'CurrentFundingCap' instead.")]
		public int MaxFunding => CurrentFundingBand;

	    public int CurrentFundingBand { get; set; }

		public string IntroductoryText { get; set; }

        public string EntryRequirements { get; set; }

        public string WhatApprenticesWillLearn { get; set; }

        public string Qualifications { get; set; }

        public string ProfessionalRegistration { get; set; }

        public string OverviewOfRole { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }

        public ProvidersHref Providers { get; set; }

        public bool IsActiveStandard { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public int StandardSectorCode { get; set; }

        public string StandardPageUri { get; set; }

	    public List<FundingPeriod> FundingPeriods { get; set; }
    }
}
