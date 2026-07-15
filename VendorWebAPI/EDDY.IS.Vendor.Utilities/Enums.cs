namespace EDDY.IS.Vendor.Utilities
{
    public enum CampaignType
    {
        Standard = 1,
        QDF,
        APIorDirectory,
        CallCenter,
        WarmTransfer
    }
    public enum Channel
    {
        CallCenterPartners = 15,
        OnlinePartners = 18
    }
    public enum SubChannel
    {
        CallCenterPartners = 8,
        OnlinePartners = 12
    }

    public enum IsApplication
    {
        eLDrupal = 1,
        EMDDrupal = 2,
        ExpressDirectories = 3,
        FormsEngine = 4,
        MatchingEngine = 5,
        VendorAPI = 6,
        HostAndPost = 7,
        PixelService = 8,
        Apollo = 9,
        TDC = 10,
        CE = 11,
        SC = 12,
        LandingPages = 13,
        ProspectService = 14,
        Tracking = 15,
        LeadService = 16,
        ABEAService = 17,
        Leadping = 18,
        LeadScoring = 19
    }

    public enum NorthAmericanCountry
    {
        UnitedStates = 1,
        Canada = 2
    }

    public enum AppSourceProspect
    {
        Other = 0,
        API = 1
    }

    //public enum Levels
    //{
    //    Level1 = 1,
    //    Level2 = 2
    //}
}
