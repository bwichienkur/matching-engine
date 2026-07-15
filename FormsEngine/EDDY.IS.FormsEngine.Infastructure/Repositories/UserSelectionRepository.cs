using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO.Responses;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class UserSelectionRepository : IUserSelectionRepository
    {
        private readonly MatchingServiceClient _matchingServiceProd;
        private readonly MatchingServiceClient _matchingServiceBeta;

        public UserSelectionRepository()
        {
            _matchingServiceProd = new MatchingServiceClient("BasicHttpBinding_IMatchingService");
            _matchingServiceBeta = new MatchingServiceClient("BasicHttpBinding_IMatchingService_Beta");
        }

        public UserSelectionResponse GetUserSelectionsForSchoolPicker(FormInput formInput, IEnumerable<int> excludedInstitutionIds = null)
        {
            return GetSchoolSelections(formInput, MatchingEngine.LeadCreationType.WizardUserSelection, excludedInstitutionIds?.ToArray());
        }

        private UserSelectionResponse GetSchoolSelections(FormInput formInput, MatchingEngine.LeadCreationType leadCreationType, int[] excludedInstitutionIds = null)
        {
            WizardMatchRequest request = new WizardMatchRequest();

            var mapper = new WizardMatchRequestMapper();
            mapper.MapFormInputToWizardMatchRequest(request, formInput);

            request.IncludeSchoolSelectionList = true;
            request.IncludeSmartMatchList = false;
            request.LeadCreationType = leadCreationType;
            request.SmartMatchedInstituionIdList = excludedInstitutionIds;
            
            WizardMatchResponse response;

            if (formInput.IsBeta)
            {
                response = _matchingServiceBeta.GetWizardMatches(request);
            }
            else
            {
                response = _matchingServiceProd.GetWizardMatches(request);
            }

            var campusMapper = new CampusMapper();

            return new UserSelectionResponse
            {
                UserSelections = campusMapper.MapCampuses(response?.SchoolSelectionList),
                MatchResponseGuid = response.MatchResponseGuid
            };
        }

    }
}
