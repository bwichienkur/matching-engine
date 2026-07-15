using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class MatchValidationService : IMatchValidationService
    {
        private readonly IProgramValidationService _programValidationService;

        public MatchValidationService(IProgramValidationService programValidationService)
        {
            _programValidationService = programValidationService;
        }

        public List<Match> GetMatchesThatFailedValidation(FormInput formInput)
        {
            List<Match> matchesThatFailedValidation = new List<Match>();

            if (formInput?.Matches != null)
            {
                HashSet<int> failedProgramIds = GetFailedProgramIds(formInput);
                matchesThatFailedValidation.AddRange(formInput.Matches.Where(match => failedProgramIds.Contains(match.ProgramId)).Select(match => match));
            }

            return matchesThatFailedValidation;
        }

        public List<Match> GetMatchesThatPassedValidation(FormInput formInput)
        {
            List<Match> matchesThatPassedValidation = new List<Match>();

            if (formInput.Matches != null)
            {
                HashSet<int> failedProgramIds = GetFailedProgramIds(formInput);
                matchesThatPassedValidation.AddRange(formInput.Matches.Where(match => !failedProgramIds.Contains(match.ProgramId)).Select(match => match));
            }

            return matchesThatPassedValidation;
        }

        public void ValidateMatches(FormInput formInput)
        {   
            if (formInput.Matches?.Count > 0)
            {
                Dictionary<int, Match> matches = CreateMatchMap(formInput.Matches);
                List<ValidatedProgram> validatedPrograms = _programValidationService.GetValidatedPrograms(formInput);

                foreach (var validatedProgram in validatedPrograms)
                {
                    if (validatedProgram.ProgramProductId.HasValue && matches.TryGetValue(validatedProgram.ProgramProductId.Value, out Match match))
                    {
                        MapValidatedProgramFieldsToMatchFields(validatedProgram, match);
                    }
                }

            }
        }

        private Dictionary<int, Match> CreateMatchMap(IEnumerable<Match> matches)
        {
            Dictionary<int, Match> programIdMatchMap = new Dictionary<int, Match>();

            foreach (var match in matches)
            {
                programIdMatchMap.Add(match.ProgramProductId, match);
            }

            return programIdMatchMap;
        }

        private void MapValidatedProgramFieldsToMatchFields(ValidatedProgram validatedProgram, Match match)
        {
            match.PassedValidation = validatedProgram.PassedValidation;
            match.IsExternalDuplicate = validatedProgram.IsExternalDuplicate;
            match.IsInternalDuplicate = validatedProgram.IsInternalDuplicate;
            match.PaidStatusType = validatedProgram.PaidStatusType;
            match.RuleFailures = validatedProgram.RuleFailures;
            match.AlternativeProgramProductId = validatedProgram.AlternateProgramProductId;
            match.Score = validatedProgram.Score;
            match.ScoreId = validatedProgram.ScoreId;
        }

        private HashSet<int> GetFailedProgramIds(FormInput formInput)
        {
            IEnumerable<int> programIdsThatFailedValidation = _programValidationService.GetProgramIdsThatFailedValidation(formInput);
            return new HashSet<int>(programIdsThatFailedValidation);
        }

    }
}
