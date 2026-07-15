using EDDY.IS.Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Helpers
{
    public class JsonParser
    {
        public Dictionary<string, string> JsonStringToDictionary(string json)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                new ISException(applicationID: Base.ISApplication.FormsEngine, e: ex).Save();
            }

            return result;
        }

    }
}
