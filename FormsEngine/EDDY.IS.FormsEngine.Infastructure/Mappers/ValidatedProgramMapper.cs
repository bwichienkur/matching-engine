using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class ValidatedProgramMapper
    {
        public ValidatedProgram MapProgramValidateResponseToValidatedProgram(int programProductId, ProgramValidateResponse programValidateResponse)
        {
            var validatedProgram = new ValidatedProgram();

            validatedProgram.ProgramProductId = programProductId;
            validatedProgram.ProgramId = programValidateResponse.ProgramId;
            validatedProgram.PassedValidation = programValidateResponse.PassedValidation;
            validatedProgram.PaidStatusType = (int?) programValidateResponse.PaidStatusTypeId;
            validatedProgram.AlternateProgramProductId = programValidateResponse.AlternateProgramProductId;
            validatedProgram.Score = programValidateResponse.Score;
            validatedProgram.ScoreId = programValidateResponse.ScoreId;

            MapRuleFailures(validatedProgram, programValidateResponse.RuleFailures);

            return validatedProgram;
        }

        private void MapRuleFailures(ValidatedProgram validatedProgram, RuleFailure[] ruleFailures)
        {
            if (ruleFailures?.Length > 0)
            {
                foreach (var ruleFailure in ruleFailures)
                {
                    AddRuleFailureToValidatedProgram(validatedProgram, ruleFailure.RuleFailureType.ToString(), ruleFailure.RuleFailureName);

                    if (ruleFailure.RuleFailureType == BaseRuleType.InternalDuplicate)
                    {
                        validatedProgram.IsInternalDuplicate = true;
                    }
                    else if (ruleFailure.RuleFailureType == BaseRuleType.ExternalDuplicate)
                    {
                        validatedProgram.IsExternalDuplicate = true;
                    }
                }
            }
        }

        private void AddRuleFailureToValidatedProgram(ValidatedProgram validatedProgram, string ruleFailureType, string ruleFailureName)
        {
            validatedProgram.RuleFailures.Add(new KeyValuePair<string, string>(ruleFailureType, ruleFailureName));
        }
        
    }
}
