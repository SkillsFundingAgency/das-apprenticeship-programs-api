﻿using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.Providers
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

		public IEnumerable<ContactAddress> Addresses { get; set; }
    }
}
