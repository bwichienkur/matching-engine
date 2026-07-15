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
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine.DataModel
{

    public class RenderingStrategyDataService
    {
        public Dictionary<string, HTMLRenderingStrategy> GetHTMRenderingStrategyDictionary()
        {
            Dictionary<string, HTMLRenderingStrategy> _HTMRenderingStrategyDictionary = new Dictionary<string, HTMLRenderingStrategy>(StringComparer.OrdinalIgnoreCase);
            List<HTMLRenderingStrategy> Result = new List<HTMLRenderingStrategy>();
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from dct in Context.HTMLRenderingStrategies
                             where dct.IsEnabled == true
                             select dct).ToList();
            }

            foreach (var item in Result)
            {
                if (!_HTMRenderingStrategyDictionary.ContainsKey(item.Name))
                {
                    _HTMRenderingStrategyDictionary.Add(item.Name, item);
                }
            }
            

            return _HTMRenderingStrategyDictionary;
        }

        private Dictionary<string, HTMLRenderingStrategy> HTMRenderingStrategyDictionary
        {
            get
            {
                Dictionary<string, HTMLRenderingStrategy> _HTMRenderingStrategyDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<string, HTMLRenderingStrategy>>(Constants.HTML_RENDERINGSTRATEGY_KEY);
                if (_HTMRenderingStrategyDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.HTML_RENDERINGSTRATEGY_KEY, FormsEngineCacheHelper.GetHTMRenderingStrategyDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    _HTMRenderingStrategyDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<string, HTMLRenderingStrategy>>(Constants.HTML_RENDERINGSTRATEGY_KEY);
                }
                return _HTMRenderingStrategyDictionary;
            }
        }



        public HashSet<string> GetHTMRenderingStrategyAssignmentDictionary()
        {
            HashSet<string> _HTMRenderingStrategyAssignmentDictionary = new HashSet<string>();

            List<HTMLRenderingStrategyAssignment> Result = new List<HTMLRenderingStrategyAssignment>();
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from rsa in Context.HTMLRenderingStrategyAssignments
                             select rsa).ToList();
            }

            foreach (var item in Result)
            {
                string key = string.Format(Constants.HTML_RENDERINGSTRATEGY_HASHKEY, item.HTMRenderingStrategyId, item.TemplateTypeId);
                
                if (!_HTMRenderingStrategyAssignmentDictionary.Contains(key))
                {
                    _HTMRenderingStrategyAssignmentDictionary.Add(key);
                }
            }

            return _HTMRenderingStrategyAssignmentDictionary;
        }

        private HashSet<string> HTMRenderingStrategyAssignmentDictionary
        {
            get
            {
                HashSet<string> _HTMRenderingStrategyAssignmentDictionary = FormsEngineCacheProxy.Cache.Get<HashSet<string>>(Constants.HTML_RENDERINGSTRATEGY_ASSIGNMENT_KEY);
                if (_HTMRenderingStrategyAssignmentDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.HTML_RENDERINGSTRATEGY_ASSIGNMENT_KEY, FormsEngineCacheHelper.GetHTMRenderingStrategyAssignmentDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    _HTMRenderingStrategyAssignmentDictionary = FormsEngineCacheProxy.Cache.Get<HashSet<string>>(Constants.HTML_RENDERINGSTRATEGY_ASSIGNMENT_KEY);
                }
                return _HTMRenderingStrategyAssignmentDictionary;
            }
        }

        public HTMLRenderingStrategy GetHTMLRenderingStrategy(string RenderingStrategy)
        {
            HTMLRenderingStrategy Result = new HTMLRenderingStrategy();

            if (HTMRenderingStrategyDictionary.ContainsKey(RenderingStrategy))
            {
                Result = HTMRenderingStrategyDictionary[RenderingStrategy];
            }

            return Result;
        }


        public HTMLRenderingStrategy GetHTMLRenderingStrategy(int HTMLRenderingStrategyId)
        {
            HTMLRenderingStrategy Result = new HTMLRenderingStrategy();

            foreach (var RenderingStrategy in HTMRenderingStrategyDictionary.Values)
            {
                if (RenderingStrategy.HTMLRenderingStrategyId == HTMLRenderingStrategyId)
                {
                    Result = RenderingStrategy;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Validates if Rendering strategy exists and is assigned to the TemplateType
        /// </summary>
        /// <param name="RenderingStrategyName"></param>
        /// <param name="TemplateTypeId"></param>
        /// <returns></returns>
        public bool ValidateRenderingStrategy(string RenderingStrategyName, FormTemplateTypes FormTemplateType, out HTMLRenderingStrategy RenderingStrategy)
        {
            bool IsValid = false;
            RenderingStrategy = null;

            if (HTMRenderingStrategyDictionary.ContainsKey(RenderingStrategyName))
            {
                RenderingStrategy = HTMRenderingStrategyDictionary[RenderingStrategyName];
                string key = string.Format(Constants.HTML_RENDERINGSTRATEGY_HASHKEY, RenderingStrategy.HTMLRenderingStrategyId, (int)FormTemplateType);
                IsValid = HTMRenderingStrategyAssignmentDictionary.Contains(key);
            }

            return IsValid;
        }
    }
}



