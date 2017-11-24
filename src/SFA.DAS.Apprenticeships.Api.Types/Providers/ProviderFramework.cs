using System;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ProviderFramework
    {
        public string FrameworkId { get; set; }
        public int FworkCode { get; set; }
        public int ProgType { get; set; }
        public int PwayCode { get; set; }
        public int Level { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string PathwayName { get; set; }
    }
}
