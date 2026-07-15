using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class FormProgramResponse : BaseMatchResponse
    {
        [DataMember]
        public List<FormProgram> FormProgramList { get; set; }
        [DataMember]
        public string CustomTCPA { get; set; }
    }

    [DataContract]
    public class FormProgram : BaseMatchEntity
    {
        [DataMember]
        public bool Is2USchool { get; set; }

        [DataMember]
        public int ProgramId { get; set; }

        [DataMember]
        public int ProgramLevelId { get; set; }

        [DataMember]
        public string ProgramLevelName { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public CampusType CampusType { get; set; }

        [DataMember]
        public int? TemplateId { get; set; }

        [DataMember]
        public PaidStatusType? PaidStatusTypeId { get; set; }

        [DataMember]
        public bool RequiresSystemTemplateUse { get; set; }

        [DataMember]
        public bool ShowTwoULeadShareControl { get; set; }

		[DataMember]
		public string CampusOptionGroup { get; set; }

		[DataMember]
		public int? CampusOptionGroupPosition { get; set; }
	}

    //[DataContract]
    //public class FormCampus : BaseMatchEntity
    //{
    //    [DataMember]
    //    public int CampusId { get; set; }

    //    [DataMember]
    //    public string CampusName { get; set; }

    //    [DataMember]
    //    public CampusType CampusType { get; set; }

    //    [DataMember]
    //    public string Address1 { get; set; }

    //    [DataMember]
    //    public string Address2 { get; set; }

    //    [DataMember]
    //    public string City { get; set; }

    //    [DataMember]
    //    public string State { get; set; }

    //    [DataMember]
    //    public string PostalCode { get; set; }

    //    [DataMember]
    //    public string CountryCode { get; set; }

    //    [DataMember]
    //    public int? TemplateId { get; set; }

    //    [DataMember]
    //    public int ProgramProductId { get; set; }
    //}
}
