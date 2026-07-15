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
    public class TestDirectoryController
    {
        [TestMethod]
        public void GetPrograms_ShouldReturnPrograms()
        {
            Programs programs = new Programs();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("8696D456-4F79-4AEF-BAC6-D1D25B521704");
            directoryRequest.CampusType = "all";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.PostalCode = "21113";
            directoryRequest.SubjectIds = new List<int>() {658};
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;
            directoryRequest.ProspectFilters = null;
            
            VendorAPIList list = programs.GetPrograms(directoryRequest);


            Assert.IsTrue(list.ItemList.Count > 0, "We have programs");


        }

        [TestMethod]
        public void GetPrograms_ShouldNotBeNull()
        {
            Programs programs = new Programs();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("8696D456-4F79-4AEF-BAC6-D1D25B521704");
            directoryRequest.CampusType = "all";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.PostalCode = "21113";
            directoryRequest.SubjectIds = new List<int>() {658};
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;
            directoryRequest.ProspectFilters = null;
          
            VendorAPIList list = programs.GetPrograms(directoryRequest);
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetProgramMatches_ShouldReturnPrograms()
        {
            Programs programs = new Programs();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
    
            contactRequest.CategoryIds = new List<int>() { 25, 21 };
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

            contactRequest.StartPage = 1;

            contactRequest.PageSize = 10;

            contactRequest.IncludeAdditionalProgramQuestions = true;


            contactRequest.AdditionalQuestions = new List<QuestionAnswer>();
            
            //"RN_license|No,Undergraduate_Degree_Nursing|No";
            VendorResponseBase response = programs.GetProgramMatches(contactRequest);
            VendorAPIList list = response.Body as VendorAPIList;

            Assert.IsTrue(list.ItemList.Count > 0, "We have program matches");


        }

        [TestMethod]
        public void GetProgramMatches_ShouldNotBeNull()
        {
            Programs programs = new Programs();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");

            contactRequest.CategoryIds = new List<int>() { 25, 21 };
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

            contactRequest.StartPage = 1;

            contactRequest.PageSize = 10;

            contactRequest.IncludeAdditionalProgramQuestions = true;

            contactRequest.AdditionalQuestions = new List<QuestionAnswer>();


            VendorResponseBase response = programs.GetProgramMatches(contactRequest);



            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetInstitutions_ShouldNotBeNull()
        {
            Institutions institutions = new Institutions();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusType = "all";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.PostalCode = "21113";
            directoryRequest.SubjectIds = new List<int>() {658};
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = institutions.GetInstitutions(directoryRequest);

            Assert.IsNotNull(list);


        }

        [TestMethod]
        public void GetInstitutions_ShouldReturnInstitutions()
        {
            Institutions institutions = new Institutions();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusType = "all";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.PostalCode = "21113";
            directoryRequest.SubjectIds = new List<int>() {658};
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;
          
            VendorAPIList list = institutions.GetInstitutions(directoryRequest);


            


            Assert.IsTrue(list.ItemList.Count > 0, "We have Institutions");


        }

        [TestMethod]
        public void GetCampuses_ShouldNotBeNull()
        {
            Campuses campuses = new Campuses();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = campuses.GetCampuses(directoryRequest);

            Assert.IsNotNull(list);


        }

        [TestMethod]
        public void GetCampuses_ShouldReturnCampuses()
        {
            Campuses campuses = new Campuses();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;

            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = campuses.GetCampuses(directoryRequest);





            Assert.IsTrue(list.ItemList.Count > 0, "We have Institutions");


        }

        [TestMethod]
        public void GetCategories_ShouldNotBeNull()
        {
            Categories categories = new Categories();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = categories.GetCategories(directoryRequest);

            Assert.IsNotNull(list);


        }

        [TestMethod]
        public void GetCategories_ShouldReturnCategories()
        {
            Categories categories = new Categories();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = categories.GetCategories(directoryRequest);

            Assert.IsTrue(list.ItemList.Count > 0, "We have Categories");


        }

        [TestMethod]
        public void GetSubCategories_ShouldNotBeNull()
        {
            Categories categories = new Categories();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = categories.GetSubCategories(directoryRequest);

            Assert.IsNotNull(list);


        }

        [TestMethod]
        public void GetSubCategories_ShouldReturnSubCategories()
        {
            Categories categories = new Categories();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = categories.GetSubCategories(directoryRequest);

            Assert.IsTrue(list.ItemList.Count > 0, "We have SubCategories");


        }

        [TestMethod]
        public void GetLevels_ShouldNotBeNull()
        {
            Levels levels = new Levels();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.SubjectIds = new List<int>() {658};
     
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = levels.GetLevels(directoryRequest);

            Assert.IsNotNull(list);


        }

        [TestMethod]
        public void GetLevels_ShouldReturnLevels()
        {
            Levels levels = new Levels();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            directoryRequest.CampusId = 9306;
            directoryRequest.PostalCode = "21113";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.SubjectIds = new List<int>() {658};
          
            directoryRequest.StartPage = 1;
            directoryRequest.PageSize = 10;

            VendorAPIList list = levels.GetLevels(directoryRequest);

            Assert.IsTrue(list.ItemList.Count > 0, "We have Levels");


        }

        [TestMethod]
        public void GetCheckAllocation_ShouldReturnLevels()
        {
            Programs programs = new Programs();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
      
            directoryRequest.PostalCode = "21113";
            directoryRequest.CategoryIds = new List<int>() { 25, 21 };
            directoryRequest.SubjectIds = new List<int>() {658};
            directoryRequest.ProgramLevelIds = new List<int>() { 2, 3, 6, 18, 22, 8 };


            AllocationStatus response = programs.CheckAllocation(directoryRequest);

            Assert.IsNotNull(response);


        }
    }
}
