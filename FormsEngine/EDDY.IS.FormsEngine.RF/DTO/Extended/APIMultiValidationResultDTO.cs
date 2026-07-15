using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class APIMultiValidationResultDTO
    {
        [DataMember]
        public bool AllValid { get; set; }

        [DataMember]
        public List<APIProgramValidationResultDTO> ValidationResults { get; set; }

        public APIMultiValidationResultDTO()
        {
            ValidationResults = new List<APIProgramValidationResultDTO>();
        }
    }

    [DataContract]
    public class APIProgramValidationResultDTO
    {
        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public List<string> ErrorMessages { get; set; }

        public APIProgramValidationResultDTO()
        {
            ErrorMessages = new List<string>();
            IsValid = true;
        }
    }
}
