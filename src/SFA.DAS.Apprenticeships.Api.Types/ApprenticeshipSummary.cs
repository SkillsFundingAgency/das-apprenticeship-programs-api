using System;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public class ApprenticeshipSummary
    {
        public string Id { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public int Duration { get; set; }

        public int MaxFunding { get; set; }

        public int Level { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
