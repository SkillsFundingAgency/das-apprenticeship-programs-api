namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Elasticsearch
{
    public interface IQueryHelper
    {
	    string FormatKeywords(string query);
        int GetFrameworksTotalAmount();
        int GetOrganisationsAmountByStandardId(string standardId);
        int GetOrganisationsTotalAmount();
        int GetProvidersTotalAmount();
        int GetStandardsByOrganisationIdentifierAmount(string organisationId);
        int GetStandardsTotalAmount();
        int GetProvidersByFrameworkTotalAmount(string frameworkId);
        int GetProvidersByStandardTotalAmount(string standardId);
    }
}