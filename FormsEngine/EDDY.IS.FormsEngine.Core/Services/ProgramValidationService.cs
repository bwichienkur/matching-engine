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
    public class ProgramValidationService : IProgramValidationService
    {
        private readonly IProgramValidationRepository _programValidationRepository;

        public ProgramValidationService(IProgramValidationRepository programValidationRepository)
        {
            _programValidationRepository = programValidationRepository;
        }

        public IEnumerable<int> GetProgramIdsThatFailedValidation(FormInput formInput)
        {
            var failedProgramIds = new List<int>();

            List<Program> programsThatFailedValidation = GetProgramsThatFailedValidation(formInput);

            return programsThatFailedValidation.Select(p => p.ProgramId);
        }

        public List<Program> GetProgramsThatFailedValidation(FormInput formInput)
        {
            var failedPrograms = new List<Program>();

            if (formInput?.Matches?.Count > 0)
            {
                foreach (var match in formInput.Matches)
                {
                    var passedValidation = ValidateProgram(match.ProgramProductId, formInput.TrackId, formInput.Prospect, formInput.IsBeta);

                    if (!passedValidation)
                    {
                        failedPrograms.Add(new Program
                        {
                            InstitutionId = match.InstitutionId,
                            ProgramId = match.ProgramId,
                            ProgramProductId = match.ProgramProductId
                        });
                    }
                }
            }

            return failedPrograms;
        }

        public List<ValidatedProgram> GetValidatedPrograms(FormInput formInput)
        {
            var validatedPrograms = new List<ValidatedProgram>();

            if (formInput?.Matches?.Count > 0)
            {
                foreach (var match in formInput.Matches)
                {
                    ValidatedProgram validatedProgram = _programValidationRepository.ValidateProgram(match.ProgramProductId, formInput.TrackId, formInput.Prospect, formInput.IsBeta);
                    validatedPrograms.Add(validatedProgram);
                }
            }

            return validatedPrograms;
        }

        private bool ValidateProgram(int programProductId, Guid trackId, Prospect prospect, bool isBeta)
        {
            ValidatedProgram validatedProgram = _programValidationRepository.ValidateProgram(programProductId, trackId, prospect, isBeta);
            return validatedProgram.PassedValidation;
        }
    }
}
