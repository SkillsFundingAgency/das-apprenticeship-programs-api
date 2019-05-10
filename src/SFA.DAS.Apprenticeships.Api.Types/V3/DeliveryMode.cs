using System.Runtime.Serialization;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public enum DeliveryMode
    {
        [EnumMember(Value = "DayRelease")]
        DayRelease,
        [EnumMember(Value = "BlockRelease")]
        BlockRelease,
        [EnumMember(Value = "100PercentEmployer")]
        HundredPercentEmployer
    }
}
