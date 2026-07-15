using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using System.Web.Http.Description;
using Newtonsoft.Json;



namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    public class DirectoryController : BaseAPIController
    {

        //GET api/Directory/programs
        #region  api/Directory/programs documentation
        /// <summary>
        /// <para>This method returns a list of all programs currently available for this API Key that matches the given input parameters (ANDed together). Output order is based upon EDDY’s own internal optimization algorithm. </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="InstitutionId">Institution</param> 
        /// <param name="CampusId">Campus Id.</param>
        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param> 
        /// <param name="SpecialtyIds">Specialty(s) Comma-delimited list of SpecialtyIds.</param> 
        /// <param name="ProgramLevelIds">Program Level(s) list of ProgramLevelIds </param>
        /// <param name="ProspectFilters">Additional prospect data, if available</param> 
        /// <param name="CampusType">Online or Ground or All</param>
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example><![CDATA[
        /// {
        ///     "CampusType": "all",
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds": [25,21],
        ///     "SubjectIds": [658],
        ///     "SpecialtyIds": [2],
        ///     "ProgramLevelIds": [2,3,6,18,22,21],
        ///     "CampusId": 6259,
        ///     "PageSize": 10,
        ///     "StartPage": 1,
        ///     "ProspectFilters": [{"QuestionKey":"RNLicense","QuestionValue":"Yes"}]
        ///}
        /// ]]></example>
        /// <returns>
        /// <![CDATA[
        /// { 
        ///   "IsSuccessful": true, 
        ///   "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 114,
        ///        "TotalPages": 12,
        ///        "ItemList": [{
        ///             "ProgramId": 176385,
        ///              "ProgramName": "Bachelor of Science in Music Business",
        ///              "ProgramDescription": "<p>\n\tLearn what it takes to build a career in one of entertainment&rsquo;s most exciting industries through Full Sail University&rsquo;s Music Business Bachelor&rsquo;s Degree Program. Learn fundamentals such as marketing, human relations, finance, and accounting, as well as unique music industry topics like artist development, music distribution, record label development, and the negotiation process. This specialized program can prepare you for a role as a creative leader within an organization, or as the head of your own business.</p>\n<p>\n\tCourse Highlights:</p>\n<ul>\n\t<li>\n\t\tRecord Label Development</li>\n\t<li>\n\t\tConcert Management and Publicity</li>\n\t<li>\n\t\tArtist Development</li>\n\t<li>\n\t\tMusic Distribution</li>\n\t<li>\n\t\tMusic Copyright and Publishing</li>\n\t<li>\n\t\tMusic Business Law and Contract Negotiations</li>\n</ul>",
        ///              "ProgramLevel": {
        ///              "ProgramLevelId": 3,
        ///              "ProgramLevelName": "Bachelor"
        ///                 },
        ///                 "EstimatedRevShare": 9.60,
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
        ///                 "InstitutionDescription": "<p>\r\n\tFull Sail University offers campus and online degree programs that are designed for the world of entertainment, media, arts, and technology. Offering associate&#39;s, bachelor&#39;s, and master&#39;s degrees, Full Sail&#39;s approach is centered on real-world industry experience and creative problem solving.</p>\r\n<p>\r\n\tOur philosophy centers on recognizing the value in creative thinking and the importance of hands-on experience. We put this into action with an approach that emphasizes problem-solving, learning by doing, and empowering our students to generate their own concepts and ideas.</p>\r\n",
        ///                "LogoURL": "http://content.uat.educationdynamics.local/logo/179/Logo_240x80.gif",
        ///                  "Campuses": [
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
        ///             }
        ///             }
        ///         ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T15:52:03.0842179-05:00",
        ///    "RequestDateTime": "2017-01-20T15:51:59.9125753-05:00",
        ///    "ResponseGuid": "f7c3cc70-fe6e-4763-9479-9d6845cc8096",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///        }
        ///    ],
        ///    "TotalResponseTime": 684
        /// } 
        /// ]]>
        /// </returns>
        #endregion  api/Directory/programs documentation
        [HttpPost]
        [CampusTypeFilter]
        [DisabledCampusFilter]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [InstitutionFilter]
        [ProgramLevelFilter]
        [PagingActionFilter]
        [ActionName("programs")]
        public HttpResponseMessage GetPrograms([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetProgramsResponseModel responseContent = new GetProgramsResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }

        //GET api/Directory/programmatches
        #region  api/Directory/programmatches documentation
        /// <summary>
        /// <para>This method returns a list of matching programs for the user data submitted.</para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param> 
        /// <param name="SpecialtyIds">Specialty(s) Comma-delimited list of SpecialtyIds.</param> 
        /// <param name="Prefix">Prefix or title, e.g., "Mrs.", "Mr.", "Ms.". </param> 
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="City">Required | City name.</param> 
        /// <param name="PostalCode">Required | Postal code or Zip code.</param>
        /// <param name="State">Required | Two-character US state abbreviation, e.g., "NJ".</param> 
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="USCitizen">Is a Citizen of the United States. Accepted values: "Yes", "No".</param> 
        /// <param name="Email">Required | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="AlternatePhone">Optional | Alternate phone number.</param>
        /// <param name="Age">Age.</param> 
        /// <param name="YearHighestEducationCompleted">Year Highest Education was completed.</param>
        /// <param name="HighestLevelofEducationCompleted">Id of the highest level of education completed. We only accept the string value i.e., "2". Accepted Values: [{"Value": "2", "Text": "G.E.D."}, {"Value": "3", "Text": "High School Diploma"}, {"Value": "4", "Text": "Some College ,1-29 Credits"}, {"Value": "5", "Text": "Some College, 30-59 Credits"}, {"Value": "6", "Text": "Some College,60-89 Credits"}, {"Value": "7", "Text": "Some College, 90+ Credits"}, {"Value": "8", "Text": "Associate"}, {"Value": "9", "Text": "Bachelor"}, {"Value": "10", "Text": "Master"}, {"Value": "11", "Text": "Doctorate"}, {"Value": "1", "Text": "Haven't completed High School"}].</param> 
        /// <param name="MilitaryAffiliation">Military Affiliation Id. We only accept the string value i.e., "126". Accepted Values: [{"Value": "126", "Text": "NoMilitaryAffiliation"}, {"Value": "101", "Text": "AF-ActiveDuty(AD)"}, {"Value": "105", "Text": "AF-Civilian"}, {"Value": "102", "Text": "AF-SelectiveReserve(SR)"}, {"Value": "103", "Text": "AF-SpouseofADorSR"}, {"Value": "104", "Text": "AF-Veteran"}, {"Value": "106", "Text": "AR-ActiveDuty(AD)"}, {"Value": "110", "Text": "AR-Civilian"}, {"Value": "107", "Text": "AR-SelectiveReserve(SR)"}, {"Value": "108", "Text": "AR-SpouseofADorSR"}, {"Value": "109", "Text": "AR-Veteran"}, {"Value": "111", "Text": "CG-ActiveDuty(AD)"}, {"Value": "115", "Text": "CG-Civilian"}, {"Value": "112", "Text": "CG-SelectiveReserve(SR)"}, {"Value": "113", "Text": "CG-SpouseofADorSR"}, {"Value": "114", "Text": "CG-Veteran"}, {"Value": "116", "Text": "MC-ActiveDuty(AD)"}, {"Value": "120", "Text": "MC-Civilian"}, {"Value": "117", "Text": "MC-SelectiveReserve(SR)"}, {"Value": "118", "Text": "MC-SpouseofADorSR"}, {"Value": "119", "Text": "MC-Veteran"}, {"Value": "121", "Text": "NV-ActiveDuty(AD)"}, {"Value": "125", "Text": "NV-Civilian"}, {"Value": "122", "Text": "NV-SelectiveReserve(SR)"}, {"Value": "123", "Text": "NV-SpouseofADorSR"}, {"Value": "124", "Text": "NV-Veteran"}].</param>
        /// <param name="DesiredStartDate">Desired start date. We only accept the string value i.e., "Immediately". Accepted Values: [{"Value": "Immediately", "Text": "Immediately"}, {"Value": "1-3 Months", "Text":"1-3 Months"}, {"Value":"4-6 Months", "Text": "4-6 Months"}, {"Value": "7-12 Months", "Text": "7-12 Months"}, {"Value": "More than 1 Year", "Text": "More than 1 Year"}, {"Value": "Not Sure", "Text": "Not Sure"}]</param> 
        /// <param name="ProgramLevelId">We only accept the string value i.e. "2". Accepted Values:  { "ProgramLevelId": 2,"ProgramLevelName": "Associate"},{ "ProgramLevelId": 3,"ProgramLevelName": "Bachelor"},{ "ProgramLevelId": 22,"ProgramLevelName": "Diploma"},{"ProgramLevelId": 8,"ProgramLevelName": "Master"},{"ProgramLevelId": 18,"ProgramLevelName": "Undergraduate Certificate"}</param> 
        /// <param name="LeadIdToken">Required | The ID provided by Jornaya LeadiD.</param>
        /// <param name="IncludeAdditionalProgramQuestions">Include Program additional questions. Accepted values: True, False.</param>
        /// <param name="AdditionalQuestions">Additional prospect data, if available.</param> 
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <param name="LeadSourceUrl">Required | Valid URL where the lead was generated (Landing URL).</param>
        /// <param name="LeadInitiatingUrl">Required | Valid URL where the lead was initiated (Initiating URL).</param>
        /// <param name="SS1">Sub affiliate.</param>
        /// <param name="SS2">BPO Company Name.</param>
        /// <example>
        /// <![CDATA[
        /// {
        ///     "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds": [25],
        ///     "active": true,
        ///     "firstName": "Testfirst",
        ///     "lastname": "Testlast",
        ///     "prefix": "Mr.",
        ///     "Address": "801 test Corner Ct",
        ///     "Address2": "apt a",
        ///     "city": "Green Bay",
        ///     "postalcode": "55555",
        ///     "state": "WI",
        ///     "country": "US",
        ///     "Email": "test@test.com",
        ///     "Phone": "5555555555",
        ///     "AlternatePhone": "5555555555",
        ///     "Age": "21",
        ///     "YearHighestEducationCompleted": "1978",
        ///     "HighestLevelofEducationCompleted": "9",
        ///     "MilitaryAffiliation": "126",
        ///     "DesiredStartDate": "Immediately",
        ///     "ProgramLevelId": 2,
        ///     "leadIdToken" : "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "USCitizen": "Yes",
        ///     "SubjectIds": [658],
        ///     "SpecialtyIds": [2],
        ///     "IncludeAdditionalProgramQuestions": true,
        ///     "ProgramId": 7243,
        ///     "AffiliateId": "",
        ///     "StartPage": 1,
        ///     "PageSize": 10,
        ///     "LeadSourceUrl": "https://www.EducationDynamics.com/lead"
        ///     "LeadInitiatingUrl": "https://www.EducationDynamics.com/start"
        ///     "SS1": "Sub affiliate",
        ///     "SS2": "BPO Company Name",
        ///     "AdditionalQuestions":  [{"QuestionKey":"RNLicense","QuestionValue":"Yes"}]
        /// }
        ///]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///{
        /// "IsSuccessful": true,
        /// "Body": {
        /// "StartPage": 1,
        /// "PageSize": 10,
        /// "TotalItems": 4,
        /// "TotalPages": 1,
        /// "ItemList": [
        ///     {
        ///         "MatchedInstitution": {
        ///             "InstitutionId": 444,
        ///             "InstitutionName": "University of Saint Mary",
        ///             "InstitutionDescription":"Description of institution",
        ///             "LogoURL": "http://logo.educationdynamics.local/444/Logo_240x80.gif"
        ///         },
        ///         "MatchedPrograms": [
        ///             {
        ///                 "ProgramId": 2698,
        ///                 "ProgramName": "RN to Bachelor of Science in Nursing",
        ///                 "EstimatedRevShare": 9.60,
        ///                 "AdditionalQuestions": [
        ///                        {
        ///                             "Label": "GPA for highest level of education",
        ///                             "Name": "GPA",
        ///                             "Rules": [
        ///                                   {
        ///                                      "RuleName": "GPA 3.0 Minimum",
        ///                                      "RuleValue": 3,
        ///                                      "FieldName": "GPA"
        ///                                 }
        ///                             ],
        ///                             "InputType": "Drop-Down",
        ///                             "FormTemplateFieldOptions": [
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
        ///                         }
        ///                     ]
        ///                 }
        ///             ]
        ///          }
        ///          
        ///              ]
        ///                         
        ///          }
        ///      ]
        /// },
        /// "ResponseDateTime": "2017-01-23T17:10:54.9066218-05:00",
        /// "RequestDateTime": "2017-01-23T17:10:38.9139773-05:00",
        /// "ResponseGuid": "ac82298a-c54b-4a62-9de0-c8de442803a5",
        /// "Messages": [
        ///       {
        ///           "MessageCode": "msg0038",
        ///           "Message": "Validation Passed."
        ///       }
        ///   ],
        /// "TotalResponseTime": 240
        ///}
        /// ]]>
        /// </returns>
        #endregion  api/Directory/programmatches documentation
        [HttpPost]
        [ProgramMatchesLeadSourceFilter]
        [CampusTypeFilter]
        [CategoryFilter]
        [SubjectFilter]
        [ProgramLevelFilter]
        [AgeFilter]
        [MilitaryAffiliationFilter]
        [YearHighestEducationCompletedFilter]
        
        [PagingActionFilter]
        [PostProgramMatchesActionFilter]
        [ActionName("programmatches")]
        public HttpResponseMessage GetProgramMatches([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetProgramMatchesResponseModel responseContent = new GetProgramMatchesResponseModel(contactRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Match;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Match, contactRequest.Email);


            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }

        //GET api/Directory/Institutions 
        #region  api/Directory/Institutions documentation
        /// <summary>
        /// <para>This method returns a list of institutions currently available for this API Key that match the given input parameters (ORed together within a parameter, but ANDed together across parameters). Output order is based upon Eddy’s own internal optimization algorithm.  </para> 
        /// </summary>

        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>

        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param> 
        /// <param name="ProgramLevelIds">Program Level(s) Comma-delimited list of ProgramLevelIds </param>
        /// <param name="CampusType">Online or Ground or All</param>

        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example>
        /// <![CDATA[
        /// {
        ///     "InstitutionId": "141",
        ///     "CampusType": "All",
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds": [25,21],
        ///     "SubjectIds": [658],
        ///     "ProgramLevelIds": [2,3,6,18,22,8],
        ///     "PageSize": 10,
        ///     "StartPage": 1
        /// }
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 1,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///      {
        ///           "InstitutionId": 141,
        ///                "InstitutionName": "Rasmussen College",
        ///                "InstitutionDescription": "tFlex Choice: Online, On Campus",
        ///                "LogoURL": "http://content.educationdynamics.local/logo/141/Logo_240x80.gif",
        ///                "InstitutionFormURL": "www.elearners.com/a/Mydirectory/form/141",
        ///                "Campuses": [
        ///                    {
        ///                        "CampusId": 9306,
        ///                        "CampusName": "Wausau, WI",
        ///                        "Address": "1101 Westwood Dr",
        ///                        "City": "Wausau",
        ///                        "Country": {
        ///                            "CountryId": 4,
        ///                            "Name": "United States",
        ///                            "CountryCode": "US"
        ///                        },
        ///                        "State": {
        ///                            "StateId": 51,
        ///                            "Name": "Wisconsin",
        ///                            "StateCode": "WI"
        ///                        },
        ///                        "CampusType": "Ground",
        ///                        "PostalCode": "54401"
        ///                    }]
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T10:11:07.7055258-05:00",
        ///    "RequestDateTime": "2017-01-20T10:11:07.6331136-05:00",
        ///    "ResponseGuid": "a9f58bb4-dc4f-474a-ae7d-e52576139152",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///        }
        ///    ],
        ///    "TotalResponseTime": 74
        ///} ]]>
        ///
        /// </returns>
        #endregion  api/Directory/Institutions documentation
        [HttpPost]
        [CampusTypeFilter]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [InstitutionFilter]
        [ProgramLevelFilter]
        [PostInstitutionActionFilter]
        [PagingActionFilter]
        [ActionName("institutions")]
        public HttpResponseMessage GetInstitutions([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetInstitutionsResponseModel responseContent = new GetInstitutionsResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = directoryRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Search;
                //logs.LogVendorResponse(vendorResponseLog);

            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }


        //GET api/Directory/Campuses 
        #region  api/Directory/campuses documentation
        /// <summary>
        /// <para>This method returns detailed information on one particular campus, including its geographic restrictions.</para> 
        /// </summary>
        /// <ParameterDescription>Test</ParameterDescription>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="CampusId">Required | Campus Id.</param> 
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example>
        /// <![CDATA[
        /// {
        ///     "CampusId": "9306",
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "StartPage": 1,
        ///     "PageSize": 10
        /// }
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///        {
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 1,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///         {
        ///                 "Institution": {
        ///                     "InstitutionId": 5326,
        ///                     "InstitutionName": "Southern Technical College",
        ///                     "InstitutionDescription": "<p>\r\n\tSouthern Technical College is the....",
        ///                     "LogoURL": "http://content.qa.educationdynamics.local/logo/5326/Logo_240x80.gif"
        ///                 },
        ///                 "Address": null,
        ///                 "City": null,
        ///                 "Country": {
        ///                     "CountryId": 4,
        ///                     "Name": "United States",
        ///                     "CountryCode": "US"
        ///                 },
        ///                 "CampusId": 9129,
        ///                 "CampusName": "Ft Myers, FL",
        ///                 "State": {
        ///                     "StateId": 11,
        ///                     "Name": "Florida",
        ///                     "StateCode": "FL"
        ///                 },
        ///                 "CampusType": "Ground",
        ///                 "PostalCode": "33907",
        ///                 "ExcludedZips": null,
        ///                 "IncludedZips": [
        ///                     "33905",
        ///                     "34134",
        ///                     "33972",
        ///                     "33924"
        ///                 ],
        ///                 "IsActive": false,
        ///                 "LogoURL": "http://content.qa.educationdynamics.local/logo/5326/Logo_240x80.gif"
        ///            }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T13:30:10.6270768-05:00",
        ///    "RequestDateTime": "2017-01-20T13:30:10.6113456-05:00",
        ///    "ResponseGuid": "5e497d0a-1be2-458e-b282-db100436ca12",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 16
        ///} ]]>
        ///
        /// </returns>
        #endregion  api/Directory/campuses documentation
        [HttpPost]

        [PagingActionFilter]
        [ActionName("campuses")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage GetCampuses([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetCampusesResponseModel responseContent = new GetCampusesResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }

        //GET api/Directory/categories
        #region  api/Directory/categories documentation
        /// <summary>
        /// <para>This method returns a list of all categories currently available for this API Key that matches the given input parameters (ANDed together). The Category IDs returned can be used as inputs to the other API methods when filtering results.  </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="InstitutionId">Institution</param> 
        /// <param name="ProgramLevelIds">Program Level(s) Comma-delimited list of ProgramLevelIds </param>
        /// <param name="CampusType">Online or Ground or All</param>

        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example>
        /// <![CDATA[
        /// {
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "StartPage": 1,
        ///     "PageSize": 10
        /// }    
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 2,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///            {
        ///                "CategoryId": 21,
        ///                "CategoryName": "Business",
        ///                "DirectoryURL": "www.elearners.com/a/Mydirectory/search/programs?category=21",
        ///            }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T13:30:10.6270768-05:00",
        ///    "RequestDateTime": "2017-01-20T13:30:10.6113456-05:00",
        ///    "ResponseGuid": "5e497d0a-1be2-458e-b282-db100436ca12",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 16
        ///} ]]>
        ///
        /// </returns>
        #endregion  api/Directory/categories documentation
        [HttpPost]
        [CategoryFilter]
        [InstitutionFilter]
        [ProgramLevelFilter]
        [CampusTypeFilter]

        [PagingActionFilter]
        [ActionName("categories")]
        public HttpResponseMessage GetCategories([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetCategoriesResponseModel responseContent = new GetCategoriesResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = directoryRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Search;
                //logs.LogVendorResponse(vendorResponseLog);

            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }

        //GET api/Directory/subcategories
        #region  api/Directory/subcategories documentation
        /// <summary>
        /// <para>This method returns a list of all subcategories currently available for this API Key that matches the given input parameters (ANDed together). The SubCategory IDs returned can be used as inputs to the other API methods when filtering results. </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="InstitutionId">Institution</param> 
        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="ProgramLevelIds">Program Level(s) Comma-delimited list of ProgramLevelIds </param>
        /// <param name="CampusType">Online or Ground or All</param>
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example>
        /// <![CDATA[
        /// {
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds": [25],
        ///     "ProgramLevelIds": [2,3,6,18,22,8],
        ///     "PageSize": 10,
        ///     "StartPage": 1
        /// }
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 1,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///             {
        ///               "SubjectId": 610,
        ///               "SubjectName": "Accounting",
        ///               "DirectoryURL": "www.elearners.com/a/Mydirectory/search/programs?subject=610"
        ///             }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T13:30:10.6270768-05:00",
        ///    "RequestDateTime": "2017-01-20T13:30:10.6113456-05:00",
        ///    "ResponseGuid": "5e497d0a-1be2-458e-b282-db100436ca12",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 16
        ///} ]]>
        ///
        /// </returns>
        #endregion  api/Directory/subcategories documentation
        [HttpPost]
        [CampusTypeFilter]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [InstitutionFilter]
        [ProgramLevelFilter]
        [PagingActionFilter]
        [ActionName("subjects")]
        public HttpResponseMessage GetSubCategories([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetSubCategoriesResponseModel responseContent = new GetSubCategoriesResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = directoryRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Search;
                //logs.LogVendorResponse(vendorResponseLog);

            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }


        //GET api/Directory/specialties
        #region  api/Directory/specialties documentation
        /// <summary>
        /// <para>This method returns a list of all subcategories currently available for this API Key that matches the given input parameters (ANDed together). The SubCategory IDs returned can be used as inputs to the other API methods when filtering results. </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="InstitutionId">Institution</param> 
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param>
        /// <param name="ProgramLevelIds">Program Level(s) Comma-delimited list of ProgramLevelIds </param>
        /// <param name="CampusType">Online or Ground or All</param>
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example>
        /// <![CDATA[
        /// {
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "SubjectIds": [25],
        ///     "ProgramLevelIds": [2,3,6,18,22,8],
        ///     "PageSize": 10,
        ///     "StartPage": 1
        /// }
        /// ]]>
        /// </example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 1,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///             {
        ///               "SpecialtyId": 610,
        ///               "SpecialtyName": "Accounting",
        ///               "DirectoryURL": "www.elearners.com/a/Mydirectory/search/programs?subject=610"
        ///             }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T13:30:10.6270768-05:00",
        ///    "RequestDateTime": "2017-01-20T13:30:10.6113456-05:00",
        ///    "ResponseGuid": "5e497d0a-1be2-458e-b282-db100436ca12",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 16
        ///} ]]>
        ///
        /// </returns>
        #endregion  api/Directory/subcategories documentation
        [HttpPost]
        [CampusTypeFilter]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [InstitutionFilter]
        [ProgramLevelFilter]
        [PagingActionFilter]
        [ActionName("specialties")]
        public HttpResponseMessage GetSpecialties([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetSpecialtiesResponseModel responseContent = new GetSpecialtiesResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }

        //GET api/Directory/levels
        #region  api/Directory/levels documentation
        /// <summary>
        /// <para>This method returns a list of all currently available program levels for this API Key that match the given input parameters (ANDed together). The Level IDs returned can be used as inputs to the other API methods when filtering results. </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="InstitutionId">Institution</param> 

        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param> 
        /// <param name="CampusType">Online or Ground or All</param>
        /// <param name="StartPage">Required | Start page.</param>
        /// <param name="PageSize">Required | Number of records per page.</param> 
        /// <example><![CDATA[
        /// {
        ///     "CampusType": "all",
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds": [25,21],
        ///     "SubjectIds": [658],
        ///     "PageSize": 10,
        ///     "StartPage": 1
        ///   }
        /// ]]></example>
        /// <returns>
        /// <![CDATA[
        ///{
        ///    "IsSuccessful": true,
        ///    "Body": {
        ///        "StartPage": 1,
        ///        "PageSize": 10,
        ///        "TotalItems": 2,
        ///        "TotalPages": 1,
        ///        "ItemList": [
        ///              {
        ///                "ProgramLevelId": 2,
        ///                "ProgramLevelName": "Associate"
        ///            },
        ///            {
        ///                "ProgramLevelId": 3,
        ///                "ProgramLevelName": "Bachelor"
        ///             }
        ///        ]
        ///    },
        ///    "ResponseDateTime": "2017-01-20T13:30:10.6270768-05:00",
        ///    "RequestDateTime": "2017-01-20T13:30:10.6113456-05:00",
        ///    "ResponseGuid": "5e497d0a-1be2-458e-b282-db100436ca12",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///    }
        ///    ],
        ///    "TotalResponseTime": 16
        ///}
        /// ]]>
        /// </returns>
        #endregion  api/Directory/levels documentation
        [HttpPost]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [InstitutionFilter]
        [CampusTypeFilter]
        [PagingActionFilter]
        [ActionName("programlevels")]
        public HttpResponseMessage GetLevels([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetLevelsResponseModel responseContent = new GetLevelsResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }


        //GET api/Directory/allocation
        #region  api/Directory/allocation documentation
        /// <summary>
        /// <para>Partners can use this method to check if there is any allocation available to them for the specified categories, subcategories, or levels. </para> 
        /// </summary>
        /// <ParameterDescription>Test</ParameterDescription>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubjectIds">Subject(s) Comma-delimited list of SubjectIds.</param> 
        /// <param name="ProgramLevelIds">Program Level(s) list of ProgramLevelIds </param>
        /// <example>
        /// <![CDATA[
        /// {
        ///     "APIKey" : "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CategoryIds" : [25],
        ///     "SubjectIds" : [658],
        ///     "ProgramLevelIds" : [2]
        ///  }
        /// ]]>
        /// </example>
        /// <returns>
        ///<![CDATA[
        /// {
        ///"IsSuccessful": true,
        ///"Body": {
        ///    "HasAllocation": true,
        ///    "CheckedCategoryIds": "25,21",
        ///    "CheckedSubjectIds": "658",
        ///    "CheckedLevelIds": "2,3,6,18,22,8"
        ///},
        ///"ResponseDateTime": "2017-01-23T15:15:00.8528916-05:00",
        ///"RequestDateTime": "2017-01-23T15:15:00.6676364-05:00",
        ///"ResponseGuid": "4ce35e5c-374a-4ab9-86b1-cdef61e7cdf1",
        ///"Messages": [
        ///    {
        ///        "MessageCode": "msg0038",
        ///        "Message": "Validation Passed."
        ///}
        /// ],
        ///"TotalResponseTime": 188
        ///}
        /// ]]>
        /// </returns>
        #endregion  api/Directory/allocation documentation

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ActionName("allocation")]
        public HttpResponseMessage GetAllocation([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetAllocationResponseModel responseContent = new GetAllocationResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);


            }
            return httpResponseMessage;

        }

        [HttpPost]

        [ActionName("campusesformicrosites")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage GetCampusesForMicrosites([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetCampusForMicrositesResponseModel responseContent = new GetCampusForMicrositesResponseModel(directoryRequest);
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
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }


        //GET api/Directory/zipcodes
        #region  api/Directory/zipcodes documentation
        /// <summary>        
        /// <para>This method returns a list of all zip codes for this API Key that match the given input parameters for the given campus. </para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>        
        /// <param name="CampusId">Required | Campus Id.</param>
        /// <example><![CDATA[        
        /// {
        ///     "APIKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "CampusId": "622"        
        ///     
        /// }
        /// ]]></example>
        /// <returns>
        /// <![CDATA[
        /// {
        ///    "IsSuccessful": true,
        ///    "Body": 
        ///        {
        ///            "CampusId": 622,
        ///            "ZipCode": [
        ///                17565,
        ///                17564,
        ///                17563
        ///            ]
        ///        },
        ///    "ResponseDateTime": "2021-04-08T13:23:08.468054-06:00",
        ///    "RequestDateTime": "2021-04-08T13:22:29.4296603-06:00",
        ///    "ResponseGuid": "4c6e992d-df1e-4da2-8015-c07afa847b11",
        ///    "Messages": [
        ///        {
        ///            "MessageCode": "msg0038",
        ///            "Message": "Validation Passed."
        ///        }
        ///    ],
        ///    "TotalResponseTime": 39039
        ///} 
        /// ]]>
        /// </returns>
        #endregion  api/Directory/zipcodes documentation

        [HttpPost]
        [CampusFilter]        
        [ActionName("zipcodes")]

        public HttpResponseMessage GetZipsByCampusId([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;

                GetZipsResponseModel responseContent = new GetZipsResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);

            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;
        }

        [HttpPost]

        [ActionName("programsformicrosites")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage GetProgramsForMicrosites([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                GetProgramForMicrositesResponseModel responseContent = new GetProgramForMicrositesResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);
            }
            catch (Exception exc)
            {
              //Fix for DEV-7 moved logging and error messages to single common method
              // Not exposing stack trace
             return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }

        [ActionName("clearcacheprogramsformicrosites")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage DeleteProgramsCacheForMicrosites([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                DeleteProgramCacheForMicrositesResponseModel responseContent = new DeleteProgramCacheForMicrositesResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);
            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);
            }
            return httpResponseMessage;

        }

        [HttpPost]

        [ActionName("clearcampusesformicrosites")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage DeleteCampusesCacheForMicrosites([FromBody] DirectoryRequest directoryRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                DeleteCampusesCacheForMicrositesResponseModel responseContent = new DeleteCampusesCacheForMicrositesResponseModel(directoryRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, directoryRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);
            }
            catch (Exception exc)
            {
                //Fix for DEV-7 moved logging and error messages to single common method
                // Not exposing stack trace
                return CreateErrorResponse(exc);

            }
            return httpResponseMessage;

        }


        public HttpResponseMessage CreateErrorResponse(Exception exc)
        {
            // Log full exception
            Logging.LogException(exc, Log.GetCurrentMethodName());
            EDDYLogger.LogMessage(this, LogLevel.Error, exc.ToString());
            ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

            var responseContent = new VendorResponseBase
            {
                IsSuccessful = false,
                ResponseGuid = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                ResponseDateTime = DateTime.Now,
                Messages = new List<VendorResponseMessage>
        {
            new VendorResponseMessage
            {
                MessageCode = "500",
                Message = "Internal server error.",
                //MessageType = "Error"
            }
        }
            };

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new ObjectContent(
                    typeof(VendorResponseBase),
                    responseContent,
                    new JsonMediaTypeFormatter())
            };
        }


    }
}
