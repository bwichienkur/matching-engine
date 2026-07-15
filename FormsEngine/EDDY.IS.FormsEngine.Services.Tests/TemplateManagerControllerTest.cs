using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.FormsEngine;
using EDDY.IS.FormsEngine.Services.Controllers;
using Moq;
using System.Web;
using System.Web.Mvc;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.Core.Logging;
using System.Threading;

namespace EDDY.IS.FormsEngine.Services.Tests
{
    [TestClass]
    public class TemplateManagerControllerTest
    {
        static FormsEngine FE = new FormsEngine();
        TemplateManagerController tmc = new TemplateManagerController();
        const string WIZARD_DEFAULT_TEMPLATENAME = "Original EC";
        const int WIZARD_DEFAULT_TEMPLATEID = 53;
        
        [TestMethod]
        public void TestGetWizardTemplate()
        {
            //var httpContextStup = new Mock<HttpContextBase>();
            //var controllerContext = new ControllerContext();
            //controllerContext.HttpContext = httpContextStup.Object;
            //tmc.ControllerContext = controllerContext;

            //var renderings =FE.GetRenderingStrategies(Core.FormTemplateTypes.WizardTemplate);
            //foreach(var rs in renderings)
            //{
            //    var template = tmc.GetWizardTemplate(WIZARD_DEFAULT_TEMPLATEID,rs.Name,false,true,null,Guid.NewGuid().ToString());

            //    Assert.IsNotNull(template);
            //    Assert.IsNotNull(template.ToString());
            //}
        }

        [TestMethod]
        public void TemplateManagerController_GetManagedChoice_ShouldCreateLead()
        {
            PerformanceLog log = new PerformanceLog();
            string LeadData = @"TemplateId=112&RenderingStrategy=WIZARDPROFESSIONALBOOTSTRAP&IsBeta=false&TrackId=C223710F-CC09-4450-8E29-370AFB64F79A&SessionId=d4d42aa7-7088-49bf-aa9f-bb7d19a23afa&MatchGuid=340d7e93-e882-4a79-a696-1b46b5c3c904&ProspectId=15369467&LeadData=Highest_Level_of_Education_Completed=9&Year_of_Highest_Education_Completed=2000&GPA-key=11&Age=30&Postal_Code=33496&Categories=21,29,24,20,25,23,26,27,30,22&SubCategories=610,640,690,667,681,747,698,749,611,612,736,691,771,629,715,739,673,641,630,642,613,614,648,631,745,674,685,768,693,627,680,753,722,686,643,601,615,675,644,755,668,603,651,718,652,645,694,676,617,618,654,756,633,758,635,619,772,647,696,677,678,695,700,620,688,655,656,657,741,760,761,658,659,621,770,622,679,605,697,606,684,669,624,670,625,664,665,626,737,671,672,636,649,628,763,637,638,743,765,608,609&First_Name=diwejwkj&Last_Name=nhdsajh&Email=dnyuewqhjq@gmail.com&NewsLetterOptIn=Yes, I would like to be contacted by e-mail for educational opportunities!&Address=24390 rock way&City=Boca Raton&State=FL&Country=US&Postal_Code_Duplicate=33496&Phone=(561) 789-5123&Desired_Start_Date=Immediately&us_citizen=Yes&DynamicCampusSoftPreference=Both&CampusSoftPreference=Both&Military_Affiliation=126&%22UserAgreement=*I acknowledge that, by clicking the checkbox as my official signature, I consent to representatives of  EducationDynamics, LLC contacting me about educational  opportunities via email, text, or phone, including my mobile phone number(s) 561.789.5123, using an automatic dialer, or pre-recorded message.  Message and data rates may apply. I understand that my consent is not a requirement for enrollment, and I may withdraw my consent at any time.%22&leadid_token=016854EF-5DD0-5FA6-2FB8-5E41274218DF&xxTrustedFormToken=https%3A%2F%2Fcert.trustedform.com%2F77dd6e981236774ba45bc8adb4bae33c59a8dcc3&xxTrustedFormCertUrl=https%3A%2F%2Fcert.trustedform.com%2F77dd6e981236774ba45bc8adb4bae33c59a8dcc3&AffiliateId=&FormLeadUrl=http%253A%252F%252Flocalhost%253A59955%252FClients%252FWizard%252FWizardLite-Bootstrap.htm&AdditionalQuestionsShown.Start=false&CampusSoftPreferenceShown=false&CampusPreferenceShown=false&AdditionalData=Highest_Level_of_Education_Completed-key=9&Year_of_Highest_Education_Completed-key=2000&State-key=11&Country-key=4&Desired_Start_Date-key=1&us_citizen-key=22&DynamicCampusSoftPreference-key=Both&Military_Affiliation-key=126&Theme=default&SMLeadsCreatedCount=0&USLeadsCreatedCount=0&SplitCampusTypeInResults=undefined&FESessionId=f8f30f4e-66b0-4931-9e08-cf4055687d40&DeviceId=undefined&RenderingExperience=undefined&LimboAlternativeCampaignTrackid=&LimboAlternativeCampaignTrackidUtilized=false&FormTemplateType=2&ProgramTemplateId=&ProgramProductId=&ProductId=&InstitutionId=undefined&ApplicationId=7&InstitutionName=undefined&ProgramName=undefined&callback=jQuery1102020168515832832323_1560871077000&_=1560871077031";
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(509, 628046, false, "c223710f-cc09-4450-8e29-370afb64f79a", null, false, "D4D42AA7-7088-49BF-AA9F-BB7D19A23AFA", "106B5A90-5589-490C-8EA2-65EE29316127", LeadData, null, null, null, 15369467);                        
            string RenderingStrategy = "WIZARDPLAIN";
            RawPostDataDTO RawData = new RawPostDataDTO();
            ExpressConsentCheckDTO ExpressConsentCheck = tmc.CheckForMobile("5617895123", string.Empty, log); //phone1 phone2 email first last;
            ProspectInput ProspectInput = EntityBuildHelper.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null);
            CampusPreference? CampusSoftPreference = CampusPreference.Both;            
            string UserFullName = "asdf ghjk";            
            string TrackId = "c223710f-cc09-4450-8e29-370afb64f79a"; //Campaign ID
            string MatchRecordId = "106B5A90-5589-490C-8EA2-65EE29316127"; //Match Response Guid
            string FESessionId = "D4D42AA7-7088-49BF-AA9F-BB7D19A23AFA"; //Tracking Session Guid            
            WizardMatchResponse WizardMatchResponse = new WizardMatchResponse();            
            int? ApplicationId = 7;

            WizardMatchResponse response = tmc.GetSmartMatchesForWizard(false, 
                0,
                TrackId, 
                FESessionId, 
                MatchRecordId, 
                LeadData,
                null, 
                0, 
                0, 
                FESessionId, 
                false, 
                string.Empty, 
                false, 
                ApplicationId, 
                true, 
                log);
            
            ManagedChoiceResultDTO result = tmc.GetRenderedManagedChoice(LeadRequest,
            LeadData,
            null,
            RenderingStrategy,
            RawData,
            ExpressConsentCheck,
            ProspectInput,
            CampusSoftPreference,
            false,
            UserFullName,
            string.Empty,
             0,
             0,
            FESessionId,
            false,
            null,
            false,
            WizardMatchResponse,
            string.Empty,
            true,
            false,
            ApplicationId, 
            log);

            Thread.Sleep(300000);
        }
    }
}
