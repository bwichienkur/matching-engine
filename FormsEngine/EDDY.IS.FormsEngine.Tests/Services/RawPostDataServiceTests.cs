using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.LeadEngine.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class RawPostDataServiceTests
    {
        [Theory]
        [InlineData("LeadData")]
        [InlineData("")]
        [InlineData(null)]
        public void GetRawPostDataObjectTest_SetPostData(string leadData)
        {
            var mockContext = GetHttpContextMock();
            var mockIPAddressService = new Mock<IIPAddressService>();
            var rawPostDataService = new RawPostDataService(mockContext.Object, mockIPAddressService.Object);

            RawPostDataDTO rawPostData = rawPostDataService.GetRawPostDataDTO(leadData);

            Assert.Equal(leadData, rawPostData.PostData);
        }

        [Fact]
        public void GetRawPostDataObjectTest_SetReferer()
        {
            string serverVariableName = "HTTP_REFERER";
            string serverVariableValue = "TestReferer";
            var mockContext = GetHttpContextMock(serverVariableName, serverVariableValue);
            var mockIPAddressService = new Mock<IIPAddressService>();
            var rawPostDataService = new RawPostDataService(mockContext.Object, mockIPAddressService.Object);
            string leadData = string.Empty;

            RawPostDataDTO rawPostData = rawPostDataService.GetRawPostDataDTO(leadData);

            Assert.Equal(serverVariableValue, rawPostData.Referer);
        }

        [Fact]
        public void GetRawPostDataObjectTest_SetUserAgent()
        {
            string serverVariableName = "HTTP_USER_AGENT";
            string serverVariableValue = "TestUserAgent";
            var mockContext = GetHttpContextMock(serverVariableName, serverVariableValue);
            var mockIPAddressService = new Mock<IIPAddressService>();
            var rawPostDataService = new RawPostDataService(mockContext.Object, mockIPAddressService.Object);
            string leadData = string.Empty;

            RawPostDataDTO rawPostData = rawPostDataService.GetRawPostDataDTO(leadData);

            Assert.Equal(serverVariableValue, rawPostData.BrowserInfo);
        }

        [Fact]
        public void GetRawPostDataObjectTest_SetIPAddress()
        {
            var mockContext = GetHttpContextMock();
            var mockIPAddressService = new Mock<IIPAddressService>();

            string ipAddress = "0.0.0.0";
            mockIPAddressService.Setup(s => s.GetIPAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(ipAddress);

            var rawPostDataService = new RawPostDataService(mockContext.Object, mockIPAddressService.Object);
            string leadData = string.Empty;

            RawPostDataDTO rawPostData = rawPostDataService.GetRawPostDataDTO(leadData);

            Assert.Equal(ipAddress, rawPostData.RemoteIp);
        }

        private Mock<HttpContextBase> GetHttpContextMock()
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = GetHttpRequestMock();
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            return mockContext;
        }

        private Mock<HttpRequestBase> GetHttpRequestMock()
        {
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection());
            return mockRequest;
        }

        private Mock<HttpContextBase> GetHttpContextMock(string serverVariableName, string serverVariableValue)
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = GetHttpRequestMock(serverVariableName, serverVariableValue);
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            return mockContext;
        }

        private Mock<HttpRequestBase> GetHttpRequestMock(string serverVariableName, string serverVariableValue)
        {
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection { { serverVariableName, serverVariableValue } });
            return mockRequest;
        }
    }
}