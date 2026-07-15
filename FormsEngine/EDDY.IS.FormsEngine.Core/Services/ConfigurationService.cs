using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string EddyLogoImageSizeSmall => ConfigurationManager.AppSettings.Get("EddyLogoImageSizeSmall");

        public string EddyLogoImageSizeMedium => ConfigurationManager.AppSettings.Get("EddyLogoImageSizeMedium");

        public string EddyLogoImageSizeLarge => ConfigurationManager.AppSettings.Get("EddyLogoImageSizeLarge");

        public string EddyLogoImagePathDomain => ConfigurationManager.AppSettings.Get("EddyLogoImagePathDomain");

        public string EddyLogoImageFileName => ConfigurationManager.AppSettings.Get("EddyLogoImageFileName");

        public NameValueCollection GetSettingsSection(string sectionName)
        {
            return (NameValueCollection)ConfigurationManager.GetSection(sectionName);
        }
    }
}
