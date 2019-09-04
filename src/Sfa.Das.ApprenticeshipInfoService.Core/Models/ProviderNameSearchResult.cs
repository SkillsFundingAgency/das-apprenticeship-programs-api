using System.Collections.Generic;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    public class ProviderNameSearchResult
    {
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public List<string> Aliases { get; set; }
        public bool IsEmployerProvider { get; set; }

        public bool IsHigherEducationInstitute { get; set; }
        public bool HasNonLevyContract { get; set; }

        public bool IsLevyPayerOnly { get; set; }

        public bool CurrentlyNotStartingNewApprentices { get; set; }
        public bool NationalProvider { get; set; }
    }
}