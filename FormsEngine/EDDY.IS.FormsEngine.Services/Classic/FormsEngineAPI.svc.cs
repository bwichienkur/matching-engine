using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Services.Classic.Base;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.DTO.Request;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Core;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine.Services.Classic
{
    public class FormsEngineAPI : FormsEngineAPIBase, IFormsEngine
    {

        public List<HTMLRenderingStrategyDTO> GetRenderingStrategies(bool Wizard)
        {
            List<HTMLRenderingStrategyDTO> Result = null;
            try
            {
                FormTemplateTypes FormTemplateType = Wizard ? FormTemplateTypes.ProgramWizard : FormTemplateTypes.ProgramTemplate;
                Result = FormsEngineService.GetRenderingStrategies(FormTemplateType);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public List<HTMLRenderingStrategyDTO> GetRenderingStrategiesByType(FormTemplateTypes FormTemplateType)
        {
            List<HTMLRenderingStrategyDTO> Result = null;
            try
            {
                Result = FormsEngineService.GetRenderingStrategies(FormTemplateType);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }
        
        public APIValidationResultDTO ValidateForm(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, APILead Lead)
        {
            APIValidationResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ValidateForm", null, ApplicationId, ProgramProductId, IsBeta, TrackId, Lead);
                string LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineAPI.ValidateForm");
                Result = FormsEngineService.ValidateForm(ProgramProductId, IsBeta, TrackId, LeadData, true, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        //Appollo
        public APIMultiValidationResultDTO ValidateMultipleForms(int ApplicationId, List<int> ProgramProducts, bool IsBeta, string TrackId, APILead Lead)
        {
            APIMultiValidationResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ValidateMultipleForms", null, ApplicationId, ProgramProducts, IsBeta, TrackId, Lead);
                string LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineAPI.ValidateMultipleForms");
                Result = FormsEngineService.ValidateMultipleForms(ProgramProducts
                    , IsBeta
                    , TrackId
                    , LeadData
                    , true
                    , ref Log
                    , ApplicationId
                    , MatchingEngine.ISApplication.Apollo);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public ProspectInput BuildProspect(int ApplicationId, bool IsBeta, string TrackId, APILead Lead)
        {
            ProspectInput Result = null;
            try
            {
                
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.BuildProspect", null, null, IsBeta, TrackId, Lead);
                var LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineAPI.BuildProspect");
                Result = FormsEngineService.BuildProspect(IsBeta, TrackId, LeadData);
                Log.EndLogDetail();
                Log.EndLog(Result);
                
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }
        public APIValidationResultDTO ValidateFormPost(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, string LeadData)
        {
            APIValidationResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ValidateFormPost", null, ProgramProductId, IsBeta, TrackId, LeadData);
                LeadData = HttpUtility.UrlDecode(LeadData);
                Log.StartLogDetail("FormsEngineAPI.ValidateFormPost");
                Result = FormsEngineService.ValidateForm(ProgramProductId, IsBeta, TrackId, LeadData, true, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public bool ValidateEmail(string EmailAddress)
        {
            bool Result = true;
            try
            {
                Result = FormsEngineService.EmailCheck(EmailAddress);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public APIMultiSubmissionResultDTO ProcessApolloSubmission(int ApplicationId, List<KeyValuePair<int, string>> ProgramProducts, int ProspectId, string TrackId, string MatchResponseGuid, APILead Lead, int? ClientRelationContactId, bool RealtimeDelivery, int? prospectFlowId, List<int?> PaidStatusTypeIds, bool isBeta)
        {
            APIMultiSubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ProcessApolloSubmission", null, ApplicationId, ProgramProducts, ProspectId, TrackId, Lead);
                string LeadData = MapAPILeadToFormStandardControls(Lead);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("FormsEngineAPI.ProcessApolloSubmission");
                Result = FormsEngineService.ProcessApolloSubmission(ApplicationId, ProgramProducts, ProspectId, ClientRelationContactId, TrackId, MatchResponseGuid, LeadData, ref RawData, ref Log, RealtimeDelivery, prospectFlowId, PaidStatusTypeIds, isBeta);
                Log.EndLogDetail();
                Log.EndLog(Result);

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public void ReleaseTitaniumLead(int programProductId, APILead Lead, int ProspectId, int? ClientRelationContactId, int? prospectFlowId, int leadId)
        {
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ProcessApolloSubmission", EDDY.IS.Base.ISApplication.Apollo, programProductId, ProspectId, ClientRelationContactId, prospectFlowId);
                string LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineAPI.ProcessApolloSubmission");
                FormsEngineService.ReleaseTitaniumLead(ref Log, programProductId, LeadData, ProspectId, ClientRelationContactId, prospectFlowId, leadId);
                Log.EndLogDetail();
                Log.EndLog(null);

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
        }

        public List<APITemplateControlResultDTO> RetrieveTemplateControlsByProgramTemplate(Guid? trackId, int? application = null)
        {
            List<APITemplateControlResultDTO> templateControlList = new List<APITemplateControlResultDTO>();
            try
            {
                if (HttpRuntime.Cache["RetrieveTemplateControlsByProgramTemplate"] == null)
                {
                    HashSet<int> templateIds = null;
                    
                    PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.RetrieveTemplateControlsByProgramTemplate", null, null);
                    if (trackId.HasValue)
                    {
                        DirectoryMatchRequest request = new DirectoryMatchRequest();
                        request.RemoveInvalidEntities = true;
                        request.TrackGuid = trackId.Value;
                        request.SortMethod = EntitySortMethod.Alphabetical;
                        if(application != null && application == (int)EDDY.IS.FormsEngine.ISApplication.Apollo)
                            request.LeadCreationType = MatchingEngine.LeadCreationType.Advising;
                        else
                            request.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                        TemplateMatchResponse tmr = FormsEngineService.RelatedServices.GetTemplatesForMatches(request, false);
                        if (tmr.TemplateIdList != null && tmr.TemplateIdList.Length > 0)
                            templateIds = new HashSet<int>(tmr.TemplateIdList);
                    }
                    Log.StartLogDetail("FormsEngineAPI.GetControlTemplateListForProgramTemplates");
                    List<KeyValuePair<int, List<TemplateControlDTO>>> tempControls = FormsEngineService.GetControlTemplateListForProgramTemplates(templateIds);
                    Log.EndLogDetail();

                    foreach (var item in tempControls)
                    {
                        APITemplateControlResultDTO templateControlResult = new APITemplateControlResultDTO();
                        templateControlResult.TemplateControls = item.Value;
                        templateControlResult.TemplateId = item.Key;

                        templateControlList.Add(templateControlResult);
                    }

                    HttpRuntime.Cache.Add("RetrieveTemplateControlsByProgramTemplate", templateControlList, null, DateTime.Now.AddHours(2), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    templateControlList = (List<APITemplateControlResultDTO>)HttpRuntime.Cache["RetrieveTemplateControlsByProgramTemplate"];
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return templateControlList;
        }

        public APISubmissionResultDTO ProcessHostAndPostLead(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, APILead Lead)
        {
            APISubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ProcessHostAndPostLead", null, ApplicationId, ProgramProductId, IsBeta, TrackId, Lead);
                string LeadData = MapAPILeadToFormStandardControls(Lead);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("FormsEngineAPI.ProcessHostAndPostLead");
                Result = FormsEngineService.ProcessHostAndPostLead(ProgramProductId
                    , IsBeta
                    , TrackId
                    , LeadData
                    , ref RawData
                    , ref Log
                    , MatchingEngine.ISApplication.VendorAPI
                    , Lead.ClientRelationContactId);
                Log.EndLogDetail();
                Log.EndLog(Result);

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }


        public APISubmissionResultDTO ProcessHostAndPostLeadPost(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, string LeadData)
        {
            APISubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.ProcessHostAndPostLeadPost", null, ApplicationId, ProgramProductId, IsBeta, TrackId, LeadData);
                LeadData = HttpUtility.UrlDecode(LeadData);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("FormsEngineAPI.ProcessHostAndPostLeadPost");
                Dictionary<string,string> leadDictionary = LeadData.BuildCaseInsensitiveDictionary();
                int leadId;
                int clientRelationContactId = 0;

                if (leadDictionary.ContainsKey("LeadId") && Int32.TryParse(leadDictionary["LeadId"], out leadId))
                {
                    if (leadDictionary.ContainsKey("ClientRelationContactId"))
                    {
                        Int32.TryParse(leadDictionary["ClientRelationContactId"], out clientRelationContactId);
                    }
                    FormsEngineService.ReleaseTitaniumLead(ref Log, ProgramProductId, LeadData, null, clientRelationContactId, null, leadId);
                    Result = new APISubmissionResultDTO() { Valid = true, LeadId = leadId, UID = Guid.NewGuid().ToString() };
                }
                else
                {
                    Result = FormsEngineService.ProcessHostAndPostLead(ProgramProductId
                        , IsBeta
                        , TrackId
                        , LeadData
                        , ref RawData
                        , ref Log
                        , MatchingEngine.ISApplication.VendorAPI);
                }
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public TemplateDTO GetProgramTemplateModel(int ProgramProductId, bool IsBeta)
        {
            TemplateDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetProgramTemplateModel", null, ProgramProductId, IsBeta);
                Log.StartLogDetail("FormsEngineAPI.GetProgramTemplateModel");
                //Validates if ProgramProductId is not assigned to a template
                if (!FormsEngineService.IsProgramProductMappedToTemplate(ProgramProductId))
                {
                    //Validates if ProgramProductId exist in database
                    ProgramValidateRequest Request = new ProgramValidateRequest()
                    {
                        Application = MatchingEngine.ISApplication.VendorAPI,
                        BreakOnFirstValidationFailure = true,
                        ProgramProductId = ProgramProductId,
                        ProspectInput = null
                    };
                    Request.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                    if (FormsEngineService.RelatedServices.ValidateProgram(Request, IsBeta).PassedValidation)
                    {
                        //Will use the default template
                        Result = FormsEngineService.GetProgramTemplateModel(ProgramProductId);
                    }
                    else
                    {
                        throw new Exception(string.Format("FormsEngine API (GetProgramTemplateModel): ProgramProductId={0}, IsBeta={1} was not found.", ProgramProductId, IsBeta));
                    } 
                }
                else
                {
                    //will use the assigned template
                    Result = FormsEngineService.GetProgramTemplateModel(ProgramProductId);
                }
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }


        public TemplateDTO GetProgramTemplateModelByTemplate(int TemplateId, bool IsBeta)
        {
            TemplateDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetProgramTemplateModelByTemplate", null, TemplateId, IsBeta);
                Log.StartLogDetail("FormsEngineAPI.GetProgramTemplateModelByTemplate");
                Result = FormsEngineService.GetProgramTemplateModelByTemplate(TemplateId);
                //Forms Engine is designed to return the default form when no form is found, Vendor API needs a null object.
                if (Result.TemplateId != TemplateId)
                {
                    Result = null;
                    throw new Exception(string.Format("FormsEngine API (GetProgramTemplateModelByTemplate): Requested TemplateId={0}, IsBeta={1} was not found.", TemplateId, IsBeta));
                }
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }


        public APIProgramMatchesDTO GetProgramMatches(int ApplicationId, bool IsBeta, string TrackId, APILead Lead)
        {
            APIProgramMatchesDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetProgramMatches", null, ApplicationId, IsBeta, TrackId, Lead);
                var LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineService.GetProgramMatches");
                Result = FormsEngineService.GetProgramMatches(IsBeta, TrackId, LeadData, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public APIProgramMatchesDTO GetEnhancedProgramMatches(int ApplicationId, bool IsBeta, string TrackId, APILead Lead)
        {
            APIProgramMatchesDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetEnhancedProgramMatches", null, ApplicationId, IsBeta, TrackId, Lead);
                var LeadData = MapAPILeadToFormStandardControls(Lead);
                Log.StartLogDetail("FormsEngineService.GetEnhancedProgramMatches");
                Result = FormsEngineService.GetEnhancedProgramMatches(IsBeta, TrackId, LeadData, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }


        public APIProgramMatchesDTO GetProgramMatchesPost(int ApplicationId, bool IsBeta, string TrackId, string LeadData)
        {
            APIProgramMatchesDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetProgramMatchesPost", null, ApplicationId, IsBeta, TrackId, LeadData);
                LeadData = HttpUtility.UrlDecode(LeadData);
                Log.StartLogDetail("FormsEngineService.GetProgramMatches");
                Result = FormsEngineService.GetProgramMatches(IsBeta, TrackId, LeadData, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        public TemplateDTO GetProgramAllTemplatesMergedModel(int programId, Guid trackId, bool IsBeta)
        {
            TemplateDTO Result = null;
            try
            {

                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "FormsEngineAPI.GetProgramAllTemplatesMergedModel", null, programId, IsBeta);
                Log.StartLogDetail("FormsEngineAPI.GetProgramAllTemplatesMergedModel");

                //Validates if ProgramId exist in database
                ProgramValidateRequest Request = new ProgramValidateRequest()
                {
                    Application = MatchingEngine.ISApplication.VendorAPI,
                    BreakOnFirstValidationFailure = true,
                    ProgramProductId = 0,
                    ProspectInput = null,
                    ProgramId = programId,
                    TrackGuid = trackId
                };
                Request.LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost;
                if (FormsEngineService.RelatedServices.ValidateAPIProgram(Request, IsBeta).PassedValidation)
                {

                    int[] programIdList = { programId };

                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest()
                    {
                        ProgramIdList = programIdList,
                        TrackGuid = trackId,
                        SortMethod = EntitySortMethod.Alphabetical
                        //LeadCreationType = MatchingEngine.LeadCreationType.HostAndPost
                    };
                    TemplateMatchResponse templateMatchResponse = FormsEngineService.RelatedServices.GetTemplatesForMatches(directoryMatchRequest, IsBeta);
                    if (templateMatchResponse.TemplateIdList != null)
                    {
                        if (templateMatchResponse.TemplateIdList.Length > 0)
                        {
                            Result = FormsEngineService.GetProgramAllTemplatesMergedModel(templateMatchResponse.TemplateIdList);
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("FormsEngine API (GetProgramAllTemplatesMergedModel): ProgramId={0}, IsBeta={1} Templates was not found.", programId, IsBeta));
                    }
                }
                else
                {
                    throw new Exception(string.Format("FormsEngine API (GetProgramAllTemplatesMergedModel): ProgramId={0}, IsBeta={1} was not found.", programId, IsBeta));
                }

                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

    }
}
