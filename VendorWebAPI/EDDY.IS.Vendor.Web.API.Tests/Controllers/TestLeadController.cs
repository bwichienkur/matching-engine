using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;

namespace EDDY.IS.Vendor.Web.API.Tests.Controllers
{
    [TestClass]
    public class TestLeadController
    {
        [TestMethod]
        public void GetProgramMatches_ShouldReturnPrograms()
        {
            Leads leads = new Leads();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");

            contactRequest.CategoryIds = new List<int>() { 25, 21 };;
            contactRequest.PostalCode = "21113";
            contactRequest.SubjectIds = new List<int>() {658};
            contactRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            contactRequest.StartPage = 1;
            contactRequest.PageSize = 10;
            contactRequest.AdditionalQuestions = null;
            

            contactRequest.Prefix = "Mr.";
            contactRequest.FirstName = "Test";
            contactRequest.LastName = "Test";

            contactRequest.Address = "801 Iron Corner Ct";
            contactRequest.Address2 = "apt a";

            contactRequest.City = "Odenton";

            contactRequest.PostalCode = "21113";

            contactRequest.State = "MD";

            contactRequest.Country = "US";

            contactRequest.Email = "test@test.com";

            contactRequest.Phone = "8566250538";

            contactRequest.AlternatePhone = "8566250539";

            contactRequest.Age = 21;

            contactRequest.YearHighestEducationCompleted = 2016;

            contactRequest.HighestLevelofEducationCompleted = "9";
            contactRequest.MilitaryAffiliation = "126";

            contactRequest.DesiredStartDate = "Immediately";

            contactRequest.USCitizen = "Yes";

            contactRequest.SubjectIds = new List<int>() {658};

            contactRequest.ProgramLevelId = 2;

            contactRequest.ProgramId = 7243;

            contactRequest.ProgramId = 7243;
            contactRequest.LeadIdToken = "7599527B-587A-A01C-3142-CB363F84D778";


            contactRequest.IncludeAdditionalProgramQuestions = true;

            contactRequest.AdditionalQuestions = new List<QuestionAnswer>();

            VendorResponseBase response = leads.SaveLead(contactRequest);

            LeadSubmissionResponse leadResponse = response.Body as LeadSubmissionResponse;
           

            Assert.IsNotNull(leadResponse);


        }
    }
}
