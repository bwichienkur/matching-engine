using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class QuestionAnswer
    {
        private string questionKey;
        private string questionValue;
        public string QuestionKey
        {
            get
            {
                return questionKey;
            }

            set
            {
                questionKey = value;
            }
        }

       
        public string QuestionValue
        {
            get
            {
                return questionValue;
            }

            set
            {
                questionValue = value;
            }
        }
    }
}
