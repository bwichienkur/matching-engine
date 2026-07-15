using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class LeadDetails
    {

        private int leadId;
        private int prospectFlowId;
        private string firstName;
        private string lastName;
        private string address;
        private string address2;
        private string city;
        private string postalCode;
        private string country;
        private string state;
        private string email;
        private string phone;
        private string alternatePhone;
        private string currentDisposition;
        private string createdDate;
        private string modifiedDate;
        private string note;
        private List<LeadNote> notes;
        private string advisorEmail;
        private int advisorId;
        private List<Advisor> advisors;
        private List<ProspectStatus> prospectStatuses;
        private List<LeadDispositionHistory> leadDispositionHistory;
        private List<State> states;

        public int LeadId
        {
            get
            {
                return leadId;
            }

            set
            {
                leadId = value;
            }
        }
        public int ProspectFlowId
        {
            get
            {
                return prospectFlowId;
            }

            set
            {
                prospectFlowId = value;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        public string Address2
        {
            get
            {
                return address2;
            }

            set
            {
                address2 = value;
            }
        }

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
            }
        }

        public string PostalCode
        {
            get
            {
                return postalCode;
            }

            set
            {
                postalCode = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value;
            }
        }

        public string AlternatePhone
        {
            get
            {
                return alternatePhone;
            }

            set
            {
                alternatePhone = value;
            }
        }

        public string CreatedDate
        {
            get
            {
                return createdDate;
            }

            set
            {
                createdDate = value;
            }
        }
        public string ModifiedDate
        {
            get
            {
                return modifiedDate;
            }

            set
            {
                modifiedDate = value;
            }
        }

        public string Note
        {
            get
            {
                return note;
            }

            set
            {
                note = value;
            }
        }
        public List<LeadNote> Notes
        {
            get
            {
                return notes;
            }

            set
            {
                notes = value;
            }
        }

        public string AdvisorEmail
        {
            get
            {
                return advisorEmail;
            }

            set
            {
                advisorEmail = value;
            }
        }

        public int AdvisorId
        {
            get
            {
                return advisorId;
            }

            set
            {
                advisorId = value;
            }
        }

        public List<Advisor> Advisors
        {
            get
            {
                return advisors;
            }

            set
            {
                advisors = value;
            }
        }
        public List<ProspectStatus> ProspectStatuses
        {
            get
            {
                return prospectStatuses;
            }

            set
            {
                prospectStatuses = value;
            }
        }
        public List<State> States
        {
            get
            {
                return states;
            }

            set
            {
                states = value;
            }
        }

        public string State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }

        public string CurrentDisposition
        {
            get
            {
                return currentDisposition;
            }

            set
            {
                currentDisposition = value;
            }
        }


        public List<LeadDispositionHistory> LeadDispositionHistory
        {
            get
            {
                return leadDispositionHistory;
            }

            set
            {
                leadDispositionHistory = value;
            }
        }
    }
}
