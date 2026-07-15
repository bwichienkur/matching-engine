using EDDY.IS.FormsEngine.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDDY.IS.FormsEngine.Core.Helpers.Tests
{
    public class JsonParserTests
    {
        [Fact]
        public void JsonStringToDictionaryTest_NotNull()
        {
            var parser = new JsonParser();
            var result = parser.JsonStringToDictionary("");

            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("{\"8601\":631540, \"8603\":631542}")]
        [InlineData("{8601:\"631540\", 8603:\"631542\"}")]
        [InlineData("{\"8601\":\"631540\", \"8603\":\"631542\"}")]
        [InlineData("{8601:631540, 8603:631542}")]
        [InlineData("{'8601':'631540', '8603':'631542'}")]
        [InlineData("{'8601':631540, '8603':631542}")]
        [InlineData("{8601:'631540', 8603:'631542'}")]
        public void JsonStringToDictionaryTest_Integers(string json)
        {
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { "8601", "631540" },
                { "8603", "631542" }
            };
            
            var parser = new JsonParser();
            var actual = parser.JsonStringToDictionary(json);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("{\"firstName\": \"test\", \"lastName\":\"tester\"}")]
        [InlineData("{firstName: \"test\", lastName:\"tester\"}")]
        public void JsonStringToDictionaryTest_Strings(string json)
        {
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { "firstName", "test" },
                { "lastName", "tester" }
            };

            var parser = new JsonParser();
            var actual = parser.JsonStringToDictionary(json);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("{firstName: test, lastName: tester}")]
        public void JsonStringToDictionaryTest_InvalidJson(string json)
        {
            int expectedCount = 0;

            var parser = new JsonParser();
            var actual = parser.JsonStringToDictionary(json);

            Assert.Equal(expectedCount, actual.Count());
        }

    }
}