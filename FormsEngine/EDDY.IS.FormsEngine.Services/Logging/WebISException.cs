using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.FormsEngine.Services.Logging
{
    public class WebISException
    {
        public static void LogException(HttpRequestBase Request,Exception ex, params object[] arguments)
        {
            new ISException(ex,string.Format("UrlReferrer: {0}\n\rUserAgent: {1}",Request.UrlReferrer,Request.UserAgent),arguments).Save();
        }

        public static void LogException(HttpRequestBase Request, Base.ISApplication application, Exception ex, params object[] arguments)
        {
            new ISException(application, ex, string.Format("UrlReferrer: {0}\n\rUserAgent: {1}", Request.UrlReferrer, Request.UserAgent), arguments).Save();
        }

        public static void LogClientException(HttpRequestBase Request, string Title, string URL, string DebugInfo, string Exception, string ExceptionDetail)
        {
            var ClientException = new ISClientException();
            ClientException.Title = Title;
            ClientException.URL = URL;
            ClientException.ApplicationID = EDDY.IS.Base.ISApplication.FormsEngine;
            ClientException.Referer = Request.UrlReferrer!=null ? Request.UrlReferrer.ToString() : "";
            ClientException.BrowserInfo = Request.UserAgent;
            ClientException.Exception = Exception;
            ClientException.ExceptionDetail = ExceptionDetail;
            ClientException.DebugInfo = DebugInfo;
            ClientException.Save();
        }
    }
}