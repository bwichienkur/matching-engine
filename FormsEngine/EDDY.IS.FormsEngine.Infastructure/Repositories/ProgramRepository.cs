using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class ProgramRepository : IProgramRepository
    {

        private readonly MatchingServiceClient _matchingServiceProd;
        private readonly MatchingServiceClient _matchingServiceBeta;
        private readonly ProgramMapper _programMapper;

        public ProgramRepository()
        {
            _matchingServiceProd = new MatchingServiceClient("BasicHttpBinding_IMatchingService");
            _matchingServiceBeta = new MatchingServiceClient("BasicHttpBinding_IMatchingService_Beta");
            _programMapper = new ProgramMapper();
        }

        public List<Core.Models.Program> GetPrograms(Guid trackId, int applicationId, bool isBeta, IEnumerable<int> programIds, bool includeProgramDetail)
        {
            var programs = new List<Core.Models.Program>();

            var directoryMatchRequest = new DirectoryMatchRequest
            {
                TrackGuid = trackId,
                ApplicationId = applicationId,
                ProgramIdList = programIds.ToArray(),
                Application = MatchingEngine.ISApplication.FormsEngine,
                SortMethod = EntitySortMethod.RankScore
            };

            ProgramResponse programResponse;

            if (isBeta)
            {
                programResponse = _matchingServiceBeta.GetPrograms(directoryMatchRequest, includeProgramDetail);
            }
            else
            {
                programResponse = _matchingServiceProd.GetPrograms(directoryMatchRequest, includeProgramDetail);
            }

            if (programResponse?.ProgramList?.Length > 0)
            {
                programs = _programMapper.MapPrograms(programResponse.ProgramList);
            }

            return programs;
        }
    }
}
