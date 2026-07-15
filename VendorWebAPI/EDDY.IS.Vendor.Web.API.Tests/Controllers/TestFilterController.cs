using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Web.API.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace EDDY.IS.Vendor.Web.API.Tests.Controllers
{
    [TestClass]
    public class TestFilterController
    {
        #region ProgramFilter Tests

        #region ProgramId Not Numeric Tests

        [TestMethod]
        public void TestProgramFilter_ProgramIdNotNumeric()
        {
            string jsonString = "{ 'ProgramId': 'non numeric value' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new ProgramFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0074"));
        }
        
        #endregion

        #region Program Unvailable Tests

        [TestMethod]
        public void TestProgramFilter_ProgramIsUnavailableNotEnabled()
        {
            string jsonString = "{ 'ProgramId': '99999' }";

            var program = new Entities.Program
            {
                ProgramId = 99999,
                IsEnabled = false
            };

            var mockPrograms = new Mock<Business.Programs>();
            mockPrograms.Setup(m => m.GetProgram(program.ProgramId)).Returns(program);

            var programFilter = new ProgramFilter();
            programFilter.Programs = mockPrograms.Object;

            VendorResponseBase response = GetActionFilterResponse(jsonString, programFilter);
            Assert.IsTrue(response.MessageCodes.Contains("msg0085"));
        }

        [TestMethod]
        public void TestProgramFilter_ProgramIsUnavailableAndNull()
        {
            string jsonString = "{ 'ProgramId': '99999' }";

            Entities.Program program = null;

            var mockPrograms = new Mock<Business.Programs>();
            mockPrograms.Setup(m => m.GetProgram(99999)).Returns(program);

            var programFilter = new ProgramFilter();
            programFilter.Programs = mockPrograms.Object;

            VendorResponseBase response = GetActionFilterResponse(jsonString, programFilter);
            Assert.IsTrue(response.MessageCodes.Contains("msg0085"));
        }

        [TestMethod]
        public void TestProgramFilter_ProgramIsUnavailableFail()
        {
            string jsonString = "{ 'ProgramId': '99999' }";

            var program = new Entities.Program
            {
                ProgramId = 99999,
                IsEnabled = true
            };

            var mockPrograms = new Mock<Business.Programs>();
            mockPrograms.Setup(m => m.GetProgram(program.ProgramId)).Returns(program);

            var programFilter = new ProgramFilter();
            programFilter.Programs = mockPrograms.Object;

            VendorResponseBase response = GetActionFilterResponse(jsonString, programFilter);
            Assert.IsNull(response);
        }
        #endregion

        #endregion

        #region SubjectFilter Tests

        #region SubjectIds Not Numeric Tests

        [TestMethod]
        public void TestSubjectFilter_SubjectIdsNotNumericPass()
        {
            string jsonString = "{ 'SubjectIds': ['non numeric value', 'non numeric value'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new SubjectFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0042"));
        }

        [TestMethod]
        public void TestSubjectFilter_SubjectIdsNotNumericFail()
        {
            string jsonString = "{ 'SubjectIds': ['4', '5'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new SubjectFilter());
            Assert.IsNull(response);
        }

        #endregion

        #region Subject Unvailable Tests

        [TestMethod]
        public void TestSubjectFilter_SubjectIsUnavailablePass()
        {
            string jsonString = "{ 'SubjectIds': [99999] }";

            var subject = new Subject
            {
                SubjectId = 99999,
                IsEnabled = false
            };

            var mockSubjects = new Mock<Business.Subjects>();
            mockSubjects.Setup(m => m.GetSubject(subject.SubjectId)).Returns(subject);

            var disabledSubjectFilter = new DisabledSubjectFilter();
            disabledSubjectFilter.Subjects = mockSubjects.Object;
            var response = GetActionFilterResponse(jsonString, disabledSubjectFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0087"));
        }

        [TestMethod]
        public void TestSubjectFilter_SubjectIsUnavailableFail()
        {
            string jsonString = "{ 'SubjectIds': [99999] }";

            var subject = new Subject
            {
                SubjectId = 99999,
                IsEnabled = true
            };

            var mockSubjects = new Mock<Business.Subjects>();
            mockSubjects.Setup(m => m.GetSubject(subject.SubjectId)).Returns(subject);

            var disabledSubjectFilter = new DisabledSubjectFilter();
            disabledSubjectFilter.Subjects = mockSubjects.Object;
            var response = GetActionFilterResponse(jsonString, disabledSubjectFilter);

            Assert.IsNull(response);
        }

        #endregion

        #endregion

        #region Category Filter Tests
        
        #region CategoryIds Not Numeric 
        [TestMethod]
        public void TestCategoryFilter_CategoryIdsNotNumericPass()
        {
            string jsonString = "{ 'CategoryIds': ['non numeric value', 'non numeric value'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new CategoryFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0040"));
        }

        [TestMethod]
        public void TestCategoryFilter_CategoryIdsNotNumericFail()
        {
            string jsonString = "{ 'CategoryIds': ['4', '5'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new CategoryFilter());
            Assert.IsNull(response);
        }
        #endregion

        #region Category Unavailable Tests
        [TestMethod]
        public void TestCategoryFilter_CategoryIsUnavailablePass()
        {
            string jsonString = "{ 'CategoryIds': [99999] }";

            var category = new Category
            {
                CategoryId = 99999,
                IsEnabled = false
            };

            var mockCategories = new Mock<Business.Categories>();
            mockCategories.Setup(m => m.GetCategory(category.CategoryId)).Returns(category);

            var disabledCategoryFilter = new DisabledCategoryFilter();
            disabledCategoryFilter.Categories = mockCategories.Object;
            var response = GetActionFilterResponse(jsonString, disabledCategoryFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0086"));
        }

        [TestMethod]
        public void TestCategoryFilter_CategoryIsUnavailableFail()
        {
            string jsonString = "{ 'CategoryIds': [99999] }";

            var category = new Category
            {
                CategoryId = 99999,
                IsEnabled = true
            };

            var mockCategories = new Mock<Business.Categories>();
            mockCategories.Setup(m => m.GetCategory(category.CategoryId)).Returns(category);

            var disabledCategoryFilter = new DisabledCategoryFilter();
            disabledCategoryFilter.Categories = mockCategories.Object;
            var response = GetActionFilterResponse(jsonString, disabledCategoryFilter);

            Assert.IsNull(response);
        }
        #endregion

        #endregion

        #region Campus Filter Tests

        #region Campus Id Not Numeric 
        [TestMethod]
        public void TestCampusFilter_CampusIdNotNumericPass()
        {
            string jsonString = "{ 'CampusId': 'abc' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new CampusFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0072"));
        }

        [TestMethod]
        public void TestDisabledCampusFilter_CampusIdNotNumericPass()
        {
            string jsonString = "{ 'CampusId': 'abc' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new DisabledCampusFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0072"));
        }

        [TestMethod]
        public void TestCampusFilter_CampusIdNotNumericFail()
        {
            string jsonString = "{ 'CampusId': '4' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new CampusFilter());
            Assert.IsNull(response);
        }
        
        #endregion

        #region Campus Unavailable Tests
        [TestMethod]
        public void TestDisabledCampusFilter_CampusIsUnavailablePass()
        {
            string jsonString = "{ 'CampusId': 99999 }";

            var campus = new Campus
            {
                CampusId = 99999,
                IsEnabled = false
            };

            var mockCampuses = new Mock<Business.Campuses>();
            mockCampuses.Setup(m => m.GetCampus(campus.CampusId)).Returns(campus);

            var disabledCampusFilter = new DisabledCampusFilter();
            disabledCampusFilter.Campuses = mockCampuses.Object;
            var response = GetActionFilterResponse(jsonString, disabledCampusFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0090"));
        }

        [TestMethod]
        public void TestDisabledCampusFilter_CampusIsUnavailableFail()
        {
            string jsonString = "{ 'CampusId': 99999 }";

            var campus = new Campus
            {
                CampusId = 99999,
                IsEnabled = true
            };

            var mockCampuses = new Mock<Business.Campuses>();
            mockCampuses.Setup(m => m.GetCampus(campus.CampusId)).Returns(campus);

            var disabledCampusFilter = new DisabledCampusFilter();
            disabledCampusFilter.Campuses = mockCampuses.Object;
            var response = GetActionFilterResponse(jsonString, disabledCampusFilter);

            Assert.IsNull(response);
        }
        #endregion

        #endregion

        #region Institution Filter Tests

        #region InstitutionIds Not Numeric
        [TestMethod]
        public void TestInstitutionFilter_InstitutionIdsNotNumericPass()
        {
            string jsonString = "{ 'InstitutionId': ['non numeric value'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new InstitutionFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0071"));
        }
        
        #endregion

        #region Institution Unavailable Tests

        [TestMethod]
        public void TestInstitutionFilter_InstitutionIsUnavailablePass()
        {
            string jsonString = "{ 'InstitutionId': 99999 }";

            var institution = new Institution
            {
                InstitutionId = 99999,
                IsEnabled = false
            };

            var mockInstitutions = new Mock<Business.Institutions>();
            mockInstitutions.Setup(m => m.GetInstitution(institution.InstitutionId)).Returns(institution);

            var institutionFilter = new InstitutionFilter();
            institutionFilter.Institutions = mockInstitutions.Object;
            var response = GetActionFilterResponse(jsonString, institutionFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0088"));
        }

        #endregion

        [TestMethod]
        public void TestInstitutionFilter_PassesAllValidations()
        {
            string jsonString = "{ 'InstitutionId': 99999 }";

            var institution = new Institution
            {
                InstitutionId = 99999,
                IsEnabled = true
            };

            var mockInstitutions = new Mock<Business.Institutions>();
            mockInstitutions.Setup(m => m.GetInstitution(institution.InstitutionId)).Returns(institution);

            var institutionFilter = new InstitutionFilter();
            institutionFilter.Institutions = mockInstitutions.Object;
            var response = GetActionFilterResponse(jsonString, institutionFilter);

            Assert.IsNull(response);
        }

        #endregion

        #region ProgramLevel Filter Tests

        [TestMethod]
        public void TestProgramLevelFilter_ProgramLevelIdsNotNumericPass()
        {
            string jsonString = "{ 'ProgramLevelIds': ['non numeric value', 'non numeric value'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new ProgramLevelFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0070"));
        }
        
        [TestMethod]
        public void TestProgramLevelFilter_ProgramLevelIdNotNumericPass()
        {
            string jsonString = "{ 'ProgramLevelId': ['non numeric value'] }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new ProgramLevelFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0070"));
        }

        [TestMethod]
        public void TestProgramLevelFilter_PassesAllValidations()
        {
            string jsonString = "{ 'ProgramLevelIds': [99999], 'ProgramLevelId': 99999 }";

            var level = new Entities.Level
            {
                ProgramLevelId = 99999,
                IsEnabled = true
            };

            var mockProgramLevels = new Mock<Business.ProgramLevels>();
            mockProgramLevels.Setup(m => m.GetProgramLevel(level.ProgramLevelId)).Returns(level);

            var programLevelFilter = new ProgramLevelFilter();
            programLevelFilter.ProgramLevels = mockProgramLevels.Object;
            var response = GetActionFilterResponse(jsonString, programLevelFilter);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void TestProgramLevelFilter_ProgramLevelUnavailable()
        {
            string jsonString = "{ 'ProgramLevelIds': [99999] }";

            var level = new Entities.Level
            {
                ProgramLevelId = 99999,
                IsEnabled = false
            };

            var mockProgramLevels = new Mock<Business.ProgramLevels>();
            mockProgramLevels.Setup(m => m.GetProgramLevel(level.ProgramLevelId)).Returns(level);

            var programLevelFilter = new ProgramLevelFilter();
            programLevelFilter.ProgramLevels = mockProgramLevels.Object;
            var response = GetActionFilterResponse(jsonString, programLevelFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0089"));
        }

        #endregion

        #region Age Filter Tests

        [TestMethod]
        public void TestAgeFilter_AgeLimitExceeded()
        {
            string jsonString = "{ 'Age': '500' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new AgeFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0092"));
        }

        
        [TestMethod]
        public void TestAgeFilter_AgeNotNumeric()
        {
            string jsonString = "{ 'Age': 'non numeric value' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new AgeFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0036"));
        }

        #endregion

        #region YearHighestEducationCompleted Filter Tests

        [TestMethod]
        public void TestYearHighestEducationCompletedFilter_YearLimitExceeded()
        {
            string jsonString = "{ 'YearHighestEducationCompleted': '3000' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new YearHighestEducationCompletedFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0093"));
        }

        [TestMethod]
        public void TestYearHighestEducationCompletedFilter_YearNumeric()
        {
            string jsonString = "{ 'YearHighestEducationCompleted': 'non numeric value' }";
            VendorResponseBase response = GetActionFilterResponse(jsonString, new YearHighestEducationCompletedFilter());
            Assert.IsTrue(response.MessageCodes.Contains("msg0077"));
        }

        #endregion

        #region MilitaryStatusFilter Tests

        #region MilitaryStatusUnavailable Tests

        [TestMethod]
        public void TestMilitaryStatusFilter_MilitaryStatusIsUnavailablePass()
        {
            string jsonString = "{ 'MilitaryAffiliation': 99999 }";

            var militaryStatus = new MilitaryStatus
            {
                MilitaryStatusId = 99999,
                IsEnabled = false
            };

            var mockStatuses = new Mock<Business.MilitaryStatuses>();
            mockStatuses.Setup(m => m.GetMilitaryStatus(militaryStatus.MilitaryStatusId)).Returns(militaryStatus);

            var militaryStatusFilter = new MilitaryAffiliationFilter();
            militaryStatusFilter.MilitaryStatuses = mockStatuses.Object;
            var response = GetActionFilterResponse(jsonString, militaryStatusFilter);

            Assert.IsTrue(response.MessageCodes.Contains("msg0094"));
        }

        [TestMethod]
        public void TestMilitaryStatusFilter_MilitaryStatusIsUnavailableFail()
        {
            string jsonString = "{ 'MilitaryAffiliation': 99999 }";

            var militaryStatus = new MilitaryStatus
            {
                MilitaryStatusId = 99999,
                IsEnabled = true
            };

            var mockStatuses = new Mock<Business.MilitaryStatuses>();
            mockStatuses.Setup(m => m.GetMilitaryStatus(militaryStatus.MilitaryStatusId)).Returns(militaryStatus);

            var militaryStatusFilter = new MilitaryAffiliationFilter();
            militaryStatusFilter.MilitaryStatuses = mockStatuses.Object;
            var response = GetActionFilterResponse(jsonString, militaryStatusFilter);

            Assert.IsNull(response);
        }

        #endregion

        #endregion

        private VendorResponseBase GetActionFilterResponse(string content, ActionFilterBase actionFilter)
        {
            var actionContext = GetHttpActionContext(content);
            actionFilter.OnActionExecuting(actionContext);
            return actionContext.Response?.Content.ReadAsAsync<VendorResponseBase>().Result;
        }
        
        private HttpActionContext GetHttpActionContext(string content)
        {
            var controllerContext = new HttpControllerContext()
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "")
                {
                    Content = new StringContent(content)
                }
            };

            return new HttpActionContext
            {
                ControllerContext = controllerContext
            };
        }
    }
}
