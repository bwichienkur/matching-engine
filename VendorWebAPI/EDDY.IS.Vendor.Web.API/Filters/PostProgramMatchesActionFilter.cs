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

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class PostProgramMatchesActionFilter : ActionFilterBase
    {
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

            string firstName = requestValues["FirstName"];
            if (string.IsNullOrEmpty(firstName))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FirstNameRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (firstName.Length > 50)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FirstNameMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string lastName = requestValues["LastName"];
            if (string.IsNullOrEmpty(lastName))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.LastNameRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (lastName.Length > 50)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.LastNameMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string address = requestValues["Address"];
            if (string.IsNullOrEmpty(address))
            {

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.Address1Required);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (address.Length > 50)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.Address1MaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string address2 = requestValues["Address2"];
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


            string city = requestValues["City"];
            if (string.IsNullOrEmpty(city))
            {

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CityRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
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
            string country = requestValues["Country"];
            bool isCountryValid = true;
            Country lookedUpCountry = null;
            if (string.IsNullOrEmpty(country))
            {

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CountryRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {


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
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.CountryIsInvalid);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                        isCountryValid = false;
                    }

                }

            }

            string state = requestValues["State"];
            bool isStateValid = true;
            State lookedUpState = null;
            if (string.IsNullOrEmpty(state))
            {

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StateRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
                isStateValid = false;
            }
            else
            {


                if (state.Length > 2)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StateMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isStateValid = false;
                }

                if (isStateValid && isCountryValid)
                {
                    if (country.ToLower() == "us")
                    {
                        lookedUpState = locations.GetAllStates().Where(s => s.StateCode == state.ToUpper()).FirstOrDefault();
                        if (lookedUpState == null)
                        {
                            VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StateInvalid);
                            if (message != null)
                            {
                                responseContent.Messages.Add(message);
                            }
                            isStateValid = false;
                        }
                    }

                }

            }
            string postalCode = requestValues["PostalCode"];
            if (string.IsNullOrEmpty(postalCode))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PostalCodeRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                postalCode = postalCode.Trim();
                if (postalCode.Length > 20)
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PostalCodeMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
                if (isStateValid && isCountryValid)
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
                        if (InputValidation.IsValidUSPostalCode(postalCode) && isStateValid)
                        {
                            if (locations.GetAllUSPostalCodes().Where(pc => pc.PostalCodeString == postalCode && pc.StateCode == state.ToUpper()).FirstOrDefault() == null)
                            {
                                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PostalStateMisMatch);
                                if (message != null)
                                {
                                    responseContent.Messages.Add(message);
                                }
                            }
                        }
                    }
                }
            }

            string phone = requestValues["Phone"];
            string phoneNumbersDncCheck = "";
            if (string.IsNullOrEmpty(phone))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                bool isPhoneValid = true;
                if (phone.Length > 20)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                    isPhoneValid = false;
                }
                if (isPhoneValid)
                {
                    if (this.isInputNotNumeric(phone))
                    {

                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneNotNumeric);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                        isPhoneValid = false;
                    }
                }
                if (isPhoneValid)
                {
                    phoneNumbersDncCheck = phone;
                }
            }

            string email = requestValues["Email"];
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
                if (email.Length > 50)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }

                }

                if (!InputValidation.IsValidEmail(email))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailIsInvalid);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }

                }

            }
            var includeAdditionalProgramQuestions = requestValues["IncludeAdditionalProgramQuestions"];
            if (!string.IsNullOrEmpty(includeAdditionalProgramQuestions))
            {
                if (this.isInputNotBoolean(includeAdditionalProgramQuestions))
                {
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.IncludeAdditionalProgramQuestionsIsNotBool);
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
                //vendorResponseLog.APIKey = Guid.Parse(requestValues["apikey"]);
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
    }
}