using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class CampusMapper
    {

        public List<Campus> MapCampuses(MatchingEngine.CampusWithInstitution[] matchingEngineCampuses)
        {
            var campuses = new List<Campus>();

            for (int i = 0; i < matchingEngineCampuses?.Length; i++)
            {
                var campus = new Campus();
                MapCampus(campus, matchingEngineCampuses[i]);
                campuses.Add(campus);
            }

            return campuses;
        }

        private void MapCampus(Campus campus, MatchingEngine.CampusWithInstitution matchingEngineCampus)
        {
            if (campus != null && matchingEngineCampus != null)
            {
                var programMapper = new ProgramMapper();

                campus.InstitutionId = matchingEngineCampus.InstitutionId;
                campus.InstitutionName = matchingEngineCampus.InstitutionName;
                campus.InstitutionDescription = matchingEngineCampus.InstitutionDescription;
                campus.InstitutionLogoUrl = matchingEngineCampus.InstitutionLogoURL;
                campus.CampusId = matchingEngineCampus.CampusId;
                campus.CampusName = matchingEngineCampus.CampusName;
                campus.CampusLogoUrl = matchingEngineCampus.CampusLogoURL;
                campus.ProgramRankScore = matchingEngineCampus.ProgramRankScore;
                campus.Programs = programMapper.MapPrograms(matchingEngineCampus.ProgramList);
            }
        }

    }
}
