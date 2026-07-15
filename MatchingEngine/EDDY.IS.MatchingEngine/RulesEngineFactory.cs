using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.Rules;

namespace EDDY.IS.MatchingEngine
{

    internal class RulesEngineFactory
    {
        public static readonly List<Type> RuleList = new List<Type>() {
                                                                        typeof(Rules.Campaign),
                                                                        typeof(Rules.CRSchedule),
                                                                        typeof(Rules.LeadCap),
                                                                        typeof(Rules.Country),
                                                                        typeof(Rules.State),
                                                                        typeof(Rules.ZipCode),
                                                                        typeof(Rules.Age),
                                                                        typeof(Rules.EducationLevel),
                                                                        typeof(Rules.HighSchoolGradYear),
                                                                        //typeof(Rules.HighSchoolGradYearDynamic),
                                                                        typeof(Rules.IsMilitary),
                                                                        typeof(Rules.USCitizen),
                                                                        typeof(Rules.Custom_KVLookup),
                                                                        typeof(Rules.GPA),
                                                                        typeof(Rules.YearsWorkExperience),
                                                                        typeof(Rules.YearsTeachingExperience),
                                                                        typeof(Rules.TemplateAssigned),
                                                                        typeof(Rules.HoursOfOperation),
                                                                        typeof(Rules.WarmTransferDelay),
                                                                        typeof(Rules.CampusDistanceForSM),
                                                                        typeof(Rules.ProgramLevelBasedOnEducationLevelForSM),
                                                                        //typeof(Rules.EMSDefaultDuplicate),
                                                                        typeof(Rules.LenexaAgencyDuplicateByEmail),
                                                                        typeof(Rules.JournayaLeadId),
                                                                        typeof(Rules.AgentDisallowedList),
                                                                        typeof(Rules.SourceCode),
                                                                        typeof(Rules.MilitaryNonCivilian),
                                                                        typeof(Rules.CRChannelCap),
                                                                        typeof(Rules.SpamCheck)
        };

        public List<Type> ClientRelationRuleList { get; private set; }
        //public List<Type> ClientRelationWarmTransferRuleList { get; private set; }
        public List<Type> ClientCampusRelationRuleList { get; private set; }
        public List<Type> ProgramProductRuleList { get; private set; }
        public List<Type> PsiRuleList { get; private set; }
        public List<Type> ProgramRuleList { get; private set; }

        public RulesEngineFactory()
        {
            ClientRelationRuleList = new List<Type>();
            //ClientRelationWarmTransferRuleList = new List<Type>();
            ClientCampusRelationRuleList = new List<Type>();
            ProgramProductRuleList = new List<Type>();
            ProgramRuleList = new List<Type>();
            PsiRuleList = new List<Type>();

            foreach (Type t in RuleList)
            {
                if (HasEntityProcessingSet(t, EntityProcessing.ClientRelationship))
                    ClientRelationRuleList.Add(t);

                //if (HasEntityProcessingSet(t, EntityProcessing.ClientRelationship))
                //    ClientRelationWarmTransferRuleList.Add(t);

                if (HasEntityProcessingSet(t, EntityProcessing.ClientCampusRelationship))
                    ClientCampusRelationRuleList.Add(t);

                if (HasEntityProcessingSet(t, EntityProcessing.Program))
                    ProgramRuleList.Add(t);

                if (HasEntityProcessingSet(t, EntityProcessing.ProgramProduct))
                    ProgramProductRuleList.Add(t);

                if (HasEntityProcessingSet(t, EntityProcessing.PSI))
                    PsiRuleList.Add(t);
            }
        }

        public static LeadPingRuleResult ProcessLingPingRule(List<LeadPingService.InstitutionConfig> institutions,
                                                                   RuleInput ruleInput, PerformanceLog pLog)
        {
            if (HasRequiredAttributes(typeof(LeadPing), ruleInput.RuleAttributes) && HasRequiredInput(typeof(LeadPing), ruleInput))
            {
                LeadPing lp = new LeadPing(ruleInput);
                return lp.ExecuteLeadPing(institutions, pLog);
            }

            return null;
        }
                
        public static bool CanProcessLeadPingRule(RuleInput ruleInput)
        {
            if (HasRequiredAttributes(typeof(LeadPing), ruleInput.RuleAttributes) && HasRequiredInput(typeof(LeadPing), ruleInput))
                return true;
            else
                return false;
        }     
                                  
        public static void ProcessCRRule(List<ClientRelationshipProductRuleInput> input,
                                         out List<ClientRelationshipProductRuleInput> output,
                                         Type type,
                                         RuleInput ruleInput)
        {
            if (HasRequiredAttributes(type, ruleInput.RuleAttributes) && HasRequiredInput(type, ruleInput))
            {
                ICRProductRule rule = (ICRProductRule)Activator.CreateInstance(type, ruleInput);

                rule.ExecuteRule(input, out output);
            }
            else
                output = new List<ClientRelationshipProductRuleInput>();
        }

        public static void ProcessCRCampusRule(List<ClientRelationshipCampusProductRuleInput> input,
                                               out List<ClientRelationshipCampusProductRuleInput> output,
                                               Type type,
                                               RuleInput ruleInput)
        {
            if (HasRequiredAttributes(type, ruleInput.RuleAttributes) && HasRequiredInput(type, ruleInput))
            {
                ICRCampusProductRule rule = (ICRCampusProductRule)Activator.CreateInstance(type, ruleInput);

                rule.ExecuteRule(input, out output);
            }
            else
                output = new List<ClientRelationshipCampusProductRuleInput>();
        }

        public static void ProcessPsiRule(List<PSIRuleInput> input,
                                          out List<PSIRuleInput> output,
                                          Type type,
                                          RuleInput ruleInput)
        {
            if (HasRequiredAttributes(type, ruleInput.RuleAttributes) && HasRequiredInput(type, ruleInput))
            {
                IPsiRule rule = (IPsiRule)Activator.CreateInstance(type, ruleInput);
                rule.ExecuteRule(input, out output);
            }
            else
                output = new List<PSIRuleInput>();
        }

        public static void ProcessProgramProductRule(Dictionary<int, ProgramProductRuleInput> input,
                                                     out List<ProgramProductRuleInput> output,
                                                     Type type,
                                                     RuleInput ruleInput)
        {
            if (HasRequiredAttributes(type, ruleInput.RuleAttributes) && HasRequiredInput(type, ruleInput))
            {
                ICRProgramProductRule rule = (ICRProgramProductRule)Activator.CreateInstance(type, ruleInput);

                rule.ExecuteRule(input, out output);
            }
            else
                output = new List<ProgramProductRuleInput>();
        }

        public static void ProcessProgramRule(List<ProgramRuleInput> input,
                                              out List<ProgramRuleInput> output,
                                              Type type,
                                              RuleInput ruleInput)
        {
            if (HasRequiredAttributes(type, ruleInput.RuleAttributes) && HasRequiredInput(type, ruleInput))
            {
                IProgramRule rule = (IProgramRule)Activator.CreateInstance(type, ruleInput);

                rule.ExecuteRule(input, out output);
            }
            else
                output = new List<ProgramRuleInput>();
        }

        public static bool HasEntityProcessingSet(Type ruleType, EntityProcessing et)
        {
            foreach (object attr in ruleType.GetCustomAttributes(true))
            {
                if (attr is MatchingRuleAttributes)
                {
                    MatchingRuleAttributes atr = (MatchingRuleAttributes)attr;

                    if (atr.EntityProcessingTypes != null)
                    {
                        foreach (EntityProcessing ep in atr.EntityProcessingTypes)
                        {
                            if (ep == et)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool HasRequiredAttributes(Type ruleType, Dictionary<RuleAttribute, bool> attributes)
        {
            foreach (object attr in ruleType.GetCustomAttributes(true))
            {
                if (attr is MatchingRuleAttributes)
                {
                    MatchingRuleAttributes atr = (MatchingRuleAttributes)attr;

                    if (atr.RequiredAttributes != null)
                    {
                        foreach (RuleAttribute ra in atr.RequiredAttributes)
                        {
                            bool val = false;
                            if (attributes.TryGetValue(ra, out val))
                            {
                                if (val == false)
                                    return false;
                            }
                            else
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool HasRequiredInput(Type ruleType, RuleInput ri)
        {
            foreach (object attr in ruleType.GetCustomAttributes(true))
            {
                if (attr is MatchingRuleAttributes)
                {
                    MatchingRuleAttributes atr = (MatchingRuleAttributes)attr;

                    if (atr.InputsRequired != null)
                    {
                        foreach (InputRequired ir in atr.InputsRequired)
                        {
                            Debug.WriteLine(ir);
                            switch (ir)
                            {
                                case InputRequired.Age:
                                    if (!ri.prospectData.Age.HasValue)
                                        return false;
                                    break;
                                case InputRequired.Country:
                                    if (!ri.prospectData.CountryId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.EducationLevel:
                                    if (!ri.prospectData.EducationLevelId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.GPA:
                                    if (!ri.prospectData.GPAKeyValueId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.HSGraduationYear:
                                    if (!ri.prospectData.HSGraduationYear.HasValue)
                                        return false;
                                    break;
                                case InputRequired.USCitizen:
                                    if (!ri.prospectData.IsCitizen.HasValue)
                                        return false;
                                    break;
                                case InputRequired.IsMilitary:
                                    if (!ri.prospectData.IsMilitary.HasValue)
                                        return false;
                                    break;
                                case InputRequired.PostalCode:
                                    if (String.IsNullOrEmpty(ri.prospectData.PostalCode))
                                        return false;
                                    break;
                                case InputRequired.State:
                                    if (!ri.prospectData.StateId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.YearsWorkExperience:
                                    if (!ri.prospectData.YearsWorkExperienceKeyValueId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.KVCode:
                                    if (ri.prospectData.KVCodeData == null ||
                                        ri.prospectData.KVCodeData.Count() == 0)
                                        return false;
                                    break;
                                case InputRequired.YearsTeachingExperience:
                                    if (!ri.prospectData.YearsTeachingExperienceKeyValueId.HasValue)
                                        return false;
                                    break;
                                case InputRequired.City:
                                    if (String.IsNullOrEmpty(ri.prospectData.City))
                                        return false;
                                    break;
                                case InputRequired.Email:
                                    if (String.IsNullOrEmpty(ri.prospectData.Email))
                                        return false;
                                    break;
                                case InputRequired.FirstName:
                                    if (String.IsNullOrEmpty(ri.prospectData.FirstName))
                                        return false;
                                    break;
                                case InputRequired.LastName:
                                    if (String.IsNullOrEmpty(ri.prospectData.LastName))
                                        return false;
                                    break;
                                case InputRequired.Phone1:
                                    if (String.IsNullOrEmpty(ri.prospectData.Phone1))
                                        return false;
                                    break;
                                case InputRequired.Phone2:
                                    if (String.IsNullOrEmpty(ri.prospectData.Phone2))
                                        return false;
                                    break;
                                case InputRequired.StreetAddress:
                                    if (String.IsNullOrEmpty(ri.prospectData.StreetAddress))
                                        return false;
                                    break;
                                case InputRequired.MilitaryStatus:
                                    if (!ri.prospectData.MilitaryStatusId.HasValue || ri.prospectData.MilitaryStatusId.Value == 0)
                                        return false;
                                    break;
                                case InputRequired.ExternalLeadId:
                                    if (String.IsNullOrEmpty(ri.prospectData.Email)) //if we have email we should have external leadid (journaya)
                                        return false;
                                    break;
                                case InputRequired.User:
                                    if (!ri.UserID.HasValue)
                                        return false;
                                    break;
                            }
                        }
                    }
                }
            }
            return true;
        }


        public static void ProcessClickRule(List<ClickRuleInput> input,
                                         out List<ClickRuleInput> output)
        {
            IClickRule rule = (IClickRule)Activator.CreateInstance(typeof(Rules.LeadCap), new RuleInput());

            rule.ExecuteRule(input, out output);
        }

       
    }

}
