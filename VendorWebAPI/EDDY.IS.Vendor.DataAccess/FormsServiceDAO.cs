using System;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities.NoNull;
using EDDY.IS.Vendor.DataAccess.FormsEngineService;
using EDDY.IS.Vendor.DataAccess.MatchingService;
using EDDY.IS.Vendor.DataAccess.ProspectService;
using EDDY.IS.Vendor.Utilities;
using Newtonsoft.Json;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using EDDY.IS.Vendor.DataAccess.DataModels;
using EDDY.IS.ExternalMatch.DatabaseModel;
//using EDDY.IS.Vendor.Web.API.Models;
//using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor;
using EDDY.IS.Vendor.Entities;
using Channel = EDDY.IS.Vendor.Utilities.Channel;

namespace EDDY.IS.Vendor.DataAccess
{
    public class FormsServiceDAO : VendorBaseDAO
    {

        private FormsEngineClient formsEngineClient = new FormsEngineClient();
        private MatchingServiceClient matchingServiceClient = new MatchingServiceClient();

        #region Base Methods
        public VendorResponseBase GetProgramMatches(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string additionalQuestionsString = JsonConvert.SerializeObject(contactRequest.AdditionalQuestions);
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{18}{19}{20}{21}{22}{23}{24}{25}_GetProgramMataches",
                    contactRequest.APIKey,
                    contactRequest.Prefix,
                    contactRequest.FirstName,
                    contactRequest.LastName,
                    contactRequest.Address,
                    contactRequest.Address2,
                    contactRequest.City,
                    contactRequest.PostalCode,
                    contactRequest.State,
                    contactRequest.Country,
                    contactRequest.USCitizen,
                    contactRequest.Email,
                    contactRequest.Phone,
                    contactRequest.AlternatePhone,
                    contactRequest.Age,
                    contactRequest.YearHighestEducationCompleted.HasValue ? contactRequest.YearHighestEducationCompleted.Value : 0,
                    contactRequest.HighestLevelofEducationCompleted,
                    contactRequest.MilitaryAffiliation,
                    contactRequest.DesiredStartDate,
                    (contactRequest.CategoryIds != null ? string.Join(",", contactRequest.CategoryIds.ToArray()) : "noCategoryIds"),
                    contactRequest.ProgramLevelId,
                    (contactRequest.SubjectIds != null ? string.Join(",", contactRequest.SubjectIds.ToArray()) : "noSubjectIds"),
                    additionalQuestionsString,
                    contactRequest.IncludeAdditionalProgramQuestions,
                    string.IsNullOrWhiteSpace(contactRequest.LeadIdToken) ? "noLeadId" : "LeadId", //Binary caching key
                    (contactRequest.SpecialtyIds != null ? string.Join(",", contactRequest.SpecialtyIds.ToArray()) : "noSpecialtyIds")
                    );

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {

                    List<AdditionalField> questionAnswers = new List<AdditionalField>();
                    if (contactRequest.AdditionalQuestions != null)
                    {
                        if (contactRequest.AdditionalQuestions.Count > 0)
                        {
                            List<ContactStandardControlMapping> mappings = this.getAllContactPropertyToStandardControlMappings();
                            foreach (QuestionAnswer questionAnswer in contactRequest.AdditionalQuestions)
                            {
                                if (questionAnswer != null)
                                {

                                    AdditionalField currentQuestionAnswer = new AdditionalField();
                                    currentQuestionAnswer.Key = questionAnswer.QuestionKey;
                                    currentQuestionAnswer.Value = questionAnswer.QuestionValue;
                                    questionAnswers.Add(currentQuestionAnswer);

                                }
                            }

                        }
                    }


                    //hardcoded to pass in fake url for ceco ping.
                    AdditionalField leadSourceField = new AdditionalField();
                    leadSourceField.Key = "LeadSource";
                    leadSourceField.Value = HttpUtility.UrlEncode(string.IsNullOrEmpty(contactRequest.LeadSource) ? ConfigurationManager.AppSettings["DefaultLeadSource"] : contactRequest.LeadSource);
                    questionAnswers.Add(leadSourceField);

                    AdditionalField leadSourceUrlField = new AdditionalField();
                    leadSourceUrlField.Key = "LeadSourceUrl";
                    leadSourceUrlField.Value = HttpUtility.UrlEncode(string.IsNullOrEmpty(contactRequest.LeadSourceUrl) ? ConfigurationManager.AppSettings["DefaultLeadSourceURL"] : contactRequest.LeadSourceUrl);
                    questionAnswers.Add(leadSourceUrlField);

                    AdditionalField leadInitiatingUrlField = new AdditionalField();
                    leadInitiatingUrlField.Key = "LeadInitiatingUrl";
                    leadInitiatingUrlField.Value = HttpUtility.UrlEncode(string.IsNullOrEmpty(contactRequest.LeadInitiatingUrl) ? ConfigurationManager.AppSettings["DefaultLeadInitiatingURLSource"] : contactRequest.LeadInitiatingUrl);
                    questionAnswers.Add(leadInitiatingUrlField);

                    AdditionalField subSource1 = new AdditionalField();
                    subSource1.Key = "SubSource1";
                    subSource1.Value = contactRequest.SS1;
                    questionAnswers.Add(subSource1);

                    AdditionalField subSource2 = new AdditionalField();
                    subSource2.Key = "SubSource2";
                    subSource2.Value = contactRequest.SS2;
                    questionAnswers.Add(subSource2);

                    string regionString = NoNull.Convert.String(contactRequest.State);
                    if (contactRequest.Country.ToLower() != "us")
                    {
                        regionString = "N/A";
                    }

                    FormsEngineClient formsEngineClient = new FormsEngineClient();
                    var apiLead = new APILead
                    {
                        Prefix = NoNull.Convert.String(contactRequest.Prefix),
                        FirstName = NoNull.Convert.String(contactRequest.FirstName),
                        LastName = NoNull.Convert.String(contactRequest.LastName),
                        Address1 = NoNull.Convert.String(contactRequest.Address),
                        Address2 = NoNull.Convert.String(contactRequest.Address2),
                        City = NoNull.Convert.String(contactRequest.City),
                        ZipCode = NoNull.Convert.String(contactRequest.PostalCode),
                        StateProvince = regionString,
                        CountryCode = NoNull.Convert.String(contactRequest.Country),
                        USCitizen = NoNull.Convert.String(contactRequest.USCitizen),
                        EmailAddress = NoNull.Convert.String(contactRequest.Email),
                        Phone1 = NoNull.Convert.String(contactRequest.Phone),
                        Phone2 = NoNull.Convert.String(contactRequest.AlternatePhone),
                        DialerKey = NoNull.Convert.String(contactRequest.DialerKey),
                        TSR = NoNull.Convert.String(contactRequest.TSR),
                        HighestLevelOfEdu = NoNull.Convert.String(contactRequest.HighestLevelofEducationCompleted),
                        Military = NoNull.Convert.String(contactRequest.MilitaryAffiliation),
                        StartDate = NoNull.Convert.String(contactRequest.DesiredStartDate),
                        AdditionalFields = questionAnswers
                    };

                    if (contactRequest.Age > 0)
                    {
                        apiLead.Age = contactRequest.Age;
                    }
                    if (contactRequest.YearHighestEducationCompleted.HasValue)
                    {
                        apiLead.YearHighestEduCompleted = contactRequest.YearHighestEducationCompleted.Value;
                    }
                    if (contactRequest.CategoryIds != null)
                    {
                        if (contactRequest.CategoryIds.Count > 0)
                        {
                            apiLead.Categories = contactRequest.CategoryIds;
                        }
                    }
                    if (contactRequest.SubjectIds != null)
                    {
                        if (contactRequest.SubjectIds.Count > 0)
                        {
                            apiLead.SubCategories = contactRequest.SubjectIds;
                        }
                    }
                    if (contactRequest.SpecialtyIds != null)
                    {
                        if (contactRequest.SpecialtyIds.Count > 0)
                        {
                            apiLead.Specialties = contactRequest.SpecialtyIds;
                        }
                    }
                    if (contactRequest.ProgramLevelId > 0)
                    {
                        apiLead.DesiredDegreeLevel = contactRequest.ProgramLevelId;
                    }
                    if (!string.IsNullOrWhiteSpace(contactRequest.LeadIdToken))
                    {
                        Guid leadIdToken;
                        if (Guid.TryParse(contactRequest.LeadIdToken, out leadIdToken))
                        {
                            apiLead.LeadId_Token = leadIdToken;
                        }
                    }
                    if(apiLead.TSR != "")
                    {
                        var users = CacheStore.GetCacheItemByKey("VendorApi.UsersLookup") as List<Entities.User>;
                        if(users != null)
                        {
                            var user = users.FirstOrDefault(x => x.TSR == apiLead.TSR);
                            if (user != null)
                                apiLead.UserId = user.UserId;
                        }
                    }

                    Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);
                    APIProgramMatchesDTO matchedPrograms = null;
                    if (contactRequest.IncludeAdditionalProgramQuestions)
                    {
                        matchedPrograms = formsEngineClient.GetEnhancedProgramMatches(vendorCampaign.ApplicationId, contactRequest.IsBeta, contactRequest.APIKey.ToString(), apiLead);
                    }
                    else
                    {
                        matchedPrograms = formsEngineClient.GetProgramMatches(vendorCampaign.ApplicationId, contactRequest.IsBeta, contactRequest.APIKey.ToString(), apiLead);
                    }
                    if (matchedPrograms != null)
                    {

                        vendorResponseBase = new VendorResponseBase();
                        vendorResponseBase.IsSuccessful = matchedPrograms.Valid;
                        if (matchedPrograms.Valid)
                        {
                            itemList = new List<Object>();
                            List<ContactStandardControlMapping> mappings = this.getAllContactPropertyToStandardControlMappings();
                            foreach (FormsEngineService.CampusWithInstitution campusWithInstitution in matchedPrograms.SchoolSelectionList)
                            {
                                Match match = new Match();
                                if (campusWithInstitution != null)
                                {
                                    Entities.InstitutionBase mappedMatchedInstitution = new Entities.InstitutionBase();
                                    mappedMatchedInstitution.InstitutionId = (int)campusWithInstitution.InstitutionId;

                                    if (vendorCampaign.VendorId == 12093)
                                    {
                                        if (campusWithInstitution.InstitutionId == 119)
                                            mappedMatchedInstitution.InstitutionName = "Colorado Technical University CTUO";
                                        else
                                            mappedMatchedInstitution.InstitutionName = campusWithInstitution.InstitutionName;
                                    }
                                    else
                                    {
                                        mappedMatchedInstitution.InstitutionName = campusWithInstitution.InstitutionName;
                                    }

                                    mappedMatchedInstitution.InstitutionDescription = campusWithInstitution.InstitutionDescription;
                                    mappedMatchedInstitution.IsLiveTransfer = campusWithInstitution.IsLiveTransfer;

                                    if (campusWithInstitution.IsLiveTransfer && campusWithInstitution.SchoolAgents != null && campusWithInstitution.SchoolAgents.Count > 0)
                                    {
                                        mappedMatchedInstitution.Agents = new List<Entities.SchoolAgent>();

                                        foreach (var sa in campusWithInstitution.SchoolAgents)
                                            mappedMatchedInstitution.Agents.Add(new Entities.SchoolAgent() { AgentId = sa.AgentId, AgentName = sa.AgentName });
                                    }

                                    //mappedMatchedInstitution.LogoURL = EddyURLs.GetLogoURL((int)campusWithInstitution.InstitutionId, campusWithInstitution.CampusId, campusWithInstitution.HasCampusLogo);
                                    mappedMatchedInstitution.LogoURL = EddyURLs.GetLogoURL(campusWithInstitution.CampusLogoURL, campusWithInstitution.InstitutionLogoURL);
                                    match.MatchedInstitution = mappedMatchedInstitution;

                                    if (campusWithInstitution.ProgramList.Count > 0)
                                    {
                                        match.MatchedPrograms = new List<ProgramMatch>();
                                        foreach (FormsEngineService.Program matchedProgram in campusWithInstitution.ProgramList)
                                        {
                                            List<ProgramValidationRule> businessRules = matchedProgram.ExternalMatchItemGuid.HasValue == false ? this.getProgramRules(vendorCampaign.TrackId, matchedProgram.ProgramId) : new List<ProgramValidationRule>();
                                            Entities.ProgramMatch mappedProgram = new Entities.ProgramMatch();
                                            mappedProgram.ProgramName = matchedProgram.ProgramName;
                                            mappedProgram.ProgramId = matchedProgram.ProgramId;
                                            mappedProgram.CampusId = campusWithInstitution.CampusId;
                                            mappedProgram.CampusName = campusWithInstitution.CampusName;
                                            mappedProgram.CampusType = campusWithInstitution.CampusType.Value.ToString();
                                            mappedProgram.TransferNumber = matchedProgram.TransferNumber;

                                            if (vendorCampaign.AllowRevShareRPL)
                                            {

                                                if (vendorCampaign.CalculateRevShareByERPL)
                                                {
                                                    if (matchedProgram.EffectiveRevenuePerLead.HasValue)
                                                    {
                                                        mappedProgram.EstimatedRevShare = this.calculateVendorCampaignProgramRPL(matchedProgram.EffectiveRevenuePerLead.Value, vendorCampaign);
                                                    }
                                                }
                                                else
                                                {
                                                    if (matchedProgram.RevenuePerLead.HasValue)
                                                    {
                                                        mappedProgram.EstimatedRevShare = this.calculateVendorCampaignProgramRPL(matchedProgram.RevenuePerLead.Value, vendorCampaign);
                                                    }
                                                }

                                            }


                                            if (matchedPrograms.AdditionalQuestionList != null)
                                            {
                                                if (matchedPrograms.AdditionalQuestionList.Count > 0)
                                                {
                                                    mappedProgram.AdditionalQuestions = new List<FormTemplateField>();
                                                    KeyValuePair<int, List<TemplateControlDTO>> additionalQuestions = matchedPrograms.AdditionalQuestionList.FirstOrDefault(t => t.Key == matchedProgram.TemplateId);
                                                    if (additionalQuestions.Value != null)
                                                    {
                                                        foreach (TemplateControlDTO additionalQuestion in additionalQuestions.Value.OrderBy(m => m.RowSequence))
                                                        {
                                                            Entities.FormTemplateField formTemplateField = new Entities.FormTemplateField();
                                                            formTemplateField.Label = additionalQuestion.StandardControl.DefaultLabel;
                                                            formTemplateField.Name = additionalQuestion.StandardControl.StandardControlCode.Code;

                                                            switch (additionalQuestion.StandardControl.StandardControlCode.Code.ToLower())
                                                            {
                                                                case "first_name":
                                                                case "last_name":
                                                                case "address":
                                                                case "address_2":
                                                                case "city":
                                                                case "postal_code":
                                                                case "email":
                                                                case "alternate_phone":
                                                                case "phone":
                                                                case "highest_level_of_education_completed":
                                                                case "military_affiliation":
                                                                case "desired_start_date":
                                                                case "age":
                                                                case "year_of_highest_education_completed":
                                                                case "us_citizen":

                                                                    ContactStandardControlMapping map = mappings.Where(m => m.StandardControlName.ToLower() == additionalQuestion.StandardControl.StandardControlCode.Code.ToLower()).FirstOrDefault();
                                                                    formTemplateField.Name = map != null ? map.ContactPropertyName : additionalQuestion.StandardControl.StandardControlCode.Code;

                                                                    break;
                                                                default:
                                                                    formTemplateField.Name = additionalQuestion.StandardControl.StandardControlCode.Code;
                                                                    break;
                                                            }




                                                            if (businessRules != null)
                                                            {
                                                                if (businessRules.Count > 0)
                                                                {
                                                                    List<ProgramValidationRule> businessRule = businessRules.Where(r => r.FieldName == formTemplateField.Name).ToList();
                                                                    formTemplateField.Rules = businessRule != null ? businessRule : null;
                                                                }
                                                            }
                                                            formTemplateField.InputType = additionalQuestion.StandardControl.StandardControlType.StandardControlTypeName;
                                                            if (additionalQuestion.StandardControl.PreDefinedValueList != null)
                                                            {
                                                                formTemplateField.FormTemplateFieldOptions = new List<Entities.FormTemplateFieldOption>();

                                                                foreach (ValueListDTO value in additionalQuestion.StandardControl.PreDefinedValueList.ValueListItems)
                                                                {
                                                                    Entities.FormTemplateFieldOption formTemplateFieldOption = new Entities.FormTemplateFieldOption();
                                                                    formTemplateFieldOption.OptionText = value.Text;
                                                                    formTemplateFieldOption.OptionValue = value.Value;


                                                                    formTemplateField.FormTemplateFieldOptions.Add(formTemplateFieldOption);
                                                                }

                                                            }
                                                            mappedProgram.AdditionalQuestions.Add(formTemplateField);
                                                        }
                                                    }

                                                }
                                            }
                                            if ((((vendorCampaign.ChannelId == (int)Utilities.Channel.CallCenterPartners || vendorCampaign.SubChannelId == (int)SubChannel.CallCenterPartners)) || ((vendorCampaign.ChannelId == (int)Utilities.Channel.OnlinePartners || vendorCampaign.SubChannelId == (int)SubChannel.OnlinePartners))) && vendorCampaign.ApplicationId != 27)
                                            {
                                                mappedProgram.UserAgreement = getUserAgreementText(vendorCampaign, String.Empty, campusWithInstitution.CustomTCPA, campusWithInstitution.CustomContactCenterTCPA);
                                            }

                                            if (matchedProgram.ExternalMatchItemGuid.HasValue)
                                                RedisHelper.SetInCache(contactRequest.APIKey + "_" + contactRequest.Email + "_" + matchedProgram.ProgramId, matchedProgram.ExternalMatchItemGuid.Value.ToString() + "|" + matchedProgram.ProgramProductId + "|" + matchedProgram.RevenuePerLead);

                                            //Call Verified Exclusive
                                            if (matchedProgram.ProductId == 135)
                                                match.MatchedInstitution.IsExclusive = true;

                                            match.MatchedPrograms.Add(mappedProgram);
                                        }
                                    }
                                }
                                itemList.Add(match);
                            }
                            CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                        }
                        else
                        {
                            vendorResponseBase.Messages = new List<VendorResponseMessage>();
                            foreach (KeyValuePair<string, string> validationMessage in matchedPrograms.ValidationMessages)
                            {
                                VendorResponseMessage vendorResponseMessage = new VendorResponseMessage();
                                switch (validationMessage.Key)
                                {
                                    case "MatchingEngineNoResults":
                                        vendorResponseMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NoResults);
                                        break;
                                    default:

                                        vendorResponseMessage.MessageCode = validationMessage.Key;
                                        vendorResponseMessage.Message = validationMessage.Value;

                                        break;
                                }
                                vendorResponseBase.Messages.Add(vendorResponseMessage);
                            }
                        }
                    }
                }
                else
                {
                    vendorResponseBase = new VendorResponseBase();
                    vendorResponseBase.IsSuccessful = true;
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null && vendorResponseBase != null)
            {

                vendorAPIList = pageItemList(itemList, contactRequest.StartPage, contactRequest.PageSize);
                vendorResponseBase.Body = vendorAPIList;
            }

            return vendorResponseBase;
        }

        public Entities.FormTemplate GetFormTemplate(ContactRequest contactRequest)
        {
            Entities.FormTemplate formTemplate = null;


            try
            {

                string cacheKey = String.Format("{0}{1}_GetFormTemplate", contactRequest.APIKey, contactRequest.ProgramId);

                formTemplate = CacheStore.GetCacheItemByKey(cacheKey) as Entities.FormTemplate;

                if (formTemplate == null)
                {

                    Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);
                    TemplateDTO templateDto = formsEngineClient.GetProgramAllTemplatesMergedModel(contactRequest.ProgramId, vendorCampaign.TrackId, false);



                    if (templateDto != null)
                    {
                        formTemplate = new Entities.FormTemplate();


                        if (templateDto.TemplateSteps.Count > 0)
                        {

                            formTemplate.Fields = new List<FormTemplateField>();
                            List<ContactStandardControlMapping> mappings = this.getAllContactPropertyToStandardControlMappings();
                            List<ProgramValidationRule> businessRules = this.getProgramRules(vendorCampaign.TrackId, contactRequest.ProgramId);
                            foreach (TemplateStepDTO templateStep in templateDto.TemplateSteps.OrderBy(m => m.Sequence))
                            {

                                if (templateStep.TemplateSections.Count > 0)
                                {

                                    foreach (TemplateSectionDTO templateSection in templateStep.TemplateSections.OrderBy(m => m.Sequence))
                                    {

                                        if (templateSection.TemplateControls.Count > 0)
                                        {

                                            foreach (TemplateControlDTO templateControl in templateSection.TemplateControls.OrderBy(m => m.RowSequence))
                                            {

                                                if (templateControl.StandardControl.StandardControlCode.Code.ToLower() != "Program_Of_Interest".ToLower())
                                                {
                                                    Entities.FormTemplateField formTemplateField = new Entities.FormTemplateField();
                                                    formTemplateField.Label = templateControl.StandardControl.DefaultLabel;

                                                    switch (templateControl.StandardControl.StandardControlCode.Code.ToLower())
                                                    {
                                                        case "first_name":
                                                        case "last_name":
                                                        case "address":
                                                        case "address_2":
                                                        case "city":
                                                        case "postal_code":
                                                        case "email":
                                                        case "alternate_phone":
                                                        case "phone":
                                                        case "highest_level_of_education_completed":
                                                        case "military_affiliation":
                                                        case "desired_start_date":
                                                        case "age":
                                                        case "year_of_highest_education_completed":
                                                        case "us_citizen":
                                                            ContactStandardControlMapping map = mappings.Where(m => m.StandardControlName.ToLower() == templateControl.StandardControl.StandardControlCode.Code.ToLower()).FirstOrDefault();
                                                            formTemplateField.Name = map != null ? map.ContactPropertyName : templateControl.StandardControl.StandardControlCode.Code;
                                                            break;
                                                        case "useragreement":
                                                            formTemplateField.Label = getUserAgreementText(vendorCampaign, templateControl.StandardControl.DefaultLabel);
                                                            goto default;
                                                        default:
                                                            formTemplateField.Name = templateControl.StandardControl.StandardControlCode.Code;
                                                            break;
                                                    }



                                                    if (businessRules != null)
                                                    {
                                                        if (businessRules.Count > 0)
                                                        {
                                                            List<ProgramValidationRule> businessRule = businessRules.Where(r => r.FieldName == formTemplateField.Name).ToList();
                                                            formTemplateField.Rules = businessRule != null ? businessRule : null;
                                                        }
                                                    }

                                                    formTemplateField.InputType = templateControl.StandardControl.StandardControlType.StandardControlTypeName;
                                                    if (templateControl.StandardControl.PreDefinedValueList != null)
                                                    {
                                                        formTemplateField.FormTemplateFieldOptions = new List<Entities.FormTemplateFieldOption>();

                                                        foreach (ValueListDTO value in templateControl.StandardControl.PreDefinedValueList.ValueListItems)
                                                        {
                                                            Entities.FormTemplateFieldOption formTemplateFieldOption = new Entities.FormTemplateFieldOption();
                                                            formTemplateFieldOption.OptionText = value.Text;
                                                            formTemplateFieldOption.OptionValue = value.Value;


                                                            formTemplateField.FormTemplateFieldOptions.Add(formTemplateFieldOption);
                                                        }
                                                    }
                                                    formTemplate.Fields.Add(formTemplateField);
                                                }
                                            }

                                        }

                                    }
                                }


                            }





                            CacheStore.AddResponseDataCacheItem(cacheKey, formTemplate);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }



            return formTemplate;
        }

        #endregion

        #region Lead Methods

        public VendorResponseBase SaveLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponse = null;

            try
            {
                APILead lead = mapAPILead(contactRequest);

                VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);
                contactRequest.LeadSource = getLeadSource(vendorCampaign);

                ProgramValidateResponse validatedProgram;
                //Check to see if we have a value from program matches
                string externalMatchGuid = RedisHelper.GetFromCache(contactRequest.APIKey + "_" + contactRequest.Email + "_" + contactRequest.ProgramId);
                //if null Check to see if we have a value from get programs program
                if (String.IsNullOrEmpty(externalMatchGuid))
                {
                    externalMatchGuid = RedisHelper.GetFromCache(contactRequest.APIKey + "_" + contactRequest.ProgramId);
                }
                if (!String.IsNullOrEmpty(externalMatchGuid))
                {
                    string[] arr = externalMatchGuid.Split('|');
                    lead.ExternalMatchItemGuid = Guid.Parse(arr[0]); 
                    decimal rpl;
                    if (!decimal.TryParse(arr[2], out rpl))
                    {
                        rpl = 0;
                    }

                    validatedProgram = new ProgramValidateResponse { ProgramProductId = Convert.ToInt32(arr[1]), PassedValidation = true, RevenuePerLead = rpl };
                }
                else
                    validatedProgram = this.validateProgram(contactRequest.APIKey, contactRequest.ProgramId, lead.ClientRelationContactId, contactRequest.CampusId);

                if (validatedProgram != null)
                {
                    APISubmissionResultDTO submissionResult = new APISubmissionResultDTO();
                    decimal validatedProgramEstimatedRevShare = 0;

                    if (vendorCampaign.AllowRevShareRPL)
                    {
                        if (vendorCampaign.CalculateRevShareByERPL)
                        {
                            if (validatedProgram.EffectiveRevenuePerLead.HasValue)
                                validatedProgramEstimatedRevShare = this.calculateVendorCampaignProgramRPL(validatedProgram.EffectiveRevenuePerLead.Value, vendorCampaign);
                        }
                        else
                        {
                            if (validatedProgram.RevenuePerLead.HasValue)
                                validatedProgramEstimatedRevShare = this.calculateVendorCampaignProgramRPL(validatedProgram.RevenuePerLead.Value, vendorCampaign);
                        }
                    }
                    lead.ChannelId = vendorCampaign.ChannelId;
                    lead.SubChannelId = vendorCampaign.SubChannelId;
                    lead.InstitutionName = validatedProgram.InstitutionName;
                    lead.ValidateTCPA = vendorCampaign.ValidateTCPA;
                    lead.EstimatedRevShare = validatedProgramEstimatedRevShare;
                    if (validatedProgram.RuleFailures == null)
                    {


                        submissionResult = formsEngineClient.ProcessHostAndPostLead(vendorCampaign.ApplicationId, (int)validatedProgram.ProgramProductId, contactRequest.IsBeta, contactRequest.APIKey.ToString(), lead);

                        if (submissionResult.Valid)
                        {

                            if (submissionResult.LeadPingScoreCPL > 0)
                            {
                                validatedProgramEstimatedRevShare = this.calculateVendorCampaignProgramRPL(submissionResult.LeadPingScoreCPL, vendorCampaign);
                            }
                            if ((vendorCampaign.ChannelId == 13 || vendorCampaign.ChannelId == 15) && vendorCampaign.ApplicationId != 27)
                            {
                                upsertCallCenterLead(submissionResult.LeadId, contactRequest, vendorCampaign);
                            }

                            vendorResponse = createSuccessfulResponse(submissionResult, validatedProgramEstimatedRevShare);


                        }
                        else
                            vendorResponse = createFailureResponse(submissionResult.ValidationMessages.Select(m => m.Value), submissionResult);
                    }
                    else
                    {
                        var crInvalidProgramOverride = this.getCRInvalidProgramOverride(contactRequest.APIKey);
                        if (crInvalidProgramOverride != null && crInvalidProgramOverride.ProgramProductId.HasValue)
                        {
                            submissionResult.LeadId = this.CreateValidationFailureLead(MapToNexusLead(contactRequest, vendorCampaign, lead, crInvalidProgramOverride, validatedProgram));
                            if (crInvalidProgramOverride.ProductId == 101 || crInvalidProgramOverride.ProductId == 121)
                            {
                                this.CreateValidationFailureEMSLead((int)submissionResult.LeadId);
                            }
                        }
                        vendorResponse = createFailureResponse(validatedProgram.RuleFailures.Select(f => f.RuleFailureName), submissionResult);
                    }
                }
            }
            catch (Exception exc)
            {
                vendorResponse = createExceptionResponse();
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return vendorResponse;
        }

        // Sets correct ProgramId based on ProgramName & CampusId incase the programId is incorrect because of the swapped fields on microsites
        public int CheckCampusProgramForSwappedFields(int programId, int campusId, List<CampusMicrosite> campusList, List<ProgramMicrosite> programList)
        {
            int programIdResult = programId;

            //Get Campus object and ProgramName to get the matching program under this campus
            CampusMicrosite campusMicrositeSelected = campusList.Where(c => c.CampusId == campusId).FirstOrDefault();



            //string programName = programList.Where(p => p.ProgramId == programId).FirstOrDefault().ProgramDisplayValue;
            ProgramMicrosite programMicrosite = null;
            ProgramMicrosite program= programList.Where(p => p.ProgramId == programId).FirstOrDefault();
            if (program != null && campusMicrositeSelected != null)                      
                programMicrosite = campusMicrositeSelected.Programs.Where(p => p.ProgramDisplayValue == program.ProgramDisplayValue).FirstOrDefault();
            
            //Update programid if it does not match
            if ((programMicrosite != null) && !campusMicrositeSelected.Programs.Where(p => p.ProgramId == programId).Any())
                programIdResult = programMicrosite.ProgramId;

            return programIdResult;          
        }

        public VendorResponseBase SaveCallCenterLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponse = null;

            try
            {
                APILead lead = JsonConvert.DeserializeObject<APILead>(contactRequest.LeadJson);
                VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);

                List<decimal> successfulLeadIds;
                if (contactRequest.LeadId > 0)
                {
                    successfulLeadIds = new List<decimal> { contactRequest.LeadId };
                }
                else
                {
                    APIMultiSubmissionResultDTO multiSubmissionResult =
                    formsEngineClient.ProcessApolloSubmission(
                    vendorCampaign.ApplicationId,
                    contactRequest.ProgramProducts,
                    contactRequest.ProspectId,
                    contactRequest.APIKey.ToString(),
                    contactRequest.MatchResponseGuid,
                    lead,
                    contactRequest.ClientRelationContactId,
                    false,
                    contactRequest.ProspectFlowId,
                    contactRequest.PaidStatusTypes,
                    false
                    );

                    successfulLeadIds = multiSubmissionResult.SubmissionResults.Where(r => r.Valid).Select(r => (decimal)r.LeadId).ToList();
                }


                foreach (decimal leadId in successfulLeadIds)
                {
                    upsertCallCenterLead(leadId, contactRequest, vendorCampaign);
                }

                vendorResponse = new VendorResponseBase
                {
                    IsSuccessful = successfulLeadIds.Count() > 0,
                    Body = successfulLeadIds
                };
            }
            catch (Exception exc)
            {
                vendorResponse = createExceptionResponse();
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorResponse;
        }

        public VendorResponseBase SaveEduMaxLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponse = null;

            try
            {
                VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);
                if (vendorCampaign != null)
                {

                    int programProductId = this.getEDUMaxProgramProductId(contactRequest);

                    if (programProductId > 0)
                    {
                        APILead lead = mapAPILead(contactRequest);

                        contactRequest.LeadSource = getLeadSource(vendorCampaign);
                        Guid externalMatchItemGuid = Guid.NewGuid();
                        lead.ExternalMatchItemGuid = externalMatchItemGuid;
                        lead.PreValidatedProgram = true;
                        lead.ProspectFlowId = contactRequest.ProspectFlowId;
                        lead.ChannelId = vendorCampaign.ChannelId;
                        lead.SubChannelId = vendorCampaign.SubChannelId;

                        APISubmissionResultDTO submissionResult =
                            formsEngineClient.ProcessHostAndPostLead(
                                vendorCampaign.ApplicationId,
                                programProductId,
                                contactRequest.IsBeta,
                                contactRequest.APIKey.ToString(),
                                lead
                                );

                        if (submissionResult.Valid)
                        {
                            //Save External Match Item 
                            EduMaxExternalMatchItem eduMaxExternalMatchItem = new EduMaxExternalMatchItem();

                            string schoolName = lead.AdditionalFields.Where(m => m.Key.ToLower() == "school").FirstOrDefault()?.Value;
                            string match = lead.AdditionalFields.Where(m => m.Key.ToLower() == "match").FirstOrDefault()?.Value;
                            string programName = lead.AdditionalFields.Where(m => m.Key.ToLower() == "ExternalProgramName".ToLower()).FirstOrDefault()?.Value;
                            string campusName = lead.AdditionalFields.Where(m => m.Key.ToLower() == "ExternalCampusName".ToLower()).FirstOrDefault()?.Value;
                            eduMaxExternalMatchItem.InstitutionName = schoolName;
                            eduMaxExternalMatchItem.EduMaxLeadId = match;
                            eduMaxExternalMatchItem.ProgramName = programName;
                            eduMaxExternalMatchItem.CampusName = campusName;
                            eduMaxExternalMatchItem.BuyerLeadId = contactRequest.ExternalSystemId;
                            eduMaxExternalMatchItem.ExternalMatchItemGuid = externalMatchItemGuid;
                            ExternalMatchDataService.SaveExternalMatchItem(eduMaxExternalMatchItem);

                            //Save Call Center lead
                            int storeFrontId = this.getStoreFrontIdByName(contactRequest.StoreFront);

                            CallCenterLead callCenterLead = mapCallCenterLead(submissionResult.LeadId, contactRequest, vendorCampaign);

                            callCenterLead.StoreFrontId = ((storeFrontId > 0) ? (int?)storeFrontId : null);

                            createCallCenterLead(callCenterLead);

                            EduMaxLeadSubmissionResponse vendorEduMaxLeadSubmissionResponse = new EduMaxLeadSubmissionResponse
                            {
                                UID = submissionResult.UID,
                                LeadTier = this.getProductFromLead(submissionResult.LeadId),
                                MatchId = match,
                                LeadId = (int)submissionResult.LeadId
                            };

                            vendorResponse = new VendorResponseBase
                            {
                                IsSuccessful = true,
                                Body = vendorEduMaxLeadSubmissionResponse
                            };
                        }
                        else
                        {
                            vendorResponse = createEdumaxFailureResponse(submissionResult.ValidationMessages.Select(m => m.Value), submissionResult);
                        }
                    }
                    else
                    {
                        vendorResponse = new VendorResponseBase();
                        vendorResponse.IsSuccessful = false;
                        vendorResponse.Messages = new List<VendorResponseMessage>();
                        VendorResponseMessage exceptionMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ProgramUnavailable);
                        vendorResponse.Messages.Add(exceptionMessage);
                    }
                }

            }
            catch (Exception exc)
            {
                vendorResponse = new VendorResponseBase();
                vendorResponse.IsSuccessful = false;
                vendorResponse.Messages = new List<VendorResponseMessage>();
                VendorResponseMessage exceptionMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                vendorResponse.Messages.Add(exceptionMessage);
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }



            return vendorResponse;
        }

        #endregion

        #region Lead Helper Methods

        private APILead mapAPILead(ContactRequest contactRequest)
        {
            var apiLead = new APILead
            {
                Prefix = NoNull.Convert.String(contactRequest.Prefix),
                FirstName = NoNull.Convert.String(contactRequest.FirstName),
                LastName = NoNull.Convert.String(contactRequest.LastName),
                Address1 = NoNull.Convert.String(contactRequest.Address),
                Address2 = NoNull.Convert.String(contactRequest.Address2),
                City = NoNull.Convert.String(contactRequest.City),
                ZipCode = NoNull.Convert.String(contactRequest.PostalCode)?.Trim(),
                StateProvince = NoNull.Convert.String(contactRequest.State),
                CountryCode = NoNull.Convert.String(contactRequest.Country),
                USCitizen = NoNull.Convert.String(contactRequest.USCitizen),
                EmailAddress = NoNull.Convert.String(contactRequest.Email),
                Phone1 = NoNull.Convert.String(contactRequest.Phone),
                Phone2 = NoNull.Convert.String(contactRequest.AlternatePhone),
                HighestLevelOfEdu = NoNull.Convert.String(contactRequest.HighestLevelofEducationCompleted),
                Military = NoNull.Convert.String(contactRequest.MilitaryAffiliation),
                StartDate = NoNull.Convert.String(contactRequest.DesiredStartDate),
                LeadId_Token = Guid.TryParse(contactRequest.LeadIdToken, out Guid leadIdGuid) ? leadIdGuid : Guid.Empty,
                AdditionalFields = getAdditionalFields(contactRequest)
            };

            if (apiLead.Address1 != null && apiLead.Address1.Length > 50)
            {
                apiLead.Address1 = apiLead.Address1.Substring(0, 50);
            }

            mapOptionalFields(apiLead, contactRequest);

            return apiLead;
        }

        private void mapOptionalFields(APILead lead, ContactRequest contactRequest)
        {
            if (contactRequest.Age > 0)
                lead.Age = contactRequest.Age;

            if (contactRequest.YearHighestEducationCompleted.HasValue)
                lead.YearHighestEduCompleted = contactRequest.YearHighestEducationCompleted.Value;

            if (contactRequest.CategoryIds?.Count > 0)
                lead.Categories = contactRequest.CategoryIds;

            if (contactRequest.SubjectIds?.Count > 0)
                lead.SubCategories = contactRequest.SubjectIds;

            if (contactRequest.ProgramLevelId > 0)
                lead.DesiredDegreeLevel = contactRequest.ProgramLevelId;

            if (contactRequest.AgentId.HasValue)
                lead.ClientRelationContactId = contactRequest.AgentId;
        }

        private List<AdditionalField> getAdditionalFields(ContactRequest contactRequest)
        {
            var additionalFields = new List<AdditionalField>();

            if (contactRequest.AdditionalQuestions?.Count > 0)
            {
                foreach (QuestionAnswer questionAnswer in contactRequest.AdditionalQuestions)
                {
                    AdditionalField additionalField = new AdditionalField
                    {
                        Key = questionAnswer.QuestionKey,
                        Value = Uri.IsWellFormedUriString(questionAnswer.QuestionValue, UriKind.RelativeOrAbsolute) ? HttpUtility.UrlEncode(HttpUtility.UrlDecode(questionAnswer.QuestionValue)) : questionAnswer.QuestionValue
                    };

                    additionalFields.Add(additionalField);
                }
            }
            //get only the keys that don't already exist from the additional questions
            var specificAdditionalFields = getExplicitlyDefinedAdditionalFields(contactRequest).Where(s => !additionalFields.Any(a => a.Key == s.Key));

            additionalFields = additionalFields.Concat(specificAdditionalFields).ToList();

            return additionalFields;
        }

        private List<AdditionalField> getExplicitlyDefinedAdditionalFields(ContactRequest contactRequest)
        {
            var additionalFields = new List<AdditionalField>();

            var fields = new[]
            {
                new { Key = "UserAgreement", Value = contactRequest.UserAgreement, ShouldUrlEncode = false },
                new { Key = "AffiliateId", Value = contactRequest.AffiliateId, ShouldUrlEncode = false },
                new { Key = "LeadSource", Value = contactRequest.LeadSource, ShouldUrlEncode = false },
                new { Key = "LeadSourceUrl", Value = contactRequest.LeadSourceUrl, ShouldUrlEncode = true },
                new { Key = "LeadInitiatingUrl", Value = contactRequest.LeadInitiatingUrl, ShouldUrlEncode = true },
                new { Key = "VideoUrl", Value = contactRequest.VideoUrl, ShouldUrlEncode = true },
                new { Key = "SubSource1", Value = contactRequest.SS1, ShouldUrlEncode = false },
                new { Key = "SubSource2", Value = contactRequest.SS2, ShouldUrlEncode = false },
                new { Key = "SourceCode", Value = contactRequest.SourceCode, ShouldUrlEncode = false }
            };

            foreach (var field in fields)
            {
                if (!string.IsNullOrWhiteSpace(field.Value))
                    additionalFields.Add(new AdditionalField { Key = field.Key, Value = field.ShouldUrlEncode ? HttpUtility.UrlEncode(field.Value) : field.Value });

            }

            return additionalFields;
        }

        private string getLeadSource(VendorCampaign vendorCampaign)
        {
            string leadSource = null;

            if (vendorCampaign != null)
            {
                if (vendorCampaign.IsAPIDirectory)
                    leadSource = "web";

                if (vendorCampaign.IsCallCenter)
                    leadSource = "callcenter";
            }

            return leadSource;
        }

        private CallCenterLead mapCallCenterLead(decimal leadId, ContactRequest contactRequest, VendorCampaign vendorCampaign)
        {
            int callCenterId = this.getCallCenterId(vendorCampaign, contactRequest);

            int agentId = 0;
            if (!contactRequest.AgentId.HasValue && !String.IsNullOrEmpty(contactRequest.TSR))
            {
                DataModels.User agentUser = this.getAgentUserByTSR(contactRequest.TSR);
                if (agentUser != null)
                {
                    agentId = agentUser.UserID;
                }
            }
            else if (contactRequest.AgentId.HasValue)
            {
                agentId = contactRequest.AgentId.Value;
            }
            CallCenterLead callCenterLead = new CallCenterLead()
            {
                LeadId = leadId,
                AgentId = ((agentId > 0) ? (int?)agentId : null),
                TSR = String.IsNullOrEmpty(contactRequest.TSR) ? contactRequest.AgentName : contactRequest.TSR,
                DialerKey = contactRequest.DialerKey,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProspectFlowId = ((contactRequest.ProspectFlowId > 0) ? (int?)contactRequest.ProspectFlowId : null),
                CallCenterId = ((callCenterId > 0) ? (int?)callCenterId : null)
            };

            return callCenterLead;
        }
        private bool upsertCallCenterLead(decimal leadId, ContactRequest contactRequest, VendorCampaign vendorCampaign)
        {
            bool successful = false;

            using (var context = new APINexusEntities())
            {

                CallCenterLead callCenterLead = context.CallCenterLead.Where(ccl => ccl.LeadId == leadId).FirstOrDefault();

                if (callCenterLead == null)
                {
                    callCenterLead = mapCallCenterLead(leadId, contactRequest, vendorCampaign);

                    successful = createCallCenterLead(callCenterLead);
                }
                else
                {
                    // TODO: Update necessary fields
                    successful = updateCallCenterLead(callCenterLead);
                }
            }

            return successful;
        }

        private bool createCallCenterLead(CallCenterLead callCenterLead)
        {
            bool successful = false;

            using (var context = new APINexusEntities())
            {
                callCenterLead.CreatedDate = callCenterLead.UpdatedDate = DateTime.Now;
                callCenterLead.CreatedBy = callCenterLead.UpdatedBy = 1;
                context.CallCenterLead.Add(callCenterLead);
                successful = context.SaveChanges() > 0;
            }

            return successful;
        }

        private bool updateCallCenterLead(CallCenterLead callCenterLead)
        {
            bool successful = false;

            using (var context = new APINexusEntities())
            {
                context.Entry(callCenterLead).State = EntityState.Modified;
                successful = context.SaveChanges() > 0;
            }

            return successful;
        }

        private MatchingService.ProgramValidateResponse validateProgram(Guid apikey, int programId, int? agentId, int campusId)
        {
            MatchingService.ProgramValidateResponse programValidateResponse = null;

            // string cacheKey = String.Format("{0}{1}_validateProgram", apikey, programId);

            ///   programValidateResponse = CacheStore.GetCacheItemByKey(cacheKey) as MatchingService.ProgramValidateResponse;

            if (programValidateResponse == null)
            {
                try
                {
                    ProgramValidateRequest request = new ProgramValidateRequest()
                    {
                        Application = ISApplication.VendorAPI,
                        BreakOnFirstValidationFailure = true,
                        ProgramProductId = 0,
                        ProspectInput = null,
                        ProgramId = programId,
                        TrackGuid = apikey,
                        LeadCreationType = MatchingService.LeadCreationType.HostAndPost,
                        AgentId = agentId,
                        CampusId = campusId
                    };

                    MatchingService.ProgramValidateResponse currentProgramValidateResponse = matchingServiceClient.ValidateAPIProgram(request);
                    if (currentProgramValidateResponse != null)
                    {
                        programValidateResponse = currentProgramValidateResponse;
                        //     CacheStore.AddResponseDataCacheItem(cacheKey, programValidateResponse);
                    }
                }
                catch (Exception exc)
                {
                    var excs = exc;
                    throw exc;
                }

            }
            return programValidateResponse;


        }

        private List<ProgramValidationRule> getProgramRules(Guid apikey, int programId)
        {
            List<ProgramValidationRule> programValidationRules = null;
            try
            {
                ApiProgramResponse apiProgramResponse = matchingServiceClient.GetApiProgram(programId, apikey);
                if (apiProgramResponse != null)
                {

                    if (apiProgramResponse.Rules.Count > 0)
                    {
                        List<ContactStandardControlMapping> mappings = this.getAllContactPropertyToStandardControlMappings();
                        programValidationRules = new List<ProgramValidationRule>();
                        foreach (ApiRule rule in apiProgramResponse.Rules)
                        {
                            if (programValidationRules.Where(m => m.RuleName == rule.RuleName).FirstOrDefault() == null)
                            {


                                ProgramValidationRule programValidationRule = new ProgramValidationRule();

                                programValidationRule.RuleName = rule.RuleName;

                                if (rule.ValidKeyValueCodeData.Count > 0)
                                {
                                    List<FormTemplateFieldOption> formTemplateOptionValues = new List<FormTemplateFieldOption>();
                                    foreach (var kvdata in rule.ValidKeyValueCodeData)
                                    {
                                        FormTemplateFieldOption formTemplateFieldOption = new FormTemplateFieldOption();
                                        formTemplateFieldOption.OptionValue = kvdata.Value;
                                        formTemplateFieldOption.OptionText = kvdata.Value;
                                        formTemplateOptionValues.Add(formTemplateFieldOption);
                                    }
                                    programValidationRule.RuleValue = formTemplateOptionValues;
                                }
                                else
                                {
                                    programValidationRule.RuleValue = DatabaseUtilities.SetDecimalValue(rule.EntityValue);
                                }

                                ContactStandardControlMapping map = mappings.Where(m => m.StandardControlName == rule.StandardControlCode).FirstOrDefault();
                                switch (rule.StandardControlCode.ToLower())
                                {
                                    case "first_name":
                                    case "last_name":
                                    case "address":
                                    case "address_2":
                                    case "city":
                                    case "postal_code":
                                    case "email":
                                    case "alternate_phone":
                                    case "phone":
                                    case "highest_level_of_education_completed":
                                    case "military_affiliation":
                                    case "desired_start_date":
                                    case "age":
                                    case "year_of_highest_education_completed":
                                    case "us_citizen":

                                        programValidationRule.FieldName = map != null ? map.ContactPropertyName : rule.StandardControlCode;

                                        break;
                                    default:
                                        programValidationRule.FieldName = rule.StandardControlCode;
                                        break;
                                }

                                programValidationRules.Add(programValidationRule);
                            }
                        }
                    }

                }
            }
            catch (Exception exc)
            {
                throw exc;

            }

            return programValidationRules;
        }

        private string getUserAgreementText(VendorCampaign vendorCampaign, string defaultUserAgreementText)
        {
            string messageKey = string.Empty;
            int emsApplicationId = 27;

            if (string.IsNullOrEmpty(defaultUserAgreementText) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.OnlinePartnerTCPAMessageKey;
            }
            if ((vendorCampaign.ChannelId == (int)Utilities.Channel.CallCenterPartners || vendorCampaign.SubChannelId == (int)SubChannel.CallCenterPartners) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.CallCenterPartnerTCPAMessageKey;
            }
            else if ((vendorCampaign.ChannelId == (int)Utilities.Channel.OnlinePartners || vendorCampaign.SubChannelId == (int)SubChannel.OnlinePartners) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.OnlinePartnerTCPAMessageKey;
            }

            string tcpaMessage = this.getTCPAMessageByKey(messageKey);
            tcpaMessage = HttpUtility.JavaScriptStringEncode(tcpaMessage);

            return !string.IsNullOrWhiteSpace(tcpaMessage) ? tcpaMessage : defaultUserAgreementText;
        }

        private string getUserAgreementText(VendorCampaign vendorCampaign, string defaultUserAgreementText, string institutionTCPA, string institutionCCTCPA)
        {
            string messageKey = string.Empty;
            int emsApplicationId = 27;

            if (string.IsNullOrEmpty(defaultUserAgreementText) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.OnlinePartnerTCPAMessageKey;
            }
            if ((vendorCampaign.ChannelId == (int)Utilities.Channel.CallCenterPartners || vendorCampaign.SubChannelId == (int)SubChannel.CallCenterPartners) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.CallCenterPartnerTCPAMessageKey;
            }
            else if ((vendorCampaign.ChannelId == (int)Utilities.Channel.OnlinePartners || vendorCampaign.SubChannelId == (int)SubChannel.OnlinePartners) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                messageKey = Constants.OnlinePartnerTCPAMessageKey;
            }

            string tcpaMessage = null;

            if ((vendorCampaign.ChannelId == (int)Utilities.Channel.OnlinePartners || vendorCampaign.SubChannelId == (int)SubChannel.OnlinePartners) && !String.IsNullOrEmpty(institutionTCPA) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                tcpaMessage = institutionTCPA;
            }
            else if ((vendorCampaign.ChannelId == (int)Utilities.Channel.CallCenterPartners || vendorCampaign.SubChannelId == (int)SubChannel.CallCenterPartners) && !String.IsNullOrEmpty(institutionCCTCPA) && vendorCampaign.ApplicationId != emsApplicationId)
            {
                tcpaMessage = institutionCCTCPA;
            }
            else
            {
                tcpaMessage = this.getTCPAMessageByKey(messageKey);
            }

            tcpaMessage = HttpUtility.JavaScriptStringEncode(tcpaMessage);

            return !string.IsNullOrWhiteSpace(tcpaMessage) ? tcpaMessage : defaultUserAgreementText;
        }

        private int getEDUMaxProgramProductId(ContactRequest contactRequest)
        {
            int programProductId = 0;
            try
            {
                if (contactRequest.AdditionalQuestions != null && contactRequest.AdditionalQuestions.Count > 0)
                {
                    QuestionAnswer questionAnswer = contactRequest.AdditionalQuestions.Where(m => m.QuestionKey.ToLower() == "buyer").FirstOrDefault();

                    if (questionAnswer != null && !string.IsNullOrEmpty(questionAnswer.QuestionValue))
                    {
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EduMaxLenexaDataProductId"]) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["EduMaxLenexaWTProductId"]))
                        {
                            int productId = 0;

                            if (contactRequest.WarmTransferSchool == "0")
                            {
                                int.TryParse(ConfigurationManager.AppSettings["EduMaxLenexaDataProductId"], out productId);
                            }
                            else
                            {
                                int.TryParse(ConfigurationManager.AppSettings["EduMaxLenexaWTProductId"], out productId);
                            }
                            if (productId > 0)
                            {
                                using (APINexusEntities context = new APINexusEntities())
                                {
                                    ClientRelationship clientRelationship = context.ClientRelationships.Where(m => m.Buyer.Contains(questionAnswer.QuestionValue)).FirstOrDefault();
                                    if (clientRelationship != null)
                                    {
                                        VW_Matching_ProgramProduct_Prod programProduct = context.VW_Matching_ProgramProduct_Prod.Where(m => m.ClientRelationshipId == clientRelationship.ClientRelationshipId && m.ProductId == productId).FirstOrDefault();
                                        if (programProduct != null)
                                        {
                                            programProductId = programProduct.ProgramProductId;
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;

            }
            return programProductId;
        }

        private int getStoreFrontIdByName(string stroreFrontName)
        {
            int storeFrontId = 0;
            using (APINexusEntities context = new APINexusEntities())
            {
                StoreFront stroreFront = context.StoreFronts.Where(m => m.StoreFrontName.ToLower() == stroreFrontName.ToLower()).FirstOrDefault();
                if (stroreFront != null)
                {
                    storeFrontId = stroreFront.StoreFrontId;
                }

            }
            return storeFrontId;
        }

        private int getCallCenterId(VendorCampaign vendorCampaign, ContactRequest contactRequest)
        {
            int callCenterId = 0;
            switch (vendorCampaign.CampaignTypeId)
            {
                case 7:
                case 8:
                    if (!string.IsNullOrEmpty(contactRequest.TSR))
                    {
                        if (ConfigurationManager.ConnectionStrings["ContactCenter"] != null)
                        {
                            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ContactCenter"].ConnectionString);
                            conn.Open();

                            SqlCommand command = new SqlCommand("Select CallCenterID from [dbo].[AgentMaster] where tsr=@tsr", conn);



                            command.Parameters.AddWithValue("@tsr", contactRequest.TSR);


                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // Don't assume we have any rows.
                                {
                                    int.TryParse(reader["CallCenterID"].ToString(), out callCenterId);


                                }
                            }
                            conn.Close();
                        }

                    }
                    break;
                default:
                    using (APINexusEntities context = new APINexusEntities())
                    {
                        CallCenter callCenter = context.CallCenters.Where(m => m.VendorId == vendorCampaign.VendorId).FirstOrDefault();
                        if (callCenter != null)
                        {
                            callCenterId = callCenter.CallCenterId;
                        }

                    }
                    break;
            }
            return callCenterId;
        }

        private bool saveProspectDetails(ContactRequest contactRequest)
        {
            bool isSaved = false;
            ProspectServiceClient prospectServiceClient = new ProspectServiceClient();
            GetProspectFlowDetailsRequest getProspectFlowDetailsRequest = new GetProspectFlowDetailsRequest();

            getProspectFlowDetailsRequest.ProspectFlowID = contactRequest.ProspectFlowId;

            GetProspectFlowDetailsResponse prospectFlowDetailsResponse = prospectServiceClient.GetProspectFlowDetailsById(getProspectFlowDetailsRequest);
            if (prospectFlowDetailsResponse != null && prospectFlowDetailsResponse.Errors == null)
            {
                ProspectDTO prospectDto = mapProspectDTO(contactRequest);
                if (prospectDto != null && prospectFlowDetailsResponse.ProspectFlowDTO != null)
                {
                    ProspectFlowDetailsDTO prospectFlowDetailsDTO = new ProspectFlowDetailsDTO();
                    prospectFlowDetailsDTO.ProspectFlowTypeId = prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowTypeId;
                    prospectFlowDetailsDTO.ProspectReasonId = (prospectFlowDetailsResponse.ProspectFlowDTO.ProspectReasonId.HasValue ? prospectFlowDetailsResponse.ProspectFlowDTO.ProspectReasonId.Value : 0);
                    prospectFlowDetailsDTO.ProspectStatusId = prospectFlowDetailsResponse.ProspectFlowDTO.ProspectStatusId;
                    prospectFlowDetailsDTO.TrackId = contactRequest.APIKey;
                    SaveProspectDetailsRequest saveProspectDetailsRequest = new SaveProspectDetailsRequest
                    {
                        ProspectFlowId = contactRequest.ProspectFlowId,
                        Prospect = prospectDto,
                        ProspectFlowDetails = prospectFlowDetailsDTO,

                    };

                    SaveProspectDetailsResponse saveProspectDetailsResponse = prospectServiceClient.SaveProspectDetails(saveProspectDetailsRequest);
                    if (saveProspectDetailsResponse.Errors != null && saveProspectDetailsResponse.Errors.Length > 0)
                    {
                        EDDYLogger.LogMessage(this, LogLevel.Error, "Save Prospect Details Response" + string.Join(",", saveProspectDetailsResponse.Errors.ToList()));
                        return isSaved;
                    }
                    else
                    {
                        isSaved = true;
                    }
                }


            }
            else
            {
                if (prospectFlowDetailsResponse.Errors != null && prospectFlowDetailsResponse.Errors.Length > 0)
                {
                    EDDYLogger.LogMessage(this, LogLevel.Error, "Get Prospect Flow Details Response" + string.Join(",", prospectFlowDetailsResponse.Errors.ToList()));
                    return isSaved;
                }
            }
            return isSaved;
        }

        private ProspectDTO mapProspectDTO(ContactRequest contactRequest)
        {

            ProspectDTO prospectDto = new ProspectDTO();
            prospectDto.Address1 = contactRequest.Address;
            prospectDto.Address2 = contactRequest.Address2;
            prospectDto.City = contactRequest.City;

            prospectDto.Email = contactRequest.Email;
            prospectDto.FirstName = contactRequest.FirstName;
            prospectDto.LastName = contactRequest.LastName;
            prospectDto.Phone = contactRequest.Phone;
            prospectDto.AltPhone = contactRequest.AlternatePhone;
            prospectDto.PostalCode = contactRequest.PostalCode;


            Entities.Country country = this.getAllCountries().Where(c => c.CountryCode == contactRequest.Country).FirstOrDefault();
            if (country == null)
            {
                country = this.getAllCountries().Where(c => c.CountryCode == "US").FirstOrDefault();
            }

            prospectDto.CountryID = country.CountryId;

            if (!String.IsNullOrEmpty(contactRequest.State))
            {
                Entities.State state = this.getAllStates().Where(s => s.CountryId == country.CountryId && s.StateCode == contactRequest.State).FirstOrDefault();
                if (state != null)
                    prospectDto.StateID = state.StateId;
            }

            if (!String.IsNullOrEmpty(contactRequest.MilitaryAffiliation))
            {
                int militaryId;
                if (int.TryParse(contactRequest.MilitaryAffiliation, out militaryId))
                {
                    prospectDto.MilitaryStatusID = militaryId;
                }
            }

            if (contactRequest.Age != 0)
            {
                prospectDto.Age = contactRequest.Age;
            }

            if (!String.IsNullOrEmpty(contactRequest.HighestLevelofEducationCompleted))
            {
                if (contactRequest.HighestLevelofEducationCompleted != "0")
                {
                    int highestLevelofEducationCompleted;
                    if (int.TryParse(contactRequest.HighestLevelofEducationCompleted, out highestLevelofEducationCompleted))
                    {
                        prospectDto.EducationLevelID = highestLevelofEducationCompleted;
                    }
                }
            }

            if (contactRequest.YearHighestEducationCompleted != 0)
            {
                prospectDto.GraduationYear = contactRequest.YearHighestEducationCompleted;
            }

            //Cycle 16.04 - Always pass the IsUsCitizen = true
            prospectDto.IsUsCitizen = true;
            return prospectDto;
        }

        private DataModels.User getAgentUserByTSR(string tsr)
        {
            DataModels.User agentUser;

            using (var context = new APINexusEntities())
            {
                agentUser = context.Users.Where(m => m.TSR == tsr).FirstOrDefault();
            }

            return agentUser;
        }

        #endregion

        #region Helper Methods
        private VendorResponseBase createSuccessfulResponse(APISubmissionResultDTO submissionResult, decimal estimatedRevShare = 0)
        {
            LeadSubmissionResponse vendorLeadSubmissionResponse = new LeadSubmissionResponse
            {
                UID = submissionResult.UID,
                LeadTier = this.getProductFromLead(submissionResult.LeadId),
                LeadId = Convert.ToInt64(submissionResult.LeadId),
                EstimatedRevShare = estimatedRevShare

            };

            return new VendorResponseBase
            {
                IsSuccessful = true,
                Body = vendorLeadSubmissionResponse
            };
        }

        private VendorResponseBase createFailureResponse(IEnumerable<string> messages, APISubmissionResultDTO submissionResult)
        {
            var vendorResponse = new VendorResponseBase
            {
                IsSuccessful = false,
                Messages = new List<VendorResponseMessage>()
            };

            VendorResponseMessage baseMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FailedProgramRule);

            foreach (string message in messages)
            {
                VendorResponseMessage vendorResponseMessage = new VendorResponseMessage
                {
                    MessageCode = baseMessage.MessageCode,
                    Message = string.Format("{0} {1}", baseMessage.Message, message)
                };

                if (vendorResponse.Messages.FirstOrDefault(m => m.Message == vendorResponseMessage.Message) == null)
                    vendorResponse.Messages.Add(vendorResponseMessage);
            }

            if (submissionResult.LeadId > 0)
            {
                LeadSubmissionResponse vendorLeadSubmissionResponse = new LeadSubmissionResponse
                {
                    LeadId = Convert.ToInt64(submissionResult.LeadId)
                };
                vendorResponse.Body = vendorLeadSubmissionResponse;
            }

            return vendorResponse;
        }

        private VendorResponseBase createEdumaxFailureResponse(IEnumerable<string> messages, APISubmissionResultDTO submissionResult)
        {
            var vendorResponse = new VendorResponseBase
            {
                IsSuccessful = false,
                Messages = new List<VendorResponseMessage>()
            };

            VendorResponseMessage baseMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FailedProgramRule);

            foreach (string message in messages)
            {
                VendorResponseMessage vendorResponseMessage = new VendorResponseMessage
                {
                    MessageCode = baseMessage.MessageCode,
                    Message = string.Format("{0} {1}", baseMessage.Message, message)
                };

                if (vendorResponse.Messages.FirstOrDefault(m => m.Message == vendorResponseMessage.Message) == null)
                    vendorResponse.Messages.Add(vendorResponseMessage);
            }

            if (submissionResult.LeadId > 0)
            {
                EduMaxLeadSubmissionResponse vendorLeadSubmissionResponse = new EduMaxLeadSubmissionResponse
                {
                    LeadId = Convert.ToInt32(submissionResult.LeadId)
                };
                vendorResponse.Body = vendorLeadSubmissionResponse;
            }

            return vendorResponse;
        }

        private VendorResponseBase createExceptionResponse()
        {
            VendorResponseMessage exceptionMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);

            return new VendorResponseBase
            {
                IsSuccessful = false,
                Messages = new List<VendorResponseMessage> { exceptionMessage }
            };
        }
        #endregion

    }
}
