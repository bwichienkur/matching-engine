using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class APIProgramMatchesDTO
    {
        [DataMember]
        public bool Valid { get; set; }

        [DataMember]
        public List<KeyValuePair<string, string>> ValidationMessages { get; set; }

        [DataMember]
        public List<CampusWithInstitution> SchoolSelectionList { get; set; }

        [DataMember]
        public int? MaxUserSelectionsField { get; set; }

        [DataMember]
        public List<int> TemplateIdList { get; set; }

        [DataMember]
        public List<KeyValuePair<int, List<TemplateControlDTO>>> AdditionalQuestionList { get; set; }
    }
}
