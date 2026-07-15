using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramRuleDefinition
    {
        [DataMember]
        public BaseRuleDefinitionType BaseRuleType { get; set; }
        [DataMember]
        public string RuleName { get; set; }
        [DataMember]
        public decimal? EntityValue { get; set; } //lowest priority for rule string.
        [DataMember]
        public string StandardControlCode { get; set; }
        [DataMember]
        public List<int> KeyValueCodeIds { get; set; } //highest priority for rule string
        [DataMember]
        public decimal? UpperBound { get; set; } //second highest priority for rule string
        [DataMember]
        public decimal? LowerBound { get; set; } //second highest priority for rule string
        [DataMember]
        public string RuleString { get; set; }
    }
}
