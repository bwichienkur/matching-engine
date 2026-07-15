using System.Data;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramTag
    {
        public int ProgramId { get; set; }
        public int ProgramTagTypeId { get; set; }
        public string TagType { get; set; }
        public int ProgramTagId { get; set; }
        public string Tag { get; set; }

        public VW_Matching_ProgramTag(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ProgramTagTypeId = System.Convert.ToInt32(dr["ProgramTagTypeID"]);
            TagType = System.Convert.ToString(dr["TagTypeName"]);
            ProgramTagId = System.Convert.ToInt32(dr["ProgramTagID"]);
            Tag = System.Convert.ToString(dr["TagName"]);
        }
    }
}
