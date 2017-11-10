using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.enums;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests
{
    [TestFixture]
    public class JsonEnumSerializerTests
    {
       [Test]
        public void ShouldReturnExpectedListOfStringFromListOfDeliveryModeEnums()
       {
           var deliveryModes = new List<DeliveryMode>
           {
               DeliveryMode.BlockRelease,
               DeliveryMode.DayRelease,
               DeliveryMode.HundredPercentEmployer
           };

           var json = JsonConvert.SerializeObject(deliveryModes);

           var deserialisedDeliveryModes = JsonConvert.DeserializeObject<List<string>>(json, _jsonSettings);

           var listOfDeliveryModes = string.Join(",", deserialisedDeliveryModes);

               Assert.IsTrue(deserialisedDeliveryModes.Contains("100percentemployer"), $"'100percentemployer' was expected in the list of delivery modes [{listOfDeliveryModes}]");
               Assert.IsTrue(deserialisedDeliveryModes.Contains("dayrelease"), $"'dayrelease' was expected in the list of delivery modes [{listOfDeliveryModes}]");
               Assert.IsTrue(deserialisedDeliveryModes.Contains("blockrelease"), $"'blockrelease' was expected in the list of delivery modes [{listOfDeliveryModes}]");
       }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

    }
}
