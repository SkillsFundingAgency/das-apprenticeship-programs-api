using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.Apprenticeships.Api.Types.enums
{
    [JsonConverter(typeof(StringEnumConverter))]
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
