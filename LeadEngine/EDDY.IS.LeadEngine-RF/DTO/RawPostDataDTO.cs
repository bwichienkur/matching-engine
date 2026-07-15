using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.LeadEngine.DTO
{
    [DataContract]
    public partial class RawPostDataDTO
    {
        [DataMember]
        public long RawPostDataID { get; set; }
        [DataMember]
        public string RemoteIp { get; set; }
        [DataMember]
        public string BrowserInfo { get; set; }
        [DataMember]
        public string Referer { get; set; }
        [DataMember]
        public string PostData { get; set; }
        [DataMember]
        public Nullable<System.Guid> TransactionId { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public System.DateTime UpdatedDate { get; set; }
        [DataMember]
        public int UpdatedBy { get; set; }
        [DataMember]
        public System.Guid RowGuid { get; set; }
    }
}
