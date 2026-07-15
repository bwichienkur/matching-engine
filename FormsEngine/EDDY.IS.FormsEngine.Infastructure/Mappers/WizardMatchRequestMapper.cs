using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class WizardMatchRequestMapper
    {

        public void MapFormInputToWizardMatchRequest(WizardMatchRequest wizardMatchRequest, FormInput formInput)
        {
            MapBaseFields(wizardMatchRequest, formInput);
        }

        private void MapBaseFields(WizardMatchRequest wizardMatchRequest, FormInput formInput)
        {
            var baseMatchRequestMapper = new BaseMatchRequestMapper();
            baseMatchRequestMapper.MapFormInputToBaseMatchRequest(wizardMatchRequest, formInput);
        }

    }
}
