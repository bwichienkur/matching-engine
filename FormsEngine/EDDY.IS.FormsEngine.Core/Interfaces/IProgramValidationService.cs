using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IProgramValidationService
    {
        IEnumerable<int> GetProgramIdsThatFailedValidation(FormInput formInput);
        List<Program> GetProgramsThatFailedValidation(FormInput formInput);
        List<ValidatedProgram> GetValidatedPrograms(FormInput formInput);
    }
}
