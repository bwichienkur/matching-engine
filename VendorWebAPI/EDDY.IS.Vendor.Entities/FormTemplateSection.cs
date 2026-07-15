using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class FormTemplateSection
    {
        private int sectionId;
        private int sequence;
        private List<FormTemplateField> fields;

        public int SectionId
        {
            get
            {
                return sectionId;
            }

            set
            {
                sectionId = value;
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
        public List<FormTemplateField> Fields
        {
            get
            {
                return fields;
            }

            set
            {
                fields = value;
            }
        }
    }
}
