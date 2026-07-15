using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IInstitutionService
    {
        List<Institution> GetInstitutions(FormInput formInput);
        Institution GetInstitution(FormRequest formRequest);
    }
}
