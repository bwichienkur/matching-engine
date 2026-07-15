using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.LeadScoringService;
using System.Threading;
using EDDY.IS.Core.Logging;
using System.Configuration;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Caching;


namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
        private static LeadScoringService.LeadScoringServiceClient LeadScoringService = new LeadScoringService.LeadScoringServiceClient();

        #region Lead Scoring Service

        public ScoringResponse GetLeadScore(ScoringRequest request)
        {
            ScoringResponse response = new ScoringResponse();
            try
            {
                response = LeadScoringService.GetLeadScore(request);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }
            return response;
        }


        public ScoringRequest BuildLeadScoreRequest(Guid TrackGuid, Dictionary<string, string> LeadData, List<int> CategoryList, List<int> SubjectList, int? ProgramLevelId, MatchingEngine.ProspectInput Prospect)
        {
            ScoringRequest request = new ScoringRequest();
            request.CampaignTrackGuid = TrackGuid;
            request.ProspectInput = new  LeadScoringService.ProspectInput();
            request.ProspectInput.FirstName = LeadData.ContainsKey("First_Name") ? LeadData["First_Name"].ToUpper() : null;
            request.ProspectInput.LastName = LeadData.ContainsKey("Last_Name") ? LeadData["Last_Name"].ToUpper() : null;
            request.ProspectInput.Email = LeadData.ContainsKey("Email") ? LeadData["Email"].ToUpper() : null;
            request.ProspectInput.Phone1 = LeadData.ContainsKey("Phone") ? LeadData["Phone"].ToUpper() : null;
            request.ProspectInput.Phone2 = LeadData.ContainsKey("Alternate_Phone") ? LeadData["Alternate_Phone"].ToUpper() : null;
            request.ProspectInput.StreetAddress = LeadData.ContainsKey("Address") ? LeadData["Address"].ToUpper() : null;
            request.ProspectInput.City = LeadData.ContainsKey("city") ? LeadData["city"].ToUpper() : null;

            if (LeadData.ContainsKey("Age") && LeadData["Age"].ToUpper() != null)
            {
                int Age = 0;
                if (Int32.TryParse(LeadData["Age"], out Age))
                {
                    request.ProspectInput.Age = Age;
                }
            }

            request.ProspectInput.PostalCode = LeadData.ContainsKey("Postal_Code") ? LeadData["Postal_Code"].ToUpper() : null;

            if (LeadData.ContainsKey("state") && LeadData["state"].ToUpper() != null)
            {
                request.ProspectInput.StateCode = LeadData["state"].ToUpper();
            }

            if (Prospect != null)
            {
                request.ProspectInput.StateId = Prospect.StateId;
                request.ProspectInput.AddressLine2 = Prospect.AddressLine2;
            }

            if (LeadData.ContainsKey("country") && LeadData["country"].ToUpper() != null)
            {
                request.ProspectInput.CountryCode = LeadData["country"].ToUpper();
            }

            if (LeadData.ContainsKey("Highest_Level_of_Education_Completed"))
            {
                int edLevel = 0;
                if (Int32.TryParse(LeadData["Highest_Level_of_Education_Completed"], out edLevel))
                {
                    request.ProspectInput.EducationLevelId = edLevel;
                }
            }

            if (LeadData.ContainsKey("Year_of_Highest_Education_Completed"))
            {
                int hsYear = 0;
                if (Int32.TryParse(LeadData["Year_of_Highest_Education_Completed"], out hsYear))
                {
                    request.ProspectInput.HSGraduationYear = hsYear;
                }
            }

            if (LeadData.ContainsKey("Military_Affiliation"))
            {
                int military = 0;
                if (Int32.TryParse(LeadData["Military_Affiliation"], out military))
                {
                    request.ProspectInput.MilitaryStatusId = military;
                }
            }

            request.ProspectInput.IsCitizen = LeadData.ContainsKey("US_Citizen") ? LeadData["US_Citizen"].ToUpper() == "TRUE" : false;
            request.ProspectInput.IsMilitary = LeadData.ContainsKey("Military_Affiliation") ? LeadData["Military_Affiliation"].ToUpper() != "126" && LeadData["Military_Affiliation"].ToUpper() != null : false;
            request.CriteriaInput = new CriteriaInput();
            request.CriteriaInput.CategoryList = CategoryList.ToArray();
            request.CriteriaInput.SubjectList = SubjectList.ToArray();
            request.CriteriaInput.ProgramLevelId = ProgramLevelId;
            return request;
        }

        public MatchingEngine.LeadScoringProductAssignment ConvertLeadScoreToMatchingProductAssignment(LeadScoringService.ProductAssignment lsProdAssignment)
        {
            MatchingEngine.LeadScoringProductAssignment meProductAssignment = new MatchingEngine.LeadScoringProductAssignment();

            if (lsProdAssignment == null)
                meProductAssignment = null;
            else
            {
                List<MatchingEngine.CrProductAssignment> tbr = new List<MatchingEngine.CrProductAssignment>();

                meProductAssignment.ChosenModelProductId = lsProdAssignment.ChosenModelProductId;

                foreach (LeadScoringService.CrProductAssignment l in lsProdAssignment.CrProductAssignmentList)
                {
                    MatchingEngine.CrProductAssignment m = new MatchingEngine.CrProductAssignment() { CrId = l.CrId, ProductId = l.ProductId };
                    tbr.Add(m);
                }

                meProductAssignment.CrProductAssignmentList = tbr.ToArray();
            }

            return meProductAssignment;
        }

        private bool LeadScoreRequestsAreEqual(LeadScoringService.ScoringRequest originalRequest, LeadScoringService.ScoringRequest newRequest)
        {
            //if anything is not equal then they arent the same
            if (originalRequest.CampaignTrackGuid != newRequest.CampaignTrackGuid ||
                !originalRequest.CriteriaInput.CategoryList.SequenceEqual(newRequest.CriteriaInput.CategoryList) ||
                originalRequest.CriteriaInput.ProgramLevelId != newRequest.CriteriaInput.ProgramLevelId ||
                !originalRequest.CriteriaInput.SubjectList.SequenceEqual(newRequest.CriteriaInput.SubjectList))
            {
                return false;
            }

            //need to compare the prospect input fields
            if (originalRequest.ProspectInput.AddressLine2 != newRequest.ProspectInput.AddressLine2 ||
                originalRequest.ProspectInput.Age != newRequest.ProspectInput.Age ||
                originalRequest.ProspectInput.City != newRequest.ProspectInput.City ||
                originalRequest.ProspectInput.CountryCode != newRequest.ProspectInput.CountryCode ||
                originalRequest.ProspectInput.EducationLevelId != newRequest.ProspectInput.EducationLevelId ||
                originalRequest.ProspectInput.Email != newRequest.ProspectInput.Email ||
                originalRequest.ProspectInput.FirstName != newRequest.ProspectInput.FirstName ||
                originalRequest.ProspectInput.HSGraduationYear != newRequest.ProspectInput.HSGraduationYear ||
                originalRequest.ProspectInput.IsCitizen != newRequest.ProspectInput.IsCitizen ||
                originalRequest.ProspectInput.IsMilitary != newRequest.ProspectInput.IsMilitary ||
                originalRequest.ProspectInput.MilitaryStatusId != newRequest.ProspectInput.MilitaryStatusId ||
                originalRequest.ProspectInput.LastName != newRequest.ProspectInput.LastName ||
                originalRequest.ProspectInput.Phone1 != newRequest.ProspectInput.Phone1 ||
                originalRequest.ProspectInput.Phone2 != newRequest.ProspectInput.Phone2 ||
                originalRequest.ProspectInput.PostalCode != newRequest.ProspectInput.PostalCode ||
                originalRequest.ProspectInput.StateCode != newRequest.ProspectInput.StateCode ||
                originalRequest.ProspectInput.StreetAddress != newRequest.ProspectInput.StreetAddress)
            {
                return false;
            }

            return true;
        }

        public LeadScoringInput GetLeadScoringInput(LeadScoringService.ScoringRequest request, string FESessionId)
        {
            LeadScoringInput lsInput = null;
            LeadScoreDTO lastDTO = null;

            if (!String.IsNullOrWhiteSpace(FESessionId))
                lastDTO = FESession.Get<LeadScoreDTO>(FESessionId, Constants.LEADSCORE_SESSION_KEY);

            //first check if we already tested for this information. if yes just use the session dto.
            if (lastDTO != null && LeadScoreRequestsAreEqual(lastDTO.Request, request))
            {
                if (lastDTO.Response != null)
                {
                    lsInput = new LeadScoringInput() { LeadScoringTierLevel = lastDTO.Response.LeadScoringTierLevel, LeadScoringGuid = lastDTO.Response.LeadScoringGuid, ProductAssignment = ConvertLeadScoreToMatchingProductAssignment(lastDTO.Response.ProductAssignment) };
                }
            }
            else
            {
                //if we didnt then request the score.
                LeadScoringService.ScoringResponse score = GetLeadScore(request);
                if (score.ScoringResponseType == ScoringResponseType.Success)
                {
                    lsInput = new LeadScoringInput() { LeadScoringTierLevel = score.LeadScoringTierLevel, LeadScoringGuid = score.LeadScoringGuid, ProductAssignment = ConvertLeadScoreToMatchingProductAssignment(score.ProductAssignment) };
                }
                LeadScoreDTO currDTO = new LeadScoreDTO() { Request = request, Response = score };
                FESession.Set(FESessionId, Constants.LEADSCORE_SESSION_KEY, currDTO);
            }
            //return whatever we have here
            return lsInput;
        }

        public Guid? GetLeadScoreApollo(LeadScoringService.ScoringRequest request)
        {
            Guid? leadScoringGuid = null;

            LeadScoringService.ScoringResponse score = GetLeadScore(request);

            if (score != null)
                leadScoringGuid = score.LeadScoringGuid;

            return leadScoringGuid;
        }

        #endregion Lead Scoring Service

    }
}
