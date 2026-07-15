using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly MatchingServiceClient _matchingServiceProd;
        private readonly MatchingServiceClient _matchingServiceBeta;

        private readonly InstitutionMapper _institutionMapper;
        private readonly DirectoryMatchRequestMapper _directoryMatchRequestMapper;

        public InstitutionRepository()
        {
            _matchingServiceProd = new MatchingServiceClient("BasicHttpBinding_IMatchingService");
            _matchingServiceBeta = new MatchingServiceClient("BasicHttpBinding_IMatchingService_Beta");
            _institutionMapper = new InstitutionMapper();
            _directoryMatchRequestMapper = new DirectoryMatchRequestMapper();
        }

        public Core.Models.Institution GetInstitution(FormRequest formRequest)
        {
            Core.Models.Institution institution = new Core.Models.Institution();

            if (formRequest?.InstitutionId > 0)
            {
                InstitutionDetailResponse response;

                Guid.TryParse(formRequest.TrackId, out Guid trackGuid);

                if (!formRequest.IsBeta)
                {
                    response = _matchingServiceProd.GetInstitutionDetails(formRequest.ApplicationId, formRequest.InstitutionId.Value, trackGuid);
                }
                else
                {
                    response = _matchingServiceBeta.GetInstitutionDetails(formRequest.ApplicationId, formRequest.InstitutionId.Value, trackGuid);
                }

                institution = _institutionMapper.MapInstitution(response?.InstitutionDetails);
            }

            return institution;

        }

        public List<Core.Models.Institution> GetInstitutions(FormInput formInput)
        {
            var request = new DirectoryMatchRequest();
            _directoryMatchRequestMapper.MapFormInputToDirectoryMatchRequest(request, formInput);

            request.MaxResultsCount = formInput.MaxSchoolPickerMatches;
            request.SortMethod = EntitySortMethod.RankScore;

            InstitutionResponse response;

            if (!formInput.IsBeta)
            {
                response = _matchingServiceProd.GetInstitutions(request, GetInstitutionCampusOption.NoCampus);
            }
            else
            {
                response = _matchingServiceBeta.GetInstitutions(request, GetInstitutionCampusOption.NoCampus);
            }

            List<Core.Models.Institution> institutions = _institutionMapper.MapInstitutions(response.InstitutionList);

            return institutions;
        }

    }
}
