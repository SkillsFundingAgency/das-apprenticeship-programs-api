﻿using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    public class ProviderSearchResultsItem
    {
        public int Ukprn { get; set; }

        public bool IsHigherEducationInstitute { get; set; }

        public bool IsEmployerProvider { get; set; }

        public string ProviderName { get; set; }

        public List<string> Aliases { get; set; }

	    public IEnumerable<ContactAddress> Addresses { get; set; }

		public string Uri { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool NationalProvider { get; set; }

        public string Website { get; set; }

		public float? EmployerSatisfaction { get; set; }

        public float? LearnerSatisfaction { get; set; }
    }
}
