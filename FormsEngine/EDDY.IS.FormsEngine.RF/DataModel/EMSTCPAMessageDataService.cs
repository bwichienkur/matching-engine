using EDDY.IS.FormsEngine.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{
    public class EMSTCPAMessageDataService
    {
        private Dictionary<int, string> EMSInstitutionTCPAMessageDictionary
        {
            get
            {
                Dictionary<int, string> _EMSInstitutionTCPAMessageDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, string>>(Constants.EMS_INSTITUTION_CACHE_KEY);
                if (_EMSInstitutionTCPAMessageDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.EMS_INSTITUTION_CACHE_KEY, FormsEngineCacheHelper.GetEMSTCPAMessageDictionary(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheEMSTCPAMessagesTimeMinutes")));
                    _EMSInstitutionTCPAMessageDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<int, string>>(Constants.EMS_INSTITUTION_CACHE_KEY);
                }
                return _EMSInstitutionTCPAMessageDictionary;
            }
        }

        public Dictionary<int, string> GetEMSTCPAMessageDictionary()
        {
            Dictionary<int, string> _InstitutionDictionary = new Dictionary<int, string>();
            List<VW_EMSInstitutionTCPAMessages> messageList = new List<VW_EMSInstitutionTCPAMessages>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                messageList = (from i in Context.VW_EMSInstitutionTCPAMessages select i).ToList();
            }

            foreach (var message in messageList)
            {
                if (!_InstitutionDictionary.ContainsKey(message.InstitutionId))
                {
                    _InstitutionDictionary.Add(message.InstitutionId, message.TCPAMessage);
                }
            }

            return _InstitutionDictionary;
        }

        public string GetEMSInstitutionTCPAText(int InstitutionId)
        {
            EMSInstitutionTCPAMessageDictionary.TryGetValue(InstitutionId, out string TCPAMessage);
            return TCPAMessage;
        }
    }
}
