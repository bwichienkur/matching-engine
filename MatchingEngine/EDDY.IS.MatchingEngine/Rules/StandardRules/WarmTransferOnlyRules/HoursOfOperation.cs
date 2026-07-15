using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship,
                                                     EntityProcessing.ClientCampusRelationship
                                                   },
                            null
                            , new RuleAttribute[] { RuleAttribute.ExecuteWarmTransferRules })]
    public class HoursOfOperation : Rule, ICRProductRule, ICRCampusProductRule
    {
        //HARDCODED VALUES FOR SOUTH UNIVERSITY - GEORGIA HOURS
        public const int SOUTH_CR_ID = 37;
        public const int GEORGIA_STATE_ID = 12;

        public HoursOfOperation(RuleInput ri)
            : base(ri)
        {        }




        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();
            Dictionary<int, List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>> hourCache =
                        StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>>>(MatchingCacheItem.RECallCenterHours);

            int currDayOfWeek = ((int)DateTime.Now.DayOfWeek == 7 ? 1 : (int)DateTime.Now.DayOfWeek + 1);

            Boolean isValid; 

            foreach (ClientRelationshipProductRuleInput cr in input)
            {
                isValid = false;

                VW_Matching_CRCallCenterHours ccHours = null;
                List<KeyValuePair<int, VW_Matching_CRCallCenterHours>> ccHoursKeyValueList = null;
                
                if (Product.IsWarmTransferProduct(cr.ProductId) && hourCache.ContainsKey(cr.ClientRelationshipId))
                {

                    if ((from Query in hourCache[cr.ClientRelationshipId]
                         where
                           Query.Key == currDayOfWeek
                         select Query) != null)
                    {
                        ccHoursKeyValueList = (from Query in hourCache[cr.ClientRelationshipId]
                                               where
                                                 Query.Key == currDayOfWeek
                                               select Query).ToList();

                        foreach (KeyValuePair<int, VW_Matching_CRCallCenterHours> callCenterKeyValueEntry in ccHoursKeyValueList)
                        {
                            ccHours = callCenterKeyValueEntry.Value;

                            if (ccHours != null && CheckHours(ccHours.TimeZone,
                                                         new TimeSpan(ccHours.StartHour, ccHours.StartMinute, 0),
                                                         new TimeSpan(ccHours.EndHour, ccHours.EndMinute, 0),
                                                         ccHours.ClientRelationshipId) == true)
                            {
                                isValid = true;
                                break;
                            }

                        }

                        if (!isValid)
                        {
                            cr.BaseRuleType = BaseRuleDefinitionType.WT_HoursOfOperation;
                            cr.RuleName = "Warm Transfer - Hours of Operation";
                            removed.Add(cr);

                        }

                    }
                    else
                    {
                        cr.BaseRuleType = BaseRuleDefinitionType.WT_HoursOfOperation;
                        cr.RuleName = "Warm Transfer - Hours of Operation";
                        removed.Add(cr);
                    }
                }
            }
        }

        public void ExecuteRule(List<ClientRelationshipCampusProductRuleInput> input,
                             out List<ClientRelationshipCampusProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipCampusProductRuleInput>();
            Dictionary<int, List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>> hourCache =
                        StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>>>(MatchingCacheItem.RECampusCallCenterHours);

            int currDayOfWeek = ((int)DateTime.Now.DayOfWeek == 7 ? 1 : (int)DateTime.Now.DayOfWeek + 1);

            Boolean isValid;

            foreach (ClientRelationshipCampusProductRuleInput ccr in input)
            {

                isValid = false;

                VW_Matching_CampusCallCenterHours ccHours = null;
                List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>> ccHoursKeyValueList = null;

                if (Product.IsWarmTransferProduct(ccr.ProductId) && hourCache.ContainsKey(ccr.ClientCampusRelationshipId))
                {

                    if ((from Query in hourCache[ccr.ClientCampusRelationshipId]
                         where
                           Query.Key == currDayOfWeek
                         select Query) != null)
                    {
                        ccHoursKeyValueList = (from Query in hourCache[ccr.ClientCampusRelationshipId]
                                               where
                                                 Query.Key == currDayOfWeek
                                               select Query).ToList();

                        foreach (KeyValuePair<int, VW_Matching_CampusCallCenterHours> callCenterKeyValueEntry in ccHoursKeyValueList)
                        {
                            ccHours = callCenterKeyValueEntry.Value;

                            if (ccHours !=null && CheckHours(ccHours.TimeZone,
                                                         new TimeSpan(ccHours.StartHour, ccHours.StartMinute, 0),
                                                         new TimeSpan(ccHours.EndHour, ccHours.EndMinute, 0),
                                                         ccHours.ClientCampusRelationshipId) == true)
                            {
                                isValid = true;
                                break;
                            }

                        }

                        if (!isValid)
                        {
                            ccr.BaseRuleType = BaseRuleDefinitionType.WT_HoursOfOperation;
                            ccr.RuleName = "Warm Transfer - Hours of Operation";
                            removed.Add(ccr);


                        }

                    }
                    else
                    {
                        ccr.BaseRuleType = BaseRuleDefinitionType.WT_HoursOfOperation;
                        ccr.RuleName = "Warm Transfer - Hours of Operation";
                        removed.Add(ccr);
                    }
                }
            }
        }

        private bool CheckSouthUniversityForGeorgia(DateTime currDate, TimeSpan startTime)
        {
            bool isOpen = true;

            TimeSpan closedTime = new TimeSpan(19, 0, 0);

            if (currDate.TimeOfDay >= startTime && currDate.TimeOfDay <= closedTime)
                isOpen = true;
            else
                isOpen = false;

            return isOpen;
        }

        private bool CheckHours(string timeZone, TimeSpan startTime, TimeSpan endTime, int? crId)
        {
            DateTime currDate = DateTime.Now;

            bool isValid = true;

            if (timeZone != "EST")
            {
                if (timeZone == "CST")
                    currDate = DateTime.Now.AddHours(-1);
                if (timeZone == "MST")
                    currDate = DateTime.Now.AddHours(-2);
                if (timeZone == "PST")
                    currDate = DateTime.Now.AddHours(-3);
                if (timeZone == "AKST")
                    currDate = DateTime.Now.AddHours(-4);
                if (timeZone == "HST")
                    currDate = DateTime.Now.AddHours(-5);
            }

            // check on whether current time is within hours of operation
            if (crId.HasValue && crId.Value == SOUTH_CR_ID && ruleInput.prospectData != null && 
                ruleInput.prospectData.StateId.HasValue && ruleInput.prospectData.StateId == GEORGIA_STATE_ID)
                isValid = CheckSouthUniversityForGeorgia(currDate, startTime);
            else if (currDate.TimeOfDay >= startTime && currDate.TimeOfDay <= endTime)
                isValid = true;
            else
                isValid = false;

            return isValid;
        }
    }
}
