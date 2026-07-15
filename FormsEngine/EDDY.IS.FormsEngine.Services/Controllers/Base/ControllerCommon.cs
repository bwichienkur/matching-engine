using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Util.Network;
using EDDY.IS.Util.StringExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;


namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class ControllerCommon : Controller
    {
        public static FormsEngine FormsEngineService = new FormsEngine();
        public const int APPLICATION_GRADSCHOOL = 7;
        public const string GRADSCHOOL_TEMPLATEKEY = "GRADSCHOOL.TEMPLATEID";
        public const string GRADSCHOOL_FREE_TEMPLATEKEY = "GRADSCHOOL.FREE.TEMPLATEID";
        public const string GRADSCHOOL_FRAID_TEMPLATEKEY = "GRADSCHOOL.FRAID.TEMPLATEID";
        public const int DEFAULT_IS_GRAD_TEMPLATE = 2;

        public enum GradSchoolProduct
        {
            DEFAULT = 0,
            PAID = 17,
            FREE = 19,
            FRAID = 999,
        }

        public RawPostDataDTO BuildRawDataObject(string LeadData)
        {
            RawPostDataDTO RawData = new RawPostDataDTO();
            RawData.RemoteIp = IPHelper.GetClientIPAddress(HttpContext);
            RawData.BrowserInfo = HttpContext.Request.ServerVariables["HTTP_USER_AGENT"];
            RawData.Referer = HttpContext.Request.ServerVariables["HTTP_REFERER"];
            RawData.PostData = LeadData;
            return RawData;
        }

        public DirectoryMatchRequest BuildDirectoryMatchRequest(Dictionary<string, string> arguments, bool? removeInvalidEntities, EntitySortMethod? sortMethod, int? ApplicationId, Guid? TrackingSessionGUID)
        {
            string Value = StringExtensions.GetFieldValue("IsBeta", arguments, true);
            bool IsBeta = false;
            bool.TryParse(Value, out IsBeta);
            string TrackId = StringExtensions.GetFieldValue("TrackId", arguments, true);
            List<string> FormFilters = new List<string>();

            foreach (var item in arguments)
            {
                FormFilters.Add(item.Key + "=" + item.Value);
            }

            return BuildDirectoryMatchRequest(IsBeta, TrackId, string.Join("&", FormFilters), removeInvalidEntities, sortMethod, null, ApplicationId, TrackingSessionGUID);
        }


        public DirectoryMatchRequest BuildDirectoryMatchRequestForProgramTemplatePrograms(bool IsBeta, string TrackId, string FormFilterValues, int? InstitutionId, int? ProgramId, int? ApplicationId, Guid? TrackingSessionGUID)
        {
            DirectoryMatchRequest Result = BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, true, null, InstitutionId, ApplicationId, TrackingSessionGUID);
            if (ProgramId.HasValue)
            {
                Result.ProgramIdList = new int[] { ProgramId.Value };
            }

            return Result;
        }

        public DirectoryMatchRequest BuildDirectoryMatchRequestForCounter(bool IsBeta, string TrackId, string FormFilterValues, bool? removeInvalidEntities, EntitySortMethod? sortMethod, int maxNestedProgramCount, int maxResultsCount, int? ApplicationId, Guid? TrackingSessionGUID)
        {
            DirectoryMatchRequest Result = BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, removeInvalidEntities, sortMethod, null, ApplicationId, TrackingSessionGUID);
            Result.MaxNestedProgramCount = maxNestedProgramCount;
            Result.MaxResultsCount = maxResultsCount;
            return Result;
        }

        public DirectoryMatchRequest BuildDirectoryMatchRequest(bool IsBeta, string TrackId, string FormFilterValues, bool? removeInvalidEntities, EntitySortMethod? sortMethod, int? InstitutionId, int? ApplicationId, Guid? TrackingSessionGUID)
        {
            DirectoryMatchRequest request = new DirectoryMatchRequest();
            if (!string.IsNullOrEmpty(FormFilterValues))
            {
                FormFilterValues = Server.UrlDecode(FormFilterValues);
            }

            Dictionary<string, string> dictionaryAdditional = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dictionary = FormFilterValues.BuildCaseInsensitiveDictionary();

        
            var keys = (from key in dictionary.Keys
                        where key.Contains("-key")
                        select key).ToList();

            foreach (var key in keys)
            {
                dictionaryAdditional.Add(key, dictionary[key]);
            }

            //Result Control Parameters
            request.Application = MatchingEngine.ISApplication.FormsEngine;

            if (sortMethod.HasValue)
            {
                request.SortMethod = sortMethod.Value;
            }
            else
            {
                request.SortMethod = EntitySortMethod.RankScore;
            }

            request.RemoveInvalidEntities = removeInvalidEntities;

            //Filter Parameters
            Guid TrackGuid;
            if (Guid.TryParse(TrackId, out TrackGuid))
            {
                request.TrackGuid = TrackGuid;
            }

            request.CampusId = StringExtensions.GetFieldValue("Campus", dictionary, null);

            SetCampusPreferenceFilters(request, dictionary, ApplicationId);

            if (InstitutionId.HasValue)
                request.InstitutionIdList = new int[] { InstitutionId.Value };
            
            request.FeatureId = StringExtensions.GetFieldValue("FeatureId", dictionary, null, true);

            int? programId = StringExtensions.GetFieldValue("Program_Of_Interest", dictionary, null, true);

            if (programId.HasValue)
                request.ProgramIdList = new int[] { programId.Value };
            
            string categories = StringExtensions.GetFieldValue("Categories", dictionary, true);
            string subCategories = StringExtensions.GetFieldValue("SubCategories", dictionary, true);
            string specialties = StringExtensions.GetFieldValue("Specialties", dictionary, true);

            if (!String.IsNullOrWhiteSpace(categories))
            {
                request.CategoryList = categories.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (!String.IsNullOrWhiteSpace(subCategories))
            {
                request.SubjectList = subCategories.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            if (!String.IsNullOrWhiteSpace(specialties))
            {
                request.SpecialtyList = specialties.Split(',').Select(f => Convert.ToInt32(f)).ToArray();
            }

            //Prospect Parameters
            request.ProspectInput = ProspectInputBuilder.BuildProspectInput(dictionary, dictionaryAdditional, null, TrackingSessionGUID.GetValueOrDefault().ToString());

            if (ApplicationId.HasValue)
            {
                request.ApplicationId = ApplicationId.Value;
            }

            int? desiredProgramLevel = StringExtensions.GetFieldValue("EMSDesiredDegreeLevel", dictionary, null, true);

            if (desiredProgramLevel.HasValue)
            {
                request.ProgramLevelList = new int[] { desiredProgramLevel.Value };
            }

            return request;
        }

        private void SetCampusPreferenceFilters(DirectoryMatchRequest request, Dictionary<string, string> filterValues, int? ApplicationId)
        {
            string emsCampusPreference = StringExtensions.GetFieldValue("EMSCampusPreference", filterValues, true);
            string emsCampusPreferenceAndLocation = StringExtensions.GetFieldValue("EMSLearningPreferenceAndLocations", filterValues, true);
            string campusPreference = StringExtensions.GetFieldValue("CampusPreference", filterValues, true);
            string CampusPreferenceToBeUsed = null;


            if (!string.IsNullOrWhiteSpace(emsCampusPreference))
                CampusPreferenceToBeUsed = emsCampusPreference;
            else if (!string.IsNullOrWhiteSpace(emsCampusPreferenceAndLocation))
                CampusPreferenceToBeUsed = emsCampusPreferenceAndLocation;
            else if (!string.IsNullOrWhiteSpace(campusPreference))
                CampusPreferenceToBeUsed = campusPreference;

            if (!string.IsNullOrWhiteSpace(CampusPreferenceToBeUsed))
            {
                if (CampusPreferenceToBeUsed.ToLower() == "online")
                {
                    request.CampusType = CampusType.Online;

                    if (ApplicationId == 27)
                    {
                        //its ems so we want hybrid to be false not null unless the choose hybrid which is set below
                        request.IsHybrid = false;
                    }
                }
                else if (CampusPreferenceToBeUsed.ToLower() == "hybrid")
                {
                    request.CampusType = null;
                    request.IsHybrid = true;
                }
                else
                {
                    CampusWithInstitutionModel selectedCampus = GetCampusInstitutionModelFromJsonString(CampusPreferenceToBeUsed);

                    request.CampusId = selectedCampus?.CampusId;

                    if (CampusPreferenceToBeUsed.ToLower() == "campus" || selectedCampus?.CampusType == CampusType.Ground)
                    {
                        request.CampusType = CampusType.Ground;
                    }
                }
            }
        }

        private CampusWithInstitutionModel GetCampusInstitutionModelFromJsonString(string json)
        {
            CampusWithInstitutionModel result;

            try
            {
                result = JsonConvert.DeserializeObject<CampusWithInstitutionModel>(json);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        
        /// <summary>
        /// Gets the shared session object
        /// </summary>
        /// <param name="FESessionId"></param>
        /// <returns></returns>
        public FormsEngineWorkflowStatus GetWorkflowSession(string FESessionId)
        {
            return FESession.Get<FormsEngineWorkflowStatus>(FESessionId, Constants.WORKFLOW_SESSIONKEY);
        }
                
        public void AssignApplicationOverrideTemplatesToPrograms(FormProgramResponse ProgramResponse, string AlternativeTemplates, int applicationId)
        {
            var OverridesForApplication = FormsEngineService.GetTemplateApplicationOverrideForApplication(applicationId);
            if (ProgramResponse != null && ProgramResponse.FormProgramList != null && OverridesForApplication.Count > 0)
            {
                for (int i = 0; i < ProgramResponse.FormProgramList.Count(); i++)
                {
                    int? thePaidStatusId = null;
                    if (ProgramResponse.FormProgramList[i].PaidStatusTypeId != null)
                    {
                        thePaidStatusId = Convert.ToInt32(ProgramResponse.FormProgramList[i].PaidStatusTypeId.GetValueOrDefault());
                    }
                    var theTemplate = OverridesForApplication.Where(t => t.PaidStatusTypeId == thePaidStatusId).FirstOrDefault();
                    if (theTemplate != null)
                    {
                        if ((ProgramResponse.FormProgramList[i].PaidStatusTypeId == PaidStatusType.Free || ProgramResponse.FormProgramList[i].PaidStatusTypeId == PaidStatusType.Fraid) && theTemplate != null)
                        {
                            ProgramResponse.FormProgramList[i].TemplateId = theTemplate.TemplateId;
                        }
                        else if (ProgramResponse.FormProgramList[i].ProductId == (int)GradSchoolProduct.PAID
                            && ProgramResponse.FormProgramList[i].RequiresSystemTemplateUse == false)
                        {
                            ProgramResponse.FormProgramList[i].TemplateId = theTemplate.TemplateId;
                        } //TODO: Add condition for PaidStatusTypePaid which currently wont be handled when the application is NOT grad schools but status type is paid.
                        //this scenario currently does not exist but needs to work.
                    }
                    ProgramResponse.FormProgramList[i].TemplateId = FormsEngineService.FindAlternativeTemplateId(ProgramResponse.FormProgramList[i].TemplateId.GetValueOrDefault(), AlternativeTemplates);
                    
                }
            }
        }

        public void AssignApplicationOverrideTemplatesToPrograms(CrossSellProgramResponse ProgramResponse, string AlternativeTemplates, int applicationId)
        {
            var OverridesForApplication = FormsEngineService.GetTemplateApplicationOverrideForApplication(applicationId);
            if (ProgramResponse != null && ProgramResponse.ProgramList != null && OverridesForApplication.Count > 0)
            {
                for (int i = 0; i < ProgramResponse.ProgramList.Count(); i++)
                {
                    int? thePaidStatusId = null;
                    if (ProgramResponse.ProgramList[i].PaidStatusTypeId != null)
                    {
                        thePaidStatusId = Convert.ToInt32(ProgramResponse.ProgramList[i].PaidStatusTypeId.GetValueOrDefault());
                    }
                    var theTemplate = OverridesForApplication.Where(t => t.PaidStatusTypeId == thePaidStatusId).FirstOrDefault();

                    if (theTemplate != null)
                    {
                        if ((ProgramResponse.ProgramList[i].PaidStatusTypeId == PaidStatusType.Free || ProgramResponse.ProgramList[i].PaidStatusTypeId == PaidStatusType.Fraid) && theTemplate != null)
                        {
                            ProgramResponse.ProgramList[i].TemplateId = theTemplate.TemplateId;
                        }
                        else if (ProgramResponse.ProgramList[i].ProductId == (int)GradSchoolProduct.PAID
                            && ProgramResponse.ProgramList[i].RequiresSystemTemplateUse == false)
                        {
                            ProgramResponse.ProgramList[i].TemplateId = theTemplate.TemplateId;
                        }//TODO: Add condition for PaidStatusTypePaid which currently wont be handled when the application is NOT grad schools but status type is paid.
                        //this scenario currently does not exist but needs to work.
                    }
                    ProgramResponse.ProgramList[i].TemplateId = FormsEngineService.FindAlternativeTemplateId(ProgramResponse.ProgramList[i].TemplateId.GetValueOrDefault(), AlternativeTemplates);
                }
            }
        }
        
        /// <summary>
        /// Gets the list of programs and replaces TemplateId to each program if application is GS and is a GS product
        /// </summary>
        /// <param name="InstitutionId"></param>
        /// <param name="ProgramId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="FormFilterValues"></param>
        /// <param name="ApplicationId"></param>
        /// <param name="DeviceId"></param>
        /// <returns></returns>
        public FormProgramResponse GetProgramTemplatePrograms(int InstitutionId, int? ProgramId, bool IsBeta, string TrackId, string FormFilterValues, int? ApplicationId, string DeviceId, string AlternativeTemplates, bool IsProgramWizard)
        {
            DirectoryMatchRequest matchRequest = BuildDirectoryMatchRequestForProgramTemplatePrograms(IsBeta, TrackId, FormFilterValues, InstitutionId, ProgramId, ApplicationId, null);


            if (IsProgramWizard)
            {
                matchRequest.LeadCreationType = MatchingEngine.LeadCreationType.ProgramWizardInitial;
            }
            else
            {
                matchRequest.LeadCreationType = MatchingEngine.LeadCreationType.InstitutionFormInitial;
            }

            Guid trackingDeviceGuid;
            if (Guid.TryParse(DeviceId, out trackingDeviceGuid))
            {
                matchRequest.TrackingDeviceGuid = trackingDeviceGuid;
            }

            if (ApplicationId == APPLICATION_GRADSCHOOL)
            {
                matchRequest.ProgramLevelList = new int[] {
                    7, //Doctorate
                    8, //Master
                    11 //Graduate cert
                }; 
            }

            FormProgramResponse Result = FormsEngineService.RelatedServices.GetFormPrograms(matchRequest, IsBeta);
            //if we are in an application that has application overrides then assign those templates to programs
            var Templates = FormsEngineService.GetTemplateApplicationOverrideForApplication(ApplicationId.GetValueOrDefault());
            if (Templates.Count > 0)
            {
                AssignApplicationOverrideTemplatesToPrograms(Result, AlternativeTemplates, ApplicationId.GetValueOrDefault());
            }

            return Result;
        }


        /// <summary>
        /// Gets programs DDL results
        /// </summary>
        /// <param name="InstitutionId"></param>
        /// <param name="CampusId"></param>
        /// <param name="ProgramId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="FormFilterValues"></param>
        /// <param name="ApplicationId"></param>
        /// <param name="DeviceId"></param>
        /// <param name="AlternativeTemplates"></param>
        /// <param name="IsProgramWizard"></param>
        /// <param name="optOption"></param>
        /// <returns></returns>
        public string GetProgramTemplateProgramsDDL(int InstitutionId, int? ProgramId, bool IsBeta, string TrackId, string FormFilterValues, int? ApplicationId, string DeviceId, string AlternativeTemplates, bool IsProgramWizard, ProgramOptGroup optOption)
        {
            string Result = string.Empty;
            string ProgramTemplateProgramDDLRazorView = string.Empty;
            FormProgramResponse ProgramResponse = GetProgramTemplatePrograms(InstitutionId, ProgramId, IsBeta, TrackId, FormFilterValues, ApplicationId, DeviceId, AlternativeTemplates, IsProgramWizard);
            ProgramDDL ProgramModel = new ProgramDDL();

            ProgramModel.CustomTCPA = ProgramResponse.CustomTCPA;
            ProgramModel.ProgramOptGroupList = new List<Models.ProgramOptGroup>();
            ProgramModel.AnyCampusResult = (from a in ProgramResponse.FormProgramList
                                            where a.CampusType == CampusType.Ground
                                            select a.ProgramId).Count() > 0;

            ProgramModel.AnyOnlineResult = (from a in ProgramResponse.FormProgramList
                                            where a.CampusType == CampusType.Online
                                            select a.ProgramId).Count() > 0;

            ProgramModel.AnyCustomOptGroup = (from a in ProgramResponse.FormProgramList
                                            where !string.IsNullOrWhiteSpace(a.CampusOptionGroup)
                                            select a.ProgramId).Count() > 0;


            if(optOption== ProgramOptGroup.Default||(optOption==ProgramOptGroup.Custom && !ProgramModel.AnyCustomOptGroup))
            {
                ProgramTemplateProgramDDLRazorView = "~/Templates/Common/ProgramTemplateProgramDDL.cshtml";
                ProgramModel.ProgramOptGroupList = BuildDefaultProgramLevelsModel(ProgramResponse);
            }
            else if(optOption == ProgramOptGroup.None)
            {
                ProgramTemplateProgramDDLRazorView = "~/Templates/Common/ProgramTemplateProgramDDLWithoutGroups.cshtml";
                ProgramModel.ProgramOptGroupList = BuildDefaultProgramLevelsModel(ProgramResponse);
            }
            else
            {
                ProgramTemplateProgramDDLRazorView = "~/Templates/Common/ProgramTemplateProgramDDL.cshtml";
                ProgramModel.ProgramOptGroupList = BuildCustomOptGroupsProgramLevelsModel(ProgramResponse);
            }

            
            return this.RazorViewToString(ProgramTemplateProgramDDLRazorView, ProgramModel, true, false);
        }

        private List<Models.ProgramOptGroup> BuildCustomOptGroupsProgramLevelsModel(FormProgramResponse request)
        {
            List<Models.ProgramOptGroup> programOptGroupList = new List<Models.ProgramOptGroup>();

            var programOptGroupLevels = (from a in request.FormProgramList
                                         select a).Distinct(ProgramOptGroupComparer.Instance).ToList().OrderBy(o=>o.CampusOptionGroupPosition??-1);


            foreach (var level in programOptGroupLevels)
            {
                var programOptGroup = new Models.ProgramOptGroup();
                programOptGroup.Id = level.CampusOptionGroupPosition??-1;
                programOptGroup.Name = level.CampusOptionGroup;
                programOptGroup.ProgramList = new List<Models.Program>();

                var programs = (from p in request.FormProgramList
                                where p.CampusOptionGroup == level.CampusOptionGroup
                                && p.TemplateId.HasValue
                                select new Models.Program()
                                {
                                    Id = p.ProgramId,
                                    Name = p.ProgramName,
                                    ProductId = p.ProductId,
                                    ProgramProductId = p.ProgramProductId,
                                    TemplateId = (int)p.TemplateId,
                                    PaidStatusTypeId = p.PaidStatusTypeId == null ? 0 : (int)p.PaidStatusTypeId,
                                    Is2USchool = p.Is2USchool,
                                    ShowTwoULeadShareControl = p.ShowTwoULeadShareControl,
                                    ProgramType = p.CampusType.ToString(),
                                    ProgramRankScore = p.ProgramRankScore
                                }).ToList();

                programOptGroup.ProgramList.AddRange(programs);
                programOptGroupList.Add(programOptGroup);
            }

            return programOptGroupList;
        }

        private List<Models.ProgramOptGroup> BuildDefaultProgramLevelsModel(FormProgramResponse request)
        {
            List<Models.ProgramOptGroup> programOptGroupList = new List<Models.ProgramOptGroup>();

            var programLevels = (from a in request.FormProgramList
                                 select a).Distinct(ProgramListLevelComparer.Instance).OrderBy(a => a.ProgramLevelName).ToList();


            foreach (var level in programLevels)
            {
                var programOptGroup = new Models.ProgramOptGroup();
                programOptGroup.Id = level.ProgramLevelId;
                programOptGroup.Name = FixProgramLevelName(level.ProgramLevelName);
                programOptGroup.ProgramList = new List<Models.Program>();

                var programs = (from p in request.FormProgramList
                                where p.ProgramLevelId == level.ProgramLevelId
                                && p.TemplateId.HasValue
                                select new Models.Program()
                                {
                                    Id = p.ProgramId,
                                    Name = p.ProgramName,
                                    ProductId = p.ProductId,
                                    ProgramProductId = p.ProgramProductId,
                                    TemplateId = (int)p.TemplateId,
                                    PaidStatusTypeId = p.PaidStatusTypeId == null ? 0 : (int)p.PaidStatusTypeId,
                                    Is2USchool = p.Is2USchool,
                                    ShowTwoULeadShareControl = p.ShowTwoULeadShareControl,
                                    ProgramType = p.CampusType.ToString(),
                                    ProgramRankScore = p.ProgramRankScore
                                }).ToList();

                programOptGroup.ProgramList.AddRange(programs);
                programOptGroupList.Add(programOptGroup);
            }

            return programOptGroupList;
        }


        /// <summary>
        /// Display program Opt groups names based on Business requiremnts
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private string FixProgramLevelName(string Name)
        {
            string Result = Name;

            if (Name == "Master" || Name == "Bachelor" || Name == "Associate")
            {
                Result += "'s Degree Programs:";
            }
            else
            {
                Result += " Programs:";
            }

            return Result;
        }

        public static string GetClientIPAddress(HttpRequestBase Request)
        {            
            string ipAddress = Request.UserHostAddress;

            if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_VIA"]))
            {
                if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_TRUE_CLIENT_IP"]))
                {
                    ipAddress = Request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
                }
                else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) && Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToLower() != "unknown")
                {
                    ipAddress = !Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Contains(",") ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last();
                }
            }
            else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) && Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToLower() != "unknown")
            {
                ipAddress = !Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Contains(",") ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',').Last();
            }
            else if (!String.IsNullOrWhiteSpace(Request.ServerVariables["REMOTE_ADDR"]))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }

            return ipAddress;
        }
    }



    class ProgramListLevelComparer : IEqualityComparer<FormProgram>
    {
        public static readonly ProgramListLevelComparer Instance = new ProgramListLevelComparer();

        public bool Equals(FormProgram x, FormProgram y)
        {
            return x.ProgramLevelId.Equals(y.ProgramLevelId);
        }

        public int GetHashCode(FormProgram obj)
        {
            return obj.ProgramLevelId;
        }
    }

    class ProgramOptGroupComparer : IEqualityComparer<FormProgram>
    {
        public static readonly ProgramOptGroupComparer Instance = new ProgramOptGroupComparer();

        public bool Equals(FormProgram x, FormProgram y)
        {
            string a = x.CampusOptionGroup ?? string.Empty;
            string b = y.CampusOptionGroup ?? string.Empty;
            return a.Equals(b);
        }

        public int GetHashCode(FormProgram obj)
        {
            string a = obj.CampusOptionGroup ?? string.Empty;
            return a.GetHashCode();
        }
    }
}