using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class BaseRuleDefinition
    {
        public BaseRuleDefinitionType RuleType { get; set; }
        public List<RuleDefinition> Rules { get; private set; }
        public Dictionary<int, List<RuleDefinition>> ProgramProductAssignments { get; private set; }

        public BaseRuleDefinition()
        {
            Rules = new List<RuleDefinition>();
            ProgramProductAssignments = new Dictionary<int, List<RuleDefinition>>();
        }
    }

    public class RuleDefinition
    {
        public BaseRuleDefinitionType BaseRuleType { get; set; }
        public int RuleId { get; set; }
        public string RuleName { get; set; }
        public decimal? EntityValue { get; set; }
        public string StandardControlCode { get; set; }
        public List<int> KeyValueCodeIds { get; private set; }
        public bool IsStatic { get; set; }
        public bool IsLowerBound { get; set; }
        public bool IsUpperBound { get; set; }
        public bool UtilizeAcademicYear { get; set; }

        public RuleDefinition()
        {
            KeyValueCodeIds = new List<int>();
        }
    }

    public enum BaseRuleDefinitionType
    {
        GPA = 1,
        HSGradYear = 2,
        YearsWorkExperience = 3,
        YearsTeachingExperience = 4,
        MinimumAge = 5,
        IsMilitary = 6,
        Custom_KVLookup = 7,
        HSGradYearMin = 8,
        USCitizen = 9,
        MaximumAge = 10,
        MilitaryNonCivilian = 11,
        LeadCap = 100,
        Minimum_Education_Level = 101,
        Geographic_Restriction = 102,
        TemplateAssigned = 103,
        ExternalDuplicate = 104,
        InternalDuplicate = 105,
        LeadScore = 106,
        CampaignRestriction = 107,
        WT_HoursOfOperation = 108,
        WT_TimeDelay = 109,
        DynamicHSGradYearMax = 110,
        DynamicHSGradYearMin = 111,
        CRSchedule = 112,
        CampusDistanceSM = 113,
        InvalidProgramLevelForSM = 114,
        CampaignInactive = 200,
        ProgramNotAvailable = 201,
        CampaignCapReached = 202,
        LeadScoringMinimumTierLevel = 203,
        LeadScoreReservation = 204,
        CampaignInvalid = 405,
        CurrentDevice = 406,
        AgentIdMissing = 407,
        SourceCodeMissing = 408,
        AgentDisallowedWT = 409,
        LeadScorePassed = 500,
        Spam = 501,
        SpamReportingOnly = 502
    }

    public enum BaseRuleDefinitionLevel { 
        ProgramLevel = 1
    }
}
