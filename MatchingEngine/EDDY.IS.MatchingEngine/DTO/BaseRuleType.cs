using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public enum BaseRuleType
    {
        GPA = 1,
        HSGradYearMax = 2,
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
        Spam = 501,
        SpamReportingOnly = 502
    }
}
