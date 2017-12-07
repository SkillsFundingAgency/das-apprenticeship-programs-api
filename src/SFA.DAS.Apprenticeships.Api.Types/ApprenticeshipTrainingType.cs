using System.Runtime.Serialization;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public enum ApprenticeshipTrainingType
    {
        [EnumMember(Value = "Framework")]
        Framework,
        [EnumMember(Value = "Standard")]
        Standard
    }
}
