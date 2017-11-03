using System.ComponentModel;

namespace SFA.DAS.Apprenticeships.Api.Types.enums
{
    public enum DeliveryMode
    {
        [Description("DayRelease")] DayRelease,
        [Description("BlockRelease")] BlockRelease,
        [Description("100PercentEmployer")] HundredPercentEmployer
    }
}
