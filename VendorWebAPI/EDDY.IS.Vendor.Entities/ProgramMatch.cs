using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class ProgramMatch
    {

        private int programId;

        private string programName;

        private int campusId;

        private string campusName;

        private string campusType;
        private string transferNumber;

        private decimal estimatedRevShare;

        private string userAgreement;

        private List<FormTemplateField> additionalQuestions;

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

        public string ProgramName
        {
            get
            {
                return programName;
            }

            set
            {
                programName = value;
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

        public string CampusName
        {
            get
            {
                return campusName;
            }

            set
            {
                campusName = value;
            }
        }

        public string CampusType
        {
            get
            {
                return campusType;
            }

            set
            {
                campusType = value;
            }
        }

        public List<FormTemplateField> AdditionalQuestions
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



        public string TransferNumber
        {
            get { return transferNumber; }
            set { transferNumber = value; }
        }

        public decimal EstimatedRevShare
        {
            get
            {
                return estimatedRevShare;
            }

            set
            {
                estimatedRevShare = value;
            }
        }

        public string UserAgreement
        {
            get { return userAgreement; }
            set { userAgreement = value; }
        }
    }
}
