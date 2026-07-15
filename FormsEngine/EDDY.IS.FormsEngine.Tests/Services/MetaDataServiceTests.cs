using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class SchoolPickerWizardMetaDataServiceTests
    {
        private readonly Dictionary<string, string> _mockMetaDataMessages = new Dictionary<string, string>
        {
            { "SCHOOLPICKER.MESSAGE.ONE", "First Test Message" },
            { "SCHOOLPICKER.MESSAGE.TWO", "Second Test Message" },
            { "SCHOOLPICKER.MESSAGE.THREE", "Third Test Message" }
        };

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("SCHOOLPICKER.MESSAGE.ONE")]
        [InlineData("SCHOOLPICKER.MESSAGE.TWO")]
        [InlineData("SCHOOLPICKER.MESSAGE.THREE")]
        public void MetaDataServiceTest_GetMetaDataMessageByKeyTest(string key)
        {
            _mockMetaDataMessages.TryGetValue(key ?? string.Empty, out string mockMessage);
            mockMessage = mockMessage ?? string.Empty;

            var mockMetaDataRepository = new Mock<IMetaDataRepository>();
            mockMetaDataRepository.Setup(r => r.GetMetaDataMessageByKey(It.Is<string>(s => s.Equals(key)))).Returns(mockMessage);
            var metaDataService = new MetaDataService(mockMetaDataRepository.Object);

            var message = metaDataService.GetMetaDataMessageByKey(key);

            Assert.Equal(mockMessage, message);
        }

        [Theory]
        [InlineData("SCHOOLPICKERWIZARD")]
        [InlineData("schoolpickerwizard")]
        public void MetaDataServiceTest_GetMetaDataMessagesByPrefix(string prefix)
        {
            var mockMetaDataRepository = new Mock<IMetaDataRepository>();
            mockMetaDataRepository.Setup(r => r.GetMetaDataMessagesByPrefix(It.Is<string>(s => s.Equals(prefix.ToUpper())))).Returns(_mockMetaDataMessages);

            var metaDataService = new MetaDataService(mockMetaDataRepository.Object);
            
            var metaDataMessages = metaDataService.GetMetaDataMessagesByPrefix(prefix);

            Assert.Equal(_mockMetaDataMessages, metaDataMessages);
        }

        [Fact]
        public void MetaDataServiceTest_GetMetaDataMessagesByPrefix_NullPrefix()
        {
            string prefix = null;

            var mockMetaDataRepository = new Mock<IMetaDataRepository>();
            var metaDataService = new MetaDataService(mockMetaDataRepository.Object);

            var metaDataMessages = metaDataService.GetMetaDataMessagesByPrefix(prefix);

            Dictionary<string, string> emptyExpectedResult = new Dictionary<string, string>();
            metaDataMessages.Should().BeEquivalentTo(emptyExpectedResult);
        }

        [Fact]
        public void MetaDataServiceTest_GetMetaDataMessagesByPrefix_NotNull()
        {
            string prefix = "PREFIX";

            Dictionary<string, string> mockMetaDataMessages = null;

            var mockMetaDataRepository = new Mock<IMetaDataRepository>();
            mockMetaDataRepository.Setup(r => r.GetMetaDataMessagesByPrefix(It.Is<string>(s => s.Equals(prefix)))).Returns(mockMetaDataMessages);

            var metaDataService = new MetaDataService(mockMetaDataRepository.Object);

            var metaDataMessages = metaDataService.GetMetaDataMessagesByPrefix(prefix);

            Dictionary<string, string> emptyExpectedResult = new Dictionary<string, string>();
            metaDataMessages.Should().BeEquivalentTo(emptyExpectedResult);
        }

    }
}