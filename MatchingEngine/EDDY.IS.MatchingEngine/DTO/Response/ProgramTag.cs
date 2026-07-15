using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Runtime.Serialization;

namespace EDDY.IS.MatchingEngine.DTO.Response
{
    [DataContract]
    public class ProgramTag
    {
        public ProgramTag(VW_Matching_ProgramTag programTag)
        {
            ProgramTagTypeId = programTag.ProgramTagTypeId;
            TagType = programTag.TagType;
            ProgramTagId = programTag.ProgramTagId;
            Tag = programTag.Tag;
        }

        [DataMember]
        public int ProgramTagTypeId { get; set; }
        
        [DataMember]
        public string TagType { get; set; }
        
        [DataMember]
        public int ProgramTagId { get; set; }
        
        [DataMember]
        public string Tag { get; set; }
    }
}
