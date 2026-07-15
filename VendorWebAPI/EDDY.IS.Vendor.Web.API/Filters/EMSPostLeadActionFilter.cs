using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Common.Utilities;
using Newtonsoft.Json;
using EDDY.IS.Common.ExceptionHandler;
namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class EMSPostLeadActionFilter : ActionFilterBase
    {
        private VendorCampaigns _vendorCampaigns = new VendorCampaigns();
        private const int _emsApplicationId = 27;
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            VendorResponseBase responseContent = new VendorResponseBase();
            responseContent.ResponseGuid = Guid.NewGuid();
            responseContent.RequestDateTime = DateTime.Now;
            responseContent.IsSuccessful = false;

            responseContent.Messages = new List<VendorResponseMessage>();
            NameValueCollection requestValues = null;
            switch (actionContext.Request.Method.Method)
            {
                case "GET":
                    requestValues = actionContext.Request.GetRequestQueryParameters();
                    break;
                case "POST":
                    requestValues = actionContext.Request.GetRequestJsonBodyParameters();
                    break;
            }
            //  DirectoryRequest request = actionContext.Request.GetRequestJsonBodyDirectoryRequest();
            bool isValid = true;
            var apiKey = Guid.Parse(requestValues["APIKey"]);
            var leadIdToken = requestValues["LeadIdToken"];

            VendorCampaign vendorCampaign = GetVendorCampaignByTrackId(apiKey);
            Guid leadIdTokenGuid = Guid.Empty;

            if (string.IsNullOrEmpty(leadIdToken) && vendorCampaign?.ApplicationId != _emsApplicationId)
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.LeadIdTokenRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isValid = false;
            }

            if (!string.IsNullOrEmpty(leadIdToken) && !Guid.TryParse(leadIdToken, out leadIdTokenGuid))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.LeadIdTokenNotGuid);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isValid = false;
            }

            int programIdInt;
            var programId = requestValues["ProgramId"];
            if (string.IsNullOrEmpty(programId))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ProgramIdRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isValid = false;
            }
            else if (!int.TryParse(programId, out programIdInt))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ProgramIdIsNotNumeric);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isValid = false;
            }

            string firstName = requestValues["FirstName"] ?? "";
            if (firstName.Length > 25)
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FirstNameMaxLength);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }

            string lastName = requestValues["LastName"] ?? "";
            if (lastName.Length > 25)
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.LastNameMaxLength);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }

            string address = requestValues["Address"] ?? "";
            if (!string.IsNullOrEmpty(address))
            {
                if (address.Length > 50)
                {
                    //VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.Address1MaxLength);
                    //if (message != null)
                    //{
                    //    responseContent.Messages.Add(message);
                    //}
                }
            }

            string address2 = requestValues["Address2"] ?? "";
            if (!string.IsNullOrEmpty(address2))
            {
                if (address2.Length > 50)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.Address2MaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string city = requestValues["City"] ?? "";
            if (!string.IsNullOrEmpty(city))
            {
                if (city.Length > 25)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CityMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string country = requestValues["Country"] ?? "";
            bool isCountryValid = true;
            Country lookedUpCountry = null;
            if (country.Length > 2)
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CountryMaxLength);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isCountryValid = false;
            }

            if (isCountryValid)
            {
                lookedUpCountry = locations.GetAllCountries().Where(c => c.CountryCode == country.ToUpper()).FirstOrDefault();
                if (lookedUpCountry == null)
                {
                    if (!string.IsNullOrEmpty(country))
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CountryIsInvalid);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                    isCountryValid = false;
                }

            }

            string state = requestValues["State"] ?? "";
            bool isStateValid = true;
            State lookedUpState = null;
            if (state.Length > 2)
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StateMaxLength);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isStateValid = false;
            }

            if (isStateValid)
            {
                lookedUpState = locations.GetAllStates().Where(s => s.StateCode == state.ToUpper()).FirstOrDefault();
                if (lookedUpState == null)
                {
                    if (!string.IsNullOrEmpty(state))
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StateInvalid);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                    isStateValid = false;
                }

            }


            string postalCode = requestValues["PostalCode"] ?? "";
            if (!string.IsNullOrEmpty(postalCode))
            {
                if (postalCode.Length > 20)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PostalCodeMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            if (isStateValid && isCountryValid && !string.IsNullOrEmpty(postalCode))
            {
                if (country.ToLower() == "us")
                {
                    if (!InputValidation.IsValidUSPostalCode(postalCode))
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NotAValidUSPostalCode);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                }
                else if (country.ToLower() == "ca")
                {
                    var strippedPostalCode = postalCode.Replace(" ", ""); //strip out spaces from postal code because candian postal codes may contain spaces
                    if (!InputValidation.IsValidCanadaPostalCode(postalCode))
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NotAValidCanadianPostalCode);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                    else if (!locations.PostalCodeExistsWithinState(postalCode, state, NorthAmericanCountry.Canada))
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PostalStateMisMatch);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                }
            }


            string phone = requestValues["Phone"] ?? "";
            string phoneNumbersDncCheck = "";
            bool isPhoneValid = true;
            if (!string.IsNullOrEmpty(phone))
            {
                if (phone.Length > 20)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isPhoneValid = false;
                }

                if (this.isInputNotNumeric(phone))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneNotNumeric);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isPhoneValid = false;
                }
                if (isPhoneValid)
                {
                    phoneNumbersDncCheck = phone;
                }
            }

            string alternatePhone = requestValues["AlternatePhone"] ?? "";
            bool isAlternatePhoneValid = true;
            if (!string.IsNullOrEmpty(alternatePhone))
            {
                if (alternatePhone.Length > 20)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.AlternatePhoneMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isAlternatePhoneValid = false;
                }

                if (this.isInputNotNumeric(alternatePhone))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.AlternatePhoneNotNumeric);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isAlternatePhoneValid = false;
                }
                if (isAlternatePhoneValid && !string.IsNullOrEmpty(alternatePhone))
                {
                    phoneNumbersDncCheck = !String.IsNullOrEmpty(phoneNumbersDncCheck) ? phoneNumbersDncCheck + "," + alternatePhone : String.Empty;
                }
            }
            if (!string.IsNullOrEmpty(phoneNumbersDncCheck))
            {
                string checkDoNotCall = requestValues["CheckDoNotCall"];
                if (!string.IsNullOrEmpty(checkDoNotCall))
                {
                    Boolean checkDoNotCallBool = Convert.ToBoolean(checkDoNotCall);
                    if (checkDoNotCallBool)
                    {
                        //todo: This Service does not seem to be working need to invesitgate
                        if (InputValidation.IsPhoneNumberDoNotCall(phoneNumbersDncCheck))
                        {
                            VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.AlternatePhoneNotNumeric);
                            if (message != null)
                            {
                                responseContent.Messages.Add(message);
                            }
                        }
                    }
                }
            }
            string email = requestValues["Email"] ?? "";
            if (string.IsNullOrEmpty(email))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (email.Length > 75)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }

                }
                if (!InputValidation.IsValidEMSEmail(email))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailIsInvalid);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }

                }
            }

            string militaryAffiliation = requestValues["MilitaryAffiliation"] ?? "";
            if (!string.IsNullOrEmpty(militaryAffiliation))
            {
                int parsedMilitaryAffiliation = 0;
                if (!int.TryParse(militaryAffiliation, out parsedMilitaryAffiliation))
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.MilitaryNotNumeric);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }


            responseContent.ResponseDateTime = DateTime.Now;

            invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
            if (responseContent.Messages.Count > 0)
            {
                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = apiKey;
                //vendorResponseLog.IPAddress = actionContext.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                //logs.LogVendorResponse(vendorResponseLog);
                logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);

                actionContext.Response = invalidHttpResponseMessage;
            }
        }

        private VendorCampaign GetVendorCampaignByTrackId(Guid trackId)
        {
            VendorCampaign vendorCampaign = null;
            try
            {
                vendorCampaign = _vendorCampaigns.GetVendorCampaignByTrackId(trackId);
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.WCF_SERVICE_POLICY);
            }
            return vendorCampaign;
        }
    }
}