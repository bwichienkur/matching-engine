using EDDY.IS.Base;
using EDDY.IS.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class TemplateResultDTO
    {
        [DataMember]
        public string Template { get; set; }
        
        [DataMember]
        public string CSS { get; set; }
        
        [DataMember]
        public int TemplateId { get; set; }

        [DataMember]
        public int DefaultTemplateId { get; set; }

        [DataMember]
        public FormTemplateTypes FormTemplateType { get; set; }

        [DataMember]
        public int OriginalTemplateId { get; set; }


        [DataMember]
        public bool UseInternationalTemplate { get; set; }

        [DataMember]
        public string InternationalCountryCode { get; set; }

        [DataMember]
        public bool IsLocalIP { get; set; }

        [DataMember]
        public bool ShowAllQuestionsOnFirstStep { get; set; }

    }
}
