using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Prospect
    {
        public int? ProspectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int? Age { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? EducationLevelId { get; set; }
        public int? HSGraduationYear { get; set; }
        public int? GenericGraduationYear { get; set; }
        public int? MilitaryStatusId { get; set; }
        public bool PreferEmail { get; set; }
        public bool PreferPhone { get; set; }
        public bool PreferText { get; set; }
        public bool? IsUSCitizen { get; set; }
        public bool? NeedsFinancialAid { get; set; }
        public string DesiredStartDate { get; set; }
        public string ExternalLeadId { get; set; }
        public int? GPAId { get; set; }
        public bool? IsMilitary { get; set; }
        public KeyValuePair<string, int>[] KVCodeData { get; set; }
        public int? YearsTeachingExperienceKeyValueId { get; set; }
        public int? YearsWorkExperienceKeyValueId { get; set; }
        public ProspectFlowDetails ProspectFlowDetails { get; set; }
    }
}
