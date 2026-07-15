using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IConfigurationService
    {
        string EddyLogoImageSizeSmall { get; }
        string EddyLogoImageSizeMedium { get; }
        string EddyLogoImageSizeLarge { get; }
        string EddyLogoImagePathDomain { get; }
        string EddyLogoImageFileName { get; }
        NameValueCollection GetSettingsSection(string sectionName);
    }
}
