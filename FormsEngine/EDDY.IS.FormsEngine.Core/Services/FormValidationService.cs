using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class FormValidationService : IFormValidationService
    {
        private readonly IFormValidationRepository _formValidationRepository;

        public FormValidationService(IFormValidationRepository formValidationRepository)
        {
            _formValidationRepository = formValidationRepository;
        }

        public FormValidationResult ValidateForm(FormInput formInput, ref PerformanceLog performanceLog)
        {
            return _formValidationRepository.ValidateForm(formInput, ref performanceLog);
        }
    }
}
