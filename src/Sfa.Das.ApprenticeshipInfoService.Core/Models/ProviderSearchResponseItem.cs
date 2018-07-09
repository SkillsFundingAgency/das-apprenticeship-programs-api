using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    public class ProviderSearchResponseItem
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

        public double? EmployerSatisfaction { get; set; }

        public double? LearnerSatisfaction { get; set; }

		public ContactAddress LegalAddress { get; set; }
    }
}
