using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(null,
                            new InputRequired[] { 
                                                  //Removed Age for Post iDatify
                                                  //InputRequired.Age,
                                                  InputRequired.Country,
                                                  InputRequired.EducationLevel,
                                                  InputRequired.IsMilitary,
                                                  InputRequired.PostalCode,
                                                  InputRequired.State,
                                                  InputRequired.City,
                                                  InputRequired.Email,
                                                  InputRequired.FirstName,
                                                  InputRequired.LastName,
                                                  InputRequired.Phone1,
                                                  InputRequired.StreetAddress,
                                                  InputRequired.HSGraduationYear,
                                                  InputRequired.MilitaryStatus                                            
                                                }
                            , null)]
    public class LeadPing : Rule
    {
        public LeadPing(RuleInput ri)
            : base(ri)
        { }

        public LeadPingRuleResult ExecuteLeadPing(List<LeadPingService.InstitutionConfig> institutions, PerformanceLog pLog)
        {
            LeadPingRuleResult result = new LeadPingRuleResult();

            pLog.StartLogDetail("LeadPingRule.CallLeadPingService");
            LeadPingService.ValidationResponse response = CallLeadPingService(institutions, pLog);
            pLog.EndLogDetail();

            pLog.StartLogDetail("LeadPingRule.ProcessResponse");
            if (response.Status == LeadPingService.ServiceCallStatus.Sucess)
            {
                HashSet<int> internalDupeInsts = new HashSet<int>();

                foreach (LeadPingService.FailedInstitutionResponse ins in response.ImmediateResponses)
                {
                    LeadPingRuleOutput lprOutput = new LeadPingRuleOutput();
                    lprOutput.InstitutionId = ins.InstitutionID;
                    lprOutput.ClientRelationshipId = ins.ClientRelationshipId;
                    lprOutput.CampusId = ins.CampusID;

                    if (ins.ProgramLevelIDs != null && ins.ProgramLevelIDs.Count() > 0)
                        lprOutput.ProgramLevelIDs = ins.ProgramLevelIDs.ToList();

                    if (ins.IsOnline.HasValue)
                        lprOutput.IsOnline = ins.IsOnline;

                    if (ins.IsInternal)
                    {
                        if (ins.ProductIDs != null && ins.ProductIDs.Count() > 0)
                        {
                            lprOutput.Products = new HashSet<int>();
                            lprOutput.Products.UnionWith(ins.ProductIDs);
                        }

                        lprOutput.BaseRuleType = BaseRuleDefinitionType.InternalDuplicate;
                        internalDupeInsts.Add(ins.InstitutionID);
                    }
                    else if (ins.Type == LeadPingService.Type.Duplicate)
                    {
                        lprOutput.BaseRuleType = BaseRuleDefinitionType.ExternalDuplicate;
                    }
                    else
                    {
                        if (ins.ProductIDs != null && ins.ProductIDs.Count() > 0)
                        {
                            lprOutput.Products = new HashSet<int>();
                            lprOutput.Products.UnionWith(ins.ProductIDs);
                            lprOutput.BaseRuleType = BaseRuleDefinitionType.LeadScore;
                        }
                        else
                        {
                            //Mark as external duplicate if no products because it's a dupe check
                            lprOutput.BaseRuleType = BaseRuleDefinitionType.ExternalDuplicate;
                        }

                        if (ins.IsEdmcWtDupe == true)
                        {
                            lprOutput.IsWtEdmcDupe = true;
                        }
                    }

                    lprOutput.RuleName = ins.Message;

                    result.RemovedInstitutions.Add(lprOutput);
                }

                foreach (int i in GetInternalDupeInstitutionsInGroups(internalDupeInsts))
                {
                    LeadPingRuleOutput lpri = new LeadPingRuleOutput();
                    lpri.InstitutionId = i;
                    lpri.Products = new HashSet<int>();
                    lpri.BaseRuleType = BaseRuleDefinitionType.InternalDuplicate;
                    result.RemovedInstitutions.Add(lpri);
                }

                foreach (LeadPingService.ScoreInstitutionResponse scoreInstitution in response.ScoreResponses) {
                    LeadPingScoreOutput lpsOutput = new LeadPingScoreOutput();
                    lpsOutput.InstitutionId = scoreInstitution.InstitutionID;
                    if (scoreInstitution.ProductIDs != null && scoreInstitution.ProductIDs.Count() > 0) {
                        lpsOutput.ProductIDs = scoreInstitution.ProductIDs.ToList();
                    }
                    lpsOutput.CampusID = scoreInstitution.CampusID;
                    lpsOutput.Score = scoreInstitution.Score;
                    lpsOutput.ScoreId = scoreInstitution.ScoreId;
                    result.ScoreInstitution.Add(lpsOutput);
                }
            }
            else
            {
                EDDY.IS.Core.Logging.ISException e = new ISException(new Exception(response.ServerMessage));
            }
            pLog.EndLogDetail();

            return result;
        }

        private HashSet<int> GetInternalDupeInstitutionsInGroups(HashSet<int> internalDupeInsts)
        {
            InstitutionGroupCache institionGroups = StaticCacheProxyHost.CacheProxy.Get<InstitutionGroupCache>(MatchingCacheItem.InstitutionGroup);
            HashSet<int> internalDupeInstsToAdd = new HashSet<int>();

            foreach (int i in internalDupeInsts)
            {
                if (institionGroups.InstitutionToGroups.ContainsKey(i))
                {
                    foreach (var group in institionGroups.InstitutionToGroups[i])
                    {
                        if(!institionGroups.GroupsThatAllowMultipleSelections.Contains(group.Item1))
                            internalDupeInstsToAdd.UnionWith(institionGroups.GroupToInstitutions[group.Item1]);
                    }
                }
            }

            if (internalDupeInstsToAdd.Count > 0)
                internalDupeInstsToAdd.ExceptWith(internalDupeInsts);

            return internalDupeInstsToAdd;
        }

        private LeadPingService.ProspectInfo CreateLeadPingProspect()
        {
            LeadPingService.ProspectInfo prospect = new LeadPingService.ProspectInfo();
           
            if(this.ruleInput.prospectData.Age.HasValue)
                prospect.Age = this.ruleInput.prospectData.Age.Value;

            prospect.AddressLine2 = this.ruleInput.prospectData.AddressLine2;
            prospect.ExternalLeadId = this.ruleInput.prospectData.ExternalLeadId;
            prospect.City = this.ruleInput.prospectData.City;
            prospect.CountryID = this.ruleInput.prospectData.CountryId.Value;
            prospect.EducationLevelID = this.ruleInput.prospectData.EducationLevelId.Value;
            prospect.Email = this.ruleInput.prospectData.Email;
            prospect.FirstName = this.ruleInput.prospectData.FirstName;
            prospect.LastName = this.ruleInput.prospectData.LastName;
            prospect.MilitaryAffiliation = this.ruleInput.prospectData.IsMilitary.Value;
            prospect.Phone1 = this.ruleInput.prospectData.Phone1;
            prospect.CampaignCECLeadScore = ruleInput.Campaign.CECLeadScore;

            if (String.IsNullOrEmpty(this.ruleInput.prospectData.Phone2))
                prospect.Phone2 = prospect.Phone1;
            else
                prospect.Phone2 = this.ruleInput.prospectData.Phone2;

            prospect.StateID = this.ruleInput.prospectData.StateId.Value;
            prospect.StreetAddress = this.ruleInput.prospectData.StreetAddress;
            prospect.ZipCode = this.ruleInput.prospectData.PostalCode;
            prospect.HighSchoolGradyear = this.ruleInput.prospectData.HSGraduationYear.Value;
            prospect.MilitaryStatusId = this.ruleInput.prospectData.MilitaryStatusId.Value;

            prospect.DesiredStartDate = this.ruleInput.prospectData.KVCodeData != null ? this.ruleInput.prospectData.KVCodeData.FirstOrDefault(kvp => kvp.Key.ToLower() == "desired_start_date").Value : (int?)null;
            prospect.CampaignTrackId = this.ruleInput.Campaign.TrackId;
            prospect.ApplicationId = this.ruleInput.Campaign.ApplicationId.Value;
            if(this.ruleInput.LeadCreationTypeID.HasValue)
                prospect.LeadCreationTypeId = (int)this.ruleInput.LeadCreationTypeID;
            prospect.SubmissionFormUrl = this.ruleInput.prospectData.FormLeadUrl;
            prospect.SubmissionLeadSourceUrl = this.ruleInput.prospectData.LeadSourceUrl;
            prospect.SubmissionLeadInitiatingUrl = this.ruleInput.prospectData.LeadInitiatingUrl;
            prospect.TrackingSessionGUID = this.ruleInput.prospectData.TrackingSessionGUID;
            prospect.AffiliateId = this.ruleInput.prospectData.AffiliateId;
            prospect.SubSource1 = this.ruleInput.prospectData.SubSource1;
            prospect.SubSource2 = this.ruleInput.prospectData.SubSource2;
            prospect.InitiatingUrl = this.ruleInput.prospectData.InitiatingURL;
            prospect.LandingUrl = this.ruleInput.prospectData.LandingURL;
            prospect.CallCenterUrl = this.ruleInput.prospectData.CallCenterURL;
            prospect.FormLeadUrl = this.ruleInput.prospectData.FormLeadUrl;
            prospect.VideoUrl = this.ruleInput.prospectData.VideoUrl;

            return prospect;
        }

        private bool ValidCecUniqueId(string cecId)
        {
            long cId;
            if (!long.TryParse(cecId, out cId))
                return false;

            if (cecId.Length < 8)
                return false;

            return true;
        }

        private LeadPingService.ValidationResponse CallLeadPingService(List<LeadPingService.InstitutionConfig> institutions, PerformanceLog pLog)
        {
            LeadPingService.ServiceClient client = new LeadPingService.ServiceClient();
            LeadPingService.ValidationRequest request = new LeadPingService.ValidationRequest();
            List<LeadPingService.InstitutionConfig> institutionConfigs = new List<LeadPingService.InstitutionConfig>();

            pLog.StartLogDetail("CreateLeadPingRequest");
            for (int i = 0; i < institutions.Count; i++)
            {
                int crId = institutions[i].ClientrelationshipID;

                if ((crId == 22 || crId == 23 || crId == 5712) && !String.IsNullOrEmpty(ruleInput.prospectData.CecUniqueId) && ValidCecUniqueId(ruleInput.prospectData.CecUniqueId))
                {
                    continue;
                }
                else
                {
                    LeadPingService.InstitutionConfig ic = new LeadPingService.InstitutionConfig();
                    ic.InstitutionID = institutions[i].InstitutionID;
                    ic.ClientrelationshipID = crId;
                    ic.CampusConfigs = CreateCampusConfigs(institutions[i].CampusConfigs.Select(c=>c.CampusID).ToList());
                    ic.BachelorsAvailable = institutions[i].BachelorsAvailable;
                    institutionConfigs.Add(ic);
                }
            }

            request.InsitutionConfigs = institutionConfigs.ToArray();
            request.ProspectInfo = CreateLeadPingProspect();
            pLog.EndLogDetail();

            try
            {
                pLog.StartLogDetail("LeadPingService.RetrieveInstitutionValidationErrors");
                LeadPingService.ValidationResponse resp = client.RetrieveInstitutionValidationErrors(request, "4db789f6-ad55-45aa-b76d-f36fe7b8ac28");
                pLog.EndLogDetail();
                return resp;
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
                return new LeadPingService.ValidationResponse() { Status = LeadPingService.ServiceCallStatus.Failure };
            }
        }

        private LeadPingService.CampusConfig[] CreateCampusConfigs(List<int> campusIds)
        {
            List<CampusContent> campusContent = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
            GeoCodeProcessor gcp = new GeoCodeProcessor();

            var campusList = (from c in campusIds
                              join cc in campusContent
                              on c equals cc.CampusId
                              select new LeadPingService.CampusConfig
                              {
                                  CampusID = cc.CampusId,
                                  ZipCode = cc.PostalCode,
                                  Address = cc.Address1,
                                  City = cc.City,
                                  State = cc.StateProvinceCode,
                                  IsOnline = true
                              }).ToList();

            foreach (LeadPingService.CampusConfig cc in campusList)
            {
                if (!md.IsCampusOnline(cc.CampusID))
                {
                    cc.IsOnline = false;
                    double? miles = gcp.GetDistanceBetweenZipCodes(cc.ZipCode, this.ruleInput.prospectData.PostalCode);

                    if (miles.HasValue)
                        cc.MilesFromUser = Convert.ToDecimal(miles.Value);
                }
            }

            return campusList.ToArray();
        }
    }
}

