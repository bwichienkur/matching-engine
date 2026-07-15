using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_Image
    {
        public int EntityId { get; set; }
        public int EntityMetaId { get; set; }
        public int Length { get; set; }
        public int Breadth { get; set; }
        public string FileURL { get; set; }
        public int ImagesVideoLogoTypeId { get; set; }

        public VW_Matching_Image(IDataReader dr)
        {
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
            EntityMetaId = System.Convert.ToInt32(dr["EntityMetaId"]);
            Length = System.Convert.ToInt32(dr["Length"]);
            Breadth = System.Convert.ToInt32(dr["Breadth"]);
            FileURL = System.Convert.ToString(dr["FileURL"]);
            ImagesVideoLogoTypeId = System.Convert.ToInt32(dr["ImagesVideoLogoTypeId"]);
        }
    }
}
