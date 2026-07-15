using EDDY.IS.FormsEngine.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class ProgramMapper
    {

        public List<Core.Models.Program> MapPrograms(IEnumerable<MatchingEngine.Program> matchingEnginePrograms)
        {
            var programs = new List<Core.Models.Program>();
            
            if (matchingEnginePrograms != null)
            {
                foreach (var matchingEngineProgram in matchingEnginePrograms)
                {
                    var program = MapProgram(matchingEngineProgram);
                    programs.Add(program);
                }
            }
            
            return programs;
        }

        public Core.Models.Program MapProgram(MatchingEngine.Program matchingEngineProgram)
        {
            var program = new Core.Models.Program();

            if (matchingEngineProgram != null)
            {
                program.ProgramId = matchingEngineProgram.ProgramId;
                program.ProgramName = matchingEngineProgram.ProgramName;
                program.ProgramDescription = matchingEngineProgram.ProgramDescription;
                program.ProgramProductId = matchingEngineProgram.ProgramProductId;
                program.ProgramRankScore = matchingEngineProgram.ProgramRankScore;
                program.ProgramTemplateId = matchingEngineProgram.TemplateId;
                program.ProductId = matchingEngineProgram.ProductId;

                if (matchingEngineProgram.GetType() == typeof(MatchingEngine.ProgramWithInstitutionCampus))
                {
                    MapProgramWithInstitutionCampus(program, (MatchingEngine.ProgramWithInstitutionCampus)matchingEngineProgram);   
                }
            }

            return program;
        }

        private void MapProgramWithInstitutionCampus(Core.Models.Program program, MatchingEngine.ProgramWithInstitutionCampus matchingEngineProgram)
        {
            program.InstitutionId = matchingEngineProgram.InstitutionId;
            program.InstitutionName = matchingEngineProgram.InstitutionName;
            program.InstitutionDescription = matchingEngineProgram.InstitutionDescription;
            program.InstitutionLogoUrl = matchingEngineProgram.InstitutionLogoURL;
            program.CampusId = matchingEngineProgram.CampusId;
            program.CampusName = matchingEngineProgram.CampusName;
            program.CampusLogoUrl = matchingEngineProgram.CampusLogoURL;
            program.CampusType = MapCampusType(matchingEngineProgram.CampusType);
        }

        private CampusType? MapCampusType(MatchingEngine.CampusType matchingEngineCampusType)
        {
            CampusType? campusType = null;

            if (matchingEngineCampusType == MatchingEngine.CampusType.Online)
            {
                campusType = CampusType.Online;
            }
            else if (matchingEngineCampusType == MatchingEngine.CampusType.Ground)
            {
                campusType = CampusType.Ground;
            }

            return campusType;
        }

    }
}
