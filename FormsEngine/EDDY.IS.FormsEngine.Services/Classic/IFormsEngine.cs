using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.LeadEngine.DTO.Request;
using EDDY.IS.Core;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine.Services.Classic
{
    [ServiceContract]
    public interface IFormsEngine
    {
        [OperationContract]
        List<HTMLRenderingStrategyDTO> GetRenderingStrategies(bool Wizard);

        [OperationContract]
        List<HTMLRenderingStrategyDTO> GetRenderingStrategiesByType(FormTemplateTypes FormTemplateType);

        [OperationContract]
        TemplateDTO GetProgramTemplateModel(int ProgramProductId, bool IsBeta);

        [OperationContract]
        TemplateDTO GetProgramTemplateModelByTemplate(int TemplateId, bool IsBeta);

        [OperationContract]
        APIValidationResultDTO ValidateForm(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, APILead Lead);

        [OperationContract]
        ProspectInput BuildProspect(int ApplicationId, bool IsBeta, string TrackId, APILead Lead);

        [OperationContract]
        APIValidationResultDTO ValidateFormPost(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, string LeadData);

        [OperationContract]
        APISubmissionResultDTO ProcessHostAndPostLead(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, APILead Lead);

        [OperationContract]
        APISubmissionResultDTO ProcessHostAndPostLeadPost(int ApplicationId, int ProgramProductId, bool IsBeta, string TrackId, string LeadData);

        [OperationContract]
        APIProgramMatchesDTO GetProgramMatches(int ApplicationId, bool IsBeta, string TrackId, APILead Lead);

        [OperationContract]
        APIProgramMatchesDTO GetEnhancedProgramMatches(int ApplicationId, bool IsBeta, string TrackId, APILead Lead);

        [OperationContract]
        APIProgramMatchesDTO GetProgramMatchesPost(int ApplicationId, bool IsBeta, string TrackId, string LeadData);

        /*************************************************************************************************
         *   Added By Erick
         * 
         *   These methods were added for use by Apollo
         *************************************************************************************************/
        [OperationContract]
        APIMultiValidationResultDTO ValidateMultipleForms(int ApplicationId, List<int> ProgramProducts, bool IsBeta, string TrackId, APILead Lead);
        
        [OperationContract]
        bool ValidateEmail(string EmailAddress);

        [OperationContract]
        APIMultiSubmissionResultDTO ProcessApolloSubmission(int ApplicationId, List<KeyValuePair<int, string>> ProgramProducts, int ProspectId, string TrackId, string MatchResponseGuid, APILead Lead, int? ClientRelationContactId, bool RealtimeDelivery, int? prospectFlowId, List<int?> PaidStatusTypeIds, bool isBeta);

        [OperationContract]
        void ReleaseTitaniumLead(int programProductId, APILead Lead, int ProspectId, int? ClientRelationContactId, int? prospectFlowId, int leadId);

        [OperationContract]
        List<APITemplateControlResultDTO> RetrieveTemplateControlsByProgramTemplate(Guid? trackId, int? application);

        //Needed by EdDy API 
        [OperationContract]
        TemplateDTO GetProgramAllTemplatesMergedModel(int programId, Guid trackId, bool IsBeta);
    }
}
