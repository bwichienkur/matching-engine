using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class CCPAMessageService : ICCPAMessageService
    {
        private readonly string _ccpaMessagePrefix = "CCPA";
        private readonly IMetaDataService _metaDataService;

        private Dictionary<string, string> CCPAMetaDataMessages
        {
            get
            {
                return _metaDataService.GetMetaDataMessagesByPrefix(_ccpaMessagePrefix);
            }
        }

        public string BaseCCPAMessage
        {
            get
            {
                return GetCCPAMessageByKey($"{_ccpaMessagePrefix}.BASEMESSAGE");
            }
        }

        public CCPAMessageService(IMetaDataService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        private string GetCCPAMessageByKey(string messageKey)
        {
            CCPAMetaDataMessages.TryGetValue(messageKey, out string ccpaMessage);
            return ccpaMessage ?? string.Empty;
        }
    }
}
