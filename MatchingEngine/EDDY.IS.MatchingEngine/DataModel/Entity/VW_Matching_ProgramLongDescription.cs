using EDDY.IS.Base.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramLongDescription
    {
        public int ProgramId { get; set; }

        private byte[] ProgramDescriptionCompressed { get; set; }

        public string ProgramDescription { get; set; }
        public int? CopyFromProgramId { get; set; }

        public VW_Matching_ProgramLongDescription(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramDescriptionCompressed")))
            {
                ProgramDescriptionCompressed = (byte[])dr.GetValue(dr.GetOrdinal("ProgramDescriptionCompressed"));

                ProgramDescription = StringCompression.DecompressUTF8(ProgramDescriptionCompressed);

                if (!String.IsNullOrEmpty(ProgramDescription))
                    ProgramDescription = ProgramDescription.Trim();
            }

            if (!dr.IsDBNull(dr.GetOrdinal("CopyFromProgramId")))
                CopyFromProgramId = (int)dr.GetValue(dr.GetOrdinal("CopyFromProgramId"));
        }
    }
}
