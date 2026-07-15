using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using EDDY.IS.EmsLeadEngine.Entities;
using Newtonsoft.Json;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
		
		public void SendLeadsToEmsLeadService(List<int> leadIds)
        {
            if (leadIds != null && leadIds?.Count() > 0)
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;

                    var leadServiceUrl = ConfigurationManager.AppSettings.Get("EmsLeadEngineCreateFromISUrl");
                    var authorizationToken = new Guid(ConfigurationManager.AppSettings.Get("EmsLeadEngineAuthToken"));
                    var transactionId = Guid.NewGuid();

                    var request = new ISLeadCreateRequest()
                    {
                        ISLeadIds = leadIds,
                        AuthenticationToken = authorizationToken,
                        TransactionId = transactionId
                    };

                    string requestData = JsonConvert.SerializeObject(request);

                    try
                    {
                        var response = client.UploadString(leadServiceUrl, "POST", requestData);
                    }
                    catch (Exception ex)
                    {
                        ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex, "Request Sent To EMS Lead Service", requestData);
                        isEx.Save();
                    }
                }
            }
        }

    }
}
