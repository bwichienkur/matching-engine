using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsEngine : FormsEngineBase
    {

        public void GetOpenMailProfiles() {
            dbOpenMailProfileDataService.OpenMailProfileDictionary();
        }

        public OpenMailValidationDTO ShouldSendToNoMatch(int profileId, Dictionary<string, string> formFields) {

            OpenMailValidationDTO result = new OpenMailValidationDTO() { SentToNoMatch = false };
            OpenMailProfile profile = dbOpenMailProfileDataService.GetOpenMailProfile(profileId);

            if (profile != null) {
                foreach (var ruleGroup in profile.OpenMailRules.GroupBy(r => r.GroupId))
                {
                    if (EvaluateGroupOfRules(formFields, ruleGroup)) {
                        return new OpenMailValidationDTO() { SentToNoMatch = true, MatchedRuleGroup = ruleGroup.Key };
                    }
                }
            }
            return result;
        }

        private bool EvaluateGroupOfRules(Dictionary<string, string> formFields, IGrouping<int, OpenMailRule> ruleGroup)
        {
            List<bool> groupResult = new List<bool>();
            foreach (var rule in ruleGroup.ToList())
            {
                if (formFields.TryGetValue(rule.StandardControlCode.Code, out string formFieldValue))
                {
                    groupResult.Add(EvaluateRule(rule, formFieldValue));
                }
            }
            return groupResult.Count() > 0 && !groupResult.Any(r => r == false);
        }

        private bool EvaluateRule(OpenMailRule rule, string formFieldValue)
        {
            bool ruleResult = false;
            if (string.IsNullOrWhiteSpace(formFieldValue)) {
                return ruleResult;
            }
            if (Enum.TryParse(rule.RuleType, out OpenMailRuleOperators ruleType))
            {
                int valueToCompare;
                string[] ruleValues;
                switch (ruleType)
                {
                    case OpenMailRuleOperators.GreaterThan:
                        if (int.TryParse(formFieldValue, out valueToCompare))
                        {
                            ruleResult = valueToCompare > int.Parse(rule.Value);
                        }
                        break;
                    case OpenMailRuleOperators.LessThan:
                        if (int.TryParse(formFieldValue, out valueToCompare))
                        {
                            ruleResult = valueToCompare < int.Parse(rule.Value);
                        }
                        break;
                    case OpenMailRuleOperators.Equals:
                        ruleResult = formFieldValue.Equals(rule.Value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case OpenMailRuleOperators.In:
                        ruleValues = rule.Value.Split(',');
                        ruleResult = ruleValues.Contains(formFieldValue, StringComparer.OrdinalIgnoreCase);
                        break;
                    case OpenMailRuleOperators.NotIn:
                        ruleValues = rule.Value.Split(',');
                        ruleResult = !ruleValues.Contains(formFieldValue, StringComparer.OrdinalIgnoreCase);
                        break;
                    case OpenMailRuleOperators.Between:
                        if (int.TryParse(formFieldValue, out valueToCompare))
                        {
                            int lowerBoundary, upperBoundary;
                            var ruleBoundaries = rule.Value.Split(',');
                            if (ruleBoundaries.Count() == 2 && int.TryParse(ruleBoundaries[0], out lowerBoundary) && int.TryParse(ruleBoundaries[1], out upperBoundary))
                            {
                                ruleResult = lowerBoundary <= valueToCompare && valueToCompare <= upperBoundary;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return ruleResult;
        }
    }
}
