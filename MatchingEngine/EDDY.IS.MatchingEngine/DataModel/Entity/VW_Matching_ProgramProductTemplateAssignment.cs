using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProductTemplateAssignment
    {
        public int programproductid { get; set; }
        public int ProgramId { get; set; }
        public int ProductId { get; set; }
        public int TemplateId { get; set; }

        public VW_Matching_ProgramProductTemplateAssignment(IDataReader dr)
        {
            programproductid = System.Convert.ToInt32(dr["ProgramProductId"]);
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            TemplateId = System.Convert.ToInt32(dr["TemplateId"]);
        }
    }
}
