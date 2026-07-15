using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    public class ExpressConsentCheckDTO
    {
        public bool ExpressConsentOn { get; set; }
        public bool ShowExpressConsent { get; set; }
        public List<string> MobilePhones { get; set; }
        public string Version { get; set; }

        public ExpressConsentCheckDTO()
        {
            this.ExpressConsentOn = (ConfigurationManager.AppSettings.Get("ExpressConsentOn") != null && ConfigurationManager.AppSettings.Get("ExpressConsentOn").ToLower() == "true") ? true : false;
            this.ShowExpressConsent = false;
            this.MobilePhones = new List<string>();
            // in the future this will probably need be able to be changed by Optimizely
            this.Version = (ConfigurationManager.AppSettings.Get("ExpressConsentVersionDefault") != null) ? ConfigurationManager.AppSettings.Get("ExpressConsentVersionDefault").ToUpper() : "B";
        }
    }
}