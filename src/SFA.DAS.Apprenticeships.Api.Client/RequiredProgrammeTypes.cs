using System;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    [Flags]
    public enum RequiredProgrammeTypes
    {
        Standard = ProgrammeType.Standard,
        Framework = ProgrammeType.Framework,
        All = ProgrammeType.Standard | ProgrammeType.Framework
    }
}