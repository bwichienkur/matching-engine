using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ApiProgramResponse : BaseMatchResponse
    {
        [DataMember]
        public ApiProgram Program { get; set; }

        [DataMember]
        public bool IncludesAllCountries { get; set; }

        [DataMember]
        public bool IncludesAllStates { get; set; }

        [DataMember]
        public List<int> Countries { get; set; }

        [DataMember]
        public List<int> States { get; set; }

        [DataMember]
        public List<ApiRule> Rules { get; set; }
    }

    [DataContract]
    public class ApiRule
    {
        [DataMember]
        public int RuleId { get; set; }

        [DataMember]
        public string RuleName { get; set; }

        [DataMember]
        public decimal? EntityValue { get; set; }

        [DataMember]
        public string StandardControlCode { get; set; }

       
        [DataMember]
        public Dictionary<int,string> ValidKeyValueCodeData { get; set; }
    }

    [DataContract]
    public class ApiProgram
    {
        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public int CampusId { get; set; }

        [DataMember]
        public string CampusName { get; set; }

        [DataMember]
        public int ProgramId { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public string ProgramDescription { get; set; }

        //[DataMember]
        //public bool HasCampusLogo { get; set; }

        [DataMember]
        public string CampusLogoURL { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        public int? SubjectId { get; set; }

        [DataMember]
        public int ProgramLevelId { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }
    }
}