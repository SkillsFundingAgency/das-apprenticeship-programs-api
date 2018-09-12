using System.Collections.Generic;
using System.Linq;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Core.Models.Responses;
using SFA.DAS.Apprenticeships.Api.Types.enums;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Mapping
{
    public class ProviderMapping : IProviderMapping
    {
        private readonly ILog _applicationLogger;

        public ProviderMapping(ILog applicationLogger)
        {
            _applicationLogger = applicationLogger;
        }

        public ProviderSummary MapToProviderDto(Provider provider)
        {
            return new ProviderSummary
            {
                Email = provider.Email,
                Aliases = provider.Aliases,
                EmployerSatisfaction = provider.EmployerSatisfaction,
                IsEmployerProvider = provider.IsEmployerProvider,
                CurrentlyNotStartingNewApprentices = provider.CurrentlyNotStartingNewApprentices,
                IsHigherEducationInstitute = provider.IsHigherEducationInstitute,
                LearnerSatisfaction = provider.LearnerSatisfaction,
                NationalProvider = provider.NationalProvider,
                Phone = provider.Phone,
                ProviderName = provider.ProviderName,
                Ukprn = provider.Ukprn,
                Uri = provider.Uri,
                Website = provider.Website
            };
        }

        ApprenticeshipDetails IProviderMapping.MapToProvider(StandardProviderSearchResultsItem item, int locationId)
        {
            if (item == null)
            {
                return null;
            }

            var details = MapFromInterface(item, locationId);
            details.Product.Apprenticeship.Code = item.StandardCode.ToString();

            return details;
        }

        public ApprenticeshipDetails MapToProvider(FrameworkProviderSearchResultsItem item, int locationId)
        {
            var details = MapFromInterface(item, locationId);

            details.Product.Apprenticeship.Code = item.FrameworkId;

            return details;
        }

        public StandardProviderSearchResultsItemResponse MapToStandardProviderResponse(
            StandardProviderSearchResultsItem item)
        {
            return new StandardProviderSearchResultsItemResponse
            {
                ProviderName = item.ProviderName,
                LegalName = item.LegalName,
                ApprenticeshipInfoUrl = item.ApprenticeshipInfoUrl,
                ApprenticeshipMarketingInfo = item.ApprenticeshipMarketingInfo,
                ContactUsUrl = item.ContactUsUrl,
                DeliveryModes = item.DeliveryModes,
                Distance = item.Distance,
                Email = item.Email,
                EmployerSatisfaction = item.EmployerSatisfaction,
                LearnerSatisfaction = item.LearnerSatisfaction,
                MarketingName = item.MarketingName,
                NationalOverallAchievementRate = item.NationalOverallAchievementRate,
                NationalProvider = item.NationalProvider,
                OverallAchievementRate = item.OverallAchievementRate,
                OverallCohort = item.OverallCohort,
                Phone = item.Phone,
                ProviderMarketingInfo = item.ProviderMarketingInfo,
                TrainingLocations = item.TrainingLocations,
                Ukprn = item.Ukprn,
                IsHigherEducationInstitute = item.IsHigherEducationInstitute,
                Website = item.Website,
                HasNonLevyContract = item.HasNonLevyContract,
                HasParentCompanyGuarantee = item.HasParentCompanyGuarantee,
                IsNew = item.IsNew,
                IsLevyPayerOnly = item.IsLevyPayerOnly,
                RegulatedStandard = item.RegulatedStandard
            };
        }

        public FrameworkProviderSearchResultsItemResponse MapToFrameworkProviderResponse(
            FrameworkProviderSearchResultsItem item)
        {
            return new FrameworkProviderSearchResultsItemResponse
            {
                ProviderName = item.ProviderName,
                ApprenticeshipInfoUrl = item.ApprenticeshipInfoUrl,
                ApprenticeshipMarketingInfo = item.ApprenticeshipMarketingInfo,
                ContactUsUrl = item.ContactUsUrl,
                DeliveryModes = item.DeliveryModes,
                Distance = item.Distance,
                Email = item.Email,
                EmployerSatisfaction = item.EmployerSatisfaction,
                LearnerSatisfaction = item.LearnerSatisfaction,
                MarketingName = item.MarketingName,
                NationalOverallAchievementRate = item.NationalOverallAchievementRate,
                NationalProvider = item.NationalProvider,
                OverallAchievementRate = item.OverallAchievementRate,
                OverallCohort = item.OverallCohort,
                Phone = item.Phone,
                ProviderMarketingInfo = item.ProviderMarketingInfo,
                TrainingLocations = item.TrainingLocations,
                Ukprn = item.Ukprn,
                IsHigherEducationInstitute = item.IsHigherEducationInstitute,
                Website = item.Website,
                FrameworkCode = item.FrameworkCode,
                Level = item.Level,
                PathwayCode = item.PathwayCode,
                HasNonLevyContract = item.HasNonLevyContract,
                HasParentCompanyGuarantee = item.HasParentCompanyGuarantee,
                IsNew = item.IsNew,
                IsLevyPayerOnly = item.IsLevyPayerOnly
            };
        }

        public ProviderSearchResponseItem MapToProviderSearchItem(ProviderSearchResultsItem provider)
        {
            var response = new ProviderSearchResponseItem
            {
                Aliases = provider.Aliases,
                Ukprn = provider.Ukprn,
                IsHigherEducationInstitute = provider.IsHigherEducationInstitute,
                IsEmployerProvider = provider.IsEmployerProvider,
                ProviderName = provider.ProviderName,
                Phone = provider.Phone,
                Email = provider.Email,
                NationalProvider = provider.NationalProvider,
                Website = provider.Website,
                Addresses = provider.Addresses
            };

            if (!string.IsNullOrWhiteSpace(provider.EmployerSatisfaction))
            {
                var employerSatisfaction = double.Parse(provider.EmployerSatisfaction);
                response.EmployerSatisfaction = employerSatisfaction;
            }

            if (!string.IsNullOrWhiteSpace(provider.LearnerSatisfaction))
            {
                var learnerSatisfaction = double.Parse(provider.LearnerSatisfaction);
                response.LearnerSatisfaction = learnerSatisfaction;
            }

            return response;
        }

        private List<DeliveryMode> ConvertToEnumeratedDeliveryModes(List<string> itemDeliveryModes)
        {
            var enumeratedDeliveryModes = new List<DeliveryMode>();
            foreach (var mode in itemDeliveryModes)
            {
                switch (mode.ToLower())
                {
                    case "dayrelease":
                        enumeratedDeliveryModes.Add(DeliveryMode.DayRelease);
                        break;
                    case "blockrelease":
                        enumeratedDeliveryModes.Add(DeliveryMode.BlockRelease);
                        break;
                    case "100percentemployer":
                        enumeratedDeliveryModes.Add(DeliveryMode.HundredPercentEmployer);
                        break;
                    default:
                        var errorMessage = $"Unknown DeliveryMode [{mode}] could not be mapped.";
                        _applicationLogger.Error(new UnknownDeliveryModeException(errorMessage), errorMessage);
                        break;
                }
            }

            return enumeratedDeliveryModes;
        }
        private ApprenticeshipDetails MapFromInterface(IApprenticeshipProviderSearchResultsItem item, int locationId)
        {
            var matchingLocation = item.TrainingLocations.Single(x => x.LocationId == locationId);

            return new ApprenticeshipDetails
            {
                Product = new ApprenticeshipProduct
                {
                    Apprenticeship = new ApprenticeshipBasic
                    {
                        ApprenticeshipInfoUrl = item.ApprenticeshipInfoUrl,
                        ApprenticeshipMarketingInfo =
                                           item.ApprenticeshipMarketingInfo
                    },
                    DeliveryModes = ConvertToEnumeratedDeliveryModes(item.DeliveryModes),
                    ProviderMarketingInfo = item.ProviderMarketingInfo,
                    EmployerSatisfaction = item.EmployerSatisfaction,
                    LearnerSatisfaction = item.LearnerSatisfaction,
                    AchievementRate = item.OverallAchievementRate,
                    NationalAchievementRate = item.NationalOverallAchievementRate,
                    OverallCohort = item.OverallCohort
                },
                Location = new Location
                {
                    LocationId = matchingLocation.LocationId,
                    LocationName = matchingLocation.LocationName,
                    Address = matchingLocation.Address
                },
                Provider = new ProviderDetail
                {
                    Name = item.ProviderName,
                    LegalName = item.LegalName,
                    UkPrn = item.Ukprn,
                    IsHigherEducationInstitute = item.IsHigherEducationInstitute,
                    NationalProvider = item.NationalProvider,
                    ContactInformation = new ContactInformation
                    {
                        Phone = item.Phone,
                        Email = item.Email,
                        Website = item.Website,
                        ContactUsUrl = item.ContactUsUrl
                    },
                    HasNonLevyContract = item.HasNonLevyContract,
                    HasParentCompanyGuarantee = item.HasParentCompanyGuarantee,
                    IsNew = item.IsNew,
                    IsLevyPayerOnly = item.IsLevyPayerOnly,
                    CurrentlyNotStartingNewApprentices = item.CurrentlyNotStartingNewApprentices
                }
            };
        }
    }
}
