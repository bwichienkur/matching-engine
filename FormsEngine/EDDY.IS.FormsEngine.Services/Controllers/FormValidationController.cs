using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Validation;
using EDDY.IS.Validation.DTO;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class FormValidationController : FormValidationControllerBase
    {
        
        [HttpGet]
        public ActionResult ProfanityCheck(string Value)
        {
            bool Result = true;
            try
            {
                Result = Validation.ProfanityCheck(Value);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult PhoneNumberCheck(string PhoneNumber, string CountryCode)
        {
            bool Result = true;
            try
            {
                Result = Validation.PhoneCheck(PhoneNumber, CountryCode);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult EmailCheck(string EmailAddress, int EmailVerificationLevel)
        {
            bool Result = true;
            try
            {
                Result = Validation.EmailCheck(EmailAddress, EmailVerificationLevel);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult EmailCheckEx(string EmailAddress, bool XVerify, bool Immediate, string Experiment, string TrackId)
        {
            Guid TrackIdGuid = new Guid();
            bool Result = true;

            if(!Guid.TryParse(TrackId, out TrackIdGuid))
            {
                TrackIdGuid = Guid.Empty;
            }

            try
            {

                if (XVerify && Immediate)
                {
                    Result = Validation.EmailXverify(EmailAddress, Experiment, TrackIdGuid);
                }
                else if(XVerify)
                {
                    Task.Run(() => Validation.EmailXverify(EmailAddress, Experiment, TrackIdGuid));
                    return EmailCheck(EmailAddress, 3);
                }
                else
                {
                    return EmailCheck(EmailAddress, 3);
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCityStateCountry(string ZipCode)
        {
            List<KeyValuePair<string, string>> Result = null;
            try
            {
                Result = Validation.GetCityStateCountry(ZipCode);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult ZipCodeStateCountryCheck(string ZipCode, string CountryCode, string StateCode, bool ZipOnly = false, int? ApplicationId = null)
        {
            bool Result = true;
            try
            {
                Result = Validation.ZipCodeStateCountryCheck(ZipCode, CountryCode, StateCode, ZipOnly: ZipOnly, ApplicationId: ApplicationId);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult BirthDateCheck(string BirthDate)
        {
            bool Result = true;
            try
            {
                Result = Validation.BirthDateCheck(BirthDate);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetStatesByCountry(string CountryCode)
        {
            List<Tuple<int, string, string>> Result = null;
            try
            {
                Result = Validation.GetStatesByCountry(CountryCode);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult IsMobilePhone(string PhoneNumber)
        {
            bool Result = false;
            try
            {
                string key = "MB_" + PhoneNumber;
                Result = LeadPingService.IsMobile(PhoneNumber) == true;
                FormsEngineCacheProxy.Cache.Set(key, Result, Convert.ToInt32(ConfigurationManager.AppSettings["FormsEngineSessionMinutes"]));
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetISMapping(string set, string category, string source)
        {
            string Result = string.Empty;
            try
            {
                Result = Validation.GetISMapping(set, category, source);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

    }
}
