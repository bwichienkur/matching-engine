using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Util.Memory;
using EDDY.IS.Util.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{

    public class TemplateDataService
    {
        private const string TEMPLATE_STOREDPROCEDURESSCHEMA = "[FE].";
        private const string TEMPLATETYPE_PROGRAM = "Program Template";
        private const string TEMPLATETYPE_WIZARD = "Wizard Template";
        private const string TEMPLATETYPE_PROGRAMWIZARD = "ProgramWizard";
        private const string QUERY_FEATURED_LIST = " SELECT DISTINCT FL.FeatureId " +
                                                   " FROM FeatureList FL(NOLOCK) " +
                                                   "    JOIN FeatureDetail FD(NOLOCK) ON  FD.FeatureId = FL.FeatureId " +
                                                   " WHERE FL.IsEnabled = 1 " +
                                                   "    AND FD.IsEnabled = 1 " +
                                                   "    AND FL.FeatureTypeId = 3 " +
                                                   "    AND FD.EntityMetaId = 23 " +
                                                   "    AND EMSInstitutionId IS NOT NULL " +
                                                   " GROUP BY FL.FeatureId " +
                                                   " HAVING COUNT(1) = 1 ";
        private readonly List<int> IGNORE_SC_IDS = new List<int> { 1, 7, 11, 33 };


        public Dictionary<string, int> GetStandardControlCodeDictionary()
        {
            Dictionary<string, int> standardControlCodes = FormsEngineCacheProxy.Cache.Get<Dictionary<string, int>>(Constants.STANARDARD_CONTROL_CODE_CACHE_KEY);

            if (standardControlCodes == null)
            {
                standardControlCodes = GetStandardControlCodes();
                FormsEngineCacheProxy.Cache.Set(Constants.STANARDARD_CONTROL_CODE_CACHE_KEY, standardControlCodes);
            }
            return standardControlCodes;
        }

        private Dictionary<string, int> GetStandardControlCodes()
        {
            Dictionary<string, int> standardControlCodes = new Dictionary<string, int>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                var controlCodes = Context.StandardControlCodes;
                foreach (var controlCodeItem in controlCodes)
                {
                    standardControlCodes.Add(controlCodeItem.Code, controlCodeItem.StandardControlCodeId);
                }
            }
            return standardControlCodes;
        }


         

       /// <summary>
       /// Builds template mapping cache dictionaries based on Template mapping view
       /// </summary>
       /// <returns></returns>
        public TemplateMappingCache GetTemplateMappingCache()
        {
            TemplateMappingCache _TemplateMappingCache = null;
            _TemplateMappingCache = new Caching.TemplateMappingCache();
            List<VW_ProgramProductTemplate> TemplateAssignmentList = new List<VW_ProgramProductTemplate>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                TemplateAssignmentList = (from ta in Context.VW_ProgramProductTemplate
                                          select ta).ToList();
            }

            foreach (var item in TemplateAssignmentList)
            {
                //Program Product based dictionary
                if (!_TemplateMappingCache.ProgramProductTemplateDictionary.ContainsKey(item.ProgramProductId))
                {
                    _TemplateMappingCache.ProgramProductTemplateDictionary.Add(item.ProgramProductId, item.TemplateId);
                }

                //Program based dictionary
                if (!_TemplateMappingCache.ProgramTemplateDictionary.ContainsKey(item.ProgramId))
                {
                    _TemplateMappingCache.ProgramTemplateDictionary.Add(item.ProgramId, item.TemplateId);
                }

                //Templates With ProgramsBound
                if (!_TemplateMappingCache.TemplatesWithProgramsBound.Contains(item.TemplateId))
                {
                    _TemplateMappingCache.TemplatesWithProgramsBound.Add(item.TemplateId);
                }

            }

            return _TemplateMappingCache;
         }


        /// <summary>
        /// Template Mapping dependent dictionaries
        /// </summary>
        private TemplateMappingCache TemplateMappingCache
        {
            get
            {
                TemplateMappingCache _TemplateMappingCache = FormsEngineCacheProxy.Cache.Get<TemplateMappingCache>(Constants.TEMPLATEMAPPING_CACHE_KEY);
                if (_TemplateMappingCache == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.TEMPLATEMAPPING_CACHE_KEY, FormsEngineCacheHelper.GetTemplateMappingCache, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    _TemplateMappingCache = FormsEngineCacheProxy.Cache.Get<TemplateMappingCache>(Constants.TEMPLATEMAPPING_CACHE_KEY);
                }
                return _TemplateMappingCache;
            }
        }


        public Dictionary<string, List<string>> GetControlCodeFiltersDictionary()
        {
            Dictionary<string, List<string>> _ControlCodeFiltersDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            List<StandardControlCode> ControlCodes = new List<StandardControlCode>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                ControlCodes = (from c in Context.StandardControlCodes.Include("StandardControlCodeFilters.StandardControlCodeFilterEntity")
                                where c.IsEnabled == true
                                select c).ToList();
            }

            foreach (var ControlCode in ControlCodes)
            {
                string ControlCodeValue = ControlCode.Code;
                List<string> ControlFilters = new List<string>();
                foreach (var Filter in ControlCode.StandardControlCodeFilters)
                {
                    ControlFilters.Add(Filter.StandardControlCodeFilterEntity.Code);
                }
                if (!_ControlCodeFiltersDictionary.ContainsKey(ControlCode.Code))
                {
                    _ControlCodeFiltersDictionary.Add(ControlCode.Code, ControlFilters);
                }

            }

            return _ControlCodeFiltersDictionary;
        }

        private Dictionary<string, List<string>> ControlCodeFiltersDictionary
        {
            get
            {
                Dictionary<string, List<string>> _ControlCodeFiltersDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<string, List<string>>>(Constants.TEMPLATECONTROLCODEFILTER_CACHE_KEY);
                if (_ControlCodeFiltersDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.TEMPLATECONTROLCODEFILTER_CACHE_KEY, FormsEngineCacheHelper.GetControlCodeFiltersDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    _ControlCodeFiltersDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<string, List<string>>>(Constants.TEMPLATECONTROLCODEFILTER_CACHE_KEY);
                }
                return _ControlCodeFiltersDictionary;
            }
        }


        public Dictionary<string, List<ValueList>> GetStandardControlPreDefinedValueList()
        {
            Dictionary<string, List<ValueList>> _StandardControlPreDefinedValueList = new Dictionary<string,List<ValueList>>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                var valueListResult = (from sc in Context.StandardControls.Include("StandardControlCode, PreDefinedValueList")
                          where sc.PreDefinedValueList != null
                          && sc.IsEnabled 
                          && sc.PreDefinedValueList.IsEnabled
                          select  new {
                            PreDefinedValueList = sc.PreDefinedValueList,
                            Code = sc.StandardControlCode.Code,
                            StoredProcedureName = sc.PreDefinedValueList.StoredProcedureName
                        }).ToList();

                foreach(var vl in valueListResult)
                {
                    if(!_StandardControlPreDefinedValueList.ContainsKey(vl.Code))
                    {
                        var result = GetTemplateControlData(TEMPLATE_STOREDPROCEDURESSCHEMA + vl.StoredProcedureName);
                        if(result.Count()>0)
                        {
                            _StandardControlPreDefinedValueList.Add(vl.Code,result);
                                }
                            }
                        }
                    }
            return _StandardControlPreDefinedValueList;
                }

        public Dictionary<string, List<ValueList>> StandardControlPreDefinedValueList
        {
            get
            {
                Dictionary<string, List<ValueList>> _StandardControlPreDefinedValueList = FormsEngineCacheProxy.Cache.Get<Dictionary<string, List<ValueList>>>(Constants.STANDARDCONTROLLIST_CACHE_KEY);

                if(_StandardControlPreDefinedValueList==null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.STANDARDCONTROLLIST_CACHE_KEY, FormsEngineCacheHelper.GetStandardControlPreDefinedValueList, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    _StandardControlPreDefinedValueList = FormsEngineCacheProxy.Cache.Get<Dictionary<string, List<ValueList>>>(Constants.STANDARDCONTROLLIST_CACHE_KEY);
            }

                return _StandardControlPreDefinedValueList;
        }
        }

        public Dictionary<int, List<TemplateControl>> ControlTemplateDictionary
        {
            get
            {
                Dictionary<int, List<TemplateControl>> _TemplateControlDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, List<TemplateControl>>>(Constants.TEMPLATECONTROL_CACHE_KEY);
                if (_TemplateControlDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.TEMPLATECONTROL_CACHE_KEY, FormsEngineCacheHelper.GeControlTemplateDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    _TemplateControlDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, List<TemplateControl>>>(Constants.TEMPLATECONTROL_CACHE_KEY);
                }
                return _TemplateControlDictionary;
            }
        }


        public Dictionary<int, Template> GetTemplateDictionary()
        {
            Dictionary<int, Template> _TemplateDictionary = new Dictionary<int, Template>();
            List<Template> TemplateList = new List<Template>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                    TemplateList = (from t in Context.Templates.Include("TemplateType")
                                    where t.IsEnabled == true
                                    select t).ToList();
            }

            foreach (var item in TemplateList)
            {
                if (!_TemplateDictionary.ContainsKey(item.TemplateId))
                {
                    _TemplateDictionary.Add(item.TemplateId, item);
                }
            }

            return _TemplateDictionary;
        }

        private Dictionary<int, Template> TemplateDictionary
        {
            get
            {
                Dictionary<int, Template> _TemplateDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, Template>>(Constants.BASIC_TEMPLATE_CACHE_KEY);
                if (_TemplateDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.BASIC_TEMPLATE_CACHE_KEY, FormsEngineCacheHelper.GetTemplateDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    _TemplateDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, Template>>(Constants.BASIC_TEMPLATE_CACHE_KEY);
                }
                return _TemplateDictionary;
            }
        }

        /// <summary>
        /// Gets the list of standard control code filters
        /// </summary>
        /// <param name="StandardControlCode"></param>
        /// <returns></returns>
        public List<string> GetStandardControlCodeFilters(string StandardControlCode)
        {
            List<string> Result = new List<string>();

            if (ControlCodeFiltersDictionary.ContainsKey(StandardControlCode))
            {
                Result = ControlCodeFiltersDictionary[StandardControlCode];
            }

            return Result;
        }


        /// <summary>
        /// Gets the template and template type by template id from the templates dictionary (cached)
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public Template GetShallowTemplate(int TemplateId)
        {
            Template Result = null;

            if (TemplateDictionary.ContainsKey(TemplateId))
            {
                Result = TemplateDictionary[TemplateId];
            }

            return Result;
        }

        /// <summary>
        /// Gets the template controls, no error messages included
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public Dictionary<int, List<TemplateControl>> GetControlTemplateDictionary()
        {
            Dictionary<int, List<TemplateControl>> Result = new Dictionary<int, List<TemplateControl>>();
            List<TemplateControl> Controls = null;
            List<int> TemplateKeys =  TemplateDictionary.Keys.ToList();
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Controls = (from control in Context.TemplateControls.Include("StandardControl.StandardControlType")
                    .Include("StandardControl.PreDefinedValueList")
                    .Include("StandardControl.StandardControlDataType")
                    .Include("StandardControl.StandardControlCode")
                    .Include("TemplateControlDefaults")
                            where TemplateKeys.Contains(control.TemplateId)
                orderby control.TemplateId, control.RowSequence
                select control).ToList();
            }


            var GroupedControls = (from c in Controls
                                  group c by c.TemplateId into g
                                  select new 
                                  { 
                                      TemplateId = g.Key,
                                      Templatecontrols = g
                                  }).ToList();

            foreach (var tc in GroupedControls)
            {
                var TemplateControls = tc.Templatecontrols.ToList();
                
                foreach (var control in TemplateControls)
                {
                    if (control.StandardControl.PreDefinedValueList != null && control.StandardControl.PreDefinedValueList.IsEnabled == true && control.StandardControl.StandardControlCode.Code != "Program_Of_Interest")
                    {
                        //Clone StandardControl 
                        control.StandardControl = control.StandardControl.CloneEntity();

                        var StoredProc = control.StandardControl.PreDefinedValueList.StoredProcedureName;
                        var StoredProcResult = GetTemplateControlData(TEMPLATE_STOREDPROCEDURESSCHEMA + StoredProc);
                        //Multiple items default on items
                        if (StoredProcResult.Count() > 0)
                        {
                            if (control.TemplateControlDefaults.Count() > 0)
                            {
                                foreach (var def in control.TemplateControlDefaults)
                                {
                                    ValueList found = StoredProcResult.Find(a => a.Value.ToLower() == def.Text.ToLower());
                                    if (found != null)
                                    {
                                        found.IsDefault = true;
                                    }
                                }
                            }
                            control.StandardControl.PreDefinedValueList.ValueListItems = StoredProcResult;
                        }
                    }
                    //single item default on standardcontrol.defaultvalue
                    else if (control.TemplateControlDefaults.Count() > 0)
                    {
                        //Clone StandardControl 
                        control.StandardControl = control.StandardControl.CloneEntity();
                        control.StandardControl.DefaultValue = control.TemplateControlDefaults.First().Text;
                    }
                }
                Result.Add(tc.TemplateId, TemplateControls);
            }

            return Result;
        }

        /// <summary>
        /// Gets the template controls, no error messages included
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public List<TemplateControl> GetTemplateControlsFromDB(int TemplateId)
        {
            List<TemplateControl> Result = null;
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from control in Context.TemplateControls.Include("StandardControl.StandardControlType")
                    .Include("StandardControl.PreDefinedValueList")
                    .Include("StandardControl.StandardControlDataType")
                    .Include("StandardControl.StandardControlCode")
                    .Include("TemplateControlDefaults")
                    //.Include("StandardControl.StandardControlType.ErrorMessages.ErrorCode")
                    //.Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlType.ErrorMessages.TemplateType")
                    //.Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlValidations.ValidationLibrary.ErrorCode")
                    where control.TemplateId == TemplateId
                    select control).ToList();
            }

            foreach (var control in Result)
            {
                if (control.StandardControl.PreDefinedValueList != null && control.StandardControl.PreDefinedValueList.IsEnabled == true && control.StandardControl.StandardControlCode.Code != "Program_Of_Interest")
                {
                    //Clone StandardControl 
                    control.StandardControl = control.StandardControl.CloneEntity();

                    var StoredProc = control.StandardControl.PreDefinedValueList.StoredProcedureName;
                    var StoredProcResult = GetTemplateControlData(TEMPLATE_STOREDPROCEDURESSCHEMA + StoredProc);
                    //Multiple items default on items
                    if (StoredProcResult.Count() > 0)
                    {
                        if (control.TemplateControlDefaults.Count() > 0)
                        {
                            foreach (var def in control.TemplateControlDefaults)
                            {
                                ValueList found = StoredProcResult.Find(a => a.Value.ToLower() == def.Text.ToLower());
                                if (found != null)
                                {
                                    found.IsDefault = true;
                                }
                            }
                        }
                        control.StandardControl.PreDefinedValueList.ValueListItems = StoredProcResult;
                    }
                }
                //single item default on standardcontrol.defaultvalue
                else if (control.TemplateControlDefaults.Count() > 0)
                {
                    //Clone StandardControl 
                    control.StandardControl = control.StandardControl.CloneEntity();
                    control.StandardControl.DefaultValue = control.TemplateControlDefaults.First().Text;
                }
            }

            return Result;
        }


        /// <summary>
        /// Gets the template and all related entities based on the Prod schema
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public Template GetFullTemplate(int TemplateId)
        {
            Template TemplateResult = null;
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
                {
                    TemplateResult = (from template in Context.Templates.Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlType")
                        .Include("TemplateSteps.TemplateSections.StandardControlGroup")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.PreDefinedValueList")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlDataType")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlCode")
                        //.Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlCode.StandardControlCodeFilters.StandardControlCodeFilterEntity")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.TemplateControlDefaults")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlType.ErrorMessages.ErrorCode")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlType.ErrorMessages.TemplateType")
                        .Include("TemplateSteps.TemplateSections.TemplateControls.StandardControl.StandardControlValidations.ValidationLibrary.ErrorCode")
                        .Include("TemplateType")
                                      where template.IsEnabled == true
                                              && template.TemplateId == TemplateId
                                      select template).FirstOrDefault();
                }

            if (TemplateResult != null)
            {
                bool IsWizardTemplate = TemplateResult.TemplateType.Name == TEMPLATETYPE_WIZARD
                    || TemplateResult.TemplateType.Name == TEMPLATETYPE_PROGRAMWIZARD;

                for (int iStep = TemplateResult.TemplateSteps.Count - 1; iStep >= 0; iStep--)
                {
                    var step = TemplateResult.TemplateSteps.ElementAt(iStep);
                        for (int iSection = step.TemplateSections.Count - 1; iSection >= 0; iSection--)
                        {
                            var section = step.TemplateSections.ElementAt(iSection);
                                for (int iControl = section.TemplateControls.Count - 1; iControl >= 0; iControl--)
                                {
                                    var control = section.TemplateControls.ElementAt(iControl);
                                        for (int iErrorMessage = control.StandardControl.StandardControlType.ErrorMessages.Count - 1; iErrorMessage >= 0; iErrorMessage--)
                                        {
                                            var ErrorMessage = control.StandardControl.StandardControlType.ErrorMessages.ElementAt(iErrorMessage);

                                            if (IsWizardTemplate && ErrorMessage.TemplateType.Name != TEMPLATETYPE_WIZARD)
                                            {
                                                control.StandardControl.StandardControlType.ErrorMessages.Remove(ErrorMessage);
                                            }
                                            else if (!IsWizardTemplate && ErrorMessage.TemplateType.Name == TEMPLATETYPE_WIZARD)
                                            {
                                                control.StandardControl.StandardControlType.ErrorMessages.Remove(ErrorMessage);
                                            }
                                        }

                                        if (control.StandardControl.PreDefinedValueList != null && control.StandardControl.PreDefinedValueList.IsEnabled == true && control.StandardControl.StandardControlCode.Code != "Program_Of_Interest")
                                        {
                                            //Clone StandardControl 
                                            control.StandardControl = control.StandardControl.CloneEntity();

                                            var StoredProc = control.StandardControl.PreDefinedValueList.StoredProcedureName;
                                            var result = GetTemplateControlData(TEMPLATE_STOREDPROCEDURESSCHEMA + StoredProc);
                                            //Multiple items default on items
                                            if (result.Count() > 0)
                                            {
                                                if (control.TemplateControlDefaults.Count() > 0)
                                                {
                                                    foreach (var def in control.TemplateControlDefaults)
                                                    {
                                                        ValueList found = result.Find(a => a.Value.ToLower() == def.Text.ToLower());
                                                        if (found != null)
                                                        {
                                                            found.IsDefault = true;
                                                        }
                                                    }
                                                }
                                                control.StandardControl.PreDefinedValueList.ValueListItems = result;
                                            }
                                        }
                                        //single item default on standardcontrol.defaultvalue
                                        else if (control.TemplateControlDefaults.Count() > 0)
                                        {
                                            //Clone StandardControl 
                                            control.StandardControl = control.StandardControl.CloneEntity();
                                            control.StandardControl.DefaultValue = control.TemplateControlDefaults.First().Text;
                                        }
                        }
                    }
                }

            }
            return TemplateResult;
        }

        public List<ValueList> ValueListCopy(List<ValueList> Input)
        {
            List<ValueList> Result = new List<ValueList>();

            foreach (var value in Input)
            {
                ValueList vl = new ValueList();
                vl.IsDefault = value.IsDefault;
                vl.Key = value.Key;
                vl.Text = value.Text;
                vl.Value = value.Value;
                Result.Add(vl);
            }

            return Result;
        }


        public List<ValueList> GetTemplateControlData(String dbCommand)
        {
            String CommandKey = "GetTemplateControlData.CachedData.dbCommand" + dbCommand;
            List<ValueList> ReturnValue;
            ReturnValue = FormsEngineCacheProxy.Cache.Get<List<ValueList>>(CommandKey);

            if (ReturnValue == null)
            {
                List<PreDefinedListBaseResult> PreDefined = null;
                ReturnValue = new List<ValueList>();
                using (FEEntitiesContainer Context = new FEEntitiesContainer())
                {
                    PreDefined = Context.Database.SqlQuery<PreDefinedListBaseResult>(dbCommand).ToList();
                }

                foreach (var item in PreDefined)
                {
                    ReturnValue.Add(new ValueList() { 
                         Key = item.Key,
                         Value = item.Value,
                         Text = item.Text
                    });
                }

                int GetTemplateControlData_Cache_MinuteTimeOut = 30;
                int.TryParse(ConfigurationManager.AppSettings["GetTemplateControlData_Cache_MinuteTimeOut"], out GetTemplateControlData_Cache_MinuteTimeOut);

                /*
                FormsEngineCacheProxy.Cache.Set(CommandKey,
                    ReturnValue,
                    GetTemplateControlData_Cache_MinuteTimeOut);
                    */
                FormsEngineCacheProxy.Cache.Set(CommandKey,
                ReturnValue);
            }
            return ValueListCopy(ReturnValue);
                                        }



        public List<TemplateControl> GetTemplateControls(int TemplateId)
        {
            List<TemplateControl> Result = new List<TemplateControl>();

            if (ControlTemplateDictionary.ContainsKey(TemplateId))
            {
                Result = ControlTemplateDictionary[TemplateId];
            }

            return Result;
        }

        public List<int> GetProgramTemplatesCoveredByWizardQuestions(int WizardTemplateId)
        {
            List<int> Result = new List<int>();
            string CacheKey = string.Format(Constants.WIZARDTEMPLATE_TEMPLATESCOVERED_KEY, WizardTemplateId);
            Result = FormsEngineCacheProxy.Cache.Get<List<int>>(CacheKey);

            if (Result == null && ControlTemplateDictionary.ContainsKey(WizardTemplateId))
            {
                Result = new List<int>();
                List<TemplateControl> WizardTemplateControls = ControlTemplateDictionary[WizardTemplateId];

                //Program templates with programs bound.
                var ProgramTemplates = (from t in TemplateMappingCache.TemplatesWithProgramsBound
                                        select t).Distinct().ToList();

                //foreach program template
                foreach (int ProgramTemplateId in ProgramTemplates)
                {
                    if (ControlTemplateDictionary.ContainsKey(ProgramTemplateId))
                    {
                        List<TemplateControl> ProgramTemplateControls = ControlTemplateDictionary[ProgramTemplateId];

                        var MissingControls = (from ptc in ProgramTemplateControls
                                               where ptc.StandardControl.StandardControlCode.Code != "UserAgreement"
                                                 && ptc.StandardControl.StandardControlCode.Code != "Program_Of_Interest"
                                                 && ptc.StandardControl.StandardControlCode.Code != "EDDYUserAgreement"
                                                 && ptc.IsRequired == true
                                               select ptc.StandardControl.StandardControlCodeId
                                               ).Except(
                                                from wtc in WizardTemplateControls
                                                select wtc.StandardControl.StandardControlCodeId
                                               ).Count();

                        if (MissingControls == 0 && !Result.Contains(ProgramTemplateId))
                        {
                            Result.Add(ProgramTemplateId);
                        }

                    }
                }

                FormsEngineCacheProxy.Cache.Set(CacheKey, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
            }
            return Result;

        }



        public List<int> GetSystemProgramTemplatesCoveredByQuestions(List<string> StandardControlCodes)
        {
            List<int> Result = new List<int>();
            HashSet<int> MissingControlsFound = new HashSet<int>();

            var SystemProgramTemplates = from t in TemplateDictionary.Values
                                         where t.IsSystemTemplate
                                                && t.TemplateTypeId == (int)FormTemplateTypes.ProgramTemplate
                                         select t;

            foreach (var template in SystemProgramTemplates)
            {
                if(ControlTemplateDictionary.ContainsKey(template.TemplateId))
                {
                    List<TemplateControl> AllControls = ControlTemplateDictionary[template.TemplateId];

                    int MissingControls = (from tc in AllControls
                                           where tc.IsRequired==true
                                           && tc.StandardControl.StandardControlCode.Code != "UserAgreement"
                                           && tc.StandardControl.StandardControlCode.Code != "Program_Of_Interest"
                                           && tc.StandardControl.StandardControlCode.Code != "EDDYUserAgreement"
                                           select tc.StandardControl.StandardControlCode.Code).Except(
                                            from scc in StandardControlCodes
                                            select scc
                                           ).Count();

                    if (MissingControls == 0)
                    {
                        Result.Add(template.TemplateId);
                    }

                }
                
            }
           
            return Result;
        }




        public List<TemplateControl> GetAdditionalTemplateQuestions(int WizardTemplateId, List<int> ProgramTemplates, bool AllowDupes)
        {
            List<TemplateControl> Result = new List<TemplateControl>();
            HashSet<int> MissingControlsFound = new HashSet<int>();
            var SortedTemplates = ProgramTemplates.OrderBy(t => t);
            string MissingTemplateQuestionsKey = string.Format(Constants.TEMPLATE_CONTROLSMISSING_KEY, WizardTemplateId, string.Join("-", SortedTemplates));

            Result = FormsEngineCacheProxy.Cache.Get<List<TemplateControl>>(MissingTemplateQuestionsKey);

            if (Result == null && ControlTemplateDictionary.ContainsKey(WizardTemplateId))
            {
                Result = new List<TemplateControl>();
                List<TemplateControl> WizardTemplateControls = ControlTemplateDictionary[WizardTemplateId];
                bool hasGoogleAddress = WizardTemplateControls.Where(x => x.StandardControl.StandardControlCode.Code == "Google_address").Any();

                //foreach program template
                foreach (int ProgramTemplateId in ProgramTemplates)
                {
                    if (ControlTemplateDictionary.ContainsKey(ProgramTemplateId))
                    {
                        List<TemplateControl> ProgramTemplateControls = ControlTemplateDictionary[ProgramTemplateId];

                        var MissingSCIds = (from ptc in ProgramTemplateControls
                                            select ptc.StandardControl.StandardControlCodeId
                                            ).Except(
                                            from wtc in WizardTemplateControls
                                            select wtc.StandardControl.StandardControlCodeId
                                            );
                        if (hasGoogleAddress)
                            MissingSCIds = MissingSCIds.Except(IGNORE_SC_IDS);
                        var MissingControls = from ptc in ProgramTemplateControls
                                              where MissingSCIds.Contains(ptc.StandardControl.StandardControlCodeId)
                                              select ptc;

                        foreach (var MissingControl in MissingControls)
                        {
                            if (!MissingControlsFound.Contains(MissingControl.StandardControl.StandardControlCodeId)
                                && MissingControl.StandardControl.StandardControlCode.Code != "UserAgreement"
                                && MissingControl.StandardControl.StandardControlCode.Code != "Program_Of_Interest"
                                && MissingControl.StandardControl.StandardControlCode.Code != "EDDYUserAgreement"
                                && MissingControl.IsRequired == true
                               )
                            {
                                if (!AllowDupes)
                                {
                                    MissingControlsFound.Add(MissingControl.StandardControl.StandardControlCodeId);
                                }
                                Result.Add(MissingControl);
                            }
                        }

                    }
                }

                FormsEngineCacheProxy.Cache.Set(MissingTemplateQuestionsKey, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
            }
            return Result;
        }

        public List<TemplateControl> GetAdditionalTemplateQuestionsForProgramMatch(List<int> CoveredTemplateIds, int TemplateId, bool AllowDupes)
        {
            List<TemplateControl> Result = new List<TemplateControl>();
            HashSet<int> MissingControlsFound = new HashSet<int>();
            var SortedTemplates = CoveredTemplateIds.OrderBy(t => t);
            string MissingTemplateQuestionsKey = string.Format(Constants.TEMPLATE_CONTROLS_ENHANCEDPROGMATCH_KEY, TemplateId, string.Join("-", SortedTemplates));

            Result = FormsEngineCacheProxy.Cache.Get<List<TemplateControl>>(MissingTemplateQuestionsKey);

            if (Result == null)
            {
                Result = new List<TemplateControl>();
                if (ControlTemplateDictionary.ContainsKey(TemplateId))
                {
                    List<TemplateControl> UnMatchedTemplateControls = ControlTemplateDictionary[TemplateId];

                    //foreach program template
                    List<int> standardControlCodeIdList = null;
                    foreach (int ProgramTemplateId in CoveredTemplateIds)
                    {
                        if (ControlTemplateDictionary.ContainsKey(ProgramTemplateId))
                        {
                            List<TemplateControl> ProgramTemplateControls = ControlTemplateDictionary[ProgramTemplateId];
                            if (standardControlCodeIdList != null)
                            {
                                standardControlCodeIdList = standardControlCodeIdList.Union(ProgramTemplateControls
                                    .Where(x => x.IsRequired.GetValueOrDefault(false))
                                    .Select(p => p.StandardControl.StandardControlCodeId)).ToList();
                            }
                            else
                            {
                                standardControlCodeIdList = ProgramTemplateControls.Select(p => p.StandardControl.StandardControlCodeId).ToList();
                            }
                        }
                    }

                    var MissingSCIds = (from umtc in UnMatchedTemplateControls
                                        select umtc.StandardControl.StandardControlCodeId
                                        ).Except(standardControlCodeIdList);

                    var MissingControls = from umtc in UnMatchedTemplateControls
                                          where MissingSCIds.Contains(umtc.StandardControl.StandardControlCodeId)
                                          select umtc;

                    foreach (var MissingControl in MissingControls)
                    {
                        if (!MissingControlsFound.Contains(MissingControl.StandardControl.StandardControlCodeId)
                            && MissingControl.StandardControl.StandardControlCode.Code != "UserAgreement"
                            && MissingControl.StandardControl.StandardControlCode.Code != "Program_Of_Interest"
                            && MissingControl.StandardControl.StandardControlCode.Code != "EDDYUserAgreement"
                            && MissingControl.IsRequired == true
                            )
                        {
                            if (!AllowDupes)
                            {
                                MissingControlsFound.Add(MissingControl.StandardControl.StandardControlCodeId);
                            }
                            Result.Add(MissingControl);
                        }
                    }
                    
                    FormsEngineCacheProxy.Cache.Set(MissingTemplateQuestionsKey, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                }
            }
            return Result;
        }

        /// <summary>
        /// Gets the template by programid or default
        /// </summary>
        /// <param name="ProgramId"></param>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public int GetTemplateIdByProgramId(int ProgramId, bool IsWizard)
        {
            int TemplateId;

            if (TemplateMappingCache.ProgramTemplateDictionary.ContainsKey(ProgramId))
            {
                TemplateId = TemplateMappingCache.ProgramTemplateDictionary[ProgramId];
            }
            else 
            {
                TemplateId = GetDefaultTemplateId(IsWizard);
            }

            return TemplateId;
        }

        /// <summary>
        /// Gets the default template based on type
        /// </summary>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public Template GetDefaultTemplate(bool IsWizard)
        {
            Template Result = new Template();

            if (IsWizard)
            {
                var Template = (from t in TemplateDictionary.Values
                                where t.IsDefaultTemplate && t.TemplateType.Name == TEMPLATETYPE_WIZARD
                                select t).FirstOrDefault();

                Result = Template == null ? Result : Template;
            }
            else
            {
                var Template = (from t in TemplateDictionary.Values
                                where t.IsDefaultTemplate && t.TemplateType.Name == TEMPLATETYPE_PROGRAM
                                select t).FirstOrDefault();

                Result = Template == null ? Result : Template;
            }
            return Result;
        }


        /// <summary>
        /// Gets default template id
        /// </summary>
        /// <returns></returns>
        public int GetDefaultTemplateId(bool IsWizard)
        {
            return GetDefaultTemplate(IsWizard).TemplateId;
        }

        /// <summary>
        /// Validates if template is available otherwise returns default template
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public int GetTemplateIdOrDefault(int TemplateId, bool IsWizard, int? ProgramId, int? ProgramProductId)
        {
            int Result;

            // TODO: Add another check for using the international template..
            if (TemplateDictionary.ContainsKey(TemplateId))
            {
                Result = TemplateId;
            }
            else if(ProgramId.HasValue && !IsWizard) 
            {
                Result = GetTemplateIdByProgramId(ProgramId.Value, IsWizard);
            }
            else if (ProgramProductId.HasValue && !IsWizard)
            {
                Result = GetTemplateIdByProgramProductId(ProgramProductId.Value, IsWizard);
            }
            else
            {
                Result = GetDefaultTemplateId(IsWizard);
            }

            return Result;
        }

        /// <summary>
        /// Returns if program product is mapped to template
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <returns></returns>
        public bool IsProgramProductMappedToTemplate(int ProgramProductId)
        {
            return TemplateMappingCache.ProgramProductTemplateDictionary.ContainsKey(ProgramProductId);
        }


        /// <summary>
        /// Gets templateid name for a given programproduct id
        /// </summary>
        /// <param name="ProgramId"></param>
        /// <returns></returns>
        public int GetTemplateIdByProgramProductId(int ProgramProductId, bool IsWizard)
        {
            int TemplateId;
            if (TemplateMappingCache.ProgramProductTemplateDictionary.ContainsKey(ProgramProductId))
            {
                TemplateId = TemplateMappingCache.ProgramProductTemplateDictionary[ProgramProductId];
            }
            else 
            {
                TemplateId = GetDefaultTemplateId(IsWizard);
            }

            return TemplateId;
         }

        /// <summary>
        /// Validates if template exists
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public bool TemplateExists(int TemplateId)
            {
            return TemplateDictionary.ContainsKey(TemplateId);
        }

        /// <summary>
        /// Gets the list of rendering strategies
        /// </summary>
        /// <param name="Wizard"></param>
        /// <returns></returns>
        public List<HTMLRenderingStrategy> GetRenderingStrategies(FormTemplateTypes FormTemplateType)
        {
            List<HTMLRenderingStrategy> Result = new List<HTMLRenderingStrategy>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from re in Context.HTMLRenderingStrategies
                          join ta in Context.HTMLRenderingStrategyAssignments on re.HTMLRenderingStrategyId equals ta.HTMRenderingStrategyId
                          join tt in Context.TemplateTypes on ta.TemplateTypeId equals tt.TemplateTypeId
                          where re.IsEnabled == true
                            && tt.TemplateTypeId == (int)FormTemplateType
                          select re).Distinct().ToList();
            }

            return Result;
        }


        /// <summary>
        /// Shallow list of templates with not related entities other than TemplateType and BusinessUnit
        /// </summary>
        /// <param name="FormTemplateType"></param>
        /// <param name="BusinessUnit"></param>
        /// <returns></returns>
        public List<Template> GetShallowTemplateList(FormTemplateTypes FormTemplateType, BusinessDivisionType? BusinessDivision,int? institutionId = null)
        {
            List<Template> Result = TemplateDictionary.Values.Where(i => i.TemplateType.TemplateTypeId == (int)FormTemplateType).ToList();

            if (BusinessDivision != null)
                Result = Result.Where(t => t.BusinessDivisionId == (int)BusinessDivision).ToList();
            if (institutionId != null && institutionId.HasValue)
                Result = Result.Where(t => t.InstitutionId == (int)institutionId.Value).ToList();
            return Result;
        }

        public Dictionary<int, List<TemplateControl>> GetControlTemplateDictionaryForProgramTemplates(HashSet<int> templateIds)
        {
            Dictionary<int, List<TemplateControl>> programTemplateControls = new Dictionary<int, List<TemplateControl>>();

            foreach (var item in TemplateDictionary.Values)
            {
                if (item.TemplateType.Name == TEMPLATETYPE_PROGRAM && (templateIds == null || templateIds.Count == 0 || templateIds.Contains(item.TemplateId)))
                {
                    if (ControlTemplateDictionary.ContainsKey(item.TemplateId))
                        programTemplateControls.Add(item.TemplateId, ControlTemplateDictionary[item.TemplateId]);
                }
            }

            return programTemplateControls;
        }

        public int GetTemplateApplicationOverrideForApplicationAndPaidStatus(int ApplicationId, int PaidStatusTypeId)
        {
            var template = TemplateApplicationOverrides.Where(t => t.ApplicationId == ApplicationId && t.PaidStatusTypeId == PaidStatusTypeId).FirstOrDefault();
            if (template != null)
            {
                return template.TemplateId;
            }

            return 0;
        }

        public List<TemplateApplicationOverride> GetTemplateApplicationOverrideForApplication(int ApplicationId)
        {
            return TemplateApplicationOverrides.Where(t => t.ApplicationId == ApplicationId).ToList();
        }

        public List<TemplateApplicationOverride> TemplateApplicationOverrides
        {
            get
            {
                List<TemplateApplicationOverride> _TemplateControlDictionary = FormsEngineCacheProxy.Cache.Get<List<TemplateApplicationOverride>>(Constants.TEMPLATE_APPLICATION_OVERRIDES);
                if (_TemplateControlDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.TEMPLATE_APPLICATION_OVERRIDES, FormsEngineCacheHelper.GetTemplateApplicationOverrides, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    _TemplateControlDictionary = FormsEngineCacheProxy.Cache.Get<List<TemplateApplicationOverride>>(Constants.TEMPLATE_APPLICATION_OVERRIDES);
                }
                return _TemplateControlDictionary;
            }
        }

        public List<TemplateApplicationOverride> GetTemplateApplicationOverrides()
        {
            List<TemplateApplicationOverride> Result = new List<TemplateApplicationOverride>();
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = Context.TemplateApplicationOverrides.ToList();
            }
            return Result;
        }

        /// <summary>
        /// Merges a group of templates into a single template and all related entities based on the Prod schema
        /// </summary>
        /// <param name="templateIds"></param>
        /// <returns></returns>
        public Template GetFullMergedTemplates(int[] templateIds)
        {
            Template TemplateResult = null;
            List<Template> templateResults = new List<Template>();
            if (templateIds.Length > 0)
            {

                foreach (int templateId in templateIds)
                {


                    templateResults.Add(TemplateDictionary[templateId]);


                }

                if (templateResults.Count > 0)
                {
                    TemplateResult = new Template();
                    TemplateResult.TemplateSteps = new List<TemplateStep>();


                    TemplateSection templateResultStepSection = new TemplateSection();
                    templateResultStepSection.Sequence = 0;
                    templateResultStepSection.TemplateControls = new List<TemplateControl>();

                    TemplateStep templateResultStep = new TemplateStep();
                    templateResultStep.Sequence = 0;

                    templateResultStep.TemplateSections.Add(templateResultStepSection);
                    TemplateResult.TemplateSteps.Add(templateResultStep);




                    foreach (Template template in templateResults)
                    {

                        foreach (TemplateControl templateControl in ControlTemplateDictionary[template.TemplateId])
                        {
                            if (templateResultStepSection.TemplateControls.Where(m => m.StandardControlId == templateControl.StandardControlId).FirstOrDefault() == null && ((bool)templateControl.IsRequired || templateControl.ShowInEddyApi))
                            {
                                templateResultStepSection.TemplateControls.Add(templateControl);
                            }

                        }



                    }
                    TemplateResult.TemplateName = "Merged Template";

                    if (TemplateResult != null)
                    {


                        for (int iStep = TemplateResult.TemplateSteps.Count - 1; iStep >= 0; iStep--)
                        {
                            var step = TemplateResult.TemplateSteps.ElementAt(iStep);
                            for (int iSection = step.TemplateSections.Count - 1; iSection >= 0; iSection--)
                            {
                                var section = step.TemplateSections.ElementAt(iSection);
                                for (int iControl = section.TemplateControls.Count - 1; iControl >= 0; iControl--)
                                {
                                    var control = section.TemplateControls.ElementAt(iControl);
                                    for (int iErrorMessage = control.StandardControl.StandardControlType.ErrorMessages.Count - 1; iErrorMessage >= 0; iErrorMessage--)
                                    {
                                        var ErrorMessage = control.StandardControl.StandardControlType.ErrorMessages.ElementAt(iErrorMessage);


                                    }

                                    if (control.StandardControl.PreDefinedValueList != null && control.StandardControl.PreDefinedValueList.IsEnabled == true && control.StandardControl.StandardControlCode.Code != "Program_Of_Interest")
                                    {
                                        //Clone StandardControl 
                                        control.StandardControl = control.StandardControl.CloneEntity();

                                        var StoredProc = control.StandardControl.PreDefinedValueList.StoredProcedureName;
                                        var result = GetTemplateControlData(TEMPLATE_STOREDPROCEDURESSCHEMA + StoredProc);
                                        //Multiple items default on items
                                        if (result.Count() > 0)
                                        {
                                            if (control.TemplateControlDefaults.Count() > 0)
                                            {
                                                foreach (var def in control.TemplateControlDefaults)
                                                {
                                                    ValueList found = result.Find(a => a.Value.ToLower() == def.Text.ToLower());
                                                    if (found != null)
                                                    {
                                                        found.IsDefault = true;
                                                    }
                                                }
                                            }
                                            control.StandardControl.PreDefinedValueList.ValueListItems = result;
                                        }
                                    }
                                    //single item default on standardcontrol.defaultvalue
                                    else if (control.TemplateControlDefaults.Count() > 0)
                                    {
                                        //Clone StandardControl 
                                        control.StandardControl = control.StandardControl.CloneEntity();
                                        control.StandardControl.DefaultValue = control.TemplateControlDefaults.First().Text;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return TemplateResult;
        }

        private Dictionary<int, KVCodeData> EntryTermDictionary
        {
            get
            {
                Dictionary<int, KVCodeData> _KVCodeDataDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, KVCodeData>>(Constants.ENTRYTERM_CACHE_KEY);
                if (_KVCodeDataDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.ENTRYTERM_CACHE_KEY, FormsEngineCacheHelper.GetEntryTermDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheKVCodeDataMinutes")));
                    _KVCodeDataDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, KVCodeData>>(Constants.ENTRYTERM_CACHE_KEY);
                }
                return _KVCodeDataDictionary;
            }
        }

        private Dictionary<int, KVCodeData> ReferralDictionary
        {
            get
            {
                Dictionary<int, KVCodeData> _KVCodeDataDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, KVCodeData>>(Constants.REFERRAL_CACHE_KEY);
                if (_KVCodeDataDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.REFERRAL_CACHE_KEY, FormsEngineCacheHelper.GetReferralDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheKVCodeDataMinutes")));
                    _KVCodeDataDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, KVCodeData>>(Constants.REFERRAL_CACHE_KEY);
                }
                return _KVCodeDataDictionary;
            }
        }

        public Dictionary<int, KVCodeData> GetKVCodeData(string name)
        {
            Dictionary<int, KVCodeData> kvCodeData = new Dictionary<int, KVCodeData>();
            List<KVCodeData> kvCodeDatas = new List<KVCodeData>();

            using (FEEntitiesContainer context = new FEEntitiesContainer())
            {
                kvCodeDatas = (from e in context.KVCodeDatas
                               join c in context.KVCodes on e.KVCodeId equals c.KVCodeId
                               where c.Name == name
                               && e.IsEnabled
                               && c.IsEnabled
                               select e).ToList();
            }

            foreach (KVCodeData data in kvCodeDatas)
            {
                if (!kvCodeData.ContainsKey(data.KVCodeDataId))
                {
                    kvCodeData.Add(data.KVCodeDataId, data);
                }
            }

            return kvCodeData;
        }

        public List<KeyValuePair<string, string>> GetKVCodeDataForInstitution(string kvCodeDataName, int institutionId)
        {
            Dictionary<int, KVCodeData> kvCodeData = DetermineKVCodeDataCacheDictionary(kvCodeDataName);

            var result = new List<KeyValuePair<string, string>>();
            List<KVCodeData> kvCodeDatas = kvCodeData.Where(e => e.Value.InstitutionId == institutionId).Select(e => e.Value).ToList();

            foreach (var data in kvCodeDatas)
            {
                result.Add(new KeyValuePair<string, string>(data.Key, data.Value));
            }

            return result;
        }

        private Dictionary<int, KVCodeData> DetermineKVCodeDataCacheDictionary(string kvCodeDataName)
        {
            var kvCodeData = new Dictionary<int, KVCodeData>();

            switch (kvCodeDataName)
            {
                case Constants.ENTRYTERM_KVCODEDATA:
                    kvCodeData = EntryTermDictionary;
                    break;
                case Constants.REFERRAL_KVCODEDATA:
                    kvCodeData = ReferralDictionary;
                    break;
            }

            return kvCodeData;
        }

        private Dictionary<int, ProgramLevel> ProgramLevelDictionary
        {
            get
            {
                Dictionary<int, ProgramLevel> _ProgramLevelDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, ProgramLevel>>(Constants.PROGRAMLEVEL_CACHE_KEY);
                if (_ProgramLevelDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.PROGRAMLEVEL_CACHE_KEY, FormsEngineCacheHelper.GetProgramLevelDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheProgamLevelMinutes")));
                    _ProgramLevelDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, ProgramLevel>>(Constants.PROGRAMLEVEL_CACHE_KEY);
                }
                return _ProgramLevelDictionary;
            }
        }

        public Dictionary<int, ProgramLevel> GetProgramLevelDictionary()
        {
            Dictionary<int, ProgramLevel> programLevelDictionary = new Dictionary<int, ProgramLevel>();
            List<ProgramLevel> programLevels = new List<ProgramLevel>();

            using (FEEntitiesContainer context = new FEEntitiesContainer())
            {
                programLevels = (from p in context.ProgramLevels
                              where p.IsEnabled
                              select p).ToList();
            }

            foreach (var programLevel in programLevels)
            {
                if (!programLevelDictionary.ContainsKey(programLevel.ProgramLevelId))
                {
                    programLevelDictionary.Add(programLevel.ProgramLevelId, programLevel);
                }
            }

            return programLevelDictionary;
        }

        public List<ProgramLevel> GetProgramLevels()
        {
            return ProgramLevelDictionary.Select(pl => pl.Value).ToList();
        }


        private List<int> FeaturedList
        {
            get
            {
                List<int> _FeaturedListHashSet = FormsEngineCacheProxy.Cache.Get<List<int>>(Constants.FEATUREDLIST_CACHE_KEY);
                if (_FeaturedListHashSet == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.FEATUREDLIST_CACHE_KEY, FormsEngineCacheHelper.GetFeaturedListFromDatabase, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheProgamLevelMinutes")));
                    _FeaturedListHashSet = FormsEngineCacheProxy.Cache.Get<List<int>>(Constants.FEATUREDLIST_CACHE_KEY);
                }
                return _FeaturedListHashSet;
            }
        }

        public List<int> GetFeaturedListFromDatabase()
        {
            List<int> featuredListHashSet = new List<int>();

            using (FEEntitiesContainer context = new FEEntitiesContainer())
            {
                featuredListHashSet = context.Database.SqlQuery<int>(QUERY_FEATURED_LIST).ToList();
            }

            return featuredListHashSet;
        }

        public List<int> GetFeaturedListSingleProgram()
        {
            return FeaturedList;
        }

    }
}
