using EDDY.IS.FormsEngine.Core.DTO.ShortFormSubmission;
using EDDY.IS.FormsEngine.Core.Helpers;
using EDDY.IS.FormsEngine.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Mappers
{
    public class ShortFormSubmissionMapper
    {
        private readonly JsonParser _jsonParser;

        public ShortFormSubmissionMapper()
        {
            _jsonParser = new JsonParser();
        }

        public ShortFormSubmission MapShortFormSubmission(ShortFormSubmissionRequestDTO dto)
        {
            var submission = new ShortFormSubmission();
            submission.TrackId = dto.TrackId;
            submission.IsBeta = dto.IsBeta;
            submission.SchoolPickerUserSelections = MapUserSelections(dto.ProgramSelections, dto.ProgramProductSelections);
            submission.Prospect = new Prospect();
            return submission;
        }

        private Dictionary<int, SchoolPickerUserSelection> MapUserSelections(string programSelectionsJson, string programProductSelectionsJson)
        {
            Dictionary<string, string> programSelections = _jsonParser.JsonStringToDictionary(programSelectionsJson);
            Dictionary<string, string> programProductSelections = _jsonParser.JsonStringToDictionary(programProductSelectionsJson);

            var schoolPickerUserSelections = new Dictionary<int, SchoolPickerUserSelection>();

            MapUserProgramSelections(programSelections, schoolPickerUserSelections);
            MapUserProgramProductSelections(programProductSelections, schoolPickerUserSelections);

            return schoolPickerUserSelections;
        }

        private void MapUserProgramSelections(Dictionary<string, string> programSelections, Dictionary<int, SchoolPickerUserSelection> userSelections)
        {
            foreach (var item in programSelections)
            {
                if (int.TryParse(item.Key, out int institutionId) && int.TryParse(item.Value, out int programId))
                {
                    if (userSelections.TryGetValue(institutionId, out SchoolPickerUserSelection selection))
                    {
                        selection.ProgramId = programId;
                        userSelections[institutionId] = selection;
                    }
                    else
                    {
                        var newSelection = new SchoolPickerUserSelection()
                        {
                            InstitutionId = institutionId,
                            ProgramId = programId
                        };

                        userSelections.Add(institutionId, newSelection);
                    }
                }
            }
        }

        private void MapUserProgramProductSelections(Dictionary<string, string> programProductSelections, Dictionary<int, SchoolPickerUserSelection> userSelections)
        {
            foreach (var item in programProductSelections)
            {
                if (int.TryParse(item.Key, out int institutionId) && int.TryParse(item.Value, out int programProductId))
                {
                    if (userSelections.TryGetValue(institutionId, out SchoolPickerUserSelection selection))
                    {
                        selection.ProgramProductId = programProductId;
                        userSelections[institutionId] = selection;
                    }
                    else
                    {
                        var newSelection = new SchoolPickerUserSelection()
                        {
                            InstitutionId = institutionId,
                            ProgramProductId = programProductId
                        };

                        userSelections.Add(institutionId, newSelection);
                    }
                }
            }
        }

    }
}
