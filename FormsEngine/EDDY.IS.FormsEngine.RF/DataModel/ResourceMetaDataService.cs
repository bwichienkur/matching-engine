using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Util.Memory;
using EDDY.IS.Util.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{

    public class ResourceMetaDataService
    {
        private ConcurrentDictionary<string, ResourceMetaData> ResourceMetaDataDictionary
        {
            get
            {
                ConcurrentDictionary<string, ResourceMetaData> _ResourceMetaDataDictionary = FormsEngineCacheProxy.Cache.Get<ConcurrentDictionary<string, ResourceMetaData>>(Constants.RESOURCEMETADATA_CACHE_KEY);
                if (_ResourceMetaDataDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.RESOURCEMETADATA_CACHE_KEY, this.RetrieveResourceMetaDataDictionary(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheResourceMetaDatasTimeMinutes")));
                    _ResourceMetaDataDictionary = FormsEngineCacheProxy.Cache.Get<ConcurrentDictionary<string, ResourceMetaData>>(Constants.RESOURCEMETADATA_CACHE_KEY);
                }
                return _ResourceMetaDataDictionary;
            }
        }

        private List<string> ResourceMetaDataTCPAKeys
        {
            get
            {
                List<string> _ResourceMetaDataTCPAKeys = FormsEngineCacheProxy.Cache.Get<List<string>>(Constants.RESOURCEMETADATATCPAKEYS_CACHE_KEY);
                if (_ResourceMetaDataTCPAKeys == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.RESOURCEMETADATATCPAKEYS_CACHE_KEY, this.RetrieveResourceMetaDataTCPAKeys(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheResourceMetaDatasTimeMinutes")));
                    _ResourceMetaDataTCPAKeys = FormsEngineCacheProxy.Cache.Get<List<string>>(Constants.RESOURCEMETADATATCPAKEYS_CACHE_KEY);
                }
                return _ResourceMetaDataTCPAKeys;
            }
        }

        private List<string> RetrieveResourceMetaDataTCPAKeys()
        {
            List<string> keys = new List<string>();
            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                keys = (from r in Context.ResourceMetaDatas
                                where r.IsEnabled && r.IsTCPAMessage
                                select r.ResourceMetaDataKey).ToList();
            }
            return keys;
        }

        private ConcurrentDictionary<string, ResourceMetaData> RetrieveResourceMetaDataDictionary()
        {
            ConcurrentDictionary<string, ResourceMetaData> _ResourceMetaDataDictionary = new ConcurrentDictionary<string, ResourceMetaData>();
            List<ResourceMetaData> ResourceList = new List<ResourceMetaData>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                ResourceList = (from r in Context.ResourceMetaDatas
                               where r.IsEnabled
                               select r).ToList();
            }

            foreach (var item in ResourceList)
            {
                _ResourceMetaDataDictionary.TryAdd(item.ResourceMetaDataKey, item);
            }
            return _ResourceMetaDataDictionary;
        }

        public string GetResourceMetaDataTextForKey(string Key)
        {
            string Result = "";
            ResourceMetaData MetaData = null;

            if (ResourceMetaDataDictionary.TryGetValue(Key, out MetaData))
            {
                Result = MetaData.ResourceMetaDataText;
            }

            return Result;
        }

        private List<ProgramTemplateMessage> RetrieveProgramTemplateMessageList()
        {
            List<ProgramTemplateMessage> Result = null;

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from m in Context.ProgramTemplateMessages
                         select m).ToList();
            }

            return Result; ;
        }



        private List<ProgramTemplateMessage> ProgramTemplateMessageList
        {
            get
            {
                List<ProgramTemplateMessage> _ProgramTemplateMessageList = FormsEngineCacheProxy.Cache.Get<List<ProgramTemplateMessage>>(Constants.PROGRAMTEMPLATEMESSAGE_CACHE_KEY);
                if (_ProgramTemplateMessageList == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.PROGRAMTEMPLATEMESSAGE_CACHE_KEY, this.RetrieveProgramTemplateMessageList(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheResourceMetaDatasTimeMinutes")));
                    _ProgramTemplateMessageList = FormsEngineCacheProxy.Cache.Get<List<ProgramTemplateMessage>>(Constants.PROGRAMTEMPLATEMESSAGE_CACHE_KEY);
                }
                return _ProgramTemplateMessageList;
            }
        }

        public virtual List<ProgramTemplateMessage> GetProgramTemplateMessageForType(string Type)
        {
            List<ProgramTemplateMessage> TheProgramTemplateMessageList = new List<ProgramTemplateMessage>();

            foreach (ProgramTemplateMessage TheProgramTemplateMessage in ProgramTemplateMessageList)
            {
                if (TheProgramTemplateMessage.ProgramTemplateMessageTypeName.ToUpper() == Type.Replace(' ', '_').ToUpper() ||
                    TheProgramTemplateMessage.ProgramTemplateMessageTypeName.ToUpper() == Type.Replace(" ", string.Empty).ToUpper())
                    TheProgramTemplateMessageList.Add(TheProgramTemplateMessage);
            }
            return TheProgramTemplateMessageList.Count > 0 ? TheProgramTemplateMessageList : null;
        }


        public Dictionary<string, string> GetResourceMetaDataForTCPA()
        {
            Dictionary<string, string> metadata = new Dictionary<string,string>();
            foreach(string s in ResourceMetaDataTCPAKeys)
            {
                metadata.Add(s, GetResourceMetaDataTextForKey(s));
            }
            return metadata;
        }

        public Dictionary<string, string> GetResourceMetaData()
        {
            return ResourceMetaDataDictionary.ToDictionary(m => m.Key, m => m.Value.ResourceMetaDataText);
        }
    }
}



