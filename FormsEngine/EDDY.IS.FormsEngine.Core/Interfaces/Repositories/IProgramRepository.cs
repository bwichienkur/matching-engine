using EDDY.IS.FormsEngine.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces.Repositories
{
    public interface IProgramRepository
    {
        List<Core.Models.Program> GetPrograms(Guid trackId, int applicationId, bool isBeta, IEnumerable<int> programIds, bool includeProgramDetail);
    }
}
