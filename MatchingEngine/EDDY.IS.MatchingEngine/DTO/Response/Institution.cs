using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Institution : BaseMatchEntity
    {
        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public List<string> StartDateList { get; set; }

        [DataMember]
        public string InstitutionDescription { get; set; }

        [DataMember]
        public string InstitutionDescriptionInternational { get; set; }

        [DataMember]
        public DisclaimerType? InstitutionDisclaimerType { get; set; }

        [DataMember]
        public string InstitutionDisclaimer { get; set; }

        [DataMember]
        public int ProgramCount { get; set; }

        [DataMember]
        public List<SFProductCode> InstitutionSFProductCodeList { get; set; }

        [DataMember]
        public List<Image> ImageList { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }

        public bool IsGeotargeted { get; set; }

        public int? SABSRAPosition_CR { get; set; }

        [DataMember]
        public bool InquiryDisabled { get; set; }

        [DataMember]
        public int? AdvertiserId { get; set; }
        
        [DataMember]
        public decimal? EstimatedRevShare { get; set; }

        [DataMember]
        public bool IsNationalOnlineAdvertiser{ get; set; }

        [DataMember]
        public int CRId { get; set; }
    }
}