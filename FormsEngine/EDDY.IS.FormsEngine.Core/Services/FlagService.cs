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
    public class FlagService: IFlagService
    {
        private readonly string _featureFlagSectionName = "featureFlags";

        private readonly IConfigurationService _configurationService;

        public FlagService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public bool IsFlagEnabled(string flagName)
        {
            bool flagIsEnabled = false;

            if (!string.IsNullOrWhiteSpace(flagName))
            {
                Dictionary<string, bool> flags = GetAllFlags();
                flags.TryGetValue(flagName, out flagIsEnabled);
            }
            
            return flagIsEnabled;
        }

        public Dictionary<string, bool> GetAllFlags()
        {
            var flags = new Dictionary<string, bool>();

            NameValueCollection featureFlagSection = _configurationService.GetSettingsSection(_featureFlagSectionName);

            for (int i = 0; i < featureFlagSection?.Count; i++)
            {
                bool.TryParse(featureFlagSection[i], out bool flagValue);
                flags.Add(featureFlagSection.AllKeys[i], flagValue);
            }

            return flags;
        }
    }
}
