using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Util.Memory;
using EDDY.IS.Util.StringExtensions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class SessionController : SessionControllerBase
    {
        


        [HttpGet]
        public ActionResult GetSessionId()
        {
            return new JsonpResult(Guid.NewGuid().ToString());
        }

        [HttpGet]
        public ActionResult GetClonedSessionId(string CFESessionId)
        {
            string NewSession = Guid.NewGuid().ToString();

            //Clone Basic Wizard session values
            CloneSessionObjectByKey(CFESessionId, NewSession, "WFORM");
            CloneSessionObjectByKey(CFESessionId, NewSession, "WFORM_DynamicQuestions");
            
            return new JsonpResult(NewSession);
        }

        [HttpGet]
        public ActionResult GetObject(string FESessionId, string Key)
        {
            object Result = null;

            try
            {
                Result = FESession.Get(FESessionId, Key);
                Result = Result == null ? "" : Result;
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult PingSession(string FESessionId)
        {
            object Result = null;

            try
            {
                Result = FESession.GetDictionary(FESessionId);
                ExtendRedisSessionCache(FESessionId);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(true);
        }


        [HttpGet]
        public ActionResult SetObject(string FESessionId, string Key, object Value)
        {
            try
            {
                if (JsonConvert.SerializeObject(Value).Length <= MAX_SIZE)
                {
                    FESession.Set(FESessionId, Key, Value);
                }

                if (Key==Constants.FORMFIELDS_SESSION||Key==Constants.FORMFIELDS_SESSION_ADDITIONAL||Key==Constants.INITIATING_URL||Key==Constants.LANDING_URL)
                {
                    SetRedisSessionCache(FESessionId, JsonConvert.SerializeObject(GetWizardFormValues(FESessionId)));
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult("");
        }

        [HttpGet]
        public ActionResult GetAdditionalQAObject(string FESessionId)
        {
            object Result = null;

            try
            {
                Result = FESession.Get(FESessionId, Constants.WIZARD_ADDITIONALQACOLLECTION_KEY);
                Result = Result == null ? "" : Result;
            }
            catch (Exception ex)
            {
                new ISException(ex).Save();
            }

            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult SetAdditionalQAObject(string FESessionId, string Value)
        {
            try
            {
                FESession.Set(FESessionId, Constants.WIZARD_ADDITIONALQACOLLECTION_KEY, Value);
            }
            catch (Exception ex)
            {
                new ISException(ex).Save();
            }
            return new JsonpResult("");
        }

        [HttpGet]
        public ActionResult SetWorkflowStatus(string FESessionId, FormsEngineWorkflowStatus Value)
        {
            SetObject(FESessionId, Constants.WORKFLOW_SESSIONKEY, Value);

            try
            {
                SetRedisSessionCache(FESessionId, JsonConvert.SerializeObject(GetWizardFormValues(FESessionId)));
            }
            catch { }
            
            return new JsonpResult("");
        }

        [HttpGet]
        public ActionResult GetWorkflowStatus(string FESessionId)
        {
            return GetObject(FESessionId, Constants.WORKFLOW_SESSIONKEY);
        }

        [HttpGet]
        public ActionResult GetFormSessionValues(string FESessionId, string fieldKeys, bool isWizard)
        {
            List<KeyValuePair<string, string>> Result = new List<KeyValuePair<string, string>>();
            fieldKeys = fieldKeys ?? "";
            string []Fields = fieldKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string FormValues = string.Empty, FormAdditionalValues = string.Empty;
            List<string> SchoolsToBeExcludedByAds = new List<string>();
            List<string> LeadCreatedProducts = new List<string>();

            if (isWizard)
            {
                try
                {
                    FormValues = ((string[])FESession.Get(FESessionId, Constants.FORMFIELDS_SESSION))[0];
                }
                catch { }

                try
                {
                    FormAdditionalValues = ((string[])FESession.Get(FESessionId, Constants.FORMFIELDS_SESSION_ADDITIONAL))[0];
                }
                catch { }

                try
                {
                    SchoolsToBeExcludedByAds = FESession.Get<List<string>>(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY);
                    SchoolsToBeExcludedByAds = SchoolsToBeExcludedByAds ?? new List<string>();
                }
                catch { }

                try
                {
                    LeadCreatedProducts = FESession.Get<List<string>>(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY);
                    LeadCreatedProducts = LeadCreatedProducts ?? new List<string>();
                }
                catch { }
            }
            else 
            {
                try
                {
                    FormValues = ((string[])FESession.Get(FESessionId, Constants.PROGRAMTEMPLATEFORMFIELDS_SESSION))[0];
                }
                catch { }
            }


            FormValues = FormValues ?? string.Empty;
            FormAdditionalValues = FormAdditionalValues ?? string.Empty;
            Dictionary<string,string> dict = FormValues.BuildCaseInsensitiveDictionary();
            Dictionary<string, string> dictAdditional = FormAdditionalValues.BuildCaseInsensitiveDictionary();

            foreach (var field in Fields)
            {
                if (field.ToLower() == "schoolsselected")
                { 
                    Result.Add(new KeyValuePair<string, string>(field,string.Join(",", SchoolsToBeExcludedByAds)));
                }
                else if (field.ToLower() == "leadcreatedproduct")
                {
                    Result.Add(new KeyValuePair<string, string>(field, string.Join(",", LeadCreatedProducts)));
                }
                else if (dictAdditional.ContainsKey(field))
                {
                    Result.Add(new KeyValuePair<string, string>(field, Server.UrlDecode(dictAdditional[field])));
                }
                else if(dict.ContainsKey(field))
                {
                    Result.Add(new KeyValuePair<string, string>(field, Server.UrlDecode(dict[field])));
                }
                else
                {
                    Result.Add(new KeyValuePair<string, string>(field, ""));
                }
                if (field.ToLower() == "formleadurl")
                {
                    if (FESession.Get(FESessionId, "FormLeadUrl") != null)
                    {
                        string[] formLeadUrl = (string[])FESession.Get(FESessionId, "FormLeadUrl");
                        if (formLeadUrl != null && formLeadUrl.Length > 0) {
                            Result.Add(new KeyValuePair<string, string>(field, formLeadUrl[0]));
                        }
                    }
                }
                if (field.ToLower() == "leadsourceurl")
                {
                    if (FESession.Get(FESessionId, "LeadSourceUrl") != null)
                    {
                        string[] leadSourceUrl = (string[])FESession.Get(FESessionId, "LeadSourceUrl");
                        if (leadSourceUrl != null && leadSourceUrl.Length > 0)
                        {
                            Result.Add(new KeyValuePair<string, string>(field, leadSourceUrl[0]));
                        }
                    }
                }

            }
            
            return new JsonpResult(Result);
        }


        [HttpGet]
        public string GetIframePixels(string FESessionId, string PixelAction)
        {
            StringBuilder PixelPage = new StringBuilder();
            PixelPage.AppendLine("<html><head><title>FormsEngine GetIframePixels</title><script type='text/javascript' src='//ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js'></script></head><body>");
            PixelPage.AppendLine("<!-- Action" + PixelAction + " DateTime: " + DateTime.Now.ToString() + "-->");

            switch (PixelAction.ToLower())
            {
                case "loadpixelsforextprogramtemplatetypages": //ProgramTemplates pixels displayed on CrosSell
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.PROGRAMTEMPLATE_INITIALEADPIXELS_KEY));
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLPIXELS_KEY));
                    break;

                case "loadinitialpixels": //ProgramTemplates pixels displayed on CrosSell
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.PROGRAMTEMPLATE_INITIALEADPIXELS_KEY));
                    break;

                case "loadpixelsforwizardtypages": //Wizard pixels on ThankYou pages
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.WIZARD_SMARTMATCHPIXELS_KEY));
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.WIZARD_USERSELECTPIXELS_KEY));
                    break;

                case "loadpixelsforwizardmchoicepages": //Wizard pixels on Managed choice pages
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.WIZARD_SMARTMATCHPIXELS_KEY));
                    break;

                case "loadpixelsforwizardadvisingflow": //Wizard pixels on No Match pages
                    PixelPage.AppendLine(GetPixel(FESessionId, Constants.WIZARD_ADVISINGFLOWPIXELS_KEY));
                    break;
            }
            PixelPage.AppendLine("</body></html>");
            Response.ContentType = "text/HTML";
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache");

            return PixelPage.ToString();
        }


        public string GetPixel(string FESessionId, string PixelKey)
        {
            StringBuilder PixelBlock = new StringBuilder();
            string Pixel = "";

            PixelBlock.AppendLine(string.Format("<!-- Start {0} -->", PixelKey));
            Pixel = (string)FESession.Get(FESessionId, PixelKey);
            Pixel = string.IsNullOrWhiteSpace(Pixel) ? "" : Pixel;
            PixelBlock.AppendLine(Pixel);
            FESession.Set(FESessionId, PixelKey, "");
            PixelBlock.AppendLine(string.Format("<!-- End {0} -->", PixelKey));

            return PixelBlock.ToString();
        }


        public bool CloneSessionObjectByKey(string SourceFESessionId, string DestinationFESessionId, string Key)
        {
            bool Result = false;

            try
            {
                object Current = FESession.Get(SourceFESessionId, Key);
                if (Current != null)
                {
                    object Cloned = GenericCopier<object>.DeepCopy(Current);
                    SetObject(DestinationFESessionId, Key, Cloned);
                    Result = true;
                }
            }
            catch (Exception ex) 
            {
                new ISException(ex).Save();
            }

            return Result;
        }
    }
}
