namespace Sfa.Das.ApprenticeshipInfoService.Application.Models
{
    using System;

    public class StandardOrganisationDocument : OrganisationDocument
    {
        public string StandardCode { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
