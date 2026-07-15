//using EDDY.IS.MatchingEngine.DataModel;
//using EDDY.IS.MatchingEngine.DataModel.Entity;
//using EDDY.IS.MatchingEngine.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EDDY.IS.MatchingEngine.Rules
//{
//    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct
//                                                   },
//                            new InputRequired[] { InputRequired.Email,
//                                                  InputRequired.FirstName,
//                                                  InputRequired.LastName,
//                                                  InputRequired.Phone1
//                                                }
//                            , new RuleAttribute[] { RuleAttribute.ExecuteEMSRules })]
//    public class EMSDefaultDuplicate : Rule, ICRProgramProductRule
//    {
//        public EMSDefaultDuplicate(RuleInput ri)
//            : base(ri)
//        { }

//        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
//        {
//            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

//            Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> dupes = RuleDataService.GetDuplicateLeadsByProgramProduct(ruleInput.prospectData.Email);

//            foreach (ProgramProductRuleInput program in input.Values)
//            {
//                if (ruleInput.Campaign.InstitutionAgencyType.HasValue && ruleInput.Campaign.InstitutionAgencyType.Value != InstitutionAgencyType.Lenexa)
//                {
//                    if (dupes.ContainsKey(program.ProgramProductId))
//                    {
//                        foreach (var dupe in dupes[program.ProgramProductId])
//                        {
//                            if (dupe.FirstName.ToLower() == ruleInput.prospectData.FirstName.ToLower() &&
//                                      dupe.LastName.ToLower() == ruleInput.prospectData.LastName.ToLower() &&
//                                      ((!string.IsNullOrEmpty(ruleInput.prospectData.Phone1) && dupe.Phone1 == ruleInput.prospectData.Phone1) ||
//                                       (!string.IsNullOrEmpty(ruleInput.prospectData.Phone2) && dupe.Phone2 == ruleInput.prospectData.Phone2)
//                                      ))
//                            {
//                                int lookbackPeriod = this.ruleInput.Campaign.CampaignDuplicateLookback.HasValue ? ruleInput.Campaign.CampaignDuplicateLookback.Value : dupe.CrDuplicateLookback;

//                                if (lookbackPeriod > 0 && dupe.DaysElapsed <= lookbackPeriod)
//                                {
//                                    program.BaseRuleType = BaseRuleDefinitionType.InternalDuplicate;
//                                    program.RuleId = (int)BaseRuleDefinitionType.InternalDuplicate;
//                                    program.RuleName = "Internal Duplicate";

//                                    removedPrograms.Add(program);

//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
            

//            output = removedPrograms;
//        }
//    }
//}
