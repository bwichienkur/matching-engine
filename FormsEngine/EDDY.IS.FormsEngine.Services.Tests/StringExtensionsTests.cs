using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Util.StringExtensions.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void BuildCaseInsensitiveDictionaryTest()
        {
            string testDataQueryString = "Postal_Code=33467&Program_Of_Interest=4529&Highest_Level_of_Education_Completed=9&Year_of_Highest_Education_Completed=2010&Desired_Start_Date=Immediately&Age=25&Country=US&us_citizen=Yes&Military_Affiliation=126&Email=brett7432842379879813248@gmail.com&First_Name=test&Last_Name=tester&Address=1234 Straight Road&City=Lake Worth&State=FL&Postal_Code_Duplicate=33467&Phone=(987) 123-4567&Undergraduate_Degree_Education=Yes&GPA=3.5 or Higher&Teaching_certificate=Yes&Years_of_Teaching_Experience=None&International_Credits=Yes&\"UserAgreement = *I acknowledge that, by clicking the checkbox as my official signature, I consent to representatives of Grand Canyon University and  EducationDynamics, LLC contacting me about educational opportunities via email, text, or phone, including my mobile phone number(s) 561.635.6641, using an automatic dialer, or pre-recorded message.Message and data rates may apply. I understand that my consent is not a requirement for enrollment, and I may withdraw my consent at any time.\"&CampusPreference=Online&AffiliateId=&LeadSourceUrl=http://localhost:59955/Clients/Wizard/SinglePage-Start.htm?testparam=test&AdditionalQuestionsShown.Start=true&CampusSoftPreferenceShown=false&CampusPreferenceShown=false";
            Dictionary<string, string> dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            int expectedCount = 28;
            int actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);


            testDataQueryString = "Highest_Level_of_Education_Completed-key=9&Year_of_Highest_Education_Completed-key=2010&Desired_Start_Date-key=1&Country-key=4&us_citizen-key=22&Military_Affiliation-key=126&State-key=11&Undergraduate_Degree_Education-key=22&GPA-key=11&Teaching_certificate-key=22&Years_of_Teaching_Experience-key=17&International_Credits-key=22&Theme=Default";
            dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            expectedCount = 13;
            actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);


            testDataQueryString = "International_Credits-key=22&Theme=Default";
            dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            expectedCount = 2;
            actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);


            testDataQueryString = "Theme=Default";
            dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            expectedCount = 1;
            actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public void BuildCaseInsensitiveDictionaryTest_EmptyString()
        {
            string testDataQueryString = "";
            Dictionary<string, string> dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            int expectedCount = 0;
            int actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);
        }


        [TestMethod]
        public void BuildCaseInsensitiveDictionaryTest_Null()
        {
            string testDataQueryString = null;
            Dictionary<string, string> dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            int expectedCount = 0;
            int actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public void BuildCaseInsensitiveDictionaryTest_NonEncodedUrlValues()
        {
            string testDataQueryString = "FormLeadUrl=http://localhost:59955/Clients/Callcenter/default.htm?trackid=351C687F-E7A2-42F8-A3AA-31B4D4C792F2&LeadSourceUrl=%22http%3A%2F%2Fgoogle.com%2Ftestads.html%3Ftestvendorparam%3Dtest1%26anothertestvendorparam%3Dtest2%22&testparam=testparamvalue&LeadSourceUrl=http://google.com/testads.html?testvendorparam=test1&anothertestvendorparam=test2";
            Dictionary<string, string> dataComposedFromQueryString = testDataQueryString.BuildCaseInsensitiveDictionary();

            int expectedCount = 4;
            int actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public void BuildCaseInsensitiveDictionaryTest_EncodedUrlValues()
        {
            string encodedTestDataQueryStringParameter = "FormLeadUrl=http%3A%2F%2Flocalhost%3A59955%2FClients%2FCallcenter%2Fdefault.htm%3Ftrackid%3D351C687F-E7A2-42F8-A3AA-31B4D4C792F2%26LeadSourceUrl%3D%2522http%253A%252F%252Fgoogle.com%252Ftestads.html%253Ftestvendorparam%253Dtest1%2526anothertestvendorparam%253Dtest2%2522%26testparam%3Dtestparamvalue&LeadSourceUrl=%22http%3A%2F%2Fgoogle.com%2Ftestads.html%3Ftestvendorparam%3Dtest1%26anothertestvendorparam%3Dtest2%22";
            Dictionary<string, string> dataComposedFromQueryString = encodedTestDataQueryStringParameter.BuildCaseInsensitiveDictionary();

            int expectedCount = 2;
            int actualCount = dataComposedFromQueryString.Count();

            Assert.AreEqual(expectedCount, actualCount);
        }

    }
}