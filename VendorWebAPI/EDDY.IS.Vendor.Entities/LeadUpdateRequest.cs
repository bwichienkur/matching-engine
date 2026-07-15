using System;
using System.Collections.Generic;
using System.Linq;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;


namespace EDDY.IS.Vendor.Entities
{
    public class LeadUpdateRequest : VendorRequestBase
    {
        public List<int> LookUpKeyList { get; set; } = new List<int>();
        public decimal? EMSLeadId { get; set; }
        public decimal? ISLeadId { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


        public int? EMSInstitutionId { get; set; }
        public int? LeadStateId { get; set; }
        public string ClosedReasonCode { get; set; }


        // Client Data
        public string ClientApplicationDegreeName { get; set; }
        public string CustomFields { get; set; }
        public string ClientRegistered { get; set; }
        public string ClientInitialStartTerm { get; set; }
        public string ClientNotes { get; set; }
        public string ClientStatus { get; set; }
        public string ClientApplicationStartTerm { get; set; }

        // Not included because this is an internal/SF field:
        // public string PipelineNotes { get; set; }

        public string PendingApplicationChecklistItems { get; set; }
        public string CompletedApplicationChecklistItems { get; set; }

        // Client Dates - client sends strings but will parse out later
        public string ClientApplicationDate { get; set; }
        public string ClientStatusUpdatedDate { get; set; }
        public string ClientInterviewDate { get; set; }
        public string ClientStartDate { get; set; }
        public string ClientStartReceivedDate { get; set; }
        public string ClientContactDate { get; set; }
        public string ClientEnrollDate { get; set; }
        public string ClientApplicationStartDate { get; set; }
        public string ClientAdmitDate { get; set; }
        public string ClientAppointmentDate { get; set; }
        public string ClientQualifiedDate { get; set; }
        public string ClientGraduateDate { get; set; }
        public string ClientFirstTermPersistDate { get; set; }
        public string ClientFAFSAReceivedDate { get; set; }
        public string ClientApplicationSubmittedDate { get; set; }
        public string ClientDepositDate { get; set; }
        public string ClientApplicationDeniedDate { get; set; }

    }
}