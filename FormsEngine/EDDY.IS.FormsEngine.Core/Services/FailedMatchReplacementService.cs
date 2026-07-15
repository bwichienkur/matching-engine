using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class FailedMatchReplacementService : IFailedMatchReplacementService
    {
        private readonly IMatchValidationService _matchValidationService;
        private readonly IUserSelectionService _userSelectionService;
        private readonly IMetaDataService _metaDataService;

        public FailedMatchReplacementService(IMatchValidationService matchValidationService, IUserSelectionService userSelectionService, IMetaDataService metaDataService)
        {
            _matchValidationService = matchValidationService;
            _userSelectionService = userSelectionService;
            _metaDataService = metaDataService;
        }

        public FailedMatchReplacements GetReplacementsForFailedMatches(FormInput formInput)
        {
            var failedMatchReplacements = new FailedMatchReplacements();

            if (formInput?.Matches != null)
            {
                var institutionIdsFromMatches = formInput.Matches.Select(m => m.InstitutionId).ToArray();

                failedMatchReplacements.FailedMatches = _matchValidationService.GetMatchesThatFailedValidation(formInput);
                int numberOfFailedMatches = failedMatchReplacements.FailedMatches.Count();

                if (numberOfFailedMatches > 0)
                {
                    var userSelectionResponse = _userSelectionService.GetUserSelectionsForSchoolPicker(formInput, institutionIdsFromMatches);
                    failedMatchReplacements.ReplacementMatches = GetReplacementMatchesFromSelections(userSelectionResponse.UserSelections, numberOfFailedMatches);
                    failedMatchReplacements.Message = GetFailedMatchesMessage(failedMatchReplacements.FailedMatches);
                }
            }

            return failedMatchReplacements;
        }

        private List<Match> GetReplacementMatchesFromSelections(List<Campus> userSelections, int numberOfProgramsThatFailedValidation)
        {
            var matches = new List<Match>();

            for (int i = 0; i < userSelections.Count && matches.Count < numberOfProgramsThatFailedValidation; i++)
            {
                var campus = userSelections[i];
                var program = campus.Programs?.FirstOrDefault();

                if (campus != null && program != null)
                {
                    matches.Add(new Match(campus, program));
                }
            }

            return matches;
        }

        private string GetFailedMatchesMessage(IEnumerable<Match> failedMatches)
        {
            string message = string.Empty;

            int count = failedMatches?.Count() ?? 0;

            if (count > 0)
            {
                var schoolNames = failedMatches.Select(m => m.InstitutionName).ToArray();

                string schoolNamesWithoutAnd = string.Join(", ", schoolNames, 0, count - 1);

                string formattedSchools = string.Empty;
                formattedSchools = count > 1 && !string.IsNullOrWhiteSpace(schoolNamesWithoutAnd) ? $"{schoolNamesWithoutAnd}, and {schoolNames.Last()}" : schoolNames.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(formattedSchools))
                {
                    string unformattedMessage = _metaDataService.GetMetaDataMessageByKey("SCHOOLPICKERWIZARD.MATCHREPLACEMENTS.MESSAGE");
                    message = unformattedMessage.Replace("{schools}", formattedSchools);
                }
            }

            return message;
        }
    }
}
