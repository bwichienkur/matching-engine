using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DTO.Extended
{
    [Serializable]
    [DataContract]
    public class FiltersDTO
    {
        [DataMember]
        public List<KeyValuePair<string,string>> Filters { get; set;}
    }
}
