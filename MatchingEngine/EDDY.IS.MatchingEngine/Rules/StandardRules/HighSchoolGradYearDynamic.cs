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
    public class HighSchoolGradYearDynamic : Rule, ICRProgramProductRule
    {
        public HighSchoolGradYearDynamic(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
            BaseRuleDefinition brMax = null;
            BaseRuleDefinition brMin = null;

            var brMaxCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);
            var brMinCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            if(brMaxCache != null && brMinCache != null )
            {
                
                brMax = brMaxCache.ContainsKey(BaseRuleDefinitionType.DynamicHSGradYearMax) ? brMaxCache[BaseRuleDefinitionType.DynamicHSGradYearMax] : null;
                brMin = brMinCache.ContainsKey(BaseRuleDefinitionType.DynamicHSGradYearMin) ? brMinCache[BaseRuleDefinitionType.DynamicHSGradYearMin] : null;            

                Dictionary<int, InstitutionContent> institutionContentDict = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, InstitutionContent>>(MatchingCacheItem.InstitutionContent);

                DateTime academicYearResetsOn;                

                foreach (ProgramProductRuleInput program in input.Values)
                {
                    if (brMax != null && brMax.ProgramProductAssignments.ContainsKey(program.Key))
                    {
                        List<RuleDefinition> ruleDefinitions = brMax.ProgramProductAssignments[program.Key];

                        if (ruleDefinitions.Count > 0)
                        {
                            RuleDefinition rd = ruleDefinitions[0];

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

                    if (brMin != null && brMin.ProgramProductAssignments.ContainsKey(program.Key))
                    {
                        List<RuleDefinition> ruleDefinitions = brMin.ProgramProductAssignments[program.Key];

                        if (ruleDefinitions.Count > 0)
                        {
                            RuleDefinition rd = ruleDefinitions[0];

                            if (institutionContentDict.ContainsKey(program.InstitutionId))
                            {
                                InstitutionContent ic = institutionContentDict[program.InstitutionId];

                                if (!String.IsNullOrEmpty(ic.AcademicYearBeginDay) && !String.IsNullOrEmpty(ic.AcademicYearBeginMonth))
                                {
                                    academicYearResetsOn = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ic.AcademicYearBeginMonth, ic.AcademicYearBeginDay, DateTime.Now.Year.ToString()));

                                    if (ruleInput.prospectData.HSGraduationYear.Value < SchoolHighSchoolGradYear(rd.EntityValue, academicYearResetsOn))
                                    {
                                        program.BaseRuleType = rd.BaseRuleType;
                                        program.RuleId = rd.RuleId;
                                        program.RuleName = rd.RuleName;
                                        removedPrograms.Add(program);
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
            int returnVal = 0;
            int subtractionVal = Convert.ToInt32(entityValue);
            DateTime currentServerMonthAndDay = DateTime.Today;

            if (currentServerMonthAndDay < AcademicYearMonthDay)
            {
                returnVal = DateTime.Today.Year - 1 - subtractionVal;
            }
            else
            {
                returnVal = DateTime.Today.Year - subtractionVal;
            }
            return returnVal;
        }
    }
}
