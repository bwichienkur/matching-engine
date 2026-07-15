using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class LeadScoreReservation
    {
        public List<LeadScoreReservationMarketingItem> MarketingItems { get; set; }
        public Dictionary<int, int> SupplyUnitToAcceptableTierLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DayBegin { get; set; }
        public int DayEnd { get; set; }
        public string ReservationName { get; set; }

        public LeadScoreReservation()
        {
            MarketingItems = new List<LeadScoreReservationMarketingItem>();
            SupplyUnitToAcceptableTierLevel = new Dictionary<int, int>();
        }

        private bool IsEnabled
        {
            get
            {
                DateTime dt = DateTime.Now;

                if (dt < StartDate)
                    return false;
                else if (EndDate != DateTime.MinValue.Date && dt > EndDate)
                    return false;
                else if (DayBegin != 0 && dt.Day < DayBegin)
                    return false;
                else if (DayEnd != 0 && dt.Day > DayEnd)
                    return false;

                return true;
            }
        }

        private bool MarketingItemMatches(Campaign c)
        {
            if (c != null)
            {
                foreach (LeadScoreReservationMarketingItem mi in MarketingItems)
                {
                    if ((mi.CampaignId != 0 && mi.CampaignId != c.CampaignId) ||
                       (mi.ChannelId != 0 && mi.ChannelId != c.ChannelId) ||
                       (mi.SubchannelId != 0 && (!c.SubChannelId.HasValue || mi.SubchannelId != c.SubChannelId.Value)) ||
                       (mi.ApplicationId != 0 && (!c.ApplicationId.HasValue || mi.ApplicationId != c.ApplicationId.Value)) ||
                       (mi.MarketingUnitId != 0 && (!c.MarketingUnitId.HasValue || c.MarketingUnitId.Value != mi.MarketingUnitId)) ||
                       (mi.VendorId != 0 && (!c.VendorId.HasValue || c.VendorId.Value != mi.VendorId)))
                        continue;
                    else
                        return true;
                }
            }

            return false;
        }

        public List<int> ProcessReservationRules(Campaign c, List<MatchItem> matches, int ProspectTierLevel)
        {
            List<int> supplyUnitsRemoved = new List<int>();

            if (IsEnabled && c != null && MarketingItemMatches(c) == false)
            {
                foreach (int supplyUnitId in SupplyUnitToAcceptableTierLevel.Keys)
                {
                    Dictionary<int, LeadScoreReservationSupplyUnit> supplyUnits = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, LeadScoreReservationSupplyUnit>>(MatchingCacheItem.LeadScoreReservationSupplyUnits);

                    if (supplyUnits.ContainsKey(supplyUnitId))
                    {
                        LeadScoreReservationSupplyUnit su = supplyUnits[supplyUnitId];

                        if (ProspectTierLevel > SupplyUnitToAcceptableTierLevel[supplyUnitId])
                        {
                            if (su.RemoveMatchItems(matches))
                                supplyUnitsRemoved.Add(supplyUnitId);
                        }
                    }
                }
            }

            return supplyUnitsRemoved;
        }
    }
    
    public class LeadScoreReservationSupplyUnit
    {
        public int LeadScoreReservationSupplyUnitId { get; set; }
        public string SupplyUnitName { get; set; }
        public List<SupplyUnitItem> SupplyUnitItems { get; set; }
        public HashSet<int> ExcludedCrs { get; set; }

        public LeadScoreReservationSupplyUnit()
        {
            SupplyUnitItems = new List<SupplyUnitItem>();
            ExcludedCrs = new HashSet<int>();
        }

        public bool RemoveMatchItems(List<MatchItem> matches)
        {
            bool removeMatches = false;

            foreach (var s in SupplyUnitItems)
            {
                int count = matches.RemoveAll(m => m.Match.ProgramLevelId == s.ProgramLevelId &&
                                    m.Match.PrimaryCategoryId == (s.CategoryId.HasValue ? s.CategoryId : m.Match.PrimaryCategoryId) &&
                                    m.Match.PrimarySubjectId == (s.SubjectId.HasValue ? s.SubjectId : m.Match.PrimarySubjectId) &&
                                    m.Match.PrimarySpecialtyId == (s.SubjectId.HasValue ? s.SpecialtyId : m.Match.PrimarySpecialtyId) &&
                                    !ExcludedCrs.Contains(m.Match.ClientRelationshipId));

                if (count > 0)
                    removeMatches = true;
            }

            return removeMatches;
        }
    }

    public class SupplyUnitItem
    {
        public int ProgramLevelId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? SpecialtyId { get; set; }
    }

    public class LeadScoreReservationMarketingItem
    {
        public long CampaignId { get; set; }
        public int VendorId { get; set; }
        public int ChannelId { get; set; }
        public int MarketingUnitId { get; set; }
        public int SubchannelId { get; set; }
        public int ApplicationId { get; set; }
    }
    
}
