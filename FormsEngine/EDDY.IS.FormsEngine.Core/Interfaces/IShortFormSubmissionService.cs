using EDDY.IS.FormsEngine.Core.DTO.ShortFormSubmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IShortFormSubmissionService
    {
        ShortFormSubmissionResponseDTO SubmitForm(ShortFormSubmissionRequestDTO submission);
    }
}
