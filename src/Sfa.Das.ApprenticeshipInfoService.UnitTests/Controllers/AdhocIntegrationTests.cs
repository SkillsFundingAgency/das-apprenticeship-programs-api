namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NUnit.Framework;
    using SFA.DAS.Apprenticeships.Api.Client;
    using SFA.DAS.Providers.Api.Client;

    [TestFixture]
    public class AdhocIntegrationTests
    {
        private readonly string _url = "http://das-prd-apprenticeshipinfoservice.cloudapp.net/";
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private ProviderApiClient _sut;

        [OneTimeSetUp]
        public void TestSetup()
        {
            // provider indexer api client defaults to production uri.
            _sut = new ProviderApiClient(_url);
        }

        [Test]
        [Ignore("Adhoc tests to verify live provider information")]
        public void ShouldReturnFrameworks()
        {
            var frameworkApiClient = new FrameworkApiClient(_url);
            var frameworks = frameworkApiClient.FindAll().ToList();
            frameworks.ForEach(x => Console.WriteLine($"{x.Id}"));
        }

        [Test]
        [Ignore("Integraion tests to verify live framework information")]
        public void ShouldHaveSameFrameworkName()
        {
            var frameworkApiClient = new FrameworkApiClient(_url);
            var frameworks = frameworkApiClient.FindAll().ToList();
            var errors = new List<string>();

            foreach (var framework in frameworks)
            {
                var fsummary = frameworkApiClient.Get(framework.Id);
                var jobTitlesfromApi = fsummary.JobRoleItems.Select(x => x.Title).ToList();
                var jsonFile = $"{framework.Id}.json";
                using (StreamReader r = new StreamReader(jsonFile))
                {
                    string json = r.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(json, _jsonSettings);
                    dynamic jobRoleItemsfromJsonFile = array.JobRoleItems;
                    int titleCount = 0;
                    foreach (var item in jobRoleItemsfromJsonFile)
                    {
                        titleCount++;
                           var title = item.Title.Value;
                        if (!jobTitlesfromApi.Contains(title))
                        {
                            // errors.Add($"framework job tilte did not match for {framework.Id} - from api - {string.Join(",", jobTitlesfromApi)} but in json file {title}");
                            errors.Add($"framework job tilte did not match for {framework.Id} in json file {title}");
                        }
                    }

                    Assert.AreEqual(jobTitlesfromApi.Count, titleCount, $"Title count did not match for {framework.Id} - from api - {jobTitlesfromApi.Count} but in json {titleCount}");
                }
            }

            Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
        }

        [Test]
        [Ignore("Integraion tests to verify live framework information")]
        public void ShouldFindFramework()
        {
            var providers = _sut.GetStandardProviders("117");
            Console.WriteLine("Standard Providers for 117" + Environment.NewLine);
            Console.WriteLine(string.Join(Environment.NewLine, providers.OrderBy(t => t.Ukprn).Select(x => $"{x.Ukprn},{x.ProviderName}")));

            providers = _sut.GetStandardProviders("11");
            Console.WriteLine("Standard Providers for 11" + Environment.NewLine);
            Console.WriteLine(string.Join(Environment.NewLine, providers.OrderBy(t => t.Ukprn).Select(x => $"{x.Ukprn},{x.ProviderName}")));

            providers = _sut.GetStandardProviders("128");
            Console.WriteLine("Standard Providers for 128" + Environment.NewLine);
            Console.WriteLine(string.Join(Environment.NewLine, providers.OrderBy(t => t.Ukprn).Select(x => $"{x.Ukprn},{x.ProviderName}")));

            Assert.AreEqual(true, true);
        }

        [Test]
        [Ignore("Integraion tests to verify live provider information from Coursedirectory")]
        public void ShouldFindProviderinCD()
        {
            string url = string.Empty;

            List<string> message = new List<string>();
            var fApiClient = new FrameworkApiClient(_url);
            var sApiClient = new StandardApiClient(_url);

            // set up request/response
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            string content;

            // read response content
            using (var reader = new StreamReader(stream ?? new MemoryStream(), Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            // write to file on desktop
            // string filewithPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CdResponse.txt");
            // File.WriteAllText(filewithPath, content, Encoding.UTF8);
            // Console.WriteLine($"File should be available in {filewithPath}");
            dynamic providers = JsonConvert.DeserializeObject(content, _jsonSettings);
            List<int> providerukprns = new List<int> { };
            foreach (var provider in providers)
            {
                foreach (var ukprn in providerukprns)
                {
                    if (provider.ukprn == ukprn)
                    {
                        var frameworks = provider.frameworks;
                        var standards = provider.standards;
                        message.Add($"{Environment.NewLine}{ukprn} - {provider.name} Offers");
                        foreach (var framework in frameworks)
                        {
                            var fid = $"{framework.frameworkCode}-{framework.pathwayCode}-{framework.progType}";
                            var fname = fApiClient.Get(fid);
                            message.Add($"Framework : {fname.FrameworkName} - {fid}");
                        }

                        foreach (var standard in standards)
                        {
                            var sCode = standard.standardCode;
                            var sname = sApiClient.Get((int)sCode);
                            message.Add($"Standard : {sname.Title} - {sCode}");
                        }
                    }
                }
            }

            Console.WriteLine(string.Join(Environment.NewLine, message));
            Assert.AreEqual(true, true);
        }

        [Test]
        [Ignore("Adhoc tests to verify live provider information")]
        public void ShouldReturnUniqueProvidersForAStandard()
        {
            var standardApiClient = new StandardApiClient(_url);
            var standards = standardApiClient.FindAll().ToList();
            int error = 0;
            List<string> standardsid = new List<string>();
            List<string> excludeStandards = new List<string> { "108", "116", "127", "136", "141", "149", "158", "161", "163", "164", "166", "171", "172", "173", "174", "175", "176", "177", "178", "179" };
            foreach (var standard in standards.Where(x => !excludeStandards.Contains(x.Id)))
            {
                var uniqueProviders = _sut.GetStandardProviders(standard.Id);
                var duplicate = uniqueProviders.GroupBy(x => x.Ukprn).Where(y => y.Count() > 1).ToList();
                string dupmessage = $"For standard {standard.Id} there are {uniqueProviders.Count()} providers and {uniqueProviders.GroupBy(x => x.Ukprn).Count()} after grouping";
                Console.WriteLine(dupmessage);
                if (duplicate.Count != 0)
                {
                    error++;
                    standardsid.Add(standard.Id);
                    string message = $"For Standard {standard.Id} provider/s are duplicated {string.Join(" , ", duplicate.Select(x => x.First()).Select(y => y.Ukprn))}";
                    Console.WriteLine(message);
                }
            }

            Assert.IsTrue(error == 0, $"looks like there are duplicate providers for standards {string.Join(" , ", standardsid)}");
        }

        [Test]
        [Ignore("Adhoc tests to verify live provider information")]
        public void ShouldReturnUniqueProvidersForAFramework()
        {
            var apiClient = new FrameworkApiClient(_url);
            var frameworks = apiClient.FindAll().ToList();
            int error = 0;
            List<string> appid = new List<string>();
            List<string> excludeapp = new List<string>() { "406-2-1", "406-3-1", "530-2-1", "548-2-1", "560-2-1", "563-21-1", "565-2-1", "565-20-1", "587-2-1", "403-3-10", "550-20-12", "517-2-16", "423-2-3", "446-2-2", "446-2-3", "487-3-2", "502-2-8", "513-2-4", "521-2-7", "524-2-2", "524-3-2", "548-2-3", "551-2-3", "560-2-2", "560-2-3", "560-2-4", "565-20-2", "566-21-2", "582-2-3", "563-20-4", "565-20-3", "563-21-3", "612-21-3", "423-3-6", "423-3-8", "423-3-9", "498-3-3", "502-3-7", "511-3-3", "520-3-6", "520-3-7", "551-3-3", "551-3-9", "560-3-3" };
            foreach (var framework in frameworks.Where(x => !excludeapp.Contains(x.Id)))
            {
                var uniqueProviders = _sut.GetFrameworkProviders(framework.Id);
                var duplicate = uniqueProviders.GroupBy(x => x.Ukprn).Where(y => y.Count() > 1).ToList();
                string dupmessage = $"For framework {framework.Id} there are {uniqueProviders.Count()} providers and {uniqueProviders.GroupBy(x => x.Ukprn).Count()} after grouping";
                Console.WriteLine(dupmessage);
                if (duplicate.Count != 0)
                {
                    error++;
                    appid.Add(framework.Id);
                    string message = $"For framework {framework.Id} provider/s are duplicated {string.Join(" , ", duplicate.Select(x => x.First()).Select(y => y.Ukprn))}";
                    Console.WriteLine(message);
                }
            }

            Assert.IsTrue(error == 0, $"looks like there are duplicate providers for framework {string.Join(" , ", appid)}");
        }

        /*
                [Test]
                public void ShouldNotReturnMoreProvidersComparedToProviderLocationsForAStandard()
                {
                    var standardApiClient = new StandardApiClient(_url);
                    var standards = standardApiClient.FindAll().ToList();
                    int error = 0;
                    List<string> standardsid = new List<string>();
                    List<string> excludeStandards = new List<string> { "108", "116", "127", "136", "141", "149", "158", "161", "163", "164", "166", "171", "172", "173", "174", "175", "176", "177", "178", "179" };
                    foreach (var standard in standards.Where(x => !excludeStandards.Contains(x.Id)))
                    {
                        var uniqueProviders = _sut.GetStandardProviders(standard.Id);
                        var allProviderswithLocations = _sut.GetStandardProviderLocations(int.Parse(standard.Id));
                        int totalLocations = 0;
                        foreach (var provider in uniqueProviders)
                        {
                            var providerlocations = allProviderswithLocations.FirstOrDefault(x => x.Ukprn == provider.Ukprn);
                            totalLocations = totalLocations + providerlocations.TrainingLocations.Count();
                            // string providermessage = $"For Standard {standard.Id}, provider {provider.Ukprn} offer in {providerlocations.TrainingLocations.Count()} locations";
                            // Console.WriteLine(providermessage);
                        }

                        string message = $"For Standard {standard.Id} there are {uniqueProviders.Count()} provider/s offering at {totalLocations} location/s";

                        Console.WriteLine(message);

                        if (uniqueProviders.Count() > totalLocations)
                        {
                            error++;
                            standardsid.Add(standard.Id);
                        }
                    }

                    Assert.IsTrue(error == 0, $"looks like there are more providers than providers locations for standards {string.Join(" , ", standardsid)}");
                }
                
                [Test]
                public void ShouldNotReturnMoreProvidersComparedToProviderLocationsForAFramework()
                {
                    var frameworkApiClient = new FrameworkApiClient(_url);
                    var frameworks = frameworkApiClient.FindAll().ToList();
                    int error = 0;
                    List<string> frameworksid = new List<string>();
                    List<string> excludeframeworks = new List<string>() { "406-2-1", "406-3-1", "530-2-1", "548-2-1", "560-2-1", "563-21-1", "565-2-1", "565-20-1", "587-2-1", "403-3-10", "550-20-12", "517-2-16", "423-2-3", "446-2-2", "446-2-3", "487-3-2", "502-2-8", "513-2-4", "521-2-7", "524-2-2", "524-3-2", "548-2-3", "551-2-3", "560-2-2", "560-2-3", "560-2-4", "565-20-2", "566-21-2", "582-2-3", "563-20-4", "565-20-3", "563-21-3", "612-21-3", "423-3-6", "423-3-8", "423-3-9", "498-3-3", "502-3-7", "511-3-3", "520-3-6", "520-3-7", "551-3-3", "551-3-9", "560-3-3" };
                    foreach (var framework in frameworks.Where(x => !excludeframeworks.Contains(x.Id)).Where(y => y.Id == "536-2-1"))
                    {
                        var uniqueProviders = _sut.GetFrameworkProviders(framework.Id);
                        var allProviderswithLocations = _sut.GetFrameworkProvidersLocation(framework.Id);
                        int totalLocations = 0;
                        foreach (var provider in uniqueProviders)
                        {
                            var providerlocations = allProviderswithLocations.FirstOrDefault(x => x.Ukprn == provider.Ukprn);
                            totalLocations = totalLocations + providerlocations.TrainingLocations.Count();
                             string providermessage = $"For Framework {framework.Id}, provider {provider.Ukprn} - {provider.ProviderName} offer in {providerlocations.TrainingLocations.Count()} locations";
                             Console.WriteLine(providermessage);
                        }

                        string message = $"For Framework {framework.Id} there are {uniqueProviders.Count()} provider/s offering at {totalLocations} location/s";

                        Console.WriteLine(message);

                        if (uniqueProviders.Count() > totalLocations)
                        {
                            error++;
                            frameworksid.Add(framework.Id);
                        }
                    }

                    Assert.IsTrue(error == 0, $"looks like there are more providers than providers locations for standards {string.Join(" , ", frameworksid)}");
                }
               */

        [Test]
        [Ignore("Adhoc tests to verify live provider information")]
        public void ShouldAllProvidersHasContactInformation()
        {
            var result = _sut.FindAll().ToList();

            var emptyemail = result.Where(x => (string.IsNullOrWhiteSpace(x.Email) || string.IsNullOrEmpty(x.Email))).ToList();

            if (emptyemail != null)
            {
                Console.WriteLine($"There are {emptyemail.Count} providers without email :  {string.Join(",", emptyemail.Select(x => x.Ukprn))}");
            }

            var emptywebsite = result.Where(x => (string.IsNullOrWhiteSpace(x.Website) || string.IsNullOrEmpty(x.Website))).ToList();

            if (emptywebsite != null)
            {
                Console.WriteLine($"There are {emptywebsite.Count} providers without website :  {string.Join(",", emptywebsite.Select(x => x.Ukprn))}");
            }

            var emptyphone = result.Where(x => (string.IsNullOrWhiteSpace(x.Phone) || string.IsNullOrEmpty(x.Phone))).ToList();

            if (emptyphone != null)
            {
                Console.WriteLine($"There are {emptyphone.Count} providers without phone :  {string.Join(",", emptyphone.Select(x => x.Ukprn))}");
            }

            var emptyContactInfo = result.Where(x => (string.IsNullOrWhiteSpace(x.Email) || string.IsNullOrEmpty(x.Website) || string.IsNullOrEmpty(x.Phone)));
            Assert.IsTrue(emptyContactInfo == null, $"There are {emptyContactInfo.Count()} providers without contact info :  {string.Join(",", emptyContactInfo.Select(x => x.Ukprn))}");
        }

        [Test]
        [Ignore("Adhoc tests to verify live provider information")]
        public void ShouldAllProvidersHasProviderName()
        {
            var result = _sut.FindAll().ToList();

            var emptyProviderName = result.Where(x => (string.IsNullOrWhiteSpace(x.ProviderName) || string.IsNullOrEmpty(x.ProviderName))).ToList();

            Assert.IsTrue(emptyProviderName.Count == 0, $"There are {emptyProviderName.Count} providers without provider Name :  {string.Join(",", emptyProviderName.Select(x => x.Ukprn))}");

            // Assert.IsTrue(result.TrueForAll(x => (!string.IsNullOrWhiteSpace(_sut.Get(x.Ukprn).ProviderName)) && !string.IsNullOrEmpty(_sut.Get(x.Ukprn).ProviderName)));
            foreach (var providersummary in result)
            {
                var provider = _sut.Get(providersummary.Ukprn);
                Assert.IsTrue(!string.IsNullOrEmpty(provider.ProviderName) && !string.IsNullOrWhiteSpace(provider.ProviderName), $"{provider.Ukprn}'s provider name is empty");
            }
        }
    }
}

