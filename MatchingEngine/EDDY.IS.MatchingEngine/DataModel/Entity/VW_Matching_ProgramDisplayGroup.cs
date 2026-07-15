using EDDY.IS.Base.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramDisplayGroup
    {
        public int ProgramDisplayGroupId { get; set; }
        public string ProgramDisplayGroupName { get; set; }
        public string ProgramDisplayGroupDescription { get; set; }
        public string ProgramDisplayGroupDescriptionShort { get; set; }

        private byte[] ProgramDisplayGroupDescriptionCompressed { get; set; }

        public VW_Matching_ProgramDisplayGroup(IDataReader dr)
        {
            ProgramDisplayGroupId = System.Convert.ToInt32(dr["ProgramDisplayGroupId"]);
            ProgramDisplayGroupName = System.Convert.ToString(dr["ProgramDisplayGroupName"]);
            ProgramDisplayGroupDescriptionShort = System.Convert.ToString(dr["ProgramDisplayGroupDescriptionShort"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramDisplayGroupDescriptionCompressed")))
            {
                ProgramDisplayGroupDescriptionCompressed = (byte[])dr.GetValue(dr.GetOrdinal("ProgramDisplayGroupDescriptionCompressed"));

                ProgramDisplayGroupDescription = StringCompression.DecompressUTF8(ProgramDisplayGroupDescriptionCompressed);

                if (!String.IsNullOrEmpty(ProgramDisplayGroupDescription))
                    ProgramDisplayGroupDescription = ProgramDisplayGroupDescription.Trim();
            }
        }
    }
}
