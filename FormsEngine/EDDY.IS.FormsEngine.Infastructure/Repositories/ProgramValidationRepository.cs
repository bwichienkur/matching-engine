using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class ProgramValidationRepository : IProgramValidationRepository
    {

        private readonly MatchingServiceClient _matchingServiceProd;
        private readonly MatchingServiceClient _matchingServiceBeta;
        private readonly ProspectInputMapper _prospectToProspectInputMapper;
        private readonly ValidatedProgramMapper _validatedProgramMapper;

        public ProgramValidationRepository()
        {
            _matchingServiceProd = new MatchingServiceClient("BasicHttpBinding_IMatchingService");
            _matchingServiceBeta = new MatchingServiceClient("BasicHttpBinding_IMatchingService_Beta");
            _prospectToProspectInputMapper = new ProspectInputMapper();
            _validatedProgramMapper = new ValidatedProgramMapper();
        }

        public ValidatedProgram ValidateProgram(int programProductId, Guid TrackId, Prospect prospect, bool isBeta)
        {
            ValidatedProgram validatedProgram;

            try
            {
                ProgramValidateRequest request = new ProgramValidateRequest();
                request.Application = MatchingEngine.ISApplication.FormsEngine;
                request.TrackGuid = TrackId;
                request.ProgramProductId = programProductId;
                request.BreakOnFirstValidationFailure = true;
                request.ProspectInput = _prospectToProspectInputMapper.MapProspectToProspectInput(prospect);
                request.LeadCreationType = MatchingEngine.LeadCreationType.SchoolPickerUserSelection;

                ProgramValidateResponse response = new ProgramValidateResponse();

                if (!isBeta)
                {
                    response = _matchingServiceProd.ValidateProgram(request);
                }
                else
                {
                    response = _matchingServiceBeta.ValidateProgram(request);
                }

                validatedProgram = _validatedProgramMapper.MapProgramValidateResponseToValidatedProgram(programProductId, response);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return validatedProgram;
        }
    }
}
