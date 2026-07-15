using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IProgramService
    {
        List<Program> GetPrograms(FormInput formInput, IEnumerable<int> programIds, bool includeProgramDetail = true);
        List<Program> GetPrograms(FormRequest formRequest, IEnumerable<int> programIds, bool includeProgramDetail = true);
    }
}
