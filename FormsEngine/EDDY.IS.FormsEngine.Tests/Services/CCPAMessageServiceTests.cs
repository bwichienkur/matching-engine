using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class CCPAMessageServiceTests
    {
        private Dictionary<string, string> MockCCPAMessages
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "CCPA.BASEMESSAGE", "Base CCPA Message" }
                };
            }
        }

        [Fact]
        public void BaseCCPAMessage()
        {
            string ccpaMessagePrefix = "CCPA";

            string expectedCCPAMessage = MockCCPAMessages["CCPA.BASEMESSAGE"];

            var mockMetaDataService = new Mock<IMetaDataService>();
            mockMetaDataService.Setup(s => s.GetMetaDataMessagesByPrefix(It.Is<string>(p => p == ccpaMessagePrefix))).Returns(MockCCPAMessages);

            var ccpaMessageService = new CCPAMessageService(mockMetaDataService.Object);

            string actualCCPAMessage = ccpaMessageService.BaseCCPAMessage;

            Assert.Equal(expectedCCPAMessage, actualCCPAMessage);
        }

        [Fact]
        public void BaseCCPAMessage_EmptyResult()
        {
            var mockMetaDataService = new Mock<IMetaDataService>();
            mockMetaDataService.Setup(s => s.GetMetaDataMessagesByPrefix(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            var ccpaMessageService = new CCPAMessageService(mockMetaDataService.Object);

            string actualCCPAMessage = ccpaMessageService.BaseCCPAMessage;

            Assert.Equal(string.Empty, actualCCPAMessage);
        }

    }
}
