using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.Mapper;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsEngine : FormsEngineBase
    {

        /// <summary>
        /// Gets the HTML Rendering Strategie by name
        /// </summary>
        /// <param name="RenderingStrategy"></param>
        /// <returns></returns>
        public HTMLRenderingStrategyDTO GetHTMLRenderingStrategy(string RenderingStrategy)
        {
            return dbRenderingStrategyService.GetHTMLRenderingStrategy(RenderingStrategy).ConvertToDTO();
        }

        /// <summary>
        /// Gets the HTML Rendering Strategie by Id
        /// </summary>
        /// <param name="HTMLRenderingStrategyId"></param>
        /// <returns></returns>
        public HTMLRenderingStrategyDTO GetHTMLRenderingStrategy(int HTMLRenderingStrategyId)
        {
            return dbRenderingStrategyService.GetHTMLRenderingStrategy(HTMLRenderingStrategyId).ConvertToDTO();
        }

        /// <summary>
        /// Gets the Program Template Model by ProgramProductId
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <returns></returns>
        public TemplateDTO GetProgramTemplateModel(int ProgramProductId)
        {
            int TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(ProgramProductId, false);
            return GetTemplate(TemplateId, false);
        }

        /// <summary>
        /// Gets the Program Template Model by TemplateId
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public TemplateDTO GetProgramTemplateModelByTemplate(int TemplateId)
        {
            return GetTemplate(TemplateId, false);
        }

        /// <summary>
        /// Gets the additional template questions
        /// </summary>
        /// <param name="WizardTemplateId"></param>
        /// <param name="ProgramTemplates"></param>
        /// <returns></returns>
        public List<TemplateControlDTO> GetAdditionalTemplateQuestions(int WizardTemplateId, List<int> ProgramTemplates, bool AllowDupes = false)
        {
            return (List<TemplateControlDTO>)dbTemplateService.GetAdditionalTemplateQuestions(WizardTemplateId, ProgramTemplates, AllowDupes).ConvertToDTO();
        }
        /// <summary>
        /// Get the additional template questions for unmatched tempalte
        /// </summary>
        /// <param name="CoveredTemplateIds"></param>
        /// <param name="TemplateId"></param>
        /// <param name="AllowDupes"></param>
        /// <returns></returns>
        public List<TemplateControlDTO> GetAdditionalTemplateQuestionsForProgramMatch(List<int> CoveredTemplateIds, int TemplateId, bool AllowDupes)
        {
            return (List<TemplateControlDTO>)dbTemplateService.GetAdditionalTemplateQuestionsForProgramMatch(CoveredTemplateIds, TemplateId, AllowDupes).ConvertToDTO();
        }

        /// <summary>
        /// Gets the list of program templates the wizard template covers
        /// </summary>
        /// <param name="WizardTemplateId"></param>
        /// <returns></returns>
        public List<int> GetProgramTemplatesCoveredByWizardQuestions(int WizardTemplateId)
        {
            return dbTemplateService.GetProgramTemplatesCoveredByWizardQuestions(WizardTemplateId);
        }

        /// <summary>
        /// Gets the list of controls by templateid
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public List<TemplateControlDTO> GetTemplateControls(int TemplateId)
        {
            return (List<TemplateControlDTO>)dbTemplateService.GetTemplateControls(TemplateId).ConvertToDTO();
        }

        public TemplateControlDTO GetTemplateControlByCode(int TemplateId, string standardControlCode)
        {
            var templateControls = GetTemplateControls(TemplateId);
            standardControlCode = standardControlCode.ToLowerInvariant();

            return (from tc in templateControls
                    where tc.StandardControl.StandardControlCode.Code.ToLowerInvariant() == standardControlCode
                    select tc).FirstOrDefault();
        }

        /// <summary>
        /// Light version of the template without steps/sections or controls
        /// </summary>
        /// <param name="Wizard"></param>
        /// <returns></returns>
        public List<TemplateBasicDTO> GetShallowTemplateList(FormTemplateTypes FormTemplateType, BusinessDivisionType? BusinessDivision = null, int? institutionId = null)
        {
            List<TemplateBasicDTO> Result = new List<TemplateBasicDTO>();

            foreach (var item in dbTemplateService.GetShallowTemplateList(FormTemplateType, BusinessDivision, institutionId))
            {
                TemplateBasicDTO element = new TemplateBasicDTO();
                element.TemplateId = item.TemplateId;
                element.TemplateName = item.TemplateName;
                element.Description = item.TemplateDescription;
                Result.Add(element);
            }

            return Result;
        }

        /// <summary>
        /// Gets the list of ME filters for the given StandardControlCode
        /// </summary>
        /// <param name="StandardControlCode"></param>
        /// <returns></returns>
        public List<string> GetStandardControlCodeFilters(string StandardControlCode)
        {
            return dbTemplateService.GetStandardControlCodeFilters(StandardControlCode);
        }


       
        /// <summary>
        /// Added to overload the existing GetECSchoolSelectionMessages and still support the custom Mobile Messages
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="SubmitButtonText"></param>
        /// <param name="ExpressConsentVersion"></param>
        /// <param name="CampusSoftPreferenceShown"></param>
        /// <param name="CampusSoftPreference"></param>
        /// <param name="OnlineMatchesCount"></param>
        /// <param name="CampusMatchesCount"></param>
        /// <param name="HasSmartMatches"></param>
        /// <returns></returns>
        public static MatchMessageResultsDTO GetECSchoolSelectionMessages(string FirstName, string SubmitButtonText, string ExpressConsentVersion, bool CampusSoftPreferenceShown, MatchingEngine.CampusPreference? CampusSoftPreference, int OnlineMatchesCount, int CampusMatchesCount, bool HasSmartMatches)
        {
            return GetECSchoolSelectionMessages(false, FirstName, SubmitButtonText, ExpressConsentVersion, CampusSoftPreferenceShown, CampusSoftPreference, OnlineMatchesCount, CampusMatchesCount, HasSmartMatches);
        }


        public static MatchMessageResultsDTO GetECSchoolSelectionMessages(bool IsMobile, string FirstName, string SubmitButtonText, string ExpressConsentVersion, bool CampusSoftPreferenceShown, MatchingEngine.CampusPreference? CampusSoftPreference, int OnlineMatchesCount, int CampusMatchesCount, bool HasSmartMatches)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetECSchoolSelectionMessages", null, FirstName, SubmitButtonText, ExpressConsentVersion, CampusSoftPreferenceShown, CampusSoftPreference, OnlineMatchesCount, CampusMatchesCount, HasSmartMatches);
            Log.StartLogDetail("FormsEngine.GetECSchoolSelectionMessages");

            MatchMessageResultsDTO Result = new MatchMessageResultsDTO();

            try
            {
                /*
                 * DISPLAY LAYOUT:
                 * --------------------------------------------------------------------
                 * 
                 *      <NameText> + ( <StandardMessage> OR <SmartMatchMessage1> )
                 *      
                 *          [SM1] [SM2] [SM3]
                 *      
                 *      <UserSelectMessage1>
                 *      <UserSelectMessage2>
                 * 
                 *      OnlineTab / CampusTab (default selected when both available) 
                 *          [US1]
                 *          [US2]
                */

                Result.NameText = "Thank you " + FirstName + ".";
                Result.UserSelectMessage1 = FormsEngine.GetResourceMetaDataTextForKey("FE.ECUSERSELECTMESSAGE1");

                if (HasSmartMatches)
                {
                    if (ExpressConsentVersion.ToUpper() != "D")
                        Result.SmartMatchMessage1 = FormsEngine.GetResourceMetaDataTextForKey("FE.HSM.ECVD.ECSMARTMATCHMESSAGE1");
                    else
                        Result.SmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.HSM.ECV.ECSMARTMATCHMESSAGE1"), SubmitButtonText);

                    if (CampusSoftPreferenceShown && CampusSoftPreference == MatchingEngine.CampusPreference.Online && OnlineMatchesCount == 0)
                        Result.UserSelectMessage2 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.HSM.CSP.ONLINE.ECUSERSELECTMESSAGE2"), SubmitButtonText);
                    else if (CampusSoftPreferenceShown && CampusSoftPreference == MatchingEngine.CampusPreference.Ground && CampusMatchesCount == 0)
                        Result.UserSelectMessage2 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.HSM.CSP.GROUND.ECUSERSELECTMESSAGE2"), SubmitButtonText);
                    else if (IsMobile)//Added specifically for Mobile
                        Result.UserSelectMessage2 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("MOBILE.FE.HSM.ECUSERSELECTMESSAGE2"), SubmitButtonText);
                    else
                        Result.UserSelectMessage2 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.HSM.ECUSERSELECTMESSAGE2"), SubmitButtonText);
                }
                else
                {
                    if (CampusSoftPreferenceShown && CampusSoftPreference == MatchingEngine.CampusPreference.Online && OnlineMatchesCount == 0)
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.NSM.CSP.ONLINE.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                    else if (CampusSoftPreferenceShown && CampusSoftPreference == MatchingEngine.CampusPreference.Ground && CampusMatchesCount == 0)
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.NSM.CSP.GROUND.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                    else if (OnlineMatchesCount > 0 && CampusMatchesCount > 0 && IsMobile)//Added specifically for Mobile
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("MOBILE.FE.NSM.CSP.BOTH.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                    else if (OnlineMatchesCount > 0 && CampusMatchesCount > 0)
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.NSM.CSP.BOTH.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                    else if (IsMobile)//Added specifically for Mobile
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("MOBILE.FE.NSM.CSP.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                    else
                        Result.NoSmartMatchMessage1 = string.Format(FormsEngine.GetResourceMetaDataTextForKey("FE.NSM.CSP.ECNOSMARTMATCHMESSAGE1"), SubmitButtonText);
                }
            }
            catch (Exception)
            {
                throw;
            }

            Log.EndLogDetail();
            Log.EndLog(Result);
            return Result;
        }



        public CrossSellProgramResponse GetProgramsForCrossSell(Guid TrackGuid, ProspectInput ProspectInput, CampusType? CType, bool IsBeta, int ProgramProductId, int InstitutionId, int? RSMaxProgramsToDisplayTotal, int TemplateId, Guid? TrackingDeviceGUID, string FESessionId, Dictionary<string, string> LeadDataDictionary, bool ProgramWizard, bool initialLeadSuccess, int? ApplicationId)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetProgramsForCrossSell", null, TrackGuid, ProspectInput, CType, IsBeta, ProgramProductId, InstitutionId, RSMaxProgramsToDisplayTotal, TemplateId, TrackingDeviceGUID, FESessionId, LeadDataDictionary, ProgramWizard, initialLeadSuccess, ApplicationId);
            Log.StartLogDetail("FormsEngine.GetProgramsForCrossSell");

            CrossSellMatchRequest Request = new CrossSellMatchRequest();
            Request.Application = MatchingEngine.ISApplication.FormsEngine;
            Request.ApplicationId = ApplicationId.HasValue ? ApplicationId.Value : 0;
            Request.TrackGuid = TrackGuid;
            Request.ProspectInput = ProspectInput;
            if (CType != null)
            {
                Request.CampusType = CType;
            }
            Request.FormProgramProductId = ProgramProductId;
            Request.FormInstitutionId = InstitutionId;
            Request.MaxResultsCount = RSMaxProgramsToDisplayTotal;
            Request.FormTemplateId = TemplateId;
            Request.FormDefaultTemplateId = dbTemplateService.GetDefaultTemplateId(false);
            Request.TrackingDeviceGuid = TrackingDeviceGUID;
            Request.InitialLeadSuccess = initialLeadSuccess;

            if (ProgramWizard)
            {
                Request.LeadCreationType = MatchingEngine.LeadCreationType.ProgramWizardUserSelection;
            }
            else
            {
                Request.LeadCreationType = MatchingEngine.LeadCreationType.InstitutionFormCrossSell;
            }
            

            if (LeadDataDictionary != null && LeadDataDictionary.Count > 0)
            {
                LeadScoringService.ScoringRequest scoreRequest = RelatedServices.BuildLeadScoreRequest(TrackGuid, LeadDataDictionary, new List<int>(), new List<int>(), null, ProspectInput);
                Request.LeadScoringInput = RelatedServices.GetLeadScoringInput(scoreRequest, FESessionId);
            }

            CrossSellProgramResponse Response = RelatedServices.GetProgramsForCrossSell(Request, IsBeta);

            Log.EndLogDetail();
            Log.EndLog(Response);

            return Response;
        }

        public WizardMatchResponse GetProgramsForManagedChoice(Guid TrackGuid, Guid LimboAlternativeTrackIdField, bool LimboAlternativeTrackIdUtilizedField, ProspectInput ProspectInput, CampusPreference? CampusPreference, bool IsBeta, int previousSMLeadCount, int previousUSLeadCount, bool includeSMs, bool includeUSs, Dictionary<string, string> LeadDataDictionary, List<int> institutuionsSmartMatched, bool? SplitCampusTypeInResults, Guid? TrackingDeviceGUID, string FESessionId, int? ApplicationId, List<ProgramWithInstitutionCampus> ThirdPartySmartMatchList1)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetProgramsForManagedChoice", null, TrackGuid, LimboAlternativeTrackIdField, LimboAlternativeTrackIdUtilizedField, ProspectInput, CampusPreference, IsBeta, previousSMLeadCount, previousUSLeadCount, includeSMs, includeUSs, LeadDataDictionary, institutuionsSmartMatched, SplitCampusTypeInResults, TrackingDeviceGUID, FESessionId);
            Log.StartLogDetail("FormsEngine.GetProgramsForManagedChoice");
            WizardMatchRequest Request = BuildWizardMatchRequest(IsBeta, previousSMLeadCount, previousUSLeadCount, includeSMs, includeUSs, TrackGuid, LeadDataDictionary, ProspectInput, CampusPreference, institutuionsSmartMatched, SplitCampusTypeInResults, null, TrackingDeviceGUID, ApplicationId, FESessionId, ThirdPartySmartMatchList1);
            if (LimboAlternativeTrackIdUtilizedField)
            {
                Request.TrackGuid = LimboAlternativeTrackIdField;
            }
            Request.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelection;        

            WizardMatchResponse Response = RelatedServices.GetWizardMatches(Request, IsBeta);
            if (LimboAlternativeTrackIdUtilizedField)
            {
                Response.LimboAlternativeTrackId= LimboAlternativeTrackIdField;
                Response.LimboAlternativeTrackIdUtilized = true;
            }
            Log.EndLogDetail();
            Log.EndLog(Response);
            return Response;
        }

        public WizardMatchRequest BuildWizardMatchRequest(bool IsBeta, int PreviousSMLeadCount, int PreviousUSLeadCount, bool IncludeSMs, bool IncludeUSs
                                 , Guid TrackGuid, Dictionary<string, string> LeadData, ProspectInput Prospect, CampusPreference? CampusPreference
                                 , List<int> institutuionsSmartMatched, bool? SplitCampusTypeInResults
                                 , List<int> ProgramTemplates
                                 , Guid? TrackingDeviceGUID
                                 , int? ApplicationId
                                 , string FESessionId
                                 , List<ProgramWithInstitutionCampus> ThirdPartySmartMatchList1)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.BuildWizardMatchRequest", null, IsBeta, PreviousSMLeadCount, PreviousUSLeadCount, IncludeSMs, IncludeUSs, TrackGuid, LeadData, Prospect, CampusPreference, institutuionsSmartMatched, SplitCampusTypeInResults, ProgramTemplates, TrackingDeviceGUID, FESessionId);
            Log.StartLogDetail("FormsEngine.BuildWizardMatchRequest");

            WizardMatchRequest Request = new WizardMatchRequest();
            Request.Application = MatchingEngine.ISApplication.FormsEngine;
            Request.TrackGuid = TrackGuid;
            Request.ProspectInput = Prospect;
            Request.IsBeta = IsBeta;
            Request.CampusPreference = CampusPreference;
            Request.ThirdPartySmartMatchList = ThirdPartySmartMatchList1 == null ? null : ThirdPartySmartMatchList1.ToArray();

            if (ApplicationId.HasValue)
            {
                Request.ApplicationId = ApplicationId.Value;
            }

            if (institutuionsSmartMatched != null && institutuionsSmartMatched.Count() > 0)
            {
                Request.SmartMatchedInstituionIdList = institutuionsSmartMatched.ToArray();
            }

            string desiredDegree = StringExtensions.GetFieldValue("Desired_Degree_Level", LeadData, false);
            if (!String.IsNullOrWhiteSpace(desiredDegree))
            {
                Request.DesiredProgramLevelList = desiredDegree.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            string dialerKey = StringExtensions.GetFieldValue("DialerKey", LeadData, false);
            if (!String.IsNullOrWhiteSpace(dialerKey))
            {
                Request.DialerKey = dialerKey;
            }

            string tsr = StringExtensions.GetFieldValue("Tsr", LeadData, false);
            if (!String.IsNullOrWhiteSpace(tsr))
            {
                Request.TSR = tsr;
            }

            string userIdStr = StringExtensions.GetFieldValue("UserId", LeadData, false);
            if(!string.IsNullOrWhiteSpace(userIdStr))
            {
                int userId;
                if (int.TryParse(userIdStr, out userId))
                    Request.UserId = userId;
            }

            Request.IncludeSmartMatchList = IncludeSMs;
            Request.IncludeSchoolSelectionList = IncludeUSs;

            Request.TotalSmartMatchesSaved = PreviousSMLeadCount;
            Request.TotalLeadsSaved = PreviousSMLeadCount + PreviousUSLeadCount;

            Request.SplitCampusTypeInResults = SplitCampusTypeInResults;

            Request.TrackingDeviceGuid = TrackingDeviceGUID;

            string categories = StringExtensions.GetFieldValue("Categories", LeadData, false);
            string subCategories = StringExtensions.GetFieldValue("SubCategories", LeadData, false);
            string specialties = StringExtensions.GetFieldValue("Specialties", LeadData, false);
            string boostSpecialties = StringExtensions.GetFieldValue("BSpecialties", LeadData, false);

            if (!String.IsNullOrWhiteSpace(categories))
            {
                Request.CategoryList = categories.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (!String.IsNullOrWhiteSpace(subCategories))
            {
                Request.SubjectList = subCategories.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (!String.IsNullOrWhiteSpace(specialties))
            {
                Request.SpecialtyList = specialties.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (!String.IsNullOrWhiteSpace(boostSpecialties))
            {
                Request.BoostSpecialtyList = boostSpecialties.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (ProgramTemplates != null && ProgramTemplates.Count() > 0)
            {
                Request.TemplateList = ProgramTemplates.ToArray();
            }

            LeadScoringService.ScoringRequest scoreRequest = RelatedServices.BuildLeadScoreRequest(TrackGuid
                ,LeadData
                ,Request.CategoryList != null ? Request.CategoryList.ToList() : new List<int>()
                ,Request.SubjectList != null ? Request.SubjectList.ToList() : new List<int>()
                ,Request.DesiredProgramLevelList?.First()
                ,Prospect);

            Request.LeadScoringInput = RelatedServices.GetLeadScoringInput(scoreRequest, FESessionId);

            //Geo input for International templates
            int? DesiredCountryId = StringExtensions.GetFieldValue("Desired_Country", LeadData, null);
            int? DesiredCityId = StringExtensions.GetFieldValue("Desired_City", LeadData, null);

            if (DesiredCountryId.HasValue || DesiredCityId.HasValue)
            {
                Request.GeoTarget = new GeoTarget();
                if (DesiredCountryId.HasValue)
                {
                    Request.GeoTarget.CountryList = new int[] { DesiredCountryId.Value };
                }

                if (DesiredCityId.HasValue)
                {
                    Request.GeoTarget.CityList = new int[] { DesiredCityId.Value };
                }
            }

            //ME International Callcenter Filter for only CPL
            if(StringExtensions.GetFieldValue("InternationalCallcenterForm", LeadData) == "true")
            {
                /*ProgramType SAB and CPL */
                Request.SFProductCodes = new SFProductCode[] { SFProductCode.SAB_CPL };
                Request.ProgramTypeList = new ProgramType[] { ProgramType.StudyAbroad };
            }

            Log.EndLogDetail();
            Log.EndLog(Request);
            return Request;
        }


        /// <summary>
        /// Method to return dummy data for autoamic Cross Sell pop-up for optimizely use
        /// </summary>
        /// <param name="crossSellRequest"></param>
        /// <returns></returns>
        public CrossSellProgramResponse GetProgramsForOptimizelyCrossSell(int numberOfPrograms, int maxCrossSellUserSelections)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetProgramsForOptimizelyCrossSell", null, numberOfPrograms, maxCrossSellUserSelections);
            Log.StartLogDetail("FormsEngine.GetProgramsForOptimizelyCrossSell");
            CrossSellProgramResponse programResponse = new CrossSellProgramResponse();

            var ProgramList = new List<ProgramWithInstitutionCampus>();
            //programResponse.MaxCrossSellUserSelections = maxCrossSellUserSelections;

            bool first;
            for (int i = 1; i <= numberOfPrograms; i++)
            {
                first = (i == 1) ? true : false;

                ProgramWithInstitutionCampus program = new ProgramWithInstitutionCampus();
                program.ProgramProductId = 123000 + i;
                program.ProgramDescription = "ProgramDescription__" + i.ToString() + "_ AIU Online is offering a Bachelor of Science in Criminal Justice degree completion program with a specialization in Forensic Science for those students interested in pursuing career opportunities like Crime Scene Analyst, Non-Sworn Forensic Personnel and many others. This course of study offers an opportunity to study career-focused course material that includes:  clear explanations of the techniques, abilities and limitations within the field of forensic science and its applications to criminal investigations; forensic science techniques of criminal investigations, how to process a crime scene and notify the next of kin; and appropriate techniques to safeguard evidence and interact with investigative authorities. This degree is an accelerated program that can be completed online with great flexibility. AIU is aware that focused professionals need to keep their knowledge and expertise current with industry standards in order to be acknowledged for their accomplishments and considered for the future, so AIU degree programs are updated to keep pace with the ever-changing theories and practices of their respective disciplines. The AIU community believes it has a special commitment to support each individual's goals and emphasis on the educational, professional, and personal growth of each student. This is a Bachelor's degree completion program and assumes that all associate-level requirements have been met through an associate degree or the equivalent. AIU cannot guarantee employment or salary.";
                program.ProgramName = "BS Business Administration__" + i.ToString();
                program.ProgramType = ProgramType.FullDegree;
                program.ProgramLevelId = 5;
                program.ProgramRankScore = 0.98M;
                program.ProgramId = 53423;
                program.ProgramDisclaimerType = first ? DisclaimerType.Text : DisclaimerType.Link;
                program.ProgramDisclaimer = first ? "ProgramDisclaimer__" + i.ToString() + "_ The Classification of Instructional Programs (CIP) provides a taxonomic scheme that supports the accurate tracking and reporting of fields of study and program completions activity. CIP was originally developed by the U.S. Department of Education's National Center for Education Statistics (NCES).* \n *US Department of Education Institute of Education Sciences/National Center for Education Statistics website." : "https://www.kaplanuniversity.edu/Consumer_Info.aspx";
                program.ProgramCampusType = first ? CampusType.Online : CampusType.Ground;
                program.InstitutionId = 226;
                program.InstitutionName = "Kaplan University__" + i.ToString();
                program.InstitutionDisclaimerType = first ? DisclaimerType.Link : DisclaimerType.Text;
                program.InstitutionDisclaimer = first ? "https://www.kaplanuniversity.edu/Consumer_Info.aspx" : "InstitutionDisclaimer__" + i.ToString() + "_ The Classification of Instructional Programs (CIP) provides a taxonomic scheme that supports the accurate tracking and reporting of fields of study and program completions activity. CIP was originally developed by the U.S. Department of Education's National Center for Education Statistics (NCES).* \n *US Department of Education Institute of Education Sciences/National Center for Education Statistics website.";
                program.CampusId = 42;
                program.CampusType = first ? CampusType.Online : CampusType.Ground;
                program.CampusName = "Fort Lauderdale";
                program.CampusAddress1 = "123 Test Street";
                program.CampusAddress2 = "";
                program.CampusCity = "Fort Lauderdale";
                program.CampusState = "FL";
                program.CampusPostalCode = "33312";
           

                ProgramList.Add(program);
            }
            programResponse.ProgramList = ProgramList.ToArray();

            Log.EndLogDetail();
            Log.EndLog(programResponse);
            return programResponse;
        }

        /// <summary>
        /// Validates if Rendering strategy exists and is assigned to the TemplateType
        /// </summary>
        /// <param name="RenderingStrategyName"></param>
        /// <param name="TemplateTypeId"></param>
        /// <returns></returns>
        public bool ValidateRenderingStrategy(string RenderingStrategyName, FormTemplateTypes FormTemplateType, out HTMLRenderingStrategyDTO RenderingStrategy)
        {
            HTMLRenderingStrategy Rendering = null;
            RenderingStrategy = null;
            bool Result = dbRenderingStrategyService.ValidateRenderingStrategy(RenderingStrategyName, FormTemplateType, out Rendering);

            if (Result)
            {
                RenderingStrategy = Rendering.ConvertToDTO();
            }

            return Result;
        }


        /// <summary>
        /// Gets forms meta data value from key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetResourceMetaDataTextForKey(string Key)
        {
            return dbMetaDataService.GetResourceMetaDataTextForKey(Key);
        }


        /// <summary>
        /// Program template messges
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<ProgramTemplateMessageDTO> GetProgramTemplateMessageForType(string Type)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetProgramTemplateMessageForType", null, Type);
            Log.StartLogDetail("FormsEngine.ResourceMetaDataService.GetProgramTemplateMessageForType");

            List<ProgramTemplateMessageDTO> TheProgramTemplateMessageDTOList = null;
            try 
            {
                List<ProgramTemplateMessage> TheProgramTemplateMessageList = dbMetaDataService.GetProgramTemplateMessageForType(Type);
                if (TheProgramTemplateMessageList != null)
                {
                    TheProgramTemplateMessageDTOList = (List<ProgramTemplateMessageDTO>)TheProgramTemplateMessageList.ConvertToDTO();
                }
            }
            catch (Exception ex) { new ISException(Base.ISApplication.FormsEngine, new Exception(string.Format("FormsEngine.ResourceMetaDataService.GetProgramTemplateMessageForType failed with the message - {0}", ex.Message)), "FormsEngine.ResourceMetaDataService.GetProgramTemplateMessageForType failed", Type).Save(); }

            Log.EndLogDetail();
            Log.EndLog(TheProgramTemplateMessageDTOList);
            return TheProgramTemplateMessageDTOList;
        }



        /// <summary>
        /// Warmup FE cache
        /// Partially loads FE cache, some dictioaries will be lazy loaded.
        /// </summary>
        public void WarmupCache()
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.WarmupCache", null);

            //Force Template Controls Dictionary to load
            Log.StartLogDetail("TemplateControls Dictionary");
            GetTemplateControls(1);
            Log.EndLogDetail();

            //Force ProgramProduct Dictionary to load
            Log.StartLogDetail("ProgramProducts Dictionary");
            dbTemplateService.GetTemplateIdByProgramProductId(1,true);
            Log.EndLogDetail();

            Log.StartLogDetail("GetProgramLevels");
            GetProgramLevels();
            Log.EndLogDetail();

            Log.StartLogDetail("GetFeaturedList");
            GetFeaturedListSingleProgram();
            Log.EndLogDetail();

            Log.StartLogDetail("OpenMailProfiles");
            GetOpenMailProfiles();
            Log.EndLogDetail();

            Log.EndLog(1);
        }

        /// <summary>
        /// Gets the list Template Application Overrides for the provided Application
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <returns></returns>
        public List<TemplateApplicationOverride> GetTemplateApplicationOverrideForApplication(int ApplicationId)
        {
            return dbTemplateService.GetTemplateApplicationOverrideForApplication(ApplicationId);
        }

        /// <summary>
        /// Gets the Template Id from the Template Application Override table for the Application and PaidStatus provided
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <param name="PaidStatusTypeId"></param>
        /// <returns></returns>
        public int GetTemplateApplicationOverrideForApplicationAndPaidStatus(int ApplicationId, int PaidStatusType)
        {
            return dbTemplateService.GetTemplateApplicationOverrideForApplicationAndPaidStatus(ApplicationId, PaidStatusType);
        }


        public static Dictionary<string, string> GetResourceMetaDataForTCPA()
        {
            return dbMetaDataService.GetResourceMetaDataForTCPA();
        }

        /// <summary>
        /// Gets the specific Institution TCPA message by InstitutionId
        /// </summary>
        /// <param name="InstitutionId"></param>
        /// <returns></returns>
        public static string GetInstitutionTCPAText(int InstitutionId)
        {
            return dbTCPAMessageService.GetEMSInstitutionTCPAText(InstitutionId);
        }

        public static VW_LandingPageSettingsDTO GetLandingPageSettings(string formLeadUrl)
        {
            VW_LandingPageSettingsDTO r = null;

            VW_LandingPageSettings result = dbLandingPageDataService.GetLandingPageByUrl(formLeadUrl);
            if (result != null)
            {
                return result.ConvertToDTO();
            }
            else {
                return r;
            }
        }

        /// <summary>
        /// Gets the Program Template Model by ProgramProductId
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <returns></returns>
        public TemplateDTO GetProgramAllTemplatesMergedModel(int[] templateIds)
        {
            return GetMergedTemplates(templateIds, false);
        }

        public Dictionary<string, List<ValueList>> GetStandardControlPreDefinedValueList() {
            return dbTemplateService.StandardControlPreDefinedValueList;
        }

        public List<KeyValuePair<string, string>> GetKVCodeData(string kvCodeDataName, int institutionId)
        {
            return dbTemplateService.GetKVCodeDataForInstitution(kvCodeDataName, institutionId);
        }

        public List<DataModel.ProgramLevel> GetProgramLevels()
        {
            return dbTemplateService.GetProgramLevels();
        }

        public List<int> GetFeaturedListSingleProgram()
        {
            return dbTemplateService.GetFeaturedListSingleProgram();
        }
    }
}
