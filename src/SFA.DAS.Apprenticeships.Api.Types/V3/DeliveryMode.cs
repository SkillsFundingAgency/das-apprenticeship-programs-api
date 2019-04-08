using System.Runtime.Serialization;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
    public enum DeliveryMode
    {
        [EnumMember(Value = "dayrelease")]
        DayRelease,
        [EnumMember(Value = "blockrelease")]
        BlockRelease,
        [EnumMember(Value = "100percentemployer")]
        HundredPercentEmployer
    }
}
