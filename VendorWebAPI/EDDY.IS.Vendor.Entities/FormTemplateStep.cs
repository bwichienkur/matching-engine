using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class FormTemplateStep
    {
        private int stepId;
        private int sequence;
        private List<FormTemplateSection> sections;

        public int StepId
        {
            get
            {
                return stepId;
            }

            set
            {
                stepId = value;
            }
        }
        public int Sequence
        {
            get
            {
                return sequence;
            }

            set
            {
                sequence = value;
            }
        }
        public List<FormTemplateSection> Sections
        {
            get
            {
                return sections;
            }

            set
            {
                sections = value;
            }
        }
    }
}
