# Apprenticeship Programmes API - *prerelease*

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/static/images/govuk-crest-bb9e22aff7881b895c2ceb41d9340804451c474b883f09fe1b4026e76456f44b.png)|Apprenticeship Programmes API|
| Build | <img alt="Build Status" src="https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/166/badge" /> |
| Apprenticeship Programmes  Client | [![](https://img.shields.io/nuget/v/SFA.DAS.Apprenticeships.Api.Client.svg)](https://www.nuget.org/packages/SFA.DAS.Apprenticeships.Api.Client/) |
| Providers Client | [![](https://img.shields.io/nuget/v/SFA.DAS.Providers.Api.Client.svg)](https://www.nuget.org/packages/SFA.DAS.Providers.Api.Client/) |
| Assessment Orgs Client | [![](https://img.shields.io/nuget/v/SFA.DAS.AssessmentOrgs.Api.Client.svg)](https://www.nuget.org/packages/SFA.DAS.AssessmentOrgs.Api.Client/) |
| Coverity | [![](https://scan.coverity.com/projects/10689/badge.svg)](https://scan.coverity.com/projects/skillsfundingagency-das-apprenticeship-programs-api) |
| Web | https://findapprenticeshiptraining-api.sfa.bis.gov.uk/ | 
| Swagger | [![](http://online.swagger.io/validator?url=https://findapprenticeshiptraining-api.sfa.bis.gov.uk/:80/swagger/docs/v1)](http://das-prd-apprenticeshipinfoservice.cloudapp.net:80/swagger/docs/v1) |

A public API from the Skills Funding Agency to provide a list of 
- Standards
- Frameworks
- Assessment Organisations
- Providers

and their relationships

## Consumers
- [Find Apprenticeship Training](https://github.com/SkillsFundingAgency/das-search)
- [Employer Apprenticeships Service](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice)

**if you're not on here let us know so we don't break your application**

## Architecture

### Other components
- [Apprenticeship Programmes Indexer](https://github.com/SkillsFundingAgency/das-apprenticeship-programs-indexer)

### Dependencies 
- Elasticsearch 2.3.5

## Usage

### Basic
```c#
using(var client = new StandardApiClient())
{
   var standard = client.Get(12);
}

using(var client = new FrameworkApiClient())
{
   var framework = client.Get("403-1-8"); // NOTE the ID format changing
}

using(var client = new ProviderApiClient())
{
   var provider = client.Get(1003141);
}

using(var client = new AssessmentOrgsApiClient())
{
	var org = client.Get("EPA0001");
}
```

### StructureMap
```c#
For<IStandardApiClient>().Use<StandardApiClient>();
For<IFrameworkApiClient>().Use<FrameworkApiClient>();
For<IProviderApiClient>().Use<ProviderApiClient>();
For<IAssessmentOrgsApiClient>().Use<AssessmentOrgsApiClient>();
```

