using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DTO
{
    public partial class TemplateControlDTO
    {
        private Dictionary<string, string> _JsonExtendedProperties;
        private Dictionary<string, string> JsonExtendedProperties
        {
            get
            {
                if (_JsonExtendedProperties == null)
                {
                    if (!string.IsNullOrWhiteSpace(ExtendedProperties))
                    {
                        try
                        {
                            _JsonExtendedProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(ExtendedProperties);
                        }
                        catch
                        {
                            _JsonExtendedProperties = new Dictionary<string, string>();
                        }
                    }
                    else
                    {
                        _JsonExtendedProperties = new Dictionary<string, string>();
                    }
                }
                return _JsonExtendedProperties;
            }
        }

        public T GetExtendedPropertyValue<T>(string propertyName)
        {
            if(JsonExtendedProperties.ContainsKey(propertyName) && !string.IsNullOrWhiteSpace(JsonExtendedProperties[propertyName]))
            {
                return (T)Convert.ChangeType(JsonExtendedProperties[propertyName], typeof(T));
            }
            else
            {
                return default;
            }
        }
    }
}
