using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class ProgramFormValidationRules
    {
        private List<FormTemplateField> fields;
        private List<ProgramValidationRule> businessRules;
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
        public List<ProgramValidationRule> BusinessRules
        {
            get
            {
                return businessRules;
            }

            set
            {
                businessRules = value;
            }
        }
    }
}
