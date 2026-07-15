using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    [APIDirectoryCampaignFilter]
    public class ProgramController : BaseAPIController
    {
        //GET api/program/details
        #region  api/program/details documentation
        /// <summary>
        /// <para>This method returns program detail information</para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="ProgramId">Required | Program Id.</param>
        /// <param name="CampusId">Required | Campus Id.</param>
        /// <example><![CDATA[
        /// https://partners.educationdynamics.com/api/program/details?apiKey=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&ProgramId=7243&CampusId=4919
        /// {
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "ProgramId": "7243",
        ///     "CampusId": "4919"
        /// }
        /// ]]></example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///     {
        ///   "IsSuccessful": true,
        ///   "Body": 
        ///     }
        ///             "ProgramId": 176385,
        ///              "ProgramName": "Bachelor of Science in Music Business",
        ///              "ProgramDescription": "Program description",
        ///              "ProgramLevel": {
        ///              "ProgramLevelId": 3,
        ///              "ProgramLevelName": "Bachelor"
        ///                 },
        ///        "EducationLevels": [
        ///             {
        ///             "EducationLevelId": 2,
        ///                "EducationLevelName": "G.E.D."
        ///             },
        ///             {
        ///                "EducationLevelId": 3,
        ///                  "EducationLevelName": "High School Diploma"
        ///              },                   
        ///              {
        ///                  "EducationLevelId": 8,
        ///                  "EducationLevelName": "Associate"
        ///              },
        ///              {
        ///                   "EducationLevelId": 9,
        ///                    "EducationLevelName": "Bachelor"
        ///              }
        ///               ],
        ///               "Category": {
        ///                  "Subjects": [
        ///                      {
        ///                          "SubjectId": 617,
        ///                          "SubjectName": "Hospitality Management",
        ///                         "DirectoryURL": "www.elearners.com/a/EDDYAPI3/form617"
        ///                       }
        ///                ],
        ///                "CategoryId": 21,
        ///                 "CategoryName": "Business",
        ///                 "DirectoryURL": "www.elearners.com/a/EDDYAPI3/form21"
        ///             },
        ///             "DirectoryURL": "www.elearners.com/a/EDDYAPI3/form/179/176385",
        ///             "Institution": {
        ///                 "InstitutionId": 179,
        ///                 "InstitutionName": "Full Sail University",
        ///                 "InstitutionDescription": "Institution description",
        ///                "LogoURL": "http://content.uat.educationdynamics.local/logo/179/Logo_240x80.gif",
        ///                 "Campuses": [
        ///                     {
        ///                         "CampusId": 6259,
        ///                         "CampusName": "Winter Park, FL",
        ///                        "CampusType": "Ground",
        ///                         "Address": "3300 University Boulevard",
        ///                         "City": "Winter Park",
        ///                         "Country": {
        ///                            "Name": "United States",
        ///                            "CountryCode": "US"
        ///                         },
        ///                        "State": {
        ///                           "Name": "Florida",
        ///                           "StateCode": "FL"
        ///                         },
        ///                       "PostalCode": "32792"
        ///                     }
        ///                ]
        ///         }
        ///     },
        ///    "ResponseDateTime": "2017-03-15T16:12:58.9998237-04:00",
        ///    "RequestDateTime": "2017-03-15T16:08:21.9993076-04:00",
        ///    "ResponseGuid": "532d5f13-c526-4fd9-8571-31c421f4f24a",
        ///    "Messages": [
        ///       {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 66
        ///}
        /// ]]>
        /// </returns>
        #endregion  api/program/details documentation
        [HttpPost]
        [DisabledCampusFilter]
        [ProgramFilter]
        [PostProgramDetailsActionFilter]
        [ActionName("details")]
        public HttpResponseMessage GetProgramDetails([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetProgramDetailsResponseModel responseContent = new GetProgramDetailsResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = directoryRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Search;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.IsSuccessful = false;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = directoryRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception);


            }
            return httpResponseMessage;

        }


        //GET api/program/form
        #region  api/program/form documentation
        /// <summary>
        /// <para>Host and Post partners can use this method to get back the field names and Types for a program. The list of possible input type property values are "Text Box","Text Area","Radio Buttons","Multi Check Box List","Drop-Down","Label","Check Box","Date", and "Hidden"  </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="ProgramId">Required | Program Id.</param>
        /// <example>
        /// <![CDATA[
        ///{
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "ProgramId": "7243"
        ///}
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///       {
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "Fields": [
        ///             {
        ///                 "Label": "GPA for highest level of education",
        ///                 "Name": "GPA",
        ///                 "Rules": [
        ///                             {
        ///                                 "RuleName": "GPA 3.0 Minimum",
        ///                                 "RuleValue": 3,
        ///                                 "FieldName": "GPA"
        ///                             }
        ///                         ],
        ///                 "InputType": "Drop-Down",
        ///                 "FormTemplateFieldOptions": [
        ///                                 {
        ///                                     "OptionValue": "Less than 2.0",
        ///                                     "OptionText": "Less than 2.0",
        ///                                 },
        ///                                 {
        ///                                     "OptionValue": "2.0-2.4",
        ///                                     "OptionText": "2.0-2.4",
        ///                                 },
        ///                                 {
        ///                                     "OptionValue": "2.5-2.74",
        ///                                     "OptionText": "2.5-2.74",
        ///                                 },
        ///                                 {
        ///                                     "OptionValue": "2.75-2.9",
        ///                                     "OptionText": "2.75-2.9",
        ///                                 },
        ///                                 {
        ///                                     "OptionValue": "3.0-3.4",
        ///                                     "OptionText": "3.0-3.4",
        ///                                 },
        ///                                 {
        ///                                    "OptionValue": "3.5 or Higher",
        ///                                    "OptionText": "3.5 or Higher",
        ///                                 }
        ///                             ]
        ///          }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-03-15T16:46:22.8626779-04:00",
        ///    "RequestDateTime": "2017-03-15T16:46:05.19354-04:00",
        ///    "ResponseGuid": "5abfb29d-9ea4-4a88-a147-28edd6177fd4",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///        }
        ///    ],
        ///    "TotalResponseTime": 676
        ///}
        /// ]]>
        /// </returns>
        #endregion  api/program/details documentation
        [HttpPost]
        [ProgramFilter]
        [PostProgramFormActionFilter]
        [ActionName("form")]
        public HttpResponseMessage GetProgramFormTemplate([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetProgramFormTemplateResponseModel responseContent = new GetProgramFormTemplateResponseModel(contactRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Search;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search, contactRequest.Email);

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.IsSuccessful = false;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception, contactRequest.Email);
            }
            return httpResponseMessage;

        }


    }
}
