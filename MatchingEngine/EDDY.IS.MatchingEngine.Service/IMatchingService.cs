using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine.Service
{
    [ServiceContract]
    public interface IMatchingService
    {
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        FormProgramResponse GetFormPrograms(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        InstitutionResponse GetInstitutions(DirectoryMatchRequest directoryMatchRequest, GetInstitutionCampusOption campusOption);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CampusResponse GetCampuses(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ApolloCampusResponse GetApolloCampuses(ApolloCampusRequest apolloRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        NeoResponse GetNeoResponse(NeoMatchRequest neoRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramResponse GetPrograms(DirectoryMatchRequest directoryMatchRequest, bool includeProgramDetail);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        NavigationResponse GetFacetedNavigation(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        SiteMapResponse GetSiteMapGeoInfo(DirectoryMatchRequest directoryMatchRequest);
            
        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CategoryResponse GetCategories(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        SubjectResponse GetSubjects(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        SpecialtyResponse GetSpecialties(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramLevelResponse GetProgramLevels(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CrossSellProgramResponse GetProgramsForCrossSell(CrossSellMatchRequest crossSellRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        InstitutionDetailResponse GetInstitutionDetails(int applicationId, int institutionId, Guid TrackGuid);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramDetailResponse GetProgramDetails(int applicationId, int programId, Guid TrackGuid, bool? includeProgramGroupRollup, int? campusId);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramDetailResponse GetProgramDisplayGroupDetails(int applicationId, int programDisplayGroupId, Guid TrackGuid);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CategoryResponse GetCategoriesAllIfNone(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        SubjectResponse GetSubjectsAllIfNone(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        SpecialtyResponse GetSpecialtiesAllIfNone(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramLevelResponse GetProgramLevelsAllIfNone(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CampusTypeMatchResponse GetCampusTypes(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        WizardMatchResponse GetWizardMatches(WizardMatchRequest wizardMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        TemplateMatchResponse GetTemplatesForMatches(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CampusZipCodeRuleResponse GetCampusZipCodeRules(int campusId, Guid trackGuid);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<ProgramRuleDefinition> GetRulesForProgramProduct(int ProgramProductId);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramValidateResponse ValidateProgram(ProgramValidateRequest request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramProductValidateResponse ValidateProgramProducts(ProgramProductValidateRequest request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramValidateResponse ValidateCountryAndProgram(CountryValidateRequest request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ProgramValidateResponse ValidateAPIProgram(ProgramValidateRequest request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ApiProgramResponse GetApiProgram(int programId, Guid trackGuid);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetProgramDescription(int programId);

        [OperationContract]
        void RefreshCacheItem(MatchingCacheItem key);

        [OperationContract]
        void RemoveCacheItem(string key);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CampaignDetailResponse GetCampaignDetailByTrackID(Guid TrackID);


        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        CategoryResponse GetCategoriesWithSubjects(DirectoryMatchRequest directoryMatchRequest);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        AdServerClientRelationshipResponse GetAdServerClientRelationships(AdServerMatchRequest adServerMatchRequest);
    }

}
