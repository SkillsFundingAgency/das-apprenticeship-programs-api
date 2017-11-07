using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.Apprenticeships.Api.Types.enums
{
    [JsonConverter(typeof(StringEnumConverter))]
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
