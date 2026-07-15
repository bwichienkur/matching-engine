using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ComponentTemplateService : IComponentTemplateService
    {
        private readonly IConfigurationService _configurationService;
        private readonly string _componentTemplatesSectionName = "componentTemplates";

        public ComponentTemplateService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public string SchoolPickerMatchComponentTemplateKey => "SchoolPickerMatch";
        public string FailedMatchReplacementComponentTemplateKey => "FailedMatchReplacement";

        public string SchoolPickerMatchComponentTemplatePath
        {
            get
            {
                Dictionary<string, string> componentTemplatePaths = GetAllComponentTemplates();
                componentTemplatePaths.TryGetValue(SchoolPickerMatchComponentTemplateKey, out string schoolPickerMatchTemplatePath);
                return schoolPickerMatchTemplatePath;
            }
        }

        public string FailedMatchReplacementComponentTemplatePath
        {
            get
            {
                Dictionary<string, string> componentTemplatePaths = GetAllComponentTemplates();
                componentTemplatePaths.TryGetValue(FailedMatchReplacementComponentTemplateKey, out string failedMatchReplacementTemplatePath);
                return failedMatchReplacementTemplatePath;
            }
        }

        public Dictionary<string, string> GetAllComponentTemplates()
        {
            var result = new Dictionary<string, string>();

            Dictionary<string, string> componentTemplateSettings = GetComponentTemplatesPathSettings();

            string componentDirectoryPathKey = "ComponentDirectory";
            if (componentTemplateSettings.TryGetValue(componentDirectoryPathKey, out string directory))
            {
                componentTemplateSettings.Remove(componentDirectoryPathKey);
                foreach (var componentTemplateSetting in componentTemplateSettings)
                {
                    result[componentTemplateSetting.Key] = directory + componentTemplateSetting.Value;
                }
            }

            return result;
        }

        private Dictionary<string, string> GetComponentTemplatesPathSettings()
        {
            var result = new Dictionary<string, string>();

            var settings = _configurationService.GetSettingsSection(_componentTemplatesSectionName);

            for (int i = 0; i < settings?.Count; i++)
            {
                string templateKey = settings.AllKeys[i];
                string templatePath = settings[i];
                result[templateKey] = templatePath;
            }

            return result;
        }
    }
}
