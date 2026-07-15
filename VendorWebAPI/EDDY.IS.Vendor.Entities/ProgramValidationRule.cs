using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class ProgramValidationRule
    {
     
        private string ruleName;
        private object ruleValue;
        private string fieldName;

    

        public string RuleName
        {
            get
            {
                return ruleName;
            }

            set
            {
                ruleName = value;
            }
        }

        public object RuleValue
        {
            get
            {
                return ruleValue;
            }

            set
            {
                ruleValue = value;
            }
        }

        public string FieldName
        {
            get
            {
                return fieldName;
            }

            set
            {
                fieldName = value;
            }
        }
    }
}
