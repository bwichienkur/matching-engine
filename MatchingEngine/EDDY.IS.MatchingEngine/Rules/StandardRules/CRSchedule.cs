using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship
                                                   },
                            null
                            , null)]
    public class CRSchedule : Rule, ICRProductRule
    {
        public CRSchedule(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();
            Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>> schedules = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>>>(MatchingCacheItem.ClientRelationshipSchedules);

            foreach (var mi in input)
            {
                if(schedules.ContainsKey(mi.ClientRelationshipId))
                {
                    foreach(var schedule in schedules[mi.ClientRelationshipId])
                    {
                        if(schedule.ProductId.HasValue == false || schedule.ProductId.Value == mi.ProductId)
                        {
                            DayOfWeek dwSchedule = (DayOfWeek)schedule.WeekDay;

                            if(dwSchedule == DateTime.Now.DayOfWeek)
                            {
                                //this is a schedule we need so lets search the entity meta schedules to see if we need to overwrite
                                VW_Matching_ClientRelationshipSchedule finalschedule = GetCRProductEntitySchedule(schedule, dwSchedule);
                                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                                if (finalschedule.IsOvernightSchedule && (currentTime > finalschedule.TurnOffTime && !(currentTime >= finalschedule.TurnOnTime))
                                   )
                                {
                                    mi.BaseRuleType = BaseRuleDefinitionType.CRSchedule;
                                    mi.RuleName = "CR Schedule Rule";
                                    removed.Add(mi);
                                }
                                else if(!finalschedule.IsOvernightSchedule && (currentTime < finalschedule.TurnOnTime || currentTime >= finalschedule.TurnOffTime))
                                {
                                    mi.BaseRuleType = BaseRuleDefinitionType.CRSchedule;
                                    mi.RuleName = "CR Schedule Rule";
                                    removed.Add(mi);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            //#if DEBUG
            //            sw.Stop();
            //            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrId(s) , out " + removed.Count + " CrId(s) ) - Campaign: " + sw.ElapsedMilliseconds.ToString() + "ms");
            //#endif
        }

        private VW_Matching_ClientRelationshipSchedule GetCRProductEntitySchedule(VW_Matching_ClientRelationshipSchedule schedule, DayOfWeek day)
        {
            VW_Matching_ClientRelationshipSchedule result = null;

            bool schedulefound = false;
            Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>> scheduleList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>>>(MatchingCacheItem.ClientRelationshipEntitySchedules);

            if (scheduleList != null && scheduleList.ContainsKey(schedule.ClientRelationProductAutomationId))
            {
                List<VW_Matching_ClientRelationshipEntitySchedule> found = scheduleList[schedule.ClientRelationProductAutomationId].Where(f=>(DayOfWeek)f.WeekDay == day).OrderBy(f=>f.Priority).ToList();
                foreach(var s in found)
                {
                    switch(s.EntityMetaId)
                    {
                        case (int)EntityMeta.State:
                            int i = 0;
                            Int32.TryParse(s.EntityValue, out i);

                            if(ruleInput.prospectData.StateId == i)
                            {
                                schedulefound = true;

                                result = new VW_Matching_ClientRelationshipSchedule();

                                result.TurnOffTime = s.TurnOffTime;
                                result.TurnOnTime = s.TurnOnTime;

                                result.IsOvernightSchedule = schedule.IsOvernightSchedule;
                                result.ClientRelationProductAutomationId = schedule.ClientRelationProductAutomationId;
                                result.ClientRelationshipId = schedule.ClientRelationshipId;
                                result.ProductId = schedule.ProductId;
                                result.WeekDay = schedule.WeekDay;
                            }

                            break;
                    }
                    if (schedulefound)
                        break;                        
                }
            }

            if (result == null)
                return schedule;
            else
                return result;
        }
    }
}
