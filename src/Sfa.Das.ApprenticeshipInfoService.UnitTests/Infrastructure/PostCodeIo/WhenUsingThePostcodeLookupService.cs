using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.PostCodeIo;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.PostCodeIo
{
    public class WhenUsingThePostcodeLookupService
    {
        private readonly Uri _expectedPostCodeUri = new Uri("https://testpostcode");
        private const string ExpectedPostcode = "AA1 2BB";
        private PostCodeIoLocator _postCodeIoLocator;
        private Mock<IConfigurationSettings> _configurationSettings;
        private Mock<IHttpClient> _httpClient;

        [SetUp]
        public void Arrange()
        {
            _configurationSettings = new Mock<IConfigurationSettings>();
            _configurationSettings.Setup(x => x.PostCodeUrl).Returns(_expectedPostCodeUri);
            _httpClient = new Mock<IHttpClient>();
            _httpClient.Setup(x => x.GetAsync(It.Is<Uri>(c => c.AbsoluteUri.StartsWith(_expectedPostCodeUri.AbsoluteUri))))
                .ReturnsAsync("{'status': 200,'result': {'longitude': -3.141111,'latitude': 14.513234}}");
            _httpClient.Setup(x => x.GetAsync(It.Is<Uri>(c => !c.AbsoluteUri.Contains("AA1"))))
                .ReturnsAsync("{'status': 404,'result': {}}");
            _postCodeIoLocator = new PostCodeIoLocator(_configurationSettings.Object, _httpClient.Object);
        }

        [Test]
        public async Task Then_The_Uri_Is_Taken_From_The_Application_Settings()
        {
            await _postCodeIoLocator.GetLatLongFromPostcode(string.Empty);

            _configurationSettings.Verify(x => x.PostCodeUrl);
        }

        [Test]
        public async Task Then_The_Url_Has_The_Postcode_In_The_Params_Used_For_The_Get_Request()
        {
            var expectedUri = new Uri(_expectedPostCodeUri, ExpectedPostcode);

            await _postCodeIoLocator.GetLatLongFromPostcode(ExpectedPostcode);

            _httpClient.Verify(x => x.GetAsync(It.Is<Uri>(c => c.AbsoluteUri.Equals(expectedUri.AbsoluteUri))),Times.Once);
        }

        [Test]
        public async Task Then_The_Lat_And_Long_Are_Read_From_The_Response()
        {
            var actual = await _postCodeIoLocator.GetLatLongFromPostcode(ExpectedPostcode);

            Assert.IsAssignableFrom<PostCodeResponse>(actual);
            Assert.AreEqual(200, actual.Status);
            Assert.AreEqual(14.513234, actual.Result.Latitude);
            Assert.AreEqual(-3.141111, actual.Result.Longitude);
        }

        [Test]
        public async Task Then_If_The_Postcode_Is_Not_Found_A_404_Is_Returned()
        {
            var expectedPostCode = string.Empty;

            var actual = await _postCodeIoLocator.GetLatLongFromPostcode(expectedPostCode);

            Assert.IsAssignableFrom<PostCodeResponse>(actual);
            Assert.AreEqual(404, actual.Status);
        }
    }
}
