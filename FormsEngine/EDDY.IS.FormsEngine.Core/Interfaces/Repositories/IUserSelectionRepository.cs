using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.DTO.Responses;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces.Repositories
{
    public interface IUserSelectionRepository
    {
        UserSelectionResponse GetUserSelectionsForSchoolPicker(FormInput formInput, IEnumerable<int> excludedInstitutionIds);
    }
}
