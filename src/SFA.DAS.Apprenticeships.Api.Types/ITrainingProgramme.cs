using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public interface ITrainingProgramme
    {
        string Id { get; }
        string Title { get; }
        int Level { get; }
        int CurrentFundingCap { get; }
        DateTime? EffectiveFrom { get; }
        DateTime? EffectiveTo { get; }
        IReadOnlyCollection<IFundingPeriod> FundingPeriods { get; }
        ProgrammeType ProgrammeType { get; }
    }
}
