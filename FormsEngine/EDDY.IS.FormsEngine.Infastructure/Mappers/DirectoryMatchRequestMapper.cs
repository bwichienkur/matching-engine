using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class DirectoryMatchRequestMapper
    {
        
        public void MapFormInputToDirectoryMatchRequest(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            MapBaseFields(directoryMatchRequest, formInput);
            MapSimpleFields(directoryMatchRequest, formInput);
            MapComplexFields(directoryMatchRequest, formInput);
        }

        private void MapBaseFields(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            var baseMatchRequestMapper = new BaseMatchRequestMapper();
            baseMatchRequestMapper.MapFormInputToBaseMatchRequest(directoryMatchRequest, formInput);
        }

        private void MapSimpleFields(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            directoryMatchRequest.FeatureId = formInput.FeatureId;
            directoryMatchRequest.CampusId = formInput.CampusId;
        }

        private void MapComplexFields(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            MapProgramIds(directoryMatchRequest, formInput);
            MapInsitutionIds(directoryMatchRequest, formInput);
        }

        private void MapProgramIds(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            if (formInput.ProgramId.HasValue)
            {
                directoryMatchRequest.ProgramIdList = new int[] { formInput.ProgramId.Value };
            }
            else
            {
                directoryMatchRequest.ProgramIdList = formInput.ProgramIds?.ToArray();
            }
        }

        private void MapInsitutionIds(DirectoryMatchRequest directoryMatchRequest, FormInput formInput)
        {
            if (formInput.InstitutionId.HasValue)
            {
                directoryMatchRequest.InstitutionIdList = new int[] { formInput.InstitutionId.Value };
            }
            else
            {
                directoryMatchRequest.InstitutionIdList = formInput.InstitutionIds?.ToArray();
            }
        }
    }
}
