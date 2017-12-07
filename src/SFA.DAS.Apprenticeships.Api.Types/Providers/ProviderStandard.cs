using System;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
{
    public class ProviderStandard
    {
        public int StandardId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
    }
}
