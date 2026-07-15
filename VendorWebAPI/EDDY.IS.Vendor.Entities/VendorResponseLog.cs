using System;


namespace EDDY.IS.Vendor.Entities
{
    public class VendorResponseLog : VendorResponseBase
    {
        private string methodName;
        private string ipAddress;
        private Guid apiKey;
        private string requestUrlParameters;
        private string requestBodyParameters;
        private string operationValue;
        private string request;
        private string endPoint;
        private string response;
        private decimal? leadId;
        private int? prospectFlowId;
        private string email;
        private decimal? estimatedRevShare;
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public Guid APIKey
        {
            get
            {
                return apiKey;
            }

            set
            {
                apiKey = value;
            }
        }
        public String RequestUrlParameters
        {
            get
            {
                return requestUrlParameters;
            }

            set
            {
                requestUrlParameters = value;
            }
        }
        public String RequestBodyParameters
        {
            get
            {
                return requestBodyParameters;
            }

            set
            {
                requestBodyParameters = value;
            }
        }
        public String RequestParameters
        {
            get
            {
                return String.Format("RequestBodyParameters={0}RequestUrlParameters={1}", requestBodyParameters, requestUrlParameters);
            }

           
        }

        public String OperationValue
        {
            get
            {
                return operationValue;
            }

            set
            {
                operationValue = value;
            }
        }

        public String Request
        {
            get
            {
                return request;
            }
            set
            {
                request = value;
            }
        }

        public String EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                endPoint = value;
            }
        }

        public String Response
        {
            get
            {
                return response;
            }
            set
            {
                response = value;
            }
        }

        public decimal? LeadId
        {
            get
            {
                return leadId;
            }
            set
            {
                leadId = value;
            }
        }

        public int? ProspectFlowId
        {
            get
            {
                return prospectFlowId;
            }
            set
            {
                prospectFlowId = value;
            }
        }

        public String Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }
        public decimal? EstimatedRevShare
        {
            get
            {
                return estimatedRevShare;
            }

            set
            {
                estimatedRevShare = value;
            }
        }
    }
}
