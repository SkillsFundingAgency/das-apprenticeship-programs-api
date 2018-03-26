using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Configuration;
using Moq;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.UnitTests.Infrastructure.Services
{
    [TestFixture]
    public class GetIfaStandardsUrlServiceTests
    {
        private Mock<ILog> _mockLogger;
        private Mock<IConfigurationSettings> _mockConfigurationSettings;

        [Test]
        public void TestBadLinkReturnsEmptyStringAndIsLogged()
        {
             _mockLogger = new Mock<ILog>();
            _mockConfigurationSettings = new Mock<IConfigurationSettings>();
            const string ifaUrlBase = "http://sss.dummylink.con";
            _mockConfigurationSettings.Setup(x => x.IfaStandardApiUrl).Returns(ifaUrlBase);
            const string standardId = "5";

            var service = new GetIfaStandardsUrlService(_mockLogger.Object, _mockConfigurationSettings.Object);

            var res = service.GetStandardUrl(standardId);

            var urlToCall = $"{ifaUrlBase}/{standardId}";
            var logWarning = $"IFA Url [{urlToCall}] failed to return details";

            Assert.AreEqual(string.Empty, res);
            _mockLogger.Verify(x => x.Warn(It.IsAny<Exception>(), logWarning), Times.Once);

        }
    }
}
