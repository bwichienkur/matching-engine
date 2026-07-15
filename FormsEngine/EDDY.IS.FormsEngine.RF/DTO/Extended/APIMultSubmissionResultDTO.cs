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
    public class APIMultiSubmissionResultDTO
    {
        [DataMember]
        public bool AllValid { get; set; }

        [DataMember]
        public List<APISubmissionResultDTO> SubmissionResults { get; set; }

        public APIMultiSubmissionResultDTO()
        {
            SubmissionResults = new List<APISubmissionResultDTO>();
            AllValid = true;
        }
    }
}
