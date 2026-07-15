using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.DTO.Request;
using EDDY.IS.Core;
using EDDY.IS.Validation;

namespace EDDY.IS.FormsEngine.Services.Tests
{
    //A tester class for the FormsEngineAPI.cs which is used by the FormsEngineAPI service.
    [TestClass]
    public class FormsEngineAPITest
    {
        FormsEngine FEService = new FormsEngine();
        ValidationEngine VE = new ValidationEngine();

        //Constants
        const int UNDERGRAD_PROGRAMPRODUCTID = 330;
        const int UNDERGRAD_PROGRAMID = 330;
        const int UNDERGRAD_INSTITUTIONID = 367;
        const int UNDERGRAD_TEMPLATEID = 1; //Undergrad
        const int ACCOUNTINGAA_PROGRAMPRODUCTID = 242122;
        const int ACCOUNTINGAA_PROGRAMID = 671;
        const int ACCOUNTINGAA_INSTITUTIONID = 247;
        const int ACCOUNTINGAA_TEMPLATEID = 1; //Grad History
        const string UNDERGRAD_TEMPLATE_NAME = "Undergrad Template";
        const int WIZARD_TEMPLATEID = 44; //Original Wizard
        const string WIZARD_TEMPLATENAME = "Original Wizard"; //Original Wizard
        const string WIZARD_DEFAULT_TEMPLATENAME = "Original EC";
        const int WIZARD_DEFAULT_TEMPLATEID = 53;
        const string GRAD_TEMPLATE_NAME = "Grad Template";
        const string WIZARDDRIP_RENDERINGSTRATEGY = "WIZARDDRIP";
        const string PTEMPLATE_RENDERINGSTRATEGY = "ORIGINAL";
        const string BAD_LEAD_DATA = "Postal_Code=&Program_Of_Interest=184408&Highest_Level_of_Education_Completed=3&Year_of_Highest_Education_Completed=2008&Desired_Start_Date=More than 1 Year&Age=37&Country=US&us_citizen=Yes&Email={0}@test.com&Military_Affiliation=&Prefix=&First_Name=unit&Last_Name=test&Address=3020&City=Wellington&State=FL&Phone=5613792114";
        const string LEAD_DATA = "Postal_Code=33414&CampusPreference=Online&Program_Of_Interest=184408&Undergraduate_Degree_Grad=Yes&GPA=3.0 or Higher&Years_of_Work_Experience=1 Year&Highest_Level_of_Education_Completed=5&Year_of_Highest_Education_Completed=2008&Desired_Start_Date=More than 1 Year&Age=37&Country=US&us_citizen=Yes&Email={0}@test.com&Military_Affiliation=126&Prefix=&First_Name=unit test&Last_Name=test&Address=3020&Address_2=&City=Wellington&State=FL&Phone=5613792114&Alternate_Phone=&Preferred_Methods_of_Contact=Phone";
        const string LEAD_ADDITIONAL_DATA = "CampusPreference-key=26&Undergraduate_Degree_Grad-key=22&GPA-key=11&Years_of_Work_Experience-key=18&Highest_Level_of_Education_Completed-key=5&Year_of_Highest_Education_Completed-key=2008&Desired_Start_Date-key=5&Country-key=4&us_citizen-key=22&Military_Affiliation-key=126&State-key=11&Preferred_Methods_of_Contact-key=12";
        const string TRACKID = "F225C16D-5469-4877-BA17-6D79F9944EDB";
        
        [TestMethod]
        public void GetRenderingStrategiesTest()
        {
            List<HTMLRenderingStrategyDTO> Result = null;

            Result = FEService.GetRenderingStrategies(FormTemplateTypes.WizardTemplate);    //wizards     
            Assert.IsTrue(Result.Count > 0);

            Result = FEService.GetRenderingStrategies(FormTemplateTypes.ProgramTemplate);   //program templates  
            Assert.IsTrue(Result.Count > 0);

            Result = FEService.GetRenderingStrategies(FormTemplateTypes.ProgramWizard);   //program Wizard
            Assert.IsTrue(Result.Count > 0);                        
        }

        [TestMethod]
        public void ValidateFormTest()
        {
            APIValidationResultDTO Result = null;
            
            PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ValidateForm", null, Core.ISApplication.FormsEngine, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, null);
            
            Result = FEService.ValidateForm(UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), false, ref Log);
            Assert.IsTrue(Result.Valid);
            
            Result = FEService.ValidateForm(UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, string.Format(BAD_LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), false, ref Log);
            Assert.IsFalse(Result.Valid);

        }

        [TestMethod]
        public void ValidateMultipleFormsTest()
        {
            APIMultiValidationResultDTO Result = null;
           
            List<int> programProducts = new List<int>() { UNDERGRAD_PROGRAMPRODUCTID, ACCOUNTINGAA_PROGRAMPRODUCTID };
            PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ValidateMultipleForms.UnitTest", null, Core.ISApplication.FormsEngine, programProducts, false, TRACKID, null);
            
            Result = FEService.ValidateMultipleForms(programProducts, false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), false, ref Log);
            Assert.IsTrue(Result.AllValid);

            //No ME Validation
            Result = FEService.ValidateMultipleForms(programProducts, false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), false, ref Log);
            Assert.IsTrue(Result.AllValid);

            Result = FEService.ValidateMultipleForms(programProducts, false, TRACKID, string.Format(BAD_LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), false, ref Log);
            Assert.IsFalse(Result.AllValid);

            //No ME Validation
            Result = FEService.ValidateMultipleForms(programProducts, false, TRACKID, string.Format(BAD_LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), true, ref Log);
            Assert.IsFalse(Result.AllValid);  
        }

        [TestMethod]
        public void ProcessApolloSubmissionTest()
        {
            List<KeyValuePair<int, string>> programProducts = new List<KeyValuePair<int,string>>();
            List<int?> paidStatusTypeIds = new List<int?>(){3,3};
            programProducts.Add(new KeyValuePair<int, string>(UNDERGRAD_PROGRAMPRODUCTID, "")); 
            programProducts.Add(new KeyValuePair<int, string>(ACCOUNTINGAA_PROGRAMPRODUCTID, ""));    
            APIMultiSubmissionResultDTO Result = null;
            PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.Apollo, "FormsEngineAPI.ProcessApolloSubmission.UNITTEST", null, Core.ISApplication.Apollo, programProducts, 0, TRACKID, null);
            
            RawPostDataDTO RawData = new RawPostDataDTO();
            RawData.RemoteIp = "127.0.0.1";
            RawData.BrowserInfo = "TestZilla";
            RawData.Referer = "UNIT.TEST";
            RawData.PostData = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));

            Result = FEService.ProcessApolloSubmission((int)EDDY.IS.Base.ISApplication.Apollo, programProducts, 0, null, TRACKID, Guid.NewGuid().ToString(), string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), ref RawData, ref Log, true, 0, paidStatusTypeIds, false);
            Assert.IsTrue(Result.AllValid);
            foreach(APISubmissionResultDTO result in Result.SubmissionResults)
            {
                Assert.IsTrue(result.LeadId > 0);
                Assert.IsTrue(result.Valid);
                Assert.IsTrue(!string.IsNullOrEmpty(result.UID));
            }

            //for this method the lead data is not revalidated so bad lead will still go through as valid            
        }

        [TestMethod]
        public void RetrieveTemplateControlsByProgramTemplateTest()
        {
            List<APITemplateControlResultDTO> templateControlList = new List<APITemplateControlResultDTO>();

            //these ints are returned if our test TRACKID is used in FEService.RelatedServices.GetTemplatesForMatches
            HashSet<int> templateIds = new HashSet<int>() { 1, 2, 3, 4, 5, 6, 13 };
            //this method takes a long time because it builds the FE Template cache
            List<KeyValuePair<int, List<TemplateControlDTO>>> tempControls = FEService.GetControlTemplateListForProgramTemplates(templateIds);

            Assert.IsTrue(tempControls.Count > 0);

            foreach (var item in tempControls)
            {
                Assert.IsTrue(item.Value != null);
                Assert.IsTrue(item.Value.Count > 0);
                Assert.IsTrue(item.Key > 0);
            }
        }


        [TestMethod]
        public void GetProgramMatchesTest()
        {
            APIProgramMatchesDTO Result = null;
            
            PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.HostAndPost, "FormsEngineAPI.ProcessHostAndPostLead.UNITTEST", null, Core.ISApplication.HostAndPost, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, null);
                
            Result = FEService.GetProgramMatches(false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), Log);
            Assert.IsNotNull(Result.SchoolSelectionList);
            Assert.IsTrue(Result.Valid);
            Assert.IsTrue(Result.SchoolSelectionList.Count > 0);

            Result = FEService.GetProgramMatches(false, TRACKID, string.Format(BAD_LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), Log);
            Assert.IsNull(Result.SchoolSelectionList);
            Assert.IsFalse(Result.Valid);
            Assert.IsNotNull(Result.ValidationMessages);
            Assert.IsTrue(Result.ValidationMessages.Count > 0);
           
        }
        

        [TestMethod]
        public void ProcessHostAndPostLeadTest()
        {
            APISubmissionResultDTO Result = null;
            
            PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.HostAndPost, "FormsEngineAPI.ProcessHostAndPostLead.UNITTEST", null, Core.ISApplication.HostAndPost, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, null);

            var ValidResult = FEService.GetProgramMatches(false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), Log);

            Assert.IsTrue(ValidResult.Valid);
                
            RawPostDataDTO RawData = new RawPostDataDTO();
            RawData.RemoteIp = "127.0.0.1";
            RawData.BrowserInfo = "TestZilla";
            RawData.Referer = "UNIT.TEST";
            RawData.PostData = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));

            Result = FEService.ProcessHostAndPostLead(ValidResult.SchoolSelectionList[0].ProgramList[0].ProgramProductId, false, TRACKID, string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), ref RawData, ref Log);
            Assert.IsTrue(Result.LeadId > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(Result.UID));
            Assert.IsTrue(Result.Valid);

            Result = FEService.ProcessHostAndPostLead(ValidResult.SchoolSelectionList[0].ProgramList[0].ProgramProductId, false, TRACKID, string.Format(BAD_LEAD_DATA, Guid.NewGuid().ToString().Replace("-", "")), ref RawData, ref Log);
            Assert.IsFalse(Result.LeadId > 0);
            Assert.IsTrue(string.IsNullOrEmpty(Result.UID));
            Assert.IsFalse(Result.Valid);
        }

        
        [TestMethod]
        public void GetProgramTemplateModelTest()
        {
            TemplateDTO Result = null;

            Result = FEService.GetProgramTemplateModel(UNDERGRAD_PROGRAMPRODUCTID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count == 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);


            Result = FEService.GetProgramTemplateModel(ACCOUNTINGAA_PROGRAMPRODUCTID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);

        }

        [TestMethod]
        public void GetProgramTemplateModelByTemplateTest()
        {
            TemplateDTO Result = null;

            int TemplateId = 53;
            Result = FEService.GetProgramTemplateModelByTemplate(TemplateId);
            Assert.IsTrue(Result.TemplateId == TemplateId);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);

            TemplateId = 39872;
            Result = FEService.GetProgramTemplateModelByTemplate(TemplateId);
            //Forms Engine is designed to return the default form when no form is found, Vendor API needs a null object.
            Assert.IsFalse(Result.TemplateId == TemplateId);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);


            TemplateId = 68;
            Result = FEService.GetProgramTemplateModelByTemplate(TemplateId);
            //Forms Engine is designed to return the default form when no form is found, Vendor API needs a null object.
            Assert.IsTrue(Result.TemplateId == TemplateId);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);

        }
    }
}
