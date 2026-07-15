using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramContent
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDisclaimerType { get; set; }
        public string ProgramDisclaimer { get; set; }
        public string GradSchoolsDegreeName { get; set; }
        //private byte[] ProgramDescriptionCompressed { get; set; }

        //public string ProgramDescription { get; set; }
        public int? CopyFromProgramId { get; set; }
        public int? SchoolId { get; set; }
        public string ProgramDisplayName { get; set; }

        public VW_Matching_ProgramContent(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ProgramName = System.Convert.ToString(dr["ProgramName"]);
            ProgramDisclaimerType = System.Convert.ToString(dr["ProgramDisclaimerType"]);
            ProgramDisclaimer = System.Convert.ToString(dr["ProgramDisclaimer"]);
            GradSchoolsDegreeName = System.Convert.ToString(dr["GradSchoolsDegreeName"]);

            //if (!dr.IsDBNull(dr.GetOrdinal("ProgramDescriptionCompressed")))
            //{
            //    ProgramDescriptionCompressed = (byte[])dr.GetValue(dr.GetOrdinal("ProgramDescriptionCompressed"));

            //    ProgramDescription = StringCompression.DecompressUTF8(ProgramDescriptionCompressed);

            //    if (!String.IsNullOrEmpty(ProgramDescription))
            //        ProgramDescription = ProgramDescription.Trim();
            //}

            if (!dr.IsDBNull(dr.GetOrdinal("SchoolId")))
                SchoolId = (int)dr.GetValue(dr.GetOrdinal("SchoolId"));

            //if (!dr.IsDBNull(dr.GetOrdinal("CopyLongDescriptionFromProgramId")))
            //    CopyFromProgramId = (int)dr.GetValue(dr.GetOrdinal("CopyLongDescriptionFromProgramId"));

            //Pull the program display name if we have it. Otherwise, we pull the program name. Mostly affects EMS institutions.
            if(!String.IsNullOrWhiteSpace(System.Convert.ToString(dr["ProgramDisplayName"])))
                ProgramDisplayName = System.Convert.ToString(dr["ProgramDisplayName"]);
            else
                ProgramDisplayName = System.Convert.ToString(dr["ProgramName"]);
        }
    }
}
