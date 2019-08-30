namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Services
{
    using System;
    using ApprenticeshipInfoService.Infrastructure.Services;
    using Core.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Internal;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class GetIfaStandardsUrlServiceTests
    {
        private Mock<ILogger<GetIfaStandardsUrlService>> _mockLogger;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;

        [Test]
        public void TestBadLinkReturnsEmptyStringAndIsLogged()
        {
             _mockLogger = new Mock<ILogger<GetIfaStandardsUrlService>>();
            _mockConfigurationSettings = new Mock<IConfigurationSettings>();
            const string ifaUrlBase = "http://sss.dummylink.con";
            _mockConfigurationSettings.Setup(x => x.IfaStandardApiUrl).Returns(ifaUrlBase);
            const string standardId = "5";

            var service = new GetIfaStandardsUrlService(_mockLogger.Object, _mockConfigurationSettings.Object);

            var res = service.GetStandardUrl(standardId);

            var urlToCall = $"{ifaUrlBase}/{standardId}";
            var logWarning = $"IFA Url [{urlToCall}] failed to return details";

            Assert.AreEqual(string.Empty, res);
            _mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<FormattedLogValues>(a => a.ToString() == logWarning), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
    }
}
