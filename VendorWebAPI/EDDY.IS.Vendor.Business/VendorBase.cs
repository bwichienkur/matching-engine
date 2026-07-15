using EDDY.IS.Vendor.DataAccess;

namespace EDDY.IS.Vendor.Business
{
    public class VendorBase
    {
        protected MatchingServiceDAO matchingServiceDAO = new MatchingServiceDAO();
        protected FormsServiceDAO formsServiceDAO = new FormsServiceDAO();
        protected ProspectServiceDAO prospectServiceDAO = new ProspectServiceDAO();
        protected GpFive9ServiceDAO gpFive9ServiceDAO = new GpFive9ServiceDAO();
        protected TimezoneServiceDAO timezoneServiceDAO = new TimezoneServiceDAO();
        protected DataExchangeServiceDAO dataExchangeServiceDAO = new DataExchangeServiceDAO();

        public void LoadSupportingCache()
        {
            VendorBaseDAO vendorBase = new VendorBaseDAO();
            vendorBase.LoadSupportingCache();
        }
      
    }
}
