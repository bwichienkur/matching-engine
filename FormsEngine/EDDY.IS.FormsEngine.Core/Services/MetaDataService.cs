using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class MetaDataService : IMetaDataService
    {
        private readonly IMetaDataRepository _metaDataRepository;

        public MetaDataService(IMetaDataRepository metaDataRepository)
        {
            _metaDataRepository = metaDataRepository;
        }

        public Dictionary<string, string> GetMetaDataMessagesByPrefix(string prefix)
        {
            Dictionary<string, string> metaDataMessages = null;

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                metaDataMessages = _metaDataRepository.GetMetaDataMessagesByPrefix(prefix.ToUpper());
            }

            return metaDataMessages ?? new Dictionary<string, string>();
        }

        public string GetMetaDataMessageByKey(string key)
        {
            string message = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                message = _metaDataRepository.GetMetaDataMessageByKey(key);
            }

            return message ?? string.Empty;
        }
    }
}
