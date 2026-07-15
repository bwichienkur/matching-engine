using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class TemplateAdditionalQuestionDTO
    {
        [DataMember]
        public int TemplateId { get; set; }

        [DataMember]
        public string RenderedControl { get; set; }

        public TemplateAdditionalQuestionDTO()
        {

        }

        //user passed in a string delimiting the two values with a comma so lets split that and set properly.
        public TemplateAdditionalQuestionDTO(string delimited)
        {
            string[] splt = delimited.Split(',');
            int temp = 0;
            Int32.TryParse(splt[0], out temp);
            this.TemplateId = temp;
            this.RenderedControl = splt[1];
        }
    }
}
