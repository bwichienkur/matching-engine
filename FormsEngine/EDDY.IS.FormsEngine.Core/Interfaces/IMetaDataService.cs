using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IMetaDataService
    {
        Dictionary<string, string> GetMetaDataMessagesByPrefix(string prefix);
        string GetMetaDataMessageByKey(string key);
    }
}
