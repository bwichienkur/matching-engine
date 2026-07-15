using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.DTO;
using System.Web.Mvc;
using EDDY.IS.Util.Collections;
using System.Configuration;


namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class FormEngineModel : BaseModel
    {
        public static FormsEngine FormsEngineService = new FormsEngine();

        public string LeadPingURL { get; set; }

        public string DynamicControlsJavascript { get; set; }

        public JavascriptURLArguments DynamicControlsJavascriptURLArguments { get; set; }

        public string DefaultDDLSelectOption { get; set; }

        public List<int> FeaturedListSingleProgram { get; set; }
        

        private TemplateDTO _Template;

        public TemplateDTO Template
        {
            get
            {
                return _Template;
            }
            set
            {
                _Template = value;
            }
        }

        private HTMLRenderingStrategyDTO _RenderingStrategy;

        public HTMLRenderingStrategyDTO RenderingStrategy
        {
            get
            {
                return _RenderingStrategy;
            }
            set
            {
                _RenderingStrategy = value;
            }
        }

        public List<StandardControlTypeDTO> _ControlTypes;

        public List<StandardControlTypeDTO> ControlTypes 
        { 
            get
            {
                return _ControlTypes;
            }
            set
            {
                _ControlTypes = value;
            }
        }

        public int DefaultTemplateId { get; set; }

        public string Theme { get; set; }
        public Dictionary<int, string> TemplateControlFilters { get; set; }

        public bool Initialize(int TemplateId, string RenderingStrategy, bool IsBeta, bool IsWizard, string Theme)
        {
            //Find template
            Template = FormsEngineService.GetFullTemplate(TemplateId);
            DefaultTemplateId =  FormsEngineService.GetDefaultTemplateId(IsWizard);
            LeadPingURL = ConfigurationManager.AppSettings.Get("LeadPingJavascript");
            TemplateControlFilters = new Dictionary<int, string>();



            if (Template == null)
            {
                throw new Exception(string.Format("Template {0} was not found.", TemplateId));
            }
            else
            {
                //Error message handling based on control types
                ControlTypes = (from steps in Template.TemplateSteps
                                 from sections in steps.TemplateSections
                                 from controls in sections.TemplateControls
                                 select controls.StandardControl.StandardControlType).ToList();

                if (ControlTypes != null)
                {
                    ControlTypes = ControlTypes.DistinctBy(ct => ct.StandardControlTypeId).ToList();
                }
            }


            this.Theme = Theme;

            //Find Rendering Strategy
            HTMLRenderingStrategyDTO Rendering = null;
            
            if (!FormsEngineService.ValidateRenderingStrategy(RenderingStrategy, (FormTemplateTypes)Template.TemplateTypeId, out Rendering))
            {
                throw new Exception(string.Format("RenderingStrategy {0} not found or not assigned to TemplateId={1} TemplateTypeId={2}", RenderingStrategy, TemplateId, Template.TemplateTypeId));
            }
            this.RenderingStrategy = Rendering;
         
            return Template != null
                    && this.RenderingStrategy != null;
        }

        
    }

    [Serializable]
    public class JavascriptURLArguments
    {
        public int TemplateId { get; set; }
        public string RenderingStrategy { get; set; }
        public bool IsBeta { get; set; }
        public bool? IgnoreTemplateCache { get; set; }
    }
}
