using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Logging
{
    public static class MatchResponseLoggingSettings
    {
        public static bool LogRemovalReasons
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingRemovalReasons"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingRemovalReasons"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }
        public static bool LogAsJson
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingAsJson"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingAsJson"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public static bool LogMatchResponse
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingEnabled"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingEnabled"] == "1")
                        return true;
                    else
                        return false;
                }

                return true;
            }
        }

        public static bool LogRequestInput
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingRequestInput"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingRequestInput"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }
        public static bool LogAvailablePrograms
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingAvailable"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingAvailable"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public static bool LogDisplayedEntities
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingDisplayed"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingDisplayed"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public static bool LogSearches
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingSearch"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingSearch"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public static bool LogDisplayedFactorScores
        {
            get
            {
                if (ConfigurationManager.AppSettings["MatchResponseLoggingDisplayedFactorScores"] != null)
                {
                    if (ConfigurationManager.AppSettings["MatchResponseLoggingDisplayedFactorScores"] == "1")
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public static int? LogRandomDisplayedPercentage
        {
            get
            {
                int? randomDisplayPercentage = null;
                
                if (ConfigurationManager.AppSettings["MatchResponseLoggingRandomDisplayedPercentage"] != null)
                {
                    int percentage;

                    if(int.TryParse(ConfigurationManager.AppSettings["MatchResponseLoggingRandomDisplayedPercentage"], out percentage))
                    {
                        if (percentage > 0 && percentage < 100)
                            randomDisplayPercentage = percentage;
                    }
                }

                return randomDisplayPercentage;
            }
        }
    }
}
