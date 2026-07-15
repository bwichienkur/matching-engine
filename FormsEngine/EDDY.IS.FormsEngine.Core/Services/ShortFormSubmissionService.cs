using EDDY.IS.FormsEngine.Core.DTO.ShortFormSubmission;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ShortFormSubmissionService : IShortFormSubmissionService 
    {
        private readonly ShortFormSubmissionMapper _mapper;

        public ShortFormSubmissionService()
        {
            _mapper = new ShortFormSubmissionMapper();
        }

        public ShortFormSubmissionResponseDTO SubmitForm(ShortFormSubmissionRequestDTO submissionDto)
        {
            var response = new ShortFormSubmissionResponseDTO();

            ShortFormSubmission submission = _mapper.MapShortFormSubmission(submissionDto);


            // 2. Save Prospect

            // 3. Check Express Consent

            // 4. Save Lead

            return response;
        }

        

    }
}
