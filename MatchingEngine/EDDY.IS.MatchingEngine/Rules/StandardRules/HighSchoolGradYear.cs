using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.HSGraduationYear }
                           , null)]
    public class HighSchoolGradYear : Rule, ICRProgramProductRule
    {
        public HighSchoolGradYear(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
            BaseRuleDefinition br = null;

            var brCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);
            Dictionary<int, InstitutionContent> institutionContentDict = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, InstitutionContent>>(MatchingCacheItem.InstitutionContent);
            DateTime academicYearResetsOn;     
            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            if (brCache != null)
            {
                br = brCache.ContainsKey(BaseRuleDefinitionType.HSGradYear) ? brCache[BaseRuleDefinitionType.HSGradYear] : null;

                foreach (ProgramProductRuleInput program in input.Values)
                {
                    if (br.ProgramProductAssignments.ContainsKey(program.Key))
                    {
                        List<RuleDefinition> ruleDefinitions = br.ProgramProductAssignments[program.Key];

                        if (ruleDefinitions.Count > 0)
                        {

                            foreach (RuleDefinition rd in ruleDefinitions)
                            {

                                if (rd.IsStatic && rd.IsUpperBound)
                                {
                                    if (ruleInput.prospectData.HSGraduationYear.Value > rd.EntityValue)
                                    {
                                        program.BaseRuleType = rd.BaseRuleType;
                                        program.RuleId = rd.RuleId;
                                        program.RuleName = rd.RuleName.Contains("XXXX") ? rd.RuleName.Replace("XXXX", rd.EntityValue.ToString()) : rd.RuleName;
                                        removedPrograms.Add(program);
                                        continue;
                                    }
                                }
                                else if (rd.IsStatic && rd.IsLowerBound)
                                {
                                    if (ruleInput.prospectData.HSGraduationYear.Value < rd.EntityValue)
                                    {
                                        program.BaseRuleType = rd.BaseRuleType;
                                        program.RuleId = rd.RuleId;
                                        program.RuleName = rd.RuleName.Contains("XXXX") ? rd.RuleName.Replace("XXXX", rd.EntityValue.ToString()) : rd.RuleName;
                                        removedPrograms.Add(program);
                                    }

                                }
                                else if (!rd.IsStatic)
                                {
                                    if (institutionContentDict.ContainsKey(program.InstitutionId))
                                    {
                                        InstitutionContent ic = institutionContentDict[program.InstitutionId];

                                        if (!String.IsNullOrEmpty(ic.AcademicYearBeginDay) && !String.IsNullOrEmpty(ic.AcademicYearBeginMonth))
                                        {
                                            academicYearResetsOn = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ic.AcademicYearBeginMonth, ic.AcademicYearBeginDay, DateTime.Now.Year.ToString()));
                                            if (ruleInput.prospectData.HSGraduationYear.Value > SchoolHighSchoolGradYear(rd.EntityValue, academicYearResetsOn))
                                            {
                                                program.BaseRuleType = rd.BaseRuleType;
                                                program.RuleId = rd.RuleId;
                                                program.RuleName = rd.RuleName;

                                                removedPrograms.Add(program);
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            output = removedPrograms;
        }

        private int SchoolHighSchoolGradYear(decimal? entityValue, DateTime AcademicYearMonthDay)
        {
            int highestYearAccepted = 0;
            int subtractionVal = Convert.ToInt32(entityValue);
            DateTime currentServerMonthAndDay = DateTime.Today;

            if (currentServerMonthAndDay < AcademicYearMonthDay)
            {
                highestYearAccepted = DateTime.Today.Year - 1 - subtractionVal;
            }
            else
            {
                highestYearAccepted = DateTime.Today.Year - subtractionVal;
            }
            return highestYearAccepted;
        }

    }
}
