using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using EDDY.IS.Vendor.Entities;

namespace EDDY.IS.Vendor.Utilities
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            string ipAddress = null;
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                ipAddress = IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            return ipAddress;
        }
        public static string GetRequestQueryString(this HttpRequestMessage request)
        {
            string requestParams = null;
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {

                requestParams = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
            }
            return requestParams;
        }
        public static NameValueCollection GetRequestQueryParameters(this HttpRequestMessage request)
        {
            return HttpUtility.ParseQueryString(request.RequestUri.Query);
        }
        public static String GetRequestQueryParametersAsString(this HttpRequestMessage request)
        {
            return string.Join("&", HttpUtility.ParseQueryString(request.RequestUri.Query).AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(HttpUtility.ParseQueryString(request.RequestUri.Query)[key]))));

        }
        public static NameValueCollection GetRequestBodyParameters(this HttpRequestMessage request)
        {
            return HttpUtility.ParseQueryString(request.Content.ReadAsStringAsync().Result);
        }
        public static String GetRequestBodyParametersAsString(this HttpRequestMessage request)
        {
            return string.Join("&", HttpUtility.ParseQueryString(request.Content.ReadAsStringAsync().Result).AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(HttpUtility.ParseQueryString(request.Content.ReadAsStringAsync().Result)[key]))));

        }
        public static String GetRequestJsonBodyParametersAsString(this HttpRequestMessage request)
        {
            return request.Content.ReadAsStringAsync().Result;

        }
        public static NameValueCollection GetRequestJsonBodyParameters(this HttpRequestMessage request)
        {
            NameValueCollection nameValueCollection = null;

            string bodyString = request.Content.ReadAsStringAsync().Result;
            if (!String.IsNullOrEmpty(bodyString))
            {
                nameValueCollection = new NameValueCollection();
                Dictionary<string, object> bodyDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodyString);
                foreach (KeyValuePair<string, object> kvp in bodyDictionary)
                {
                    if (kvp.Value != null) {
                        nameValueCollection.Add(kvp.Key.ToString(), kvp.Value.ToString());
                    }
                }

            }

            return nameValueCollection;

        }

        public static DirectoryRequest GetRequestJsonBodyDirectoryRequest(this HttpRequestMessage request)
        {
            DirectoryRequest directoryRequest = null;

            string bodyString = request.Content.ReadAsStringAsync().Result;
            if (!String.IsNullOrEmpty(bodyString))
            {
                
                directoryRequest = JsonConvert.DeserializeObject<DirectoryRequest>(bodyString);
               

            }
            return directoryRequest;

        }

        public static ContactRequest GetRequestJsonBodyContactRequest(this HttpRequestMessage request)
        {
            ContactRequest contactRequest = null;

            string bodyString = request.Content.ReadAsStringAsync().Result;
            if (!String.IsNullOrEmpty(bodyString))
            {

                contactRequest = JsonConvert.DeserializeObject<ContactRequest>(bodyString);


            }
            return contactRequest;

        }
    }
}
