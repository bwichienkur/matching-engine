using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class InstitutionMapper
    {
        public Institution MapInstitution(MatchingEngine.InstitutionDetail institutionDetail)
        {
            var institution = new Institution();

            if (institutionDetail != null)
            {
                institution.InstitutionId = institutionDetail.InstitutionId;
                institution.InstitutionName = institutionDetail.InstitutionName;
                institution.InstitutionDescription = institutionDetail.InstitutionDescription;
                institution.InstitutionAccreditation = institutionDetail.AccreditationOrganization;
                institution.InstitutionLogoUrl = institutionDetail.InstitutionLogoURL;
            }

            return institution;
        }

        public List<Institution> MapInstitutions(MatchingEngine.Institution[] matchingEngineInstitutions)
        {
            var institutions = new List<Institution>();

            for (int i = 0; i < matchingEngineInstitutions?.Length; i++)
            {
                Institution institution = new Institution();

                MapInstitution(institution, matchingEngineInstitutions[i]);

                institutions.Add(institution);
            }

            return institutions;
        }

        private void MapInstitution(Institution institution, MatchingEngine.Institution matchingEngineInstitution)
        {
            if (institution != null && matchingEngineInstitution != null)
            {
                institution.InstitutionId = matchingEngineInstitution.InstitutionId;
                institution.InstitutionName = matchingEngineInstitution.InstitutionName;
                institution.InstitutionDescription = matchingEngineInstitution.InstitutionDescription;
                institution.InstitutionLogoUrl = matchingEngineInstitution.InstitutionLogoURL;
                institution.ProgramRankScore = matchingEngineInstitution.ProgramRankScore;

                if (matchingEngineInstitution.GetType() == typeof(MatchingEngine.InstitutionWithProgram))
                {
                    var institutionWithProgram = (MatchingEngine.InstitutionWithProgram)matchingEngineInstitution;

                    var programMapper = new ProgramMapper();
                    institution.Programs = programMapper.MapPrograms(institutionWithProgram.ProgramList);
                }
            }
        }
    }
}
