using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    public class ProviderSearchResultsItem
    {
        public string Ukprn { get; set; }

        public bool IsHigherEducationInstitute { get; set; }

        public bool IsEmployerProvider { get; set; }

        public string ProviderName { get; set; }

        public List<string> Aliases { get; set; }

        public string Uri { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool NationalProvider { get; set; }

        public string Website { get; set; }

        public string EmployerSatisfaction { get; set; }

        public string LearnerSatisfaction { get; set; }
    }
}
