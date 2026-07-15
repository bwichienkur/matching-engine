using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class FormTemplate
    {
        //private int formId;
        private List<FormTemplateField> fields;
        //private List<ProgramValidationRule> businessRules;

        //public int FormId
        //{
        //    get
        //    {
        //        return formId;
        //    }

        //    set
        //    {
        //        formId = value;
        //    }
        //}

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
        //public List<ProgramValidationRule> BusinessRules
        //{
        //    get
        //    {
        //        return businessRules;
        //    }

        //    set
        //    {
        //        businessRules = value;
        //    }
        //}
    }
}
