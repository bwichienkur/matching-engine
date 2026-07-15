using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Services;
using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class RawPostDataService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IIPAddressService _ipAddressService;

        public RawPostDataService(HttpContextBase httpContext, IIPAddressService ipAddressService)
        {
            _httpContext = httpContext;
            _ipAddressService = ipAddressService;
        }

        public RawPostDataDTO GetRawPostDataDTO(string leadData)
        {
            var rawPostData = new RawPostDataDTO();
            rawPostData.PostData = leadData;
            rawPostData.Referer = _httpContext.Request.ServerVariables["HTTP_REFERER"];
            rawPostData.BrowserInfo = _httpContext.Request.ServerVariables["HTTP_USER_AGENT"];
            rawPostData.RemoteIp = _ipAddressService.GetIPAddress(_httpContext.Request.ServerVariables["HTTP_VIA"], _httpContext.Request["HTTP_X_FORWARDED_FOR"], _httpContext.Request["REMOTE_ADDR"]);
            return rawPostData;
        }
    }
}
