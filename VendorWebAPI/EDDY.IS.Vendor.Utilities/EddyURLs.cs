using System;

namespace EDDY.IS.Vendor.Utilities
{
    public static class EddyURLs
    {

       

        public static string GetLogoURL(string campusLogoURL, string institutionLogoURL)
        {
            return !string.IsNullOrEmpty(campusLogoURL) ? EDDY.IS.Common.Utilities.NoNull.NoNull.AppSettings.String("LogoBaseURL") + campusLogoURL.Replace("{FILENAME}", string.Format(EDDY.IS.Common.Utilities.NoNull.NoNull.AppSettings.String("EddyLogoImageFileName"), "240x80.gif"))
                : EDDY.IS.Common.Utilities.NoNull.NoNull.AppSettings.String("LogoBaseURL") + institutionLogoURL.Replace("{FILENAME}", string.Format(EDDY.IS.Common.Utilities.NoNull.NoNull.AppSettings.String("EddyLogoImageFileName"), "240x80.gif"));
        }

        public static string GetDirectoryURL(Int32 applicationID, int CampaignTypeID, long campaignID, string type, string baseURL, string directoryName)
        {
            string dirUrl = string.Empty;

            if (CampaignTypeID == (int)CampaignType.APIorDirectory || CampaignTypeID == (int)CampaignType.WarmTransfer)
            {
                if (!string.IsNullOrEmpty(baseURL) && !string.IsNullOrEmpty(directoryName))
                {

                    switch (type.ToUpper())
                    {
                        case "INSTITUTION":
                            dirUrl = string.Format("{0}/a/{1}/form", baseURL, directoryName);
                            break;
                        case "PROGRAM":
                            dirUrl = string.Format("{0}/a/{1}/form", baseURL, directoryName);
                            break;
                        case "LEVEL":
                            dirUrl = string.Format("{0}/a/{1}/search/programs?level=", baseURL, directoryName);
                            break;
                        case "CATEGORY":
                            dirUrl = string.Format("{0}/a/{1}/search/programs?category=", baseURL, directoryName);
                            break;
                        case "SUBJECT":
                            dirUrl = string.Format("{0}/a/{1}/search/programs?subject=", baseURL, directoryName);
                            break;
                        case "SPECIALTY":
                            dirUrl = string.Format("{0}/a/{1}/search/programs?specialty=", baseURL, directoryName);
                            break;
                        default:
                            dirUrl = string.Empty;
                            break;
                    }
                }
            }

            return dirUrl;
        }
    }
}
