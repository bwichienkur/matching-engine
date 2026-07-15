using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.LeadEngine.DTO;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class RawPostDataMapperTests
    {
        [Fact]
        public void MapFormInputToRawPostDataTest_NotNull()
        {
            var formInput = new FormInput();

            RawPostDataDTO rawPostData = MapFormInputToRawPostData(formInput);

            Assert.NotNull(rawPostData);
        }

        [Fact]
        public void MapFormInputToRawPostDataTest_RefererMapped()
        {
            var formInput = new FormInput();
            formInput.HttpReferer = "TestReferer";

            RawPostDataDTO rawPostData = MapFormInputToRawPostData(formInput);

            Assert.Equal(formInput.HttpReferer, rawPostData.Referer);
        }

        [Fact]
        public void MapFormInputToRawPostDataTest_IPAddressMapped()
        {
            var formInput = new FormInput();
            formInput.IpAddress = "0.0.0.0";

            RawPostDataDTO rawPostData = MapFormInputToRawPostData(formInput);

            Assert.Equal(formInput.IpAddress, rawPostData.RemoteIp);
        }

        [Fact]
        public void MapFormInputToRawPostDataTest_UserAgentMapped()
        {
            var formInput = new FormInput();
            formInput.UserAgent = "TestUserAgent";

            RawPostDataDTO rawPostData = MapFormInputToRawPostData(formInput);

            Assert.Equal(formInput.UserAgent, rawPostData.BrowserInfo);
        }

        [Fact]
        public void MapFormInputToRawPostDataTest_LeadDataMapped()
        {
            var formInput = new FormInput();
            formInput.LeadData = "TestPostData";

            RawPostDataDTO rawPostData = MapFormInputToRawPostData(formInput);

            Assert.Equal(formInput.LeadData, rawPostData.PostData);
        }

        private RawPostDataDTO MapFormInputToRawPostData(FormInput formInput)
        {
            var mapper = new RawPostDataMapper();
            return mapper.MapFormInputToRawPostData(formInput);
        }
    }
}