using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public class ProviderSummary
    {
        /// <summary>
        /// UK provider reference number which is not unique
        /// </summary>
        public long Ukprn { get; set; }

        public bool IsHigherEducationInstitute { get; set; }

        [Obsolete("renamed to IsHigherEducationInstitute")]
        public bool Hei => IsHigherEducationInstitute;

        public string ProviderName { get; set; }

        public IEnumerable<string> Aliases { get; set; }

        public string LegalName { get; set; }

        /// <summary>
        /// Is this provider also an employer
        /// </summary>
        public bool IsEmployerProvider { get; set; }
        
        public string Uri { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool NationalProvider { get; set; }

        public string Website { get; set; }

        public double EmployerSatisfaction { get; set; }

        public double LearnerSatisfaction { get; set; }
    }
}
