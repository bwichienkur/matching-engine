using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    public class EddyLogosDTO
    {
        public string EddyLogoImageSizeSmall { get; set; }
        public string EddyLogoImageSizeMedium { get; set; }
        public string EddyLogoImageSizeLarge { get; set; }
        public string EddyLogoImagePathDomain { get; set; }

        public string EddyLogoImageFileName { get; set; }

        public string EddyLogoImagePathInstitution { get; set; }
   

        public EddyLogosDTO ()
	    {
            this.EddyLogoImageSizeSmall = ConfigurationManager.AppSettings.Get("EddyLogoImageSizeSmall");
            this.EddyLogoImageSizeMedium = ConfigurationManager.AppSettings.Get("EddyLogoImageSizeMedium");
            this.EddyLogoImageSizeLarge = ConfigurationManager.AppSettings.Get("EddyLogoImageSizeLarge");
            this.EddyLogoImagePathDomain = ConfigurationManager.AppSettings.Get("EddyLogoImagePathDomain");
            this.EddyLogoImageFileName = ConfigurationManager.AppSettings.Get("EddyLogoImageFileName");
            this.EddyLogoImagePathInstitution = ConfigurationManager.AppSettings.Get("EddyLogoImagePathInstitution");
         
        }
    }
}
