using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine
{
    public enum TransformType
    {
        Default = 0,
        ButtonList = 1
    }
    public enum LeadPaidStatusType
    {
        Free = 1,
        Fraid = 2,
        Paid = 3,
        PaidClicks = 6
    }

    public enum ProgramOptGroup
    {
        Default,
        Custom,
        None
    }

    public enum MarketingChannels
    {
        ContactCenterServices = 13,
        Display = 14,
        CallCenterPartners = 15,
        DRTV_VideoAds = 16,
        Email = 17,
        OnlinePartners = 18,
        OnLocaton = 19,
        PaidSearch = 20,
        SEO = 21,
        TestDoNoUse = 22,
        CustomCampaign = 23,
        Clicks = 24,
        OverheadCosts = 25,
        SocialMedia = 26
    }

    public enum OpenMailRuleOperators {
        GreaterThan,
        LessThan,
        Equals,
        In,
        NotIn,
        Between
    }

    public enum FormTemplateTypes
    {
        ProgramTemplate = 1,
        WizardTemplate = 2,
        ProgramWizard = 3
    }

    public enum BusinessDivisionType
    {
        ProspectDelivery = 1,
        EMS = 2
    }

    public enum ProspectFlowTypes
    {
        Prospecting = 1,
        Advising = 2,
        Jobs = 9,
        WTTitanium = 15,
        EMS = 17
    }

    public enum EducationLevel
    {
        HaventCompletedHighSchool = 1,
        GED = 2,
        HighSchoolDiploma = 3,
        SomeCollege_1_29_Credits = 4,
        SomeCollege_30_59_Credits = 5,
        SomeCollege_60_89_Credits = 6,
        SomeCollege_90_Credits = 7,
        Associate = 8,
        Bachelor = 9,
        Master = 10,
        Doctorate = 11
    }

    public enum SubmissionTypes
    {
        Full = 1,
        Prospect = 2
    }
}
