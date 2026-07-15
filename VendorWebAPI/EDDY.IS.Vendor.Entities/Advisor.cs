namespace EDDY.IS.Vendor.Entities
{
    public class Advisor
    {
        private int advisorId;

        private string loginId;

        private string firstName;

        private string lastName;

        private string email;
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

        public string LoginId
        {
            get
            {
                return loginId;
            }

            set
            {
                loginId = value;
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
    }
}
