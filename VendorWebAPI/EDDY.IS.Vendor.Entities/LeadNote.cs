using System.ComponentModel.DataAnnotations;
namespace EDDY.IS.Vendor.Entities
{
    public class LeadNote
    {

        private string advisor;
        private string createdDate;
        private string note;

        public string Advisor
        {
            get
            {
                return advisor;
            }

            set
            {
                advisor = value;
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
        [Display(Name = "Add Note:")]
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
    }
}
