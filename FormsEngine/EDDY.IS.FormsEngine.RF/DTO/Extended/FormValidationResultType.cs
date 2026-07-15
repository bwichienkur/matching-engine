using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DTO.Extended
{
    

    public enum FormValidationResultType
    {
        [Description("{0} is a required field.")]
        RequiredField,
        [Description("{0} data type must be {1}.")]
        DataType,
        [Description("{0} was provided in an invalid format.")]
        DataFormat,
        [Description("{0} field length must be between {1} and {2} characters.")]
        FieldLength,
        [Description("{0} contains profanity words.")]
        ProfanityCheck,
        [Description("Invalid Email address.")]
        EmailCheck,
        [Description("Zip Code, Country and State combination is not valid.")]
        ZipCodeStateCountryCheck,
        [Description("{0} is not a valid phone number.")]
        PhoneNumberCheck,
        [Description("{0} selected value is not part of the pre defined list.")]
        SelectedValueCheck,
        [Description("{0} Validation Library Failure: {1}.")]
        ValidationLibrary,
        [Description("Business Rule not passed: {0}")]
        BusinessRulesCheck,
        [Description("No results matching your search were found.")]
        MatchingEngineNoResults,
        [Description("No program templates matching your search were found.")]
        TemplateNoResults,
        [Description("Cap Limit is reached and status is FRAID.")]
        FraidStatus,
        [Description("Date of Birth Is Invalid.")]
        BirthDate,
        [Description("User Agreement is Invalid.")]
        TCPACheck,
    }
}
