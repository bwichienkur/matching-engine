using EDDY.IS.Core;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.Mapper;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using EDDY.IS.Util.Collections;
using System.Collections;
using Newtonsoft.Json;
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsEngine : FormsEngineBase
    {
        public APIValidationResultDTO QuickCheckValidateForm(int TemplateId, bool IsBeta, Guid TrackGuid, Dictionary<string, string> LeadData, ref PerformanceLog Log)
        {
            APIValidationResultDTO submissionResultDTO = new APIValidationResultDTO();

            if (ConfigurationManager.AppSettings.Get("WebLeadsServerSideValidationEnabled").ToLower() == "true")
            {
                // Form Validation
                Log.StartLogDetail("ValidateWizardForm.InternalFormValidation");
                int campaignApplicationId = dbCampaignDataService.GetCampaignApplicationIdByTrackId(TrackGuid);
                var ValidateResultDTO = InternalValidateForm(TemplateId, IsBeta, LeadData, true, true, true, campaignApplicationId);
                Log.EndLogDetail();
                submissionResultDTO.Valid = ValidateResultDTO.Valid;
                submissionResultDTO.ValidationMessages = ValidateResultDTO.ValidationMessages;
            }
            else
            {
                //SSV is turned off so always return empty messages list and true for valid
                submissionResultDTO.Valid = true;
                submissionResultDTO.ValidationMessages = new List<KeyValuePair<string, string>>();
            }
            return submissionResultDTO;
        }

        public ProspectInput BuildProspect(bool IsBeta, string TrackId, string LeadData)
        {
            ProspectInput Result = null;
            try
            {
                LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, 0, IsBeta, TrackId, "", false, "", "", LeadData, null, null, LeadCreationType.HostAndPost, null);
                CreateMatchingEngineMappings(LeadRequest);
                ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                Result = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        /// <summary>
        /// Validates a form against ME, and FE and returns a full list of validation errors
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="LeadData"></param>
        /// <returns></returns>
        public APIValidationResultDTO ValidateForm(int ProgramProductId
            , bool IsBeta
            , string TrackId
            , string LeadData
            , bool MatchingEngineValidation
            , ref PerformanceLog Log
            , MatchingEngine.ISApplication ISApplicationID = MatchingEngine.ISApplication.FormsEngine)
        {
            APIValidationResultDTO submissionResultDTO = new APIValidationResultDTO();
            int TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(ProgramProductId, false);

            //0. create LeadCreateRequest object
            Log.StartLogDetail("ValidateForm.CreateLeadRequest");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, ProgramProductId, IsBeta, TrackId, "", false, "", "", LeadData, null, null, null, null);
            CreateMatchingEngineMappings(LeadRequest);
            Log.EndLogDetail();

            //1. Form Validation
            Log.StartLogDetail("ValidateForm.InternalFormValidation");
            var ValidateResultDTO = InternalValidateForm(TemplateId, IsBeta, LeadRequest.LeadData, false, false);
            Log.EndLogDetail();
            submissionResultDTO.Valid = ValidateResultDTO.Valid;
            submissionResultDTO.ValidationMessages = ValidateResultDTO.ValidationMessages;

            if (MatchingEngineValidation)
            {

                //2. Matching Engine rules validation
                ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
                ProgramValidationRequest.Application = MatchingEngine.ISApplication.FormsEngine;
                ProgramValidationRequest.TrackGuid = LeadRequest.TrackId.HasValue ? LeadRequest.TrackId.Value : Guid.Empty;
                ProgramValidationRequest.ProgramProductId = ProgramProductId;
                ProgramValidationRequest.BreakOnFirstValidationFailure = false;
                ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
                ProgramValidationRequest.ProspectInput = Prospect;
                Log.StartLogDetail("ValidateForm.MatchingEngineValidation");
                ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                ProgramValidateResponse ProgramValidationResponse = RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);
                Log.EndLogDetail();

                if (!ProgramValidationResponse.PassedValidation)
                {
                    submissionResultDTO.Valid = false;
                    foreach (var Failure in ProgramValidationResponse.RuleFailures)
                    {
                        submissionResultDTO.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.BusinessRulesCheck, Failure.RuleFailureName));
                    }
                }
            }
            return submissionResultDTO;
        }


        public APIMultiValidationResultDTO ValidateMultipleForms(List<int> ProgramProducts
            , bool IsBeta
            , string TrackId
            , string LeadData
            , bool MatchingEngineValidation
            , ref PerformanceLog Log
            , int ApplicationId = 0
            , MatchingEngine.ISApplication ISApplicationID = MatchingEngine.ISApplication.FormsEngine)
        {
            APIMultiValidationResultDTO submissionResultDTO = new APIMultiValidationResultDTO();
            bool allValid = true;

            foreach (int programProductId in ProgramProducts)
            {
                APIProgramValidationResultDTO validationResult = new APIProgramValidationResultDTO();

                int TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(programProductId, false);

                //0. create LeadCreateRequest object
                Guid TrackGuid = Guid.Empty;
                Guid.TryParse(TrackId, out TrackGuid);
                Guid NoGuid = Guid.Empty;
                Log.StartLogDetail("ValidateForm.CreateLeadRequest");
                LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, programProductId, IsBeta, TrackId, "", false, "", "", LeadData, null, null, null, null);
                CreateMatchingEngineMappings(LeadRequest);
                Log.EndLogDetail();

                //1. Form Validation
                Log.StartLogDetail("ValidateForm.InternalFormValidation");
                var ValidateResultDTO = InternalValidateForm(TemplateId, IsBeta, LeadRequest.LeadData, false, false, false, ApplicationId);
                Log.EndLogDetail();

                if (!ValidateResultDTO.Valid) validationResult.IsValid = false;

                foreach (var item in ValidateResultDTO.ValidationMessages)
                    validationResult.ErrorMessages.Add(item.Value);

                if (MatchingEngineValidation)
                {

                    //2. Matching Engine rules validation
                    ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
                    ProgramValidationRequest.Application = ISApplicationID;
                    ProgramValidationRequest.IgnoreCaps = true;
                    ProgramValidationRequest.TrackGuid = TrackGuid;
                    ProgramValidationRequest.ProgramProductId = programProductId;
                    ProgramValidationRequest.BreakOnFirstValidationFailure = false;
                    ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                    ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
                    ProgramValidationRequest.ProspectInput = Prospect;
                    Log.StartLogDetail("ValidateForm.MatchingEngineValidation");
                    ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.Advising;
                    ProgramValidateResponse ProgramValidationResponse = RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);
                    Log.EndLogDetail();

                    if (!ProgramValidationResponse.PassedValidation)
                    {
                        validationResult.IsValid = false;
                        foreach (var Failure in ProgramValidationResponse.RuleFailures)
                        {
                            validationResult.ErrorMessages.Add(Failure.RuleFailureName);
                        }
                    }
                }

                if (allValid && !validationResult.IsValid)
                {
                    allValid = false;
                }

                submissionResultDTO.ValidationResults.Add(validationResult);
            }

            submissionResultDTO.AllValid = allValid;

            return submissionResultDTO;
        }

        public APISubmissionResultDTO ProcessHostAndPostLead(int ProgramProductId
            , bool IsBeta
            , string TrackId
            , string LeadData
            , ref RawPostDataDTO RawData
            , ref PerformanceLog Log
            , MatchingEngine.ISApplication ISApplicationID = MatchingEngine.ISApplication.FormsEngine
            , int? ClientRelationContactId = null)
        {
            APISubmissionResultDTO submissionResultDTO = new APISubmissionResultDTO();
            submissionResultDTO.ValidationMessages = new List<KeyValuePair<string, string>>();
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);

            int TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(ProgramProductId, false);

            //0. create LeadCreateRequest object
            Guid TrackGuid = Guid.Empty;
            Guid.TryParse(TrackId, out TrackGuid);
            int campaignApplicationId = dbCampaignDataService.GetCampaignApplicationIdByTrackId(TrackGuid);
            Log.StartLogDetail("ProcessHostAndPostLead.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, ProgramProductId, IsBeta, TrackId, "", false, "", "", LeadData, null, null, LeadCreationType.HostAndPost, null);
            bool DoMEValidation = true;
            bool DoFEValidation = true;
            bool DoOnlyDupeValidation = false;
            //we want profanity to run for EMS. 
            bool DoProfanityCheck = campaignApplicationId == (int)MatchingEngine.ISApplication.EMS;

            //Skip FE/ME validation (program related) if is the special EMS template for EMS app.
            int emsAPIPreValidatedTemplateId = 0;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("EMSAPIPreValidatedTemplateId"))
                && (int.TryParse(ConfigurationManager.AppSettings.Get("EMSAPIPreValidatedTemplateId"), out emsAPIPreValidatedTemplateId)))
            {
                if (campaignApplicationId == (int)MatchingEngine.ISApplication.EMS && TemplateId == emsAPIPreValidatedTemplateId)
                {
                    DoMEValidation = false;
                    DoFEValidation = false;
                    DoOnlyDupeValidation = true;
                }
            }

            //Skip FE Validation for EMS app on Partner channels only coming from the API, validate only for EMS Partners
            if (campaignApplicationId == (int)MatchingEngine.ISApplication.EMS
                && LeadRequest.ChannelId != (int)EDDY.IS.Base.Channel.CallCenterPartners
                && LeadRequest.ChannelId != (int)EDDY.IS.Base.Channel.OnlinePartners)
            {
                DoMEValidation = true;
                DoFEValidation = false;
                DoOnlyDupeValidation = false;
            }

            //Edu MAX
            if (LeadRequest.PreValidatedProgram)
            {
                DoMEValidation = false;
                DoFEValidation = false;
                DoOnlyDupeValidation = false;
            }

            //Non US/Canada fix for H&P
            string Country = LeadRequest.LeadData.ContainsKey("Country") ? LeadRequest.LeadData["Country"] : "";
            if (!string.IsNullOrWhiteSpace(Country) && Country.ToUpper() != "US" && Country.ToUpper() != "CA")
            {
                if (LeadRequest.LeadData.ContainsKey("State"))
                {
                    LeadRequest.LeadData["State"] = "N/A";
                }
                else
                {
                    LeadRequest.LeadData.Add("State", "N/A");
                }
            }
            CreateMatchingEngineMappings(LeadRequest);
            Log.EndLogDetail();

            //1. Form Validation
            if (DoFEValidation)
            {
                Log.StartLogDetail("ProcessHostAndPostLead.InternalFormValidation");
                var ValidationResult = InternalValidateForm(TemplateId, IsBeta, LeadRequest.LeadData, false, false, ApplicationId: campaignApplicationId, leadCreationType: LeadRequest.LeadCreationTypeId.Value);
                Log.EndLogDetail();
                submissionResultDTO.Valid = ValidationResult.Valid;
                submissionResultDTO.ValidationMessages = ValidationResult.ValidationMessages;
            }
            else
            {
                if (DoProfanityCheck)
                {
                    Log.StartLogDetail("ProcessHostAndPostLead.ProfanityCheckForm");
                    var ValidationResult = ProfanityCheckForm(TemplateId, LeadRequest.LeadData);
                    Log.EndLogDetail();
                    submissionResultDTO.Valid = ValidationResult.Valid;
                    submissionResultDTO.ValidationMessages = ValidationResult.ValidationMessages;
                }
                else
                {
                    submissionResultDTO.Valid = true;
                }
            }

            //2. Matching Engine rules validation
            if (DoMEValidation || DoOnlyDupeValidation)
            {
                Log.StartLogDetail("ProcessHostAndPostLead.InternalMEValidateProgram");
                Tuple<bool, int?> MEResponse = InternalMEValidateProgram(LeadRequest, ProgramProductId, IsBeta, submissionResultDTO, DoOnlyDupeValidation);
                LeadRequest.PassedValidation = MEResponse.Item1;
                LeadRequest.ScoreId = MEResponse.Item2;
                Log.EndLogDetail();
            }

            int prospectId = 0;
            int prospectFlowId = 0;

            //Re-use Prospect
            if (LeadRequest.ProspectFlowId.HasValue && LeadRequest.ProspectFlowId.Value > 0)
            {
                Log.StartLogDetail("ProcessHostAndPostLead.GetProspectFlowDetails");
                GetProspectFlowDetailsResponse getProspectFlowDetailsResponse = RelatedServices.GetProspectFlowDetails(LeadRequest.ProspectFlowId.Value);
                if (getProspectFlowDetailsResponse.ProspectFlowDTO != null && getProspectFlowDetailsResponse.ProspectFlowDTO.ProspectId > 0)
                {
                    prospectId = getProspectFlowDetailsResponse.ProspectFlowDTO.ProspectId;
                    prospectFlowId = getProspectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowId;
                }
                Log.EndLogDetail();
            }
            else //Save prospect
            {
                var prospectFlowType = campaignApplicationId == (int)MatchingEngine.ISApplication.EMS ? ProspectFlowTypes.EMS : ProspectFlowTypes.Prospecting;
                SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.HasValue ? LeadRequest.TrackId.Value : Guid.Empty, prospectFlowType);
                var Result = RelatedServices.SaveProspect(WebServiceProspect);
                prospectId = Result.ProspectId;
                prospectFlowId = Result.ProspectId;
            }

            //Save Submission and Lead
            Log.StartLogDetail("LeadSaveManager.Execute");
            List<LeadCreateResponse> leadCreateResponses = leadSaveManager.Execute(null, new List<LeadCreateRequest> { LeadRequest }, "", ISApplication.HostAndPost, RawData, prospectId, null, null, ClientRelationContactId, false, null, null, prospectFlowId);
            Log.EndLogDetail();

            LeadCreateResponse leadCreateResponse = leadCreateResponses.FirstOrDefault();
            submissionResultDTO.UID = leadCreateResponse.Lead.UID;
            submissionResultDTO.LeadId = leadCreateResponse.Lead.LeadId;
            submissionResultDTO.IsTestLead = leadCreateResponse.IsTestLead;


            return submissionResultDTO;
        }

        private string MassageMERuleFailure(RuleFailure rf)
        {
            string result = rf.RuleFailureName;

            if (rf.RuleFailureType.HasValue && rf.RuleFailureType.Value == BaseRuleType.LeadCap)
            {
                if (rf.RuleFailureName == "Forcefully Capping Out" || rf.RuleFailureName == "Cap Limit Reached")
                {
                    result = "Monthly Cap Limit Reached";
                }
                else if (rf.RuleFailureName == "Normalization Cap Limit Reached" || rf.RuleFailureName == "Day Parting Cap Limit Reached")
                {
                    result = "Day Cap Limit Reached";
                }
                if (rf.EntityType.HasValue && rf.EntityType.Value == EntityMeta.ClientRelationProductMapping)
                {
                    result += " at Institution Level";
                }
                else if (rf.EntityType.HasValue && rf.EntityType.Value == EntityMeta.ClientCampusRelationship)
                {
                    result += " at Campus Level";
                }
                else
                {
                    result += " at Program Level";
                }
            }

            if (rf.RuleFailureType.HasValue && rf.RuleFailureType.Value == BaseRuleType.LeadScoringMinimumTierLevel)
                result = "EDDY - LeadScoringMinimumTierLevel";

            return result;
        }

        public void ReleaseTitaniumLead(ref PerformanceLog Log, int programProductId, string LeadData, int? ProspectId, int? ClientRelationContactId, int? prospectFlowId, int leadId)
        {
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            LeadSaveData lead = new LeadSaveData();

            lead.ProgramProductId = programProductId;
            lead.TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(programProductId, false);

            Log.StartLogDetail("LeadSaveManager.Execute");
            LeadCreateResponse leadCreateResponse = leadSaveManager.UpdateLead(lead, LeadData, ProspectId, ClientRelationContactId, prospectFlowId, leadId);
            Log.EndLogDetail();
        }

        public APIMultiSubmissionResultDTO ProcessApolloSubmission(int ApplicationId, List<KeyValuePair<int, string>> ProgramProducts, int ProspectId, int? ClientRelationContactId, string TrackId, string MatchResponseGuid, string LeadData, ref RawPostDataDTO RawData, ref PerformanceLog Log, bool realtimeDelivery, int? prospectFlowId, List<int?> PaidStatusTypeIds, bool isBeta)
        {
            APIMultiSubmissionResultDTO submissionResultDTO = new APIMultiSubmissionResultDTO();
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            List<LeadSaveData> leadSaveDataList = new List<LeadSaveData>();

            for (int i = 0; i < ProgramProducts.Count; i++)
            {
                int TemplateId = dbTemplateService.GetTemplateIdByProgramProductId(ProgramProducts[i].Key, false);
                LeadSaveData leadSaveData = new LeadSaveData();
                leadSaveData.ProgramProductId = ProgramProducts[i].Key;
                leadSaveData.TemplateId = TemplateId;

                if (!string.IsNullOrEmpty(ProgramProducts[i].Value))
                {
                    Guid emg = new Guid();
                    if (Guid.TryParse(ProgramProducts[i].Value, out emg) && emg != new Guid())
                    {
                        leadSaveData.ExternalMatchItemGuid = emg;
                    }                   
                }
                if (PaidStatusTypeIds != null && PaidStatusTypeIds.Count > i)
                {
                    leadSaveData.PaidStatusTypeId = PaidStatusTypeIds[i];
                }

                try
                {
                    // Try upsell program products in Matching Engine
                    LeadCreateRequest leadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, leadSaveData.ProgramProductId, isBeta, TrackId, "", false, "", "", LeadData, null, null, LeadCreationType.HostAndPost, null);
                    leadRequest.ClientRelationContactId = ClientRelationContactId;
                    Tuple<bool, int> meResponse = MEValidateProgramUpsell(leadRequest, leadSaveData.ProgramProductId, isBeta);
                    if (meResponse.Item1 == true)
                        leadSaveData.ProgramProductId = meResponse.Item2;
                }
                catch (Exception ex)
                {
                    new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
                }

                leadSaveDataList.Add(leadSaveData);
            }

            Guid? leadScoringGuid = null;

            //Save Submission and Lead
            Log.StartLogDetail("LeadSaveManager.Execute");
            List<LeadCreateResponse> leadCreateResponses = leadSaveManager.Execute(null, null, LeadCreationType.Apollo, ISApplication.Apollo, leadSaveDataList, isBeta, TrackId, null, false, null, MatchResponseGuid, RawData, LeadData, null, ProspectId, null, null, null, ClientRelationContactId, realtimeDelivery, null, leadScoringGuid, prospectFlowId);
            Log.EndLogDetail();

            foreach (LeadCreateResponse response in leadCreateResponses)
            {
                APISubmissionResultDTO leadSubmission = new APISubmissionResultDTO();
                leadSubmission.IsTestLead = response.IsTestLead;
                leadSubmission.UID = response.Lead.UID;
                leadSubmission.LeadId = response.Lead.LeadId;
                leadSubmission.Valid = response.Success;

                if (!leadSubmission.Valid)
                    submissionResultDTO.AllValid = false;

                submissionResultDTO.SubmissionResults.Add(leadSubmission);
            }

            return submissionResultDTO;
        }


        public APIProgramMatchesDTO GetProgramMatches(bool IsBeta, string TrackId, string LeadData, PerformanceLog Log)
        {
            APIProgramMatchesDTO Result = new APIProgramMatchesDTO();
            Result.Valid = true;
            bool IsAnyTemplateValid = false;
            var ValidationMessages = new List<KeyValuePair<string, string>>();

            //0. create LeadCreateRequest object
            Log.StartLogDetail("ProcessHostAndPostLead.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, 0, IsBeta, TrackId, "", false, "", "", LeadData, null, null, LeadCreationType.HostAndPost, null);

            //1. Get the list of templates the set of questions fulfills.
            List<int> PotentialTemplatesCovered = dbTemplateService.GetSystemProgramTemplatesCoveredByQuestions(LeadRequest.LeadData.Keys.ToList());
            List<int> TemplatesCovered = new List<int>();

            if (PotentialTemplatesCovered.Count == 0)
            {
                ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.TemplateNoResults));
            }
            else
            {
                //2. Internal template validation
                foreach (int TemplateId in PotentialTemplatesCovered)
                {
                    Log.StartLogDetail("GetProgramMatches.InternalFormValidation");
                    var ValidationResult = InternalValidateForm(TemplateId, IsBeta, LeadRequest.LeadData, false, true);
                    Log.EndLogDetail();
                    if (ValidationResult.Valid)
                    {
                        IsAnyTemplateValid = true;
                        TemplatesCovered.Add(TemplateId);
                    }
                    else
                    {
                        ValidationMessages.AddRange(ValidationResult.ValidationMessages);
                    }
                }
                if (IsAnyTemplateValid)
                {
                    //3. Call ME to get programs
                    CreateMatchingEngineMappings(LeadRequest);
                    ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                    ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
                    WizardMatchRequest MERequest = BuildWizardMatchRequest(IsBeta, 0, 0, false, true, LeadRequest.TrackId.Value, LeadRequest.LeadData, Prospect, null, new List<int>(), false, TemplatesCovered, null, null, null, null);
                    //10-25-2016: HF to allow WTT products to be returned.
                    MERequest.Application = MatchingEngine.ISApplication.VendorAPI;
                    Log.StartLogDetail("GetProgramMatches.GetWizardMatches");
                    MERequest.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                    var MEResponse = RelatedServices.GetWizardMatches(MERequest, IsBeta);
                    Log.EndLogDetail();
                    Result.MaxUserSelectionsField = MEResponse.MaxUserSelections;
                    if (MEResponse.SchoolSelectionList != null && MEResponse.SchoolSelectionList.Count() > 0)
                    {
                        Result.SchoolSelectionList = MEResponse.SchoolSelectionList.ToList();
                    }
                    else
                    {
                        Result.SchoolSelectionList = new List<CampusWithInstitution>();
                        ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.MatchingEngineNoResults));
                        IsAnyTemplateValid = false;
                    }
                }
            }

            Result.Valid = IsAnyTemplateValid;
            Result.ValidationMessages = ValidationMessages.DistinctBy(t => t.Key).ToList();

            return Result;
        }

        public APIProgramMatchesDTO GetEnhancedProgramMatches(bool IsBeta, string TrackId, string LeadData, PerformanceLog Log)
        {
            var FormsEngineService = new FormsEngine();
            APIProgramMatchesDTO Result = new APIProgramMatchesDTO();
            Result.Valid = true;
            bool IsAnyTemplateValid = false;
            var ValidationMessages = new List<KeyValuePair<string, string>>();
            var missingTemplateIds = new List<int?>();
            var additionalQuestionList = new List<KeyValuePair<int, List<TemplateControlDTO>>>();
            //0. create LeadCreateRequest object
            Log.StartLogDetail("ProcessHostAndPostLead.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, 0, IsBeta, TrackId, "", false, "", "", LeadData, null, null, LeadCreationType.HostAndPost, null);

            //1. Get the list of templates the set of questions fulfills.
            List<int> PotentialTemplatesCovered = dbTemplateService.GetSystemProgramTemplatesCoveredByQuestions(LeadRequest.LeadData.Keys.ToList());
            List<int> TemplatesCovered = new List<int>();

            if (PotentialTemplatesCovered.Count == 0)
            {
                ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.TemplateNoResults));
            }
            if (PotentialTemplatesCovered.Count > 0)
            {
                //2. Internal template validation
                foreach (int TemplateId in PotentialTemplatesCovered)
                {
                    Log.StartLogDetail("GetProgramMatches.InternalFormValidation");
                    var ValidationResult = InternalValidateForm(TemplateId, IsBeta, LeadRequest.LeadData, false, true);
                    Log.EndLogDetail();
                    
                    TemplatesCovered.Add(TemplateId);
                    
                    if (ValidationResult.Valid)
                        IsAnyTemplateValid = true;
                    else
                        ValidationMessages.AddRange(ValidationResult.ValidationMessages);
                }
                if (IsAnyTemplateValid)
                {
                    //3. Call ME to get programs
                    CreateMatchingEngineMappings(LeadRequest);
                    ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                    ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
                    WizardMatchRequest MERequest = BuildWizardMatchRequest(IsBeta, 0, 0, false, true, LeadRequest.TrackId.Value, LeadRequest.LeadData, Prospect, null, new List<int>(), false, new List<int>(), null, null, string.Empty, null);
                    Log.StartLogDetail("GetProgramMatches.GetWizardMatches");
                    //10-25-2016: HF to allow WTT products to be returned.
                    MERequest.Application = MatchingEngine.ISApplication.VendorAPI;
                    MERequest.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                    var MEResponse = RelatedServices.GetWizardMatches(MERequest, IsBeta);
                    Log.EndLogDetail();
                    Result.MaxUserSelectionsField = MEResponse.MaxUserSelections;
                    if (MEResponse.SchoolSelectionList != null && MEResponse.SchoolSelectionList.Count() > 0)
                    {
                        Result.SchoolSelectionList = MEResponse.SchoolSelectionList.ToList();


                        missingTemplateIds = (from m in MEResponse.SchoolSelectionList
                                              from t in m.ProgramList
                                              where t.TemplateId != null && !TemplatesCovered.Contains((int)t.TemplateId)
                                              select t.TemplateId).Distinct().ToList();

                        //var nullTemplatesProgramProduct = (from m in MEResponse.SchoolSelectionList
                        //                                   from t in m.ProgramList
                        //                                   where t.TemplateId == null
                        //                                   select t.ProgramProductId).Distinct().ToList();
                        //if (nullTemplatesProgramProduct.Count() > 0)
                        //{
                        //    StringBuilder exceptionString = new StringBuilder();
                        //    exceptionString.Append("GetEnhancedProgramMatches Exception:");
                        //    exceptionString.Append("The Following ProgramProductId are missing TemplateIds : ");
                        //    foreach (var ProgramProductId in nullTemplatesProgramProduct)
                        //    {
                        //        exceptionString.AppendFormat("ProgramProductId: {0}", ProgramProductId);
                        //    }
                        //    exceptionString.AppendFormat("ME GetWizardMatches request: {0}", JsonConvert.SerializeObject(MERequest));


                        //    Exception exc = new Exception(exceptionString.ToString());
                        //    new ISException(Base.ISApplication.FormsEngine, exc).Save();
                        //}

                    }
                    else
                    {
                        Result.SchoolSelectionList = new List<CampusWithInstitution>();
                        ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.MatchingEngineNoResults));
                        IsAnyTemplateValid = false;
                    }
                    foreach (var templateId in missingTemplateIds)
                    {
                        var templateControls = FormsEngineService.GetAdditionalTemplateQuestionsForProgramMatch(TemplatesCovered, (int)templateId, false);
                        additionalQuestionList.Add(new KeyValuePair<int, List<TemplateControlDTO>>((int)templateId, templateControls));
                    }
                }
            }

            Result.Valid = IsAnyTemplateValid;
            Result.ValidationMessages = ValidationMessages.DistinctBy(t => t.Key).ToList();
            Result.TemplateIdList = TemplatesCovered;
            Result.AdditionalQuestionList = additionalQuestionList;
            return Result;
        }

        /// <summary>
        /// Additional key fields for matching engine rules based on any question
        /// </summary>
        /// <param name="LeadRequest"></param>
        private void CreateMatchingEngineMappings(LeadCreateRequest LeadRequest)
        {
            foreach (var control in LeadRequest.LeadData)
            {
                string newkey = control.Key.ToLower() + "-key";
                if (dbTemplateService.StandardControlPreDefinedValueList.ContainsKey(control.Key)
                    && !LeadRequest.LeadAdditionalData.ContainsKey(newkey))
                {
                    var leadValue = control.Value.ToLower();
                    foreach (var item in dbTemplateService.StandardControlPreDefinedValueList[control.Key])
                    {
                        if (item.Value.ToLower() == leadValue)
                        {
                            LeadRequest.LeadAdditionalData.Add(newkey, item.Key);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates if programproductid is mapped to a template
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <returns></returns>
        public bool IsProgramProductMappedToTemplate(int ProgramProductId)
        {
            return dbTemplateService.IsProgramProductMappedToTemplate(ProgramProductId);
        }

        /// <summary>
        /// Verifies in Matching Engine if the current program product has an upsell to another product.
        /// </summary>
        /// <param name="leadRequest">The Lead information.</param>
        /// <param name="programProductId">The ID of the program product to validate.</param>
        /// <param name="isBeta">Is Beta Matching Engine service.</param>
        /// <returns></returns>
        private Tuple<bool, int> MEValidateProgramUpsell(LeadCreateRequest leadRequest, int programProductId, bool isBeta)
        {
            ProgramValidateRequest programValidationRequest = new ProgramValidateRequest();

            programValidationRequest.Application = MatchingEngine.ISApplication.VendorAPI;
            programValidationRequest.TrackGuid = leadRequest.TrackId ?? Guid.Empty;
            programValidationRequest.ProgramProductId = programProductId;
            programValidationRequest.BreakOnFirstValidationFailure = false;
            ProspectInputBuilder.AggregateCreativeURLs(leadRequest);
            programValidationRequest.ProspectInput = ProspectInputBuilder.BuildProspectInput(leadRequest.LeadData, leadRequest.LeadAdditionalData, null, leadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            programValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
            programValidationRequest.AgentId = leadRequest.ClientRelationContactId;

            ProgramValidateResponse programValidationResponse = RelatedServices.ValidateProgram(programValidationRequest, isBeta);
            
            if(programValidationResponse != null && 
                programValidationResponse.AlternateProgramProductId.HasValue &&
                programValidationResponse.AlternateProgramProductId != 0)
            {
                return new Tuple<bool, int>(true, programValidationResponse.AlternateProgramProductId.Value);
            }

            return new Tuple<bool, int>(false, 0);
        }
        

        private Tuple<bool, int?> InternalMEValidateProgram(LeadCreateRequest LeadRequest, int ProgramProductId, bool IsBeta, APISubmissionResultDTO submissionResultDTO, bool OnlyRejectOnInternalDuplicate)
        {
            ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
            ProgramValidationRequest.Application = MatchingEngine.ISApplication.VendorAPI;
            ProgramValidationRequest.TrackGuid = LeadRequest.TrackId.HasValue ? LeadRequest.TrackId.Value : Guid.Empty;
            ProgramValidationRequest.ProgramProductId = ProgramProductId;
            ProgramValidationRequest.BreakOnFirstValidationFailure = false;
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            ProgramValidationRequest.ProspectInput = Prospect;

            ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
            ProgramValidateResponse ProgramValidationResponse = RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);

            if (ProgramValidationResponse.AlternateProgramProductId != null)
            {
                LeadRequest.ProgramProductId = ProgramValidationResponse.AlternateProgramProductId.GetValueOrDefault();
            }
            if (ProgramValidationResponse.PaidStatusTypeId.HasValue)
            {
                LeadRequest.PaidStatusType = Convert.ToInt32(ProgramValidationResponse.PaidStatusTypeId.GetValueOrDefault());
            }
            if (ProgramValidationResponse.LeadPingScoreCPL.HasValue)
            {
                submissionResultDTO.LeadPingScoreCPL = ProgramValidationResponse.LeadPingScoreCPL.Value;
            }

            if (LeadRequest.PaidStatusType == (int)LeadPaidStatusType.Fraid)
            {
                submissionResultDTO.Valid = false;
                submissionResultDTO.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.FraidStatus));
            }
            if (!ProgramValidationResponse.PassedValidation)
            {
                if (!OnlyRejectOnInternalDuplicate)
                {
                    submissionResultDTO.Valid = false;
                }
                RuleFailure LastRuleFailure = null;
                foreach (var Failure in ProgramValidationResponse.RuleFailures)
                {
                    submissionResultDTO.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.BusinessRulesCheck, MassageMERuleFailure(Failure)));
                    if (Failure.RuleFailureType == BaseRuleType.ExternalDuplicate)
                    {
                        LeadRequest.IsExternalDuplicate = true;
                    }
                    else if (Failure.RuleFailureType == BaseRuleType.InternalDuplicate)
                    {
                        LeadRequest.IsInternalDuplicate = true;
                        if (OnlyRejectOnInternalDuplicate)
                        {
                            LastRuleFailure = Failure;
                            submissionResultDTO.Valid = false;
                            break;
                        }
                    }
                    else if(Failure.RuleFailureType == BaseRuleType.Spam)
                    {
                        LeadRequest.IsSpam = true;
                        submissionResultDTO.Valid = ConfigurationManager.AppSettings.AllKeys.Any(a => a == "IsSpamAllowedForDelivery") ? bool.Parse(ConfigurationManager.AppSettings["IsSpamAllowedForDelivery"].ToString()) : true;
                    }
                    else if (Failure.RuleFailureType == BaseRuleType.SpamReportingOnly)
                    {
                        LeadRequest.IsSpamReportingOnly = true;
                        submissionResultDTO.Valid = true;
                    }

                    if (!OnlyRejectOnInternalDuplicate)
                    {
                        LastRuleFailure = Failure;
                    }
                }
                if (LastRuleFailure != null)
                {
                    if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailedReason"))
                    {

                        string ruleFailureType = (LastRuleFailure.RuleFailureType != null) ? LastRuleFailure.RuleFailureType.ToString() : "ME unknown";
                        string ruleFailureName = (LastRuleFailure != null) ? LastRuleFailure.RuleFailureName : "ME unknown rule";
                        LeadRequest.LeadData.Add("InitialLeadValidationFailedReason", ruleFailureType + " - " + ruleFailureName);
                    }
                    if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailed"))
                    {
                        LeadRequest.LeadData.Add("InitialLeadValidationFailed", (LeadRequest.PassedValidation) ? "0" : "1");
                    }
                }
            }

            return new Tuple<bool, int?>(submissionResultDTO.Valid, ProgramValidationResponse.ScoreId);
        }

        
        /// <summary>
        /// Form Validation
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="PostedFields"></param>
        /// <returns></returns>
        private APIValidationResultDTO ProfanityCheckForm(int TemplateId,  Dictionary<string, string> PostedFields)
        {

            APIValidationResultDTO ValidationResult = new APIValidationResultDTO() { Valid = true, ValidationMessages = new List<KeyValuePair<string, string>>() };
            
            var TemplateControls = GetTemplateControls(TemplateId);

            //Validate individual Controls
            foreach (var Control in TemplateControls)
            {
                string ControlCode = Control.StandardControl.StandardControlCode.Code;
                string ControlValue = PostedFields.ContainsKey(ControlCode) ? PostedFields[Control.StandardControl.StandardControlCode.Code] : "";

                //Break on first failure
                if (ValidationResult.ValidationMessages.Count > 0)
                {
                    break;
                }

                HashSet<string> ignoredFields = new HashSet<string> { "School_Picker_Failures", "Additional_Questions", "School_Picker", "Program_Of_Interest", "CampusPreference", "UserAgreement", "EDDYUserAgreement" };

                //0. Ignored fields
                if (ignoredFields.Contains(ControlCode) || Control.StandardControl.StandardControlType.StandardControlTypeName.ToLower() == "label")
                {
                    continue;
                }

                //6.Profanity check
                if (!ValidationEngine.ProfanityCheck(ControlValue))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.ProfanityCheck, Control.StandardControl.StandardControlCode.Code));
                    continue;
                }

            }
            if (ValidationResult.ValidationMessages.Count > 0)
            {
                if (!PostedFields.ContainsKey("InitialLeadValidationFailedReason"))
                {
                    KeyValuePair<string, string> initialLeadValidationFailedReason = ValidationResult.ValidationMessages.FirstOrDefault();
                    string ruleFailureType = (initialLeadValidationFailedReason.Key != null) ? initialLeadValidationFailedReason.Key.ToString() : "FE unknown";
                    string ruleFailureName = (initialLeadValidationFailedReason.Value != null) ? initialLeadValidationFailedReason.Value.ToString() : "FE unknown rule";
                    PostedFields.Add("InitialLeadValidationFailedReason", ruleFailureType + " - " + ruleFailureName);
                }
            }
            return ValidationResult;

        }

        /// <summary>
        /// Form Validation
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="PostedFields"></param>
        /// <returns></returns>
        private APIValidationResultDTO InternalValidateForm(int TemplateId, bool IsBeta, Dictionary<string, string> PostedFields, bool IsWizard, bool BreakOnFirstFailure, bool IsQuickCheck = false, int ApplicationId = 0, int leadCreationType = 0)
        {
            //aded flag to skip certain validations for EMS since phone validation was re introduced.
            bool IsEMSCampaign = ApplicationId == (int)MatchingEngine.ISApplication.EMS;
            //added flag for advising application, used ultimately to skip phone number validation
            Boolean IsAdvising = false;
            if (ApplicationId == 13)
            {
                IsAdvising = true;
            }

            APIValidationResultDTO ValidationResult = new APIValidationResultDTO() { Valid = true, ValidationMessages = new List<KeyValuePair<string, string>>() };
            string CountryCode = PostedFields.ContainsKey("Country") ? PostedFields["Country"] : "US";
            string StateAbbreviation = PostedFields.ContainsKey("State") ? PostedFields["State"] : "";

            int channelId = 0;
            int subChannelId = 0;
            if (PostedFields.ContainsKey("ChannelId"))
            {
                int.TryParse(PostedFields["ChannelId"], out channelId);
            }
            if (PostedFields.ContainsKey("SubChannelId"))
            {
                int.TryParse(PostedFields["SubChannelId"], out subChannelId);
            }

            bool validateTCPA = false;
            if (PostedFields.ContainsKey("ValidateTCPA"))
            {
                if (!string.IsNullOrEmpty(PostedFields["ValidateTCPA"]))
                {

                    bool.TryParse(PostedFields["ValidateTCPA"], out validateTCPA);

                }
            }

            var TemplateControls = GetTemplateControls(TemplateId);

            TemplateControlDTO stateControl = TemplateControls.Where(t => t.StandardControl.StandardControlCode.Code == "State").SingleOrDefault();
            TemplateControlDTO countryControl = TemplateControls.Where(t => t.StandardControl.StandardControlCode.Code == "Country").SingleOrDefault();
            bool stateRequired = stateControl?.IsRequired ?? false;
            bool countryRequired = countryControl?.IsRequired ?? false;

            //Validate individual Controls
            foreach (var Control in TemplateControls)
            {
                string ControlCode = Control.StandardControl.StandardControlCode.Code;
                string ControlValue = PostedFields.ContainsKey(ControlCode) ? PostedFields[Control.StandardControl.StandardControlCode.Code] : "";
                int ControlValueLength = ControlValue.Length;

                //Break on first failure
                if (BreakOnFirstFailure && ValidationResult.ValidationMessages.Count > 0)
                {
                    break;
                }

                HashSet<string> ignoredFields = new HashSet<string> { "School_Picker_Failures", "Additional_Questions", "School_Picker", "Program_Of_Interest", "CampusPreference", "UserAgreement", "EDDYUserAgreement" };

                // TCPA Check
                if (ApplicationId != (int)EDDY.IS.Base.ISApplication.EMS && ApplicationId > 0)
                {
                    if (ControlCode == "UserAgreement" && leadCreationType == (int)LeadCreationType.HostAndPost && (validateTCPA))
                    {
                        if ((channelId == (int)EDDY.IS.Base.Channel.CallCenterPartners || subChannelId == (int)EDDY.IS.Base.SubChannel.CallCenterPartners) || ((channelId == (int)EDDY.IS.Base.Channel.OnlinePartners) || subChannelId == (int)EDDY.IS.Base.SubChannel.OnlinePartners))
                        {
                            if (!ValidationEngine.TCPACheck(ApplicationId, ControlValue, channelId, subChannelId, PostedFields, FormsEngine.GetResourceMetaDataForTCPA()))
                            {
                                ValidationResult.Valid = false;
                                ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.TCPACheck));
                                continue;
                            }
                        }

                    }
                }

                //0. Ignored fields
                if (ignoredFields.Contains(ControlCode) || Control.StandardControl.StandardControlType.StandardControlTypeName.ToLower() == "label")
                {
                    continue;
                }

                //1. Check required fields
                if (Control.IsRequired == true && string.IsNullOrWhiteSpace(ControlValue))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.RequiredField, Control.StandardControl.StandardControlCode.Code));
                    continue;
                }

                //1.1 Not required fields can be empty
                if (Control.IsRequired == false && string.IsNullOrEmpty(ControlValue))
                {
                    continue;
                }

                var excludedControls = new List<string> { "Categories", "SubCategories", "EMSLearningPreferenceAndLocations" };

                //2. Check Values from Predefined list validation
                if (!string.IsNullOrWhiteSpace(ControlValue) && Control.StandardControl.PreDefinedValueList != null && Control.StandardControl.PreDefinedValueList.ValueListItems != null
                    && !excludedControls.Contains(ControlCode)
                   )
                {
                    var ValueFound = false;
                    string ControlValueLower = ControlValue.ToLower();
                    if (Control.StandardControl.StandardControlType.StandardControlTypeName != "Multi Check Box List")
                    {
                        if (ControlCode == "State" && !(CountryCode.ToUpper() == "US" || CountryCode.ToUpper() == "CA"))
                        {
                            continue;
                        }
                        foreach (var PredefinedValue in Control.StandardControl.PreDefinedValueList.ValueListItems)
                        {
                            if (PredefinedValue.Value.ToLower() == ControlValueLower)
                            {
                                ValueFound = true;
                                PostedFields[Control.StandardControl.StandardControlCode.Code] = PredefinedValue.Value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Multi checkbox list contains a comma separated value list with the items
                        List<string> ItemsFound = new List<string>();
                        if (ControlCode == "Desired_Degree_Level")
                        {
                            continue;
                        }
                        foreach (var item in ControlValueLower.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            bool ItemFound = false;
                            string ItemValue = "";
                            foreach (var PredefinedValue in Control.StandardControl.PreDefinedValueList.ValueListItems)
                            {
                                if (PredefinedValue.Value.ToLower() == item)
                                {
                                    ItemFound = true;
                                    ItemValue = PredefinedValue.Value;
                                    break;
                                }
                            }
                            if (ItemFound)
                            {
                                ItemsFound.Add(ItemValue);
                                ValueFound = true;
                            }
                            else
                            {
                                ValueFound = false;
                                break;
                            }
                        }

                        if (ValueFound)
                        {
                            PostedFields[Control.StandardControl.StandardControlCode.Code] = string.Join(",", ItemsFound);
                        }

                    }

                    if (!ValueFound)
                    {
                        ValidationResult.Valid = false;
                        ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.SelectedValueCheck, Control.StandardControl.StandardControlCode.Code));
                        continue;
                    }
                }

                //3. Validation Library if control requires one (PhaseI:REGEX)
                if (!IsQuickCheck && Control.StandardControl.StandardControlValidations != null)
                {
                    foreach (var validation in Control.StandardControl.StandardControlValidations)
                    {
                        if (validation.ValidationLibrary != null)
                        {
                            if (!Regex.IsMatch(ControlValue, validation.ValidationLibrary.Expression))
                            {
                                ValidationResult.Valid = false;
                                ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.ValidationLibrary, Control.StandardControl.StandardControlCode.Code, validation.ValidationLibrary.Name));
                                break;
                            }
                        }
                    }
                }

                //4. Check value length (min, max if supported)
                if ((Control.StandardControl.MinLength != null && Control.StandardControl.MinLength > ControlValueLength)
                    ||
                    (Control.StandardControl.MaxLength != null && Control.StandardControl.MaxLength < ControlValueLength)
                  )
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.FieldLength, Control.StandardControl.StandardControlCode.Code, Control.StandardControl.MinLength, Control.StandardControl.MaxLength));
                    continue;
                }

                //5. Check dataType if provided
                if (!IsQuickCheck)
                {
                    switch (Control.StandardControl.StandardControlDataType.Name)
                    {
                        case "ANY": break;
                        case "ALPHA":
                            if (!ControlValue.IsAlpha())
                            {
                                ValidationResult.Valid = false;
                                ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.DataType, Control.StandardControl.StandardControlCode.Code, Control.StandardControl.StandardControlDataType.Name));
                            }
                            break;
                        case "NUMERIC":
                            if (!ControlValue.IsNumeric())
                            {
                                ValidationResult.Valid = false;
                                ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.DataType, Control.StandardControl.StandardControlCode.Code, Control.StandardControl.StandardControlDataType.Name));
                            }
                            break;
                        case "ALPHANUMERIC":
                            if (!ControlValue.IsAlphaNumeric())
                            {
                                ValidationResult.Valid = false;
                                ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.DataType, Control.StandardControl.StandardControlCode.Code, Control.StandardControl.StandardControlDataType.Name));
                            }
                            break;
                    }
                }

                //6.Profanity check
                if (!ValidationEngine.ProfanityCheck(ControlValue))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.ProfanityCheck, Control.StandardControl.StandardControlCode.Code));
                    continue;
                }

                //7. Phone Number check
                //2/10/2021 re added phone validation for quick checks.
                if (!IsAdvising  && !IsEMSCampaign && (ControlCode == "Phone" || ControlCode == "Alternate_Phone") && !string.IsNullOrWhiteSpace(ControlValue) && !ValidationEngine.PhoneCheck(ControlValue, CountryCode))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.PhoneNumberCheck, Control.StandardControl.StandardControlCode.Code));
                    continue;
                }

                //8.Email check
                int EmailValidationLevel = IsQuickCheck ? 1 : Convert.ToInt32(ConfigurationManager.AppSettings.Get("EmailValidationLevel"));
                if (ControlCode == "Email" && !ValidationEngine.EmailCheck(ControlValue, EmailValidationLevel))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.EmailCheck));
                    continue;
                }

                // 9. ZipCode State Country check

                //#68736 - Kaplan leads are rejected for invalid State/Zipcode
                //if (!IsQuickCheck && ControlCode == "Postal_Code" && !ValidationEngine.ZipCodeStateCountryCheck(ControlValue, CountryCode, StateAbbreviation))
                if (ControlCode == "Postal_Code" && !ValidationEngine.ZipCodeStateCountryCheck(ControlValue, CountryCode, StateAbbreviation, false, stateRequired, countryRequired))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.ZipCodeStateCountryCheck));
                    continue;
                }

                //10 Birth Date Check
                if (ControlCode == "BirthDate" && !string.IsNullOrWhiteSpace(ControlValue) && !ValidationEngine.BirthDateCheck(ControlValue))
                {
                    ValidationResult.Valid = false;
                    ValidationResult.ValidationMessages.Add(BuildValidationMessage(FormValidationResultType.BirthDate));
                    continue;
                }


            }
            if (ValidationResult.ValidationMessages.Count > 0)
            {
                if (!PostedFields.ContainsKey("InitialLeadValidationFailedReason"))
                {
                    KeyValuePair<string, string> initialLeadValidationFailedReason = ValidationResult.ValidationMessages.FirstOrDefault();
                    string ruleFailureType = (initialLeadValidationFailedReason.Key != null) ? initialLeadValidationFailedReason.Key.ToString() : "FE unknown";
                    string ruleFailureName = (initialLeadValidationFailedReason.Value != null) ? initialLeadValidationFailedReason.Value.ToString() : "FE unknown rule";
                    PostedFields.Add("InitialLeadValidationFailedReason", ruleFailureType + " - " + ruleFailureName);
                }
            }
            return ValidationResult;

        }

        private KeyValuePair<string, string> BuildValidationMessage(FormValidationResultType Error, params object[] args)
        {
            string ErrorCode = Enum.GetName(Error.GetType(), Error);
            string Message = "";
            var field = Error.GetType().GetField(ErrorCode);
            if (field != null)
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
                if (attr != null)
                {
                    Message = attr.Description;
                }
            }
            Message = string.Format(Message, args);
            return new KeyValuePair<string, string>(Error.ToString(), Message);
        }


        public List<KeyValuePair<int, List<TemplateControlDTO>>> GetControlTemplateListForProgramTemplates(HashSet<int> templateIds)
        {
            List<KeyValuePair<int, List<TemplateControlDTO>>> controlItems = new List<KeyValuePair<int, List<TemplateControlDTO>>>();

            foreach (var item in dbTemplateService.GetControlTemplateDictionaryForProgramTemplates(templateIds))
            {
                controlItems.Add(new KeyValuePair<int, List<TemplateControlDTO>>(item.Key, new List<TemplateControlDTO>()));

                foreach (var templateControl in item.Value)
                {
                    controlItems[controlItems.Count - 1].Value.Add(templateControl.ConvertToDTO());
                }
            }

            return controlItems;
        }

        public bool EmailCheck(string EmailAddress)
        {
            bool Result = true;
            try
            {
                Result = ValidationEngine.EmailCheck(EmailAddress, Convert.ToInt32(ConfigurationManager.AppSettings.Get("EmailValidationLevel")));
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public List<VW_ProspectResubmissionsDTO> GetProspectResubmissions(List<int> submissionIds)
        {
            return (List<VW_ProspectResubmissionsDTO>)dbSubmissionDataService.GetProspectResubmissions(submissionIds).ConvertToDTO();
        }
    }
}
