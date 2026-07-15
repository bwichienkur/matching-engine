using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
namespace EDDY.IS.Vendor.Entities
{
    public class ProgramCategory: Category
    {
        private List<Subject> subjects;
        [DataMember(Order = 4)]
        public List<Subject> Subjects
        {
            get
            {
                return subjects;
            }

            set
            {
                subjects = value;
            }
        }
    }
}
