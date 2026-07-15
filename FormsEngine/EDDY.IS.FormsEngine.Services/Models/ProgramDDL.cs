using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class ProgramDDL : BaseModel
    {
        public bool AnyCampusResult { get; set; }
        public bool AnyOnlineResult { get; set; }
        public bool InlineDropDown { get; set; }
        public bool AnyCustomOptGroup { get; set; }
        public List<ProgramOptGroup> ProgramOptGroupList { get; set; }
        public string CustomTCPA { get; set; }
    }

    [Serializable]
    public class ProgramOptGroup : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SelectedProductId { get; set; }
        public List<Program> ProgramList { get; set; }
    }

    [Serializable]
    public class Program : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int ProgramProductId { get; set; }
        public int TemplateId { get; set; }
        public int PaidStatusTypeId { get; set; }
        public bool Is2USchool { get; set; }
        public bool ShowTwoULeadShareControl { get; set; }
        public string ProgramType { get; set; }
        public decimal ProgramRankScore { get; set; }
    }



    [Serializable]
    public class ProgramRankInstitution
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int InstitutionId { get; set; }
        public string ProgramDescription { get; set; }
    }
}
