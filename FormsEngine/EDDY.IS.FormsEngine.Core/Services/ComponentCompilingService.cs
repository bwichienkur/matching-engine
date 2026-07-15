using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ComponentCompilingService : IComponentCompilingService
    {
        private readonly IFileReaderService _fileReaderService;
        private readonly IComponentTemplateService _componentTemplateFilePathService;
        private readonly ITemplatingEngineService _templatingEngineService;

        public ComponentCompilingService(IFileReaderService fileReaderService, IComponentTemplateService componentTemplateFilePathService, ITemplatingEngineService templatingEngineService)
        {
            _fileReaderService = fileReaderService;
            _componentTemplateFilePathService = componentTemplateFilePathService;
            _templatingEngineService = templatingEngineService;
        }

        public void CompileAllComponents(string baseDirectoryPath)
        {
            var templates = _componentTemplateFilePathService.GetAllComponentTemplates();
            foreach (var template in templates)
            {
                string templateKey = template.Key;
                string templatePath = $"{baseDirectoryPath}{template.Value}";
                CompileComponent(templateKey, templatePath);
            }
        }

        private void CompileComponent(string templateKey, string templatePath)
        {
            string template = _fileReaderService.ReadAllTextFromFile(templatePath);
            _templatingEngineService.CompileTemplate(template, templateKey);
        }
    }
}
