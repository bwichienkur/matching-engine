using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CrossSellProgramResponse : BaseMatchResponse
    {        
        [DataMember]
        public List<ProgramWithInstitutionCampus> ProgramList { get; set; }

        [DataMember]
        public string InitialLeadValidMessage { get; set; }

        [DataMember]
        public string InitialLeadInvalidMessage { get; set; }

        [DataMember]
        public int? MaxProgramsToDisplay { get; set; }

        [DataMember]
        public int? MaxUserSelections { get; set; }

        [DataMember]
        public bool? IsPreCheckEnabled { get; set; }

        [DataMember]
        public bool? IsCrossSellALternateList { get; set; }
    }
}
