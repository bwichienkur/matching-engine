using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class FormTemplateField
    {
        private string label;

        private string name;

        private List<ProgramValidationRule> rules;

        private string inputType;

        private string validationRegularExpresion;

     

        private List<FormTemplateFieldOption> formTemplateFieldOptions;

        //private int sequence;

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public List<ProgramValidationRule> Rules
        {
            get
            {
                return rules;
            }

            set
            {
                rules = value;
            }
        }
        public string InputType
        {
            get
            {
                return inputType;
            }

            set
            {
                inputType = value;
            }
        }
        [JsonIgnore]
        public string ValidationRegularExpresion
        {
            get
            {
                return validationRegularExpresion;
            }

            set
            {
                validationRegularExpresion = value;
            }
        }
       

        public List<FormTemplateFieldOption> FormTemplateFieldOptions
        {
            get
            {
                return formTemplateFieldOptions;
            }

            set
            {
                formTemplateFieldOptions = value;
            }
        }

        //public int Sequence
        //{
        //    get
        //    {
        //        return sequence;
        //    }

        //    set
        //    {
        //        sequence = value;
        //    }
        //}


    }
}
