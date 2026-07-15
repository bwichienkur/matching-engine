using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DTO;
using System.Configuration;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship, 
                                                     EntityProcessing.ClientCampusRelationship,
                                                     EntityProcessing.PSI
                                                   }, null
                                                   ,
                           new RuleAttribute[] { RuleAttribute.ExecuteCapRule })]
    public class LeadCap : Rule, ICRProductRule, ICRCampusProductRule, IPsiRule, IClickRule
    {
        private int? VendorId { get; set; }
        private int? ChannelId { get; set; }
        private int? CountryId { get; set; }
        private int? ApplicationId { get; set; }
        private int? SubChannelId { get; set; }
        private int? MarketingUnitId { get; set; }
        private int? CampusTypeId { get; set; }
        private int? EducationLevelId { get; set; }
        private int? MilitaryStatusId { get; set; }
        private LeadCreationType? LeadCreationTypeID { get; set; }

        private int? StateId { get; set; }

        private int? DesiredStartDateId { get; set; }

        private int? AgeGroupId { get; set; }

        public LeadCap(RuleInput input)
            : base(input)
        {
            if (input.Campaign != null)
            {
                VendorId = input.Campaign.VendorId;
                ChannelId = input.Campaign.ChannelId;
                ApplicationId = input.Campaign.ApplicationId;
                SubChannelId = input.Campaign.SubChannelId;
                MarketingUnitId = input.Campaign.MarketingUnitId;
            }

            if (input.prospectData != null)
            {
                CountryId = input.prospectData.CountryId;
                EducationLevelId = input.prospectData.EducationLevelId;
                MilitaryStatusId = input.prospectData.MilitaryStatusId;

                StateId = input.prospectData.StateId;

                if(input.prospectData.KVCodeData != null && !this.ruleInput.prospectData.KVCodeData.FirstOrDefault(kvp => kvp.Key.ToLower() == "desired_start_date").Equals(default(KeyValuePair<string, int>)))
                {
                    //desired start date comes from here
                    DesiredStartDateId = this.ruleInput.prospectData.KVCodeData.FirstOrDefault(kvp => kvp.Key.ToLower() == "desired_start_date").Value;
                }


                if (input.prospectData.Age.HasValue && input.prospectData.Age.GetValueOrDefault() > 0)
                {
                    //desired start date comes from here
                    AgeGroupId = GetAgeGroupForAge(input.prospectData.Age.GetValueOrDefault());
                }
            }

            LeadCreationTypeID = input.LeadCreationTypeID;
        }

        /// <summary>
        /// this is a hardcoded list of age ranges 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int GetAgeGroupForAge(int v)
        {
            if (1 <= v && v <= 16)
                return 1;
            else if (17 <= v && v <= 23)
                return 2;
            else if (24 <= v && v <= 29)
                return 3;
            else if (30 <= v && v <= 35)
                return 4;
            else if (36 <= v && v <= 41)
                return 5;
            else if (42 <= v && v <= 47)
                return 6;
            else if (48 <= v && v <= 55)
                return 7;
            else if (56 <= v && v <= 69)
                return 8;

            return 9; //70-100
        }

        //Execute Rule for EntityProcessing.ClientRelationship
        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> output)
        {
            //#if DEBUG
            //            Stopwatch sw = new Stopwatch();
            //            sw.Start();
            //#endif
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);

            List<ClientRelationshipProductRuleInput> removedCRProducts = new List<ClientRelationshipProductRuleInput>();

            if (!StaticSettings.IsBeta)
            {
                foreach (ClientRelationshipProductRuleInput crProduct in input)
                {
                    if (caps.ContainsKey(crProduct.ClientRelationProductMappingId))
                    {
                        Cap c = caps[crProduct.ClientRelationProductMappingId];

                        CapReasonCode? crc;
                        EntityMeta entityMeta;
                        bool isCapped = c.IsCapped(null, null, CountryId, ChannelId, VendorId, ApplicationId, SubChannelId, MarketingUnitId, CampusTypeId, EducationLevelId, MilitaryStatusId, out crc, LeadCreationTypeID, StateId, DesiredStartDateId, AgeGroupId, out entityMeta, crProduct.ScoreId);

                        if (isCapped)
                        {
                            //string ChildLevelReason = "Institution";
                            crProduct.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                            //crProduct.RuleName = GetDetailedCappingMessage(Cap.ReasonString(crc), ChildLevelReason);
                            if (crc.HasValue)
                                crProduct.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta) + " - " + Cap.ReasonString(crc);
                            else
                                crProduct.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta);

                            removedCRProducts.Add(crProduct);
                        }
                        else if(crc == CapReasonCode.LeadType_Lead_Capped || crc == CapReasonCode.LeadType_Click_Capped)
                        {
                            crProduct.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                            crProduct.RuleName = Cap.ReasonString(crc);
                            removedCRProducts.Add(crProduct);
                        }
                    }
                    else
                    {
                        crProduct.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                        crProduct.RuleName = "Cap Record Missing";
                        removedCRProducts.Add(crProduct);
                    }
                }
            }

            output = removedCRProducts;
        }

        public void ExecuteRule(List<ClientRelationshipCampusProductRuleInput> input,
                             out List<ClientRelationshipCampusProductRuleInput> output)
        {
            //#if DEBUG
            //            Stopwatch sw = new Stopwatch();
            //            sw.Start();
            //#endif
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);
            List<ClientRelationshipCampusProductRuleInput> removedCampues = new List<ClientRelationshipCampusProductRuleInput>();

            if (!StaticSettings.IsBeta)
            {
                foreach (ClientRelationshipCampusProductRuleInput campus in input)
                {
                    if (caps.ContainsKey(campus.ClientRelationProductMappingId))
                    {
                        Cap c = caps[campus.ClientRelationProductMappingId];

                        CapReasonCode? crc;
                        EntityMeta entityMeta;
                        bool isCapped = c.IsCapped(null, campus.ClientCampusRelationshipId, CountryId, ChannelId, VendorId, ApplicationId, SubChannelId, MarketingUnitId, campus.CampusTypeId, EducationLevelId, MilitaryStatusId, out crc, LeadCreationTypeID, StateId, DesiredStartDateId, AgeGroupId, out entityMeta, campus.ScoreId);
                        if (isCapped)
                        {
                            //string ChildLevelReason = "Campus";
                            campus.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                                //campus.RuleName = GetDetailedCappingMessage(Cap.ReasonString(crc), ChildLevelReason);
                                if (crc.HasValue)
                                    campus.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta) + " - " + Cap.ReasonString(crc);
                                else
                                    campus.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta);

                                removedCampues.Add(campus);
                        }
                    }
                    else
                    {
                        removedCampues.Add(campus);
                    }
                }
            }
            output = removedCampues;
        }

        public void ExecuteRule(List<PSIRuleInput> input,
                             out List<PSIRuleInput> output)
        {
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);
            List<PSIRuleInput> removedPrograms = new List<PSIRuleInput>();

            if (!StaticSettings.IsBeta)
            {
                foreach (PSIRuleInput program in input)
                {
                    if (caps.ContainsKey(program.ClientRelationProductMappingId))
                    {
                        Cap c = caps[program.ClientRelationProductMappingId];

                        CapReasonCode? crc;
                        EntityMeta entityMeta;
                        bool isCapped = c.IsCapped(program.PsiId, program.ClientCampusRelationshipId, CountryId, ChannelId, VendorId, ApplicationId, SubChannelId, MarketingUnitId, CampusTypeId, EducationLevelId, MilitaryStatusId, out crc, LeadCreationTypeID, StateId, DesiredStartDateId, AgeGroupId, out entityMeta, program.ScoreId);
                        if (isCapped)
                        {
                            //string ChildLevelReason = "Program Group";
                            program.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                            //program.RuleName = GetDetailedCappingMessage(Cap.ReasonString(crc), ChildLevelReason);
                            if (crc.HasValue)
                                program.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta) + " - " + Cap.ReasonString(crc);
                            else
                                program.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta);

                            removedPrograms.Add(program);
                        }
                    }
                }
            }
            output = removedPrograms;
        }



        //added method to form detailed capping level / type (normalized, forced, etc) message
        //private string GetDetailedCappingMessage(string reasonString, string childString)
        //{

        //    //check if child level was reached, if not then return original capping reason message
        //    if (childString == null || childString.Equals(""))
        //    {
        //        return reasonString;
        //    }
        //    else
        //    {

        //        if (reasonString != null && !reasonString.Equals(""))
        //        {

        //            return reasonString + " at the " + childString + " level";
        //        }

        //        else
        //        {
        //            return "Cap Limit Reached";

        //        }
        //    }

        //}


        public void ExecuteRule(List<ClickRuleInput> input,
                             out List<ClickRuleInput> output)
        {
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);
            List<ClickRuleInput> removedClickPrograms = new List<ClickRuleInput>();

            if (!StaticSettings.IsBeta)
            {
                foreach (ClickRuleInput clickProgram in input)
                {
					if (caps.ContainsKey(clickProgram.ClientRelationProductMappingId))
					{
						Cap c = caps[clickProgram.ClientRelationProductMappingId];

						CapReasonCode? crc;
                        EntityMeta entityMeta;
						bool isCapped = c.IsCapped(clickProgram.PsiId, clickProgram.ClientCampusRelationshipId, null, null, null, null, null, null, null, null, null, out crc, null, null, null, null, out entityMeta, null,(int)LeadType.Click);
						if (isCapped)
						{
							//string ChildLevelReason = "Program Group";
							clickProgram.BaseRuleType = BaseRuleDefinitionType.LeadCap;
							//program.RuleName = GetDetailedCappingMessage(Cap.ReasonString(crc), ChildLevelReason);
							clickProgram.RuleName = Enum.GetName(typeof(EntityMeta), entityMeta) + " - " + Cap.ReasonString(crc);
                            removedClickPrograms.Add(clickProgram);
						}
					}
					else
					{
						clickProgram.BaseRuleType = BaseRuleDefinitionType.LeadCap;
						clickProgram.RuleName = Cap.ReasonString(CapReasonCode.LeadType_Click_Capped);
						removedClickPrograms.Add(clickProgram);
					}
                }
            }
            output = removedClickPrograms;
        }

    }
}
