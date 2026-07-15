namespace EDDY.IS.Vendor.Entities
{
    public class VendorCampaign
    {
        private long campaignId;
        private System.Guid trackId;
        private string campaignName;
        private string campaignStatus;
        private bool apiRateExceeded;
        private bool isEnabled;
        private bool isHostAndPost;
        private bool isCallCenter;
        private bool isAPIDirectory;
        private bool isSourceRequired;
        private bool isProspect;
        private int applicationId;
        private int prospectFlowTypeId;
        private int prospectStatusId;
        private int subChannelId;
        private int vendorId;
        private int campaignTypeId;
        private int channelId;
        private bool allowRevShareRPL;
        private bool calculateRevShareByERPL;
        private int? revenueSharePercentage;
        private bool validateTCPA;
        public long CampaignId { get { return campaignId; } set { campaignId = value; } }
        public System.Guid TrackId { get { return trackId; } set { trackId = value; } }
        public string CampaignName { get { return campaignName; } set { campaignName = value; } }
        public string CampaignStatus { get { return campaignStatus; } set { campaignStatus = value; } }
        public bool APIRateExceeded { get { return apiRateExceeded; } set { apiRateExceeded = value; } }

        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }

        public bool IsCallCenter { get { return isCallCenter; } set { isCallCenter = value; } }

        public bool IsHostAndPost { get { return isHostAndPost; } set { isHostAndPost = value; } }

        public bool IsAPIDirectory { get { return isAPIDirectory; } set { isAPIDirectory = value; } }

        public int ApplicationId { get { return applicationId; } set { applicationId = value; } }

        public int ProspectFlowTypeId { get { return prospectFlowTypeId; } set { prospectFlowTypeId = value; } }

        public int ProspectStatusId { get { return prospectStatusId; } set { prospectStatusId = value; } }

        public bool IsProspect { get { return isProspect; } set { isProspect = value; } }

        public bool IsSourceRequired { get { return isSourceRequired; } set { isSourceRequired = value; } }
        
        public int SubChannelId { get { return subChannelId; } set { subChannelId = value; } }

        public int VendorId { get { return vendorId; } set { vendorId = value; } }
        public int CampaignTypeId { get { return campaignTypeId; } set { campaignTypeId = value; } }

        public int ChannelId { get { return channelId; } set { channelId = value; } }

        public bool AllowRevShareRPL { get { return allowRevShareRPL; } set { allowRevShareRPL = value; } }
        public bool CalculateRevShareByERPL { get { return calculateRevShareByERPL; } set { calculateRevShareByERPL = value; } }
        public int? RevenueSharePercentage { get { return revenueSharePercentage; } set { revenueSharePercentage = value; } }

        public bool ValidateTCPA { get { return validateTCPA; } set { validateTCPA = value; } }
    }
}
