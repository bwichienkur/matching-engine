using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ProgramService : IProgramService
    {

        private readonly IProgramRepository _programRepository;

        public ProgramService(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public List<Program> GetPrograms(FormInput formInput, IEnumerable<int> programIds, bool includeProgramDetail = true)
        {
            var programs = new List<Program>();

            if (formInput?.TrackId != null && formInput?.ApplicationId > 0 && formInput?.IsBeta != null && programIds?.Count() > 0)
            {
                programs = _programRepository.GetPrograms(formInput.TrackId, formInput.ApplicationId, formInput.IsBeta, programIds, includeProgramDetail);
            }

            return programs;
        }

        public List<Program> GetPrograms(FormRequest formRequest, IEnumerable<int> programIds, bool includeProgramDetail = true)
        {
            var programs = new List<Program>();

            Guid.TryParse(formRequest?.TrackId, out Guid trackGuid);

            if (trackGuid != Guid.Empty && formRequest?.ApplicationId > 0 && formRequest?.IsBeta != null && programIds?.Count() > 0)
            {
                return _programRepository.GetPrograms(trackGuid, formRequest.ApplicationId, formRequest.IsBeta, programIds, includeProgramDetail);
            }

            return programs;
        }
    }
}
