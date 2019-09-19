using System;
using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types.V3;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ProviderNameSearchMapping : IProviderNameSearchMapping
    {

        public IEnumerable<ProviderSearchResultItem> FilterNonMatchingAliases(string searchTerm, IEnumerable<ProviderNameSearchResult> resultsToFilter)
        {
            var resultsToReturn = new List<ProviderSearchResultItem>();
            foreach (var item in resultsToFilter)
            {
                var details = new ProviderSearchResultItem
                {
                    ProviderName = item.ProviderName,
                    Ukprn = Convert.ToInt32(item.Ukprn),
                    NationalProvider = item.NationalProvider,
                    HasNonLevyContract = item.HasNonLevyContract,
                    IsLevyPayerOnly = item.IsLevyPayerOnly,
                    CurrentlyNotStartingNewApprentices = item.CurrentlyNotStartingNewApprentices,
                    IsHigherEducationInstitute = item.IsHigherEducationInstitute,
                    Aliases = item.Aliases?
                        .Where(m => m.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        .Select(s => s.Trim()).ToList()
                };

                resultsToReturn.Add(details);
            }

            return resultsToReturn;
        }
    }
}