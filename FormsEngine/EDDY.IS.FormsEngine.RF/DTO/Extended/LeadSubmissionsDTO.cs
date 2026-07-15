using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    public class LeadSubmissionsDTO
    {
        public int ProgramId { get; set; }
        public int ProgramProductId { get; set; }
        public int CampusId { get; set; }
        public int ProgramTemplateId { get; set; }
        public decimal? LeadId { get; set; }

        public LeadSubmissionsDTO(int programId, int programProductId, int campusId, int programTemplateId)
        {
            ProgramId = programId;
            ProgramProductId = programProductId;
            CampusId = campusId;
            ProgramTemplateId = programTemplateId;
        }
    }
}