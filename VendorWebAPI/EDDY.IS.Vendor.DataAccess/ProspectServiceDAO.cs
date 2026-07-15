using System;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Utilities.NoNull;
using EDDY.IS.Vendor.DataAccess.ProspectService;


using EDDY.IS.Vendor.Utilities;

namespace EDDY.IS.Vendor.DataAccess
{
    public class ProspectServiceDAO : VendorBaseDAO
    {
        ProspectServiceClient prospectServiceClient = new ProspectServiceClient();
        public VendorResponseBase SaveProspect(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;


            try
            {

                Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(contactRequest.APIKey);
                if (vendorCampaign != null)
                {
                    vendorResponseBase = new VendorResponseBase();
                    vendorResponseBase.Messages = new List<VendorResponseMessage>();
                    vendorResponseBase.IsSuccessful = false;

                    bool isDuplicateRecord = prospectServiceClient.CheckRepostingProspect(contactRequest.Email, vendorCampaign.ProspectFlowTypeId, NoNull.Convert.String(contactRequest.APIKey));
                    if (isDuplicateRecord)
                    {
                        VendorResponseMessage duplicateRecordVendorResponseMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.DuplicateRecord);
                        if (duplicateRecordVendorResponseMessage != null)
                        {

                            vendorResponseBase.Messages.Add(duplicateRecordVendorResponseMessage);
                        }
                    }
                    else
                    {
                        SaveProspectRequest saveProspectRequest = new SaveProspectRequest();
                        ProspectFlowDetailsDTO prospectFlowDetailsDto = new ProspectFlowDetailsDTO();
                        prospectFlowDetailsDto.HostAndPostLogId = 0;
                        prospectFlowDetailsDto.ProspectFlowTypeId = vendorCampaign.ProspectFlowTypeId;
                        prospectFlowDetailsDto.ProspectStatusId = vendorCampaign.ProspectStatusId;
                        prospectFlowDetailsDto.AreaofInterest = contactRequest.AreaOfInterest;
                        prospectFlowDetailsDto.ExternalSystemId = contactRequest.ExternalSystemId;
                        //Fields Being passsed
                        //prospectFlowDetailsDto.Options = OptionFields;
                        prospectFlowDetailsDto.TrackId = contactRequest.APIKey;

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

                        if (contactRequest.YearHighestEducationCompleted.HasValue)
                        {

                            prospectDto.GraduationYear = contactRequest.YearHighestEducationCompleted.Value;
                        }

                        //Cycle 16.04 - Always pass the IsUsCitizen = true
                        prospectDto.IsUsCitizen = true;
                        prospectDto.AppProspectSourceID = (int)AppSourceProspect.API;

                        saveProspectRequest.ProspectFlowDetails = prospectFlowDetailsDto;
                        saveProspectRequest.Prospect = prospectDto;
                        SaveProspectResponse saveProspectResponse = prospectServiceClient.SaveProspect(saveProspectRequest);

                        if (saveProspectResponse.ProspectFlowId != 0)
                        {
                            Dictionary<string, string> prospectAdditionalQuestions = new Dictionary<string, string>();

                            if (!String.IsNullOrEmpty(contactRequest.ProspectSourceUrl))
                            {
                                prospectAdditionalQuestions.Add("ProspectSourceUrl", contactRequest.ProspectSourceUrl);
                            }

                            if (!String.IsNullOrEmpty(contactRequest.ProspectInitiatingUrl))
                            {
                                prospectAdditionalQuestions.Add("ProspectInitiatingUrl", contactRequest.ProspectInitiatingUrl);
                            }

                            if (contactRequest.AdditionalQuestions != null && contactRequest.AdditionalQuestions.Count > 0)
                            {
                                foreach (var x in contactRequest.AdditionalQuestions)
                                {
                                    prospectAdditionalQuestions.Add(x.QuestionKey, x.QuestionValue);
                                }
                            }

                            int saved = prospectServiceClient.SaveProspectAdditionalData(saveProspectResponse.ProspectId, contactRequest.Email, prospectAdditionalQuestions);

                            vendorResponseBase.IsSuccessful = true;
                            ProspectSubmissionResponse vendorProspectSubmissionResponse = new ProspectSubmissionResponse();
                            vendorProspectSubmissionResponse.ProspectFlowId = saveProspectResponse.ProspectFlowId;
                            vendorProspectSubmissionResponse.ProspectId = saveProspectResponse.ProspectId;
                            vendorResponseBase.Body = vendorProspectSubmissionResponse;
                        }
                        else
                        {
                            vendorResponseBase.IsSuccessful = true;
                            foreach (var error in saveProspectResponse.Errors)
                            {
                                VendorResponseMessage vendorResponseMessage = new VendorResponseMessage();
                                vendorResponseMessage.MessageCode = error.Name;
                                vendorResponseMessage.Message = error.Message;
                                vendorResponseBase.Messages.Add(vendorResponseMessage);
                            }
                        }

                    }

                }
            }
            catch (Exception exc)
            {
                vendorResponseBase = new VendorResponseBase();
                vendorResponseBase.IsSuccessful = false;
                vendorResponseBase.Messages = new List<VendorResponseMessage>();
                VendorResponseMessage exceptionMessage = this.getVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                vendorResponseBase.Messages.Add(exceptionMessage);
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return vendorResponseBase;
        }

        public VendorResponseBase GetProspectFlowDetailsById(LeadDetailsRequest leadDetailsRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {

                ProspectServiceClient prospectServiceClient = new ProspectServiceClient();
                GetProspectFlowDetailsRequest prospectFlowDetailsRequest = new GetProspectFlowDetailsRequest();
                prospectFlowDetailsRequest.ProspectFlowID = leadDetailsRequest.ProspectFlowId;

                GetProspectFlowDetailsResponse prospectFlowDetailsResponse = prospectServiceClient.GetProspectFlowDetailsById(prospectFlowDetailsRequest);
                if (prospectFlowDetailsResponse != null && prospectFlowDetailsResponse.ProspectFlowDTO != null && prospectFlowDetailsResponse.ProspectDTO != null)
                {
                    vendorResponseBase = new VendorResponseBase();
                    LeadDetails leadDetails = new LeadDetails();
                    leadDetails.Advisors = this.getAllAdvisors();
                    leadDetails.ProspectFlowId = prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowId;
                    leadDetails.FirstName = prospectFlowDetailsResponse.ProspectDTO.FirstName;
                    leadDetails.LastName = prospectFlowDetailsResponse.ProspectDTO.LastName;
                    leadDetails.ProspectStatuses = this.getAllProspectStatuses();
                    Entities.ProspectStatus prospectStatus = null;
                    if (leadDetails.ProspectStatuses != null)
                    {
                        prospectStatus = leadDetails.ProspectStatuses.FirstOrDefault(x => x.ProspectStatusId == prospectFlowDetailsResponse.ProspectFlowDTO.ProspectStatusId);
                        leadDetails.CurrentDisposition = prospectStatus != null ? prospectStatus.Name : "";
                    }
                    leadDetails.Address = prospectFlowDetailsResponse.ProspectDTO.Address1;
                    leadDetails.Address2 = prospectFlowDetailsResponse.ProspectDTO.Address2;
                    leadDetails.City = prospectFlowDetailsResponse.ProspectDTO.City;
                    leadDetails.PostalCode = prospectFlowDetailsResponse.ProspectDTO.PostalCode;
                    leadDetails.Email = prospectFlowDetailsResponse.ProspectDTO.Email;
                    leadDetails.Phone = prospectFlowDetailsResponse.ProspectDTO.Phone;
                    leadDetails.AlternatePhone = prospectFlowDetailsResponse.ProspectDTO.AltPhone;
                    leadDetails.CreatedDate = prospectFlowDetailsResponse.ProspectFlowDTO.CreatedDate.ToShortDateString() + " " + prospectFlowDetailsResponse.ProspectFlowDTO.CreatedDate.ToShortTimeString();
                    leadDetails.ModifiedDate = prospectFlowDetailsResponse.ProspectFlowDTO.UpdatedDate.ToShortDateString() + " " + prospectFlowDetailsResponse.ProspectFlowDTO.UpdatedDate.ToShortTimeString();
                    leadDetails.States = this.getAllStates().ToList();
                    if (leadDetails.States != null)
                    {
                        State state = leadDetails.States.FirstOrDefault(x => x.StateId == prospectFlowDetailsResponse.ProspectDTO.StateID && x.CountryId == prospectFlowDetailsResponse.ProspectDTO.CountryID);
                        if (state != null)
                        {
                            leadDetails.State = state.Name;
                            Country country = this.getAllCountries().Where(c => c.CountryId == state.CountryId).FirstOrDefault();
                            leadDetails.Country = country.Name;
                        }
                    }

                    if (leadDetails.Advisors != null)
                    {
                        Advisor advisor = leadDetails.Advisors.FirstOrDefault(x => x.Email == leadDetailsRequest.AdvisorEmail);
                        if (advisor != null)
                        {
                            leadDetails.AdvisorEmail = advisor.Email;
                            leadDetails.AdvisorId = advisor.AdvisorId;
                        }
                    }
                    if (prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowNotes != null)
                    {
                        if (prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowNotes.Length > 0)
                        {
                            leadDetails.Notes = new List<LeadNote>();
                            foreach (ProspectFlowNoteDTO prospectFlowNote in prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowNotes.OrderByDescending(t => t.CreatedDate))
                            {
                                if (leadDetails.Advisors != null)
                                {
                                    var currentAdvisor = leadDetails.Advisors.FirstOrDefault(x => x.AdvisorId == prospectFlowNote.CreatedBy);

                                    leadDetails.Notes.Add(new LeadNote
                                    {
                                        Advisor = (currentAdvisor != null) ? currentAdvisor.FirstName + " " + currentAdvisor.LastName : "",
                                        CreatedDate = prospectFlowNote.CreatedDate.ToShortDateString() + " " + prospectFlowNote.CreatedDate.ToShortTimeString(),
                                        Note = prospectFlowNote.Notes
                                    });
                                }
                            }
                        }
                    }

                    if (prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowStatusHistories != null)
                    {
                        if (prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowStatusHistories.Length > 0)
                        {
                            leadDetails.LeadDispositionHistory = new List<LeadDispositionHistory>();
                            foreach (var prospectFlowStatusHistory in prospectFlowDetailsResponse.ProspectFlowDTO.ProspectFlowStatusHistories.OrderByDescending(t => t.CreatedDate))
                            {
                                if (leadDetails.Advisors != null)
                                {
                                    var currentAdvisor = leadDetails.Advisors.FirstOrDefault(x => x.AdvisorId == prospectFlowStatusHistory.CreatedBy);
                                    if (leadDetails.ProspectStatuses != null)
                                    {
                                        if (prospectStatus != null)
                                        {
                                            leadDetails.LeadDispositionHistory.Add(new LeadDispositionHistory
                                            {
                                                CreatedDate = prospectFlowStatusHistory.CreatedDate.ToShortDateString() + " " + prospectFlowStatusHistory.CreatedDate.ToShortTimeString(),
                                                Disposition = (prospectStatus != null) ? prospectStatus.Name : "",
                                                Advisor = (currentAdvisor != null) ? currentAdvisor.FirstName + " " + currentAdvisor.LastName : "",
                                                Note = prospectFlowStatusHistory.Notes
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    vendorResponseBase.IsSuccessful = true;
                    vendorResponseBase.Body = leadDetails;

                }
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return vendorResponseBase;
        }

        public VendorResponseBase SaveProspectNote(LeadDetailsNoteRequest leadDetailsNoteRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                ProspectServiceClient prospectServiceClient = new ProspectServiceClient();
                SaveProspectNotesRequest saveProspectNotesRequest = new SaveProspectNotesRequest();
                saveProspectNotesRequest.ProspectFlowId = leadDetailsNoteRequest.ProspectFlowId;
                saveProspectNotesRequest.Notes = leadDetailsNoteRequest.Note;
                saveProspectNotesRequest.UserId = leadDetailsNoteRequest.AdvisorId;

                SaveProspectNotesResponse saveProspectNotesResponse = prospectServiceClient.SaveProspectNotes(saveProspectNotesRequest);
                if (saveProspectNotesResponse != null)
                {
                    vendorResponseBase = new VendorResponseBase();
                    if (saveProspectNotesResponse.Errors == null)
                    {

                        LeadDetailsNoteSubmissionResponse leadDetailsNoteSubmissionResponse = new LeadDetailsNoteSubmissionResponse();
                        leadDetailsNoteSubmissionResponse.ProspectNoteId = saveProspectNotesResponse.ProspectFlowNotesId;
                        leadDetailsNoteSubmissionResponse.ProspectFlowId = leadDetailsNoteRequest.ProspectFlowId;
                        vendorResponseBase.IsSuccessful = true;
                        vendorResponseBase.Body = leadDetailsNoteSubmissionResponse;
                    }
                    else
                    {
                        if (saveProspectNotesResponse.Errors.Length > 0)
                        {
                            vendorResponseBase.Messages = new List<VendorResponseMessage>();
                            foreach (var error in saveProspectNotesResponse.Errors)
                            {
                                VendorResponseMessage vendorResponseMessage = new VendorResponseMessage();
                                vendorResponseMessage.MessageCode = error.Name;
                                vendorResponseMessage.Message = error.Message;
                                vendorResponseBase.Messages.Add(vendorResponseMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return vendorResponseBase;
        }
    }
}
