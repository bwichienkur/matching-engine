using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.Services.Caching;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class BaseModel
    {
        public string GetResourceMetaDataText(string Key)
        {
            return FormsEngine.GetResourceMetaDataTextForKey(Key);
        }
    }
}