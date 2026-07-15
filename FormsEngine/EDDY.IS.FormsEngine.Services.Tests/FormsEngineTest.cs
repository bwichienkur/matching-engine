using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.FormsEngine;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.DataModel;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.DTO;
using System.Collections.Generic;
using EDDY.IS.Core;

namespace EDDY.IS.FormsEngine.Services.Tests
{
    [TestClass]
    public class FormsEngineTest
    {
        static FormsEngine FE = new FormsEngine();
        static LeadSaveManger LSM = new LeadSaveManger(new Core.Logging.PerformanceLog());

        //Constants
        const int UNDERGRAD_PROGRAMPRODUCTID = 260629;
        const int UNDERGRAD_PROGRAMID = 15;
        const int UNDERGRAD_INSTITUTIONID = 195;
        const int UNDERGRAD_TEMPLATEID = 1; //Undergrad
        const string UNDERGRAD_TEMPLATE_NAME = "Undergrad Template";
        const int WIZARD_TEMPLATEID = 44; //Original Wizard
        const string WIZARD_TEMPLATENAME = "Original Wizard"; //Original Wizard
        const string WIZARD_DEFAULT_TEMPLATENAME = "Original EC";
        const int WIZARD_DEFAULT_TEMPLATEID = 53;
        const string GRAD_TEMPLATE_NAME = "Grad Template";
        const int GRAD_TEMPLATEID = 2;
        const string WIZARDDRIP_RENDERINGSTRATEGY = "WIZARDDRIP";
        const string PTEMPLATE_RENDERINGSTRATEGY = "ORIGINAL";
        const string PWIZARD_RENDERINGSTRATEGY = "PROGRAMWIZARD";
        const string UNDERGRAD_STANDARD_CONTROLCODE = "Undergraduate_Degree_Grad";
        const string POSTALCODE_CONTROLCODE = "Postal_Code";
        const string CATEGORIES_CONTROLCODE = "Categories";
        const string LEAD_DATA = "Postal_Code=33414&CampusPreference=Online&Program_Of_Interest=184408&Undergraduate_Degree_Grad=Yes&GPA=3.0 or Higher&Years_of_Work_Experience=1 Year&Highest_Level_of_Education_Completed=5&Year_of_Highest_Education_Completed=2008&Desired_Start_Date=More than 1 Year&Age=37&Country=US&us_citizen=Yes&Email={0}@test.com&Military_Affiliation=126&Prefix=&First_Name=unit&Last_Name=test&Address=3020&Address_2=&City=Wellington&State=FL&Phone=5613792114&Alternate_Phone=&Preferred_Methods_of_Contact=Phone";
        const string LEAD_ADDITIONAL_DATA = "CampusPreference-key=26&Undergraduate_Degree_Grad-key=22&GPA-key=11&Years_of_Work_Experience-key=18&Highest_Level_of_Education_Completed-key=5&Year_of_Highest_Education_Completed-key=2008&Desired_Start_Date-key=5&Country-key=4&us_citizen-key=22&Military_Affiliation-key=126&State-key=11&Preferred_Methods_of_Contact-key=12";
        const string TRACKID = "F225C16D-5469-4877-BA17-6D79F9944EDB";
        const int APPLICATIONID = 2;


        #region FormsEngine.cs


        [TestMethod]
        public void TestGetHTMLRenderingStrategy()
        {
            var Result = FE.GetHTMLRenderingStrategy("ORIGINAL");
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Name == "ORIGINAL");

            Result = FE.GetHTMLRenderingStrategy("WIZARDDRIP");
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Name == "WIZARDDRIP");

            Result = FE.GetHTMLRenderingStrategy(1);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Name == "ORIGINAL");
        }

        [TestMethod]
        public void TestGetProgramTemplateModel()
        {
            var Result = FE.GetProgramTemplateModel(UNDERGRAD_PROGRAMPRODUCTID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateName == "Undergrad Template");
        }

        [TestMethod]
        public void TestGetProgramTemplateModelByTemplate() 
        {
            var Result = FE.GetProgramTemplateModelByTemplate(UNDERGRAD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateId == UNDERGRAD_TEMPLATEID);

            Result = FE.GetProgramTemplateModelByTemplate(WIZARD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateId == WIZARD_TEMPLATEID);
        }

        [TestMethod]
        public void TestGetAdditionalTemplateQuestions()
        {
            var Result = FE.GetAdditionalTemplateQuestions(WIZARD_TEMPLATEID,new System.Collections.Generic.List<int>(){1,2,3,4}, false);
            Assert.IsNotNull(Result);
            
            //8 additional questions
            Assert.IsNotNull(Result.Count == 8); //Number of additional questions this combination should be returning

            //Undergrad question
            Assert.IsTrue((from c in Result
                           where c.StandardControl.StandardControlCode.Code == UNDERGRAD_STANDARD_CONTROLCODE
                           select c).Count() == 1);


            //No additional questions
            Result = FE.GetAdditionalTemplateQuestions(WIZARD_TEMPLATEID, new System.Collections.Generic.List<int>() { 1 }, false);
            Assert.IsTrue(Result.Count == 0);
            
            //Radiology questions
            Result = FE.GetAdditionalTemplateQuestions(WIZARD_TEMPLATEID, new System.Collections.Generic.List<int>() { 6 }, false);
            Assert.IsTrue(Result.Count == 4);
        }

        [TestMethod]
        public void TestGetProgramTemplatesCoveredByWizardQuestions()
        {
            var Result = FE.GetProgramTemplatesCoveredByWizardQuestions(WIZARD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 0); // Undergrad covered and exclusive
            Assert.IsTrue(Result.Contains(UNDERGRAD_TEMPLATEID));
        }

        [TestMethod]
        public void TestGetTemplateControls()
        {
            var Result = FE.GetTemplateControls(WIZARD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 10);


            Result = FE.GetTemplateControls(UNDERGRAD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 10); 
        }

        [TestMethod]
        public void TestGetShallowTemplateList()
        {
            //Wizard
            var Result = FE.GetShallowTemplateList(FormTemplateTypes.WizardTemplate);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 10);


            //ProgramTemplates
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramTemplate);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 5);

            //ProgramWizard
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramWizard);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 0);


            //Wizard and EMS BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.WizardTemplate, BusinessDivisionType.EMS);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);

            //ProgramTemplates and EMS BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramTemplate, BusinessDivisionType.EMS);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);

            //ProgramWizard and EMS BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramWizard, BusinessDivisionType.EMS);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);

            //Wizard and PD BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.WizardTemplate, BusinessDivisionType.ProspectDelivery);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);

            //ProgramTemplates and PD BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramTemplate, BusinessDivisionType.ProspectDelivery);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);

            //ProgramWizard and PD BusinessUnit
            Result = FE.GetShallowTemplateList(FormTemplateTypes.ProgramWizard, BusinessDivisionType.ProspectDelivery);
            Assert.IsNotNull(Result);
            //Assert.IsTrue(Result.Count > 0);
        }


        [TestMethod]
        public void TestGetStandardControlCodeFilters()
        {
            //Categories filters (24)
            var Result = FE.GetStandardControlCodeFilters(CATEGORIES_CONTROLCODE);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 20);
        }


        [TestMethod]
        public void TestSaveProspect()
        {
            string leaddata = string.Format(LEAD_DATA,Guid.NewGuid().ToString().Replace("-",""));
            var Result = FE.RelatedServices.SaveProspect(false, Guid.NewGuid().ToString(), leaddata, LEAD_ADDITIONAL_DATA, TRACKID, APPLICATIONID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.ProspectId > 0);
            Assert.IsTrue(Result.ProspectFlowId > 0);
            FE.RelatedServices.SaveProspectWizardStatus(Result.ProspectFlowId,true);
        }



        [TestMethod]
        public void TestGetECSchoolSelectionMessages()
        {
            //SM
            var Result = FormsEngine.GetECSchoolSelectionMessages("FirstName","SubmitButtonText","ExpressConsentVersion",true,MatchingEngine.CampusPreference.Both,10,10,true);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.NameText.Length > 0);
            Assert.IsTrue(Result.SmartMatchMessage1.Length > 0);

            //No SM
            Result = FormsEngine.GetECSchoolSelectionMessages(true, "FirstName", "SubmitButtonText", "ExpressConsentVersion", true, MatchingEngine.CampusPreference.Both, 10, 10, false);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.NameText.Length > 0);
            Assert.IsTrue(Result.SmartMatchMessage1.Length == 0);
        }


        [TestMethod]
        public void TestGetProgramsForCrossSell()
        {
            string leaddata = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(1, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), leaddata, LEAD_ADDITIONAL_DATA, null, LeadCreationType.InstitutionFormInitial, null);
            ProspectInput Prospect = EntityBuildHelper.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());

            var Result = FE.GetProgramsForCrossSell(new Guid(TRACKID), Prospect, CampusType.Online, false, UNDERGRAD_PROGRAMPRODUCTID, UNDERGRAD_INSTITUTIONID, 10, 1, null,Guid.NewGuid().ToString(),null,false,false,0);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.ResultCount > 0);
        }

        [TestMethod]
        public void TestGetProgramsForManagedChoice()
        {
            string leaddata = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(1, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), leaddata, LEAD_ADDITIONAL_DATA, null, LeadCreationType.InstitutionFormInitial, null);
            ProspectInput Prospect = EntityBuildHelper.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());

            var Result = FE.GetProgramsForManagedChoice(new Guid(TRACKID), Guid.NewGuid(), false, Prospect, CampusPreference.Both, false, 0,0,true,true,LeadRequest.LeadData, new System.Collections.Generic.List<int>(),true, Guid.NewGuid(),Guid.NewGuid().ToString(), null);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.SchoolSelectionList.Count() > 0);
        }


        [TestMethod]
        public void TestValidateRenderingStrategy()
        {
            HTMLRenderingStrategyDTO RenderingStrategyResult = null;
            //Wizard--> Wizard RE
            var Result = FE.ValidateRenderingStrategy(WIZARDDRIP_RENDERINGSTRATEGY, FormTemplateTypes.WizardTemplate, out RenderingStrategyResult);
            Assert.IsNotNull(RenderingStrategyResult);
            Assert.IsTrue(Result);


            //Wizard--> Program RE
            Result = FE.ValidateRenderingStrategy(WIZARDDRIP_RENDERINGSTRATEGY, FormTemplateTypes.ProgramTemplate, out RenderingStrategyResult);
            Assert.IsNull(RenderingStrategyResult);
            Assert.IsFalse(Result);


            //PTemplate--> PTemplate RE
            Result = FE.ValidateRenderingStrategy(PTEMPLATE_RENDERINGSTRATEGY, FormTemplateTypes.ProgramTemplate, out RenderingStrategyResult);
            Assert.IsNotNull(RenderingStrategyResult);
            Assert.IsTrue(Result);

            //PTemplate--> Wizard RE
            Result = FE.ValidateRenderingStrategy(PTEMPLATE_RENDERINGSTRATEGY, FormTemplateTypes.WizardTemplate, out RenderingStrategyResult);
            Assert.IsNull(RenderingStrategyResult);
            Assert.IsFalse(Result);

            //PWizardTemplate--> PWizard RE
            //Result = FE.ValidateRenderingStrategy(PWIZARD_RENDERINGSTRATEGY, FormTemplateTypes.WizardTemplate, out RenderingStrategyResult);
            //Assert.IsNotNull(RenderingStrategyResult);
            //Assert.IsTrue(Result);

            //WizardTemplate--> PWizard RE
            Result = FE.ValidateRenderingStrategy(WIZARDDRIP_RENDERINGSTRATEGY, FormTemplateTypes.WizardTemplate, out RenderingStrategyResult);
            Assert.IsNotNull(RenderingStrategyResult);
            Assert.IsTrue(Result);

        }


        [TestMethod]
        public void TestGetResourceMetaDataTextForKey()
        {
            string Result = FormsEngine.GetResourceMetaDataTextForKey("JS.WIZARD.USERAGREEMENTDEFAULT");
            Assert.IsFalse(string.IsNullOrWhiteSpace(Result));
            Assert.IsTrue(Result.Length > 0);
        }


        [TestMethod]
        public void TestGetProgramTemplateMessageForType()
        {
            var Result = FE.GetProgramTemplateMessageForType("GENERIC_MULTIPLESCHOOL");
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 0);
        }

        #endregion FormsEngine.cs

        #region FormsEngineBase.cs

        [TestMethod]
        public void TestGetRenderingStrategies()
        {
            var Result = FE.GetRenderingStrategies(FormTemplateTypes.WizardTemplate);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 1);

            Result = FE.GetRenderingStrategies(FormTemplateTypes.ProgramTemplate);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 0);

            Result = FE.GetRenderingStrategies(FormTemplateTypes.ProgramWizard);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.Count > 0);
        }



        [TestMethod]
        public void TestFindAlternativeTemplateId()
        {
            var Result = FE.FindAlternativeTemplateId(UNDERGRAD_TEMPLATEID,"1=2");
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result == GRAD_TEMPLATEID);
        }


        [TestMethod]
        public void TestGetTemplateIdOrDefault()
        {
            int Result = FE.GetTemplateIdOrDefault(UNDERGRAD_TEMPLATEID, false, null, null);
            Assert.IsTrue(Result == UNDERGRAD_TEMPLATEID);

            Result = FE.GetTemplateIdOrDefault(-1, false, null, null);
            Assert.IsTrue(Result == UNDERGRAD_TEMPLATEID);

            Result = FE.GetTemplateIdOrDefault(GRAD_TEMPLATEID, false, UNDERGRAD_PROGRAMID, null);
            Assert.IsTrue(Result == GRAD_TEMPLATEID);

            Result = FE.GetTemplateIdOrDefault(-1, true, null, null);
            Assert.IsTrue(Result == WIZARD_DEFAULT_TEMPLATEID);

            Result = FE.GetTemplateIdOrDefault(-1, true, UNDERGRAD_PROGRAMID, null);
            Assert.IsTrue(Result == WIZARD_DEFAULT_TEMPLATEID);

        }

        [TestMethod]
        public void TestTemplateExists()
        {
            var Result = FE.TemplateExists(WIZARD_TEMPLATEID);
            Assert.IsTrue(Result);

            Result = FE.TemplateExists(UNDERGRAD_TEMPLATEID);
            Assert.IsTrue(Result);

            Result = FE.TemplateExists(9999);
            Assert.IsFalse(Result);
        }


        [TestMethod]
        public void TestGetDefaultTemplateId()
        {
            var Result = FE.GetDefaultTemplateId(true);
            Assert.IsTrue(Result == WIZARD_DEFAULT_TEMPLATEID);

            Result = FE.GetDefaultTemplateId(false);
            Assert.IsTrue(Result == UNDERGRAD_TEMPLATEID);
        }


        [TestMethod]
        public void TestGetFullTemplate()
        {
            var Result = FE.GetFullTemplate(UNDERGRAD_TEMPLATEID);
            Assert.IsNotNull(Result);
            //Basic results from model.
            Assert.IsTrue(Result.TemplateType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.Count==4);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count == 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);


            Result = FE.GetFullTemplate(WIZARD_DEFAULT_TEMPLATEID);
            Assert.IsNotNull(Result);
            //Basic results from model.
            Assert.IsTrue(Result.TemplateType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.Count > 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);

            //TODO: more rigorous test around model
        }


        [TestMethod]
        public void TestGetShallowTemplate()
        {
            var Result = FE.GetShallowTemplate(UNDERGRAD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count == 0);
            Assert.IsTrue(Result.TemplateType.Name.Length > 0);

            Result = FE.GetShallowTemplate(WIZARD_TEMPLATEID);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count == 0);
            Assert.IsTrue(Result.TemplateType.Name.Length > 0);
        }



        [TestMethod]
        public void TestGetTemplate()
        {
            var Result = FE.GetTemplate(UNDERGRAD_TEMPLATEID, false);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateSteps.Count == 4);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count == 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);

            Result = FE.GetTemplate(WIZARD_TEMPLATEID, true);
            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.TemplateType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.Count > 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.Count >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.FieldName.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlCode.Code.Length >= 1);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlValidations.Count >= 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlDataType.Name.Length > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.ErrorMessages.Count > 0);
            Assert.IsTrue(Result.TemplateSteps.First().TemplateSections.First().TemplateControls.First().StandardControl.StandardControlType.StandardControlTypeName.Length > 0);
        }

        #endregion FormsEngineBase.cs


        #region LeadSaveManager


        [TestMethod]
        public void TestLeadSaveExecute()
        {
            //Single save
            string leaddata = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(UNDERGRAD_TEMPLATEID, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, Guid.NewGuid().ToString(), false,  Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), leaddata, LEAD_ADDITIONAL_DATA, null, LeadCreationType.HostAndPost, null);
            RawPostDataDTO RawData = new RawPostDataDTO();
            RawData.RemoteIp = "127.0.0.1";
            RawData.BrowserInfo ="TestZilla";
            RawData.Referer = "UNIT.TEST";
            RawData.PostData = LEAD_DATA;
            LeadCreateResponse Result = LSM.Execute(null, LeadRequest, "", ISApplication.HostAndPost, RawData, null, null, null, true, null,null);

            Assert.IsNotNull(Result);
            Assert.IsTrue(Result.ResultMessage == "Success");
            Assert.IsTrue(Result.Lead.LeadId > 0);
            Assert.IsTrue(Result.Lead.UID.Length>0);

            LeadDataService lds = new LeadDataService();
            Lead l = lds.GetLead(Convert.ToInt32(Result.Lead.LeadId));

            Assert.IsTrue(l.SubmissionId > 0);
            Assert.IsTrue(l.RawPostDataId > 0);

            //Multiple Save
            List<LeadSaveData> LeadSaveDataList = new List<LeadSaveData>();

            LeadSaveDataList.Add(new LeadSaveData() { IsValid = true, ProgramProductId = UNDERGRAD_PROGRAMPRODUCTID, TemplateId = UNDERGRAD_TEMPLATEID, LeadCreationType = LeadCreationType.HostAndPost });
            LeadSaveDataList.Add(new LeadSaveData() { IsValid = true, ProgramProductId = UNDERGRAD_PROGRAMPRODUCTID, TemplateId = UNDERGRAD_TEMPLATEID, LeadCreationType = LeadCreationType.HostAndPost });
            LeadSaveDataList.Add(new LeadSaveData() { IsValid = true, ProgramProductId = UNDERGRAD_PROGRAMPRODUCTID, TemplateId = UNDERGRAD_TEMPLATEID, LeadCreationType = LeadCreationType.HostAndPost });
            LeadSaveDataList.Add(new LeadSaveData() { IsValid = true, ProgramProductId = UNDERGRAD_PROGRAMPRODUCTID, TemplateId = UNDERGRAD_TEMPLATEID, LeadCreationType = LeadCreationType.HostAndPost });
            LeadSaveDataList.Add(new LeadSaveData() { IsValid = true, ProgramProductId = UNDERGRAD_PROGRAMPRODUCTID, TemplateId = UNDERGRAD_TEMPLATEID, LeadCreationType = LeadCreationType.HostAndPost });

            var MultipleResult = LSM.Execute(null, "", LeadCreationType.HostAndPost, ISApplication.Apollo, LeadSaveDataList, false, TRACKID, Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), RawData, leaddata, LEAD_ADDITIONAL_DATA, 0, null, null, null, null, true, new APIValidationResultDTO() { IsTestLead = false, Valid = true }, null, 0);
            Assert.IsNotNull(MultipleResult);

            foreach (var r in MultipleResult)
            {
                Assert.IsTrue(r.ResultMessage == "Success");
                Assert.IsTrue(r.Lead.LeadId > 0);
                Assert.IsTrue(r.Lead.UID.Length > 0);
                l = lds.GetLead(Convert.ToInt32(r.Lead.LeadId));

                Assert.IsTrue(l.SubmissionId > 0);
                Assert.IsTrue(l.RawPostDataId > 0);
            }

            //TODO: Cover all overloaded methods to save.
        }
       
        #endregion LeadSaveManager

        #region SessionManager

        [TestMethod]
        public void TestFESession()
        {
            String FESessionId = Guid.NewGuid().ToString();
            String FESession2Id = Guid.NewGuid().ToString();
            FESession.Set(FESessionId, "UT.KEY1", 7777);
            Assert.IsTrue((int)FESession.Get(FESessionId, "UT.KEY1") == 7777);
            Assert.IsTrue(FESession.Get<int>(FESessionId, "UT.KEY1") == 7777);
            FESession.Remove(FESessionId, "UT.KEY1");
            Assert.IsNull(FESession.Get(FESessionId, "UT.KEY1"));
            
            FESession.Set(FESessionId, "UT.KEY2", "qwertyuiopasdfghjklzxcvbnm");
            Assert.IsTrue((string)FESession.Get(FESessionId, "UT.KEY2") == "qwertyuiopasdfghjklzxcvbnm");
            Assert.IsTrue(FESession.Get<string>(FESessionId, "UT.KEY2") == "qwertyuiopasdfghjklzxcvbnm");

            string leaddata = string.Format(LEAD_DATA, Guid.NewGuid().ToString().Replace("-", ""));
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(UNDERGRAD_TEMPLATEID, UNDERGRAD_PROGRAMPRODUCTID, false, TRACKID, Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), leaddata, LEAD_ADDITIONAL_DATA, null, LeadCreationType.HostAndPost, null);

            FESession.Set(FESessionId, "UT.KEY3", LeadRequest);
            var Result = FESession.Get<LeadCreateRequest>(FESessionId, "UT.KEY3");
            Assert.IsNull(FESession.Get<LeadCreateRequest>(FESession2Id, "UT.KEY3"));
            Assert.IsTrue(Result.LeadData.First().Value == LeadRequest.LeadData.First().Value);
            
        }



        #endregion SessionManager

    }
}
