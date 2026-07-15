using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchResponseReplay.Console.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchResponseReplay.Console
{
    public class ReplayApp
    {
        MELogDataService DataService = new MELogDataService();

        const string ME_JSON_GETCAMPUSES_WRAPPER = "{{\"directoryMatchRequest\":{0}}}";
        const string ME_JSON_GETFORMPROGRAMS_WRAPPER = "{{\"directoryMatchRequest\":{0}}}";
        const string ME_JSON_GETINSTITUTIONS_WRAPPER = "{{\"directoryMatchRequest\":{0}}}";
        const string ME_JSON_GETNEORESPONSE_WRAPPER = "{{\"neoRequest\":{0}}";
        const string ME_JSON_GETPROGRAMS_WRAPPER = "{{\"directoryMatchRequest\":{0}}}";
        const string ME_JSON_GETPROGRAMSFORCROSSELL_WRAPPER = "{{\"crossSellRequest\":{0}}}";
        const string ME_JSON_GETWIZARDMATCHES_WRAPPER = "{{\"wizardMatchRequest\":{0}}}";
        public bool Replay(int MatchResponseID)
        {
            bool Result = false;
            Guid fileNameToken = Guid.NewGuid();
            var matchResponse = DataService.GetMatchResponse(MatchResponseID);
            if (matchResponse != null && !string.IsNullOrWhiteSpace(matchResponse.RequestInput))
            {
                try
                {
                    BaseMatchRequest baseMatchRequest = JsonConvert.DeserializeObject<BaseMatchRequest>(matchResponse.RequestInput);
                }
                catch
                {
                    System.Console.WriteLine("Not able to parse BaseMatchRequest");
                    return Result;
                }
                var requestPayload = WrapJson(matchResponse.RequestInput, matchResponse.RequestMethodName);
                var serviceResult = CallService(matchResponse.RequestMethodName, requestPayload);
                System.IO.File.WriteAllText($"Log.{matchResponse.RequestMethodName}.{{{fileNameToken}}}.Request.txt", requestPayload);
                System.IO.File.WriteAllText($"Log.{matchResponse.RequestMethodName}.{{{fileNameToken}}}.Response.txt", serviceResult.Content.ReadAsStringAsync().Result);
                System.Console.WriteLine($"Result Code :{serviceResult.StatusCode}");
            }
            else
            {
                System.Console.WriteLine("ERROR: MatchResponseID not found.");
            }
            return Result;
        }

        private HttpResponseMessage CallService(string method, string request)
        {
            var ServiceURL = ConfigurationManager.AppSettings["MEServiceURL"];
            StringContent stringContent = new StringContent(request, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            Task<HttpResponseMessage> result =  httpClient.PostAsync($"{ServiceURL}/json/{method}", stringContent);
            result.Wait();
            return result.Result;
        }

        private string WrapJson(string payload, string serviceMethod)
        {
            string Result = string.Empty;
            switch(serviceMethod)
            {
                case "GetCampuses": Result = string.Format(ME_JSON_GETCAMPUSES_WRAPPER, payload); break;
                case "GetFormPrograms": Result = string.Format(ME_JSON_GETFORMPROGRAMS_WRAPPER, payload); break;
                case "GetInstitutions": Result = string.Format(ME_JSON_GETINSTITUTIONS_WRAPPER, payload); break;
                case "GetNeoResponse": Result = string.Format(ME_JSON_GETNEORESPONSE_WRAPPER, payload); break;
                case "GetPrograms": Result = string.Format(ME_JSON_GETPROGRAMS_WRAPPER, payload); break;
                case "GetProgramsForCrossSell": Result = string.Format(ME_JSON_GETPROGRAMSFORCROSSELL_WRAPPER, payload); break;
                case "GetWizardMatches": Result = string.Format(ME_JSON_GETWIZARDMATCHES_WRAPPER, payload); break;
            }

            return Result;
        }
    }
}
