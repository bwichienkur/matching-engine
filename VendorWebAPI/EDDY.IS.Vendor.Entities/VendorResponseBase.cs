using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class VendorResponseBase
    {

        public enum OperationType { Match, Search, ProspectSave, LeadSave, Validation, Exception, GPFive9LeadSave, LeadUpdate }

        private bool isSuccessful;
        private Object body;
        private Guid responseGuid;
        private DateTime responseDateTime;
        private DateTime requestDateTime;
        private int totalResponseTime;
        private OperationType operation;



        private List<VendorResponseMessage> messages;
        private List<string> messageCodes;

        [JsonIgnore]
        public OperationType Operation
        {
            get
            {
                return operation;
            }

            set
            {
                operation = value;
            }
        }

        public Boolean IsSuccessful
        {
            get
            {
                return isSuccessful;
            }

            set
            {
                isSuccessful = value;
            }
        }

        public Object Body
        {
            get
            {
                return body;
            }

            set
            {
                body = value;
            }
        }

        public DateTime ResponseDateTime
        {
            get
            {
                return responseDateTime;
            }

            set
            {
                responseDateTime = value;
            }
        }

        public DateTime RequestDateTime
        {
            get
            {
                return requestDateTime;
            }

            set
            {
                requestDateTime = value;
            }
        }

        public Guid ResponseGuid
        {
            get
            {
                return responseGuid;
            }

            set
            {
                responseGuid = value;
            }
        }

        public List<VendorResponseMessage> Messages
        {
            get
            {
                return messages;
            }

            set
            {
                messages = value;
            }
        }

        [JsonIgnore]
        public List<string> MessageCodes
        {
            get
            {
                return messageCodes;
            }
            set
            {
                messageCodes = value;
            }
        }

        public int TotalResponseTime
        {
            get
            {

                return totalResponseTime;
            }

            set
            {
                totalResponseTime = value;
            }


        }

    }
}

