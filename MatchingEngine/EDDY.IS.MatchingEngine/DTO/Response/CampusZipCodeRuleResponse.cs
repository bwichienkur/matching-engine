using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public enum ZipCodeRuleType
    {
        None = 0,
        Inclusion = 1,
        Exclusion = 2
    }

    [DataContract]
    public class CampusZipCodeRuleResponse
    {
        [DataMember]
        public ZipCodeRuleType ZipCodeRuleType { get; set; }

        [DataMember]
        public List<string> ZipCodeList { get; set; }
    }
}
