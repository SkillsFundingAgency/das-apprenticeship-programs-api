using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.enums;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests
{
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
           Assert.IsTrue(deserialisedDeliveryModes.Contains("100PercentEmployer"), $"'100PercentEmployer' was expected in the list of delivery modes [{listOfDeliveryModes}]");
           Assert.IsTrue(deserialisedDeliveryModes.Contains("DayRelease"), $"'DayRelease' was expected in the list of delivery modes [{listOfDeliveryModes}]");
           Assert.IsTrue(deserialisedDeliveryModes.Contains("BlockRelease"), $"'BlockRelease' was expected in the list of delivery modes [{listOfDeliveryModes}]");
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

    }
}
