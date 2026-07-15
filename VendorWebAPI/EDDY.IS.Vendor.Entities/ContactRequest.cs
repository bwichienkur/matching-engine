using System;
using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class ContactRequest : VendorRequestBase
    {
        private int leadId;
        private string glassPanelLeadId;
        private string affiliateId;
        private string leadIdToken;
        private int age;
        private int campusId;
        private int programId;
        private int programLevelId;
        private int templateId;
        private int prospectId;
        private int prospectFlowId;
        private string firstName;
        private string lastName;
        private string address;
        private string address2;
        private string city;
        private string postalCode;
        private string email;
        private string phone;
        private string alternatePhone;
        private string highestLevelofEducationCompleted;
        private string desiredStartDate;
        private string prefix;
        private string preferredMethodsofContact;
        private string gPA;
        private string yearsofWorkExperience;
        private string yearsofTeachingExperience;
        private string militaryAffiliation;
        private string undergraduateDegreeGrad;
        private string undergraduateDegreeNursing;
        private string rnLicense;
        private string undergraduateDegreeEducation;
        private string teachingCertificate;
        private string registeredAndLicensure;
        private string registeredRadiology;
        private string completed1600HoursOfClinicalExperience;
        private string employedRadiologyOrGraduatedPast5Years;
        private string country;
        private string state;
        private List<QuestionAnswer> additionalQuestions;
        private string usCitizen;
        private int? yearHighestEducationCompleted;
        private string externalSystemId;
        private string areaOfInterest;
        private string dialerListName;
        private string programOfInterest;
        private string modificationDate;
        private string company;
        private bool checkDoNotCall;
        private int advisorId;
        private string userAgreement;
        private string leadSource;
        private string leadSourceUrl;
        private string leadInitiatingUrl;
        private string prospectSourceUrl;
        private string prospectInitiatingUrl;
        private string storeFront;
        private int? clientRelationContactId;
        private string matchResponseGuid;
        private List<KeyValuePair<int, string>> programProducts;
        private string leadJson;
        private List<int?> paidStatusTypes;
        private string dialerKey;
        private string tsr;
        private string warmTransferSchool;
        private string agentName;
        private int? agentId;
        private string ss1;
        private string ss2;
        private string sourceCode;
        private string videoUrl;

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
        public string AffiliateId
        {
            get
            {
                return affiliateId;
            }

            set
            {
                affiliateId = value;
            }
        }

        public string LeadIdToken
        {
            get
            {
                return leadIdToken;
            }

            set
            {
                leadIdToken = value;
            }
        }
        public int Age
        {
            get
            {
                return age;
            }

            set
            {
                age = value;
            }
        }
        public int CampusId
        {
            get
            {
                return campusId;
            }

            set
            {
                campusId = value;
            }
        }

        public int ProgramId
        {
            get
            {
                return programId;
            }

            set
            {
                programId = value;
            }
        }

        public int ProgramLevelId
        {
            get
            {
                return programLevelId;
            }

            set
            {
                programLevelId = value;
            }
        }

        public int TemplateId
        {
            get
            {
                return templateId;
            }

            set
            {
                templateId = value;
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

        public string HighestLevelofEducationCompleted
        {
            get
            {
                return highestLevelofEducationCompleted;
            }

            set
            {
                highestLevelofEducationCompleted = value;
            }
        }

        public string DesiredStartDate
        {
            get
            {
                return desiredStartDate;
            }

            set
            {
                desiredStartDate = value;
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                prefix = value;
            }
        }

        public string PreferredMethodsofContact
        {
            get
            {
                return preferredMethodsofContact;
            }

            set
            {
                preferredMethodsofContact = value;
            }
        }

        public string GPA
        {
            get
            {
                return gPA;
            }

            set
            {
                gPA = value;
            }
        }

        public string YearsofWorkExperience
        {
            get
            {
                return yearsofWorkExperience;
            }

            set
            {
                yearsofWorkExperience = value;
            }
        }

        public int? YearHighestEducationCompleted
        {
            get
            {
                return yearHighestEducationCompleted;
            }

            set
            {
                yearHighestEducationCompleted = value;
            }
        }

        public string YearsofTeachingExperience
        {
            get
            {
                return yearsofTeachingExperience;
            }

            set
            {
                yearsofTeachingExperience = value;
            }
        }

        public string MilitaryAffiliation
        {
            get
            {
                return militaryAffiliation;
            }

            set
            {
                militaryAffiliation = value;
            }
        }

        public string UndergraduateDegreeGrad
        {
            get
            {
                return undergraduateDegreeGrad;
            }

            set
            {
                undergraduateDegreeGrad = value;
            }
        }

        public string UndergraduateDegreeNursing
        {
            get
            {
                return undergraduateDegreeNursing;
            }

            set
            {
                undergraduateDegreeNursing = value;
            }
        }

        public string RNLicense
        {
            get
            {
                return rnLicense;
            }

            set
            {
                rnLicense = value;
            }
        }

        public string UndergraduateDegreeEducation
        {
            get
            {
                return undergraduateDegreeEducation;
            }

            set
            {
                undergraduateDegreeEducation = value;
            }
        }

        public string TeachingCertificate
        {
            get
            {
                return teachingCertificate;
            }

            set
            {
                teachingCertificate = value;
            }
        }

        public string RegisteredAndLicensure
        {
            get
            {
                return registeredAndLicensure;
            }

            set
            {
                registeredAndLicensure = value;
            }
        }

        public string RegisteredRadiology
        {
            get
            {
                return registeredRadiology;
            }

            set
            {
                registeredRadiology = value;
            }
        }

        public string Completed1600HoursOfClinicalExperience
        {
            get
            {
                return completed1600HoursOfClinicalExperience;
            }

            set
            {
                completed1600HoursOfClinicalExperience = value;
            }
        }



        public string EmployedRadiologyOrGraduatedPast5Years
        {
            get
            {
                return employedRadiologyOrGraduatedPast5Years;
            }

            set
            {
                employedRadiologyOrGraduatedPast5Years = value;
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

        public string USCitizen
        {
            get
            {
                return usCitizen;
            }

            set
            {
                usCitizen = value;
            }
        }

        public List<QuestionAnswer> AdditionalQuestions
        {
            get
            {
                return additionalQuestions;
            }

            set
            {
                additionalQuestions = value;
            }
        }

        public string ExternalSystemId
        {
            get
            {
                return externalSystemId;
            }

            set
            {
                externalSystemId = value;
            }
        }

        public string AreaOfInterest
        {
            get
            {
                return areaOfInterest;
            }

            set
            {
                areaOfInterest = value;
            }
        }

        public string ProgramOfInterest
        {
            get
            {
                return programOfInterest;
            }

            set
            {
                programOfInterest = value;
            }
        }

        public string DialerListName
        {
            get
            {
                return dialerListName;
            }

            set
            {
                dialerListName = value;
            }
        }
        public String ModificationDate
        {
            get
            {
                return modificationDate;
            }

            set
            {
                modificationDate = value;
            }
        }

        public string Company
        {
            get
            {
                return company;
            }

            set
            {
                company = value;
            }
        }

        public string GlassPanelLeadId
        {
            get
            {
                return glassPanelLeadId;
            }

            set
            {
                glassPanelLeadId = value;
            }
        }
        public bool CheckDoNotCall
        {
            get
            {
                return checkDoNotCall;
            }

            set
            {
                checkDoNotCall = value;
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

        public string UserAgreement
        {
            get
            {
                return userAgreement;
            }

            set
            {
                userAgreement = value;
            }
        }

        public string LeadSource
        {
            get
            {
                return leadSource;
            }

            set
            {
                leadSource = value;
            }
        }

        public string LeadSourceUrl
        {
            get
            {
                return leadSourceUrl;
            }

            set
            {
                leadSourceUrl = value;
            }
        }
        public string LeadInitiatingUrl
        {
            get
            {
                return leadInitiatingUrl;
            }

            set
            {
                leadInitiatingUrl = value;
            }
        }

        public string ProspectSourceUrl
        {
            get
            {
                return prospectSourceUrl;
            }

            set
            {
                prospectSourceUrl = value;
            }
        }
        public string ProspectInitiatingUrl
        {
            get
            {
                return prospectInitiatingUrl;
            }

            set
            {
                prospectInitiatingUrl = value;
            }
        }

        
        public int ProspectId
        {
            get
            {
                return prospectId;
            }

            set
            {
                prospectId = value;
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

        public int? ClientRelationContactId
        {
            get
            {
                return clientRelationContactId;
            }
            set
            {
                clientRelationContactId = value;
            }
        }

        public string MatchResponseGuid
        {
            get
            {
                return matchResponseGuid;
            }
            set
            {
                matchResponseGuid = value;
            }
        }

        public List<KeyValuePair<int, string>> ProgramProducts
        {
            get
            {
                return programProducts;
            }
            set
            {
                programProducts = value;
            }
        }

        public string LeadJson
        {
            get
            {
                return leadJson;
            }
            set
            {
                leadJson = value;
            }
        }

        public List<int?> PaidStatusTypes
        {
            get
            {
                return paidStatusTypes;
            }
            set
            {
                paidStatusTypes = value;
            }
        }

        public string DialerKey
        {
            get
            {
                return dialerKey;
            }
            set
            {
                dialerKey = value;
            }
        }
        public string StoreFront
        {
            get
            {
                return storeFront;
            }
            set
            {
                storeFront = value;
            }
        }

        public string TSR
        {
            get
            {
                return tsr;
            }
            set
            {
                tsr = value;
            }
        }
        public string WarmTransferSchool
        {
            get
            {
                return warmTransferSchool;
            }
            set
            {
                warmTransferSchool = value;
            }
        }

        public string AgentName
        {
            get
            {
                return agentName;
            }
            set
            {
                agentName = value;
            }
        }

        public int? AgentId
        {
            get { return agentId; }
            set { agentId = value; }
        }

        public string SS1
        {
            get
            {
                return ss1;
            }
            set
            {
                ss1 = value;
            }
        }

        public string SS2
        {
            get
            {
                return ss2;
            }
            set
            {
                ss2 = value;
            }
        }

        public string SourceCode
        {
            get
            {
                return sourceCode;
            }
            set
            {
                sourceCode = value;
            }
        }

        public string VideoUrl
        {
            get
            {
                return videoUrl;
            }
            set
            {
                videoUrl = value;
            }
        }
    }
}
