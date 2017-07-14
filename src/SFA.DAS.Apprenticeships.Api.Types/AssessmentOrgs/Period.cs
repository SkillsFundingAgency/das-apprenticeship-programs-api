using System;

namespace SFA.DAS.Apprenticeships.Api.Types.AssessmentOrgs
{
    public class Period
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}