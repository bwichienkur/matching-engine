using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine
{
    public class Cap
    {
        //Constants
        public const int NON_MILITARY_ID = 126;
        public const int HAS_MILITARY_AFFILIATION = 1;
        public const int NO_MILITARY_AFFILIATION = 0;
        public const int LEADTYPE_LEAD = 1;
        public const int LEADTYPE_CLICK = 2;

        private static readonly DayOfWeek[] weekends = new DayOfWeek[] { DayOfWeek.Sunday };

        public int CapDistributionId { get; set; }
        public bool Capped { get; set; }
        public int Level { get; set; }
        public EntityMeta CapType { get; set; }
        //public int EntityId { get; set; }
        public CapReasonCode? Reason { get; set; }
        public CapLimitType LimitType { get; set; }
        public int TotalCapAmount { get; set; }
        public int CapRoom { get; set; }
        public int TransactionCount { get; set; }
        public decimal CapPercentFromCap { get; set; }
        public List<Cap> Children { get; set; }
        public bool IsCurrentlyFree { get; set; }
        public int ProductId { get; set; }
        public CapType Type { get; set; }
        public string ChildLevelReason { get; set; }
        public HashSet<int> EntityIDSet { get; set; }
        public SFProductCode? SFProductCode { get; set; }
        public bool TreatAsMatch1 { get; set; }
        public int ParentCapDistributionId { get; set; }

        public int ClientRelationProductMappingId { get; set; }
        public Cap(decimal transactions, decimal totalCapAmount)
        {
            EntityIDSet = new HashSet<int>();
            Children = new List<Cap>();

            TransactionCount = Convert.ToInt32(transactions);
            TotalCapAmount = Convert.ToInt32(totalCapAmount);

            if (totalCapAmount != 0)
                CapPercentFromCap = 1 - (transactions / totalCapAmount);
        }

        public void AddCap(Cap c, int parentCapDistributionId)
        {
            if (parentCapDistributionId == this.CapDistributionId)
            {
                Children.Add(c);
            }
            else
            {
                foreach (Cap cp in Children)
                    cp.AddCap(c, parentCapDistributionId);
            }
        }

        public void AddEntityToCap(int capDistributionId, int entityID)
        {
            if (capDistributionId == this.CapDistributionId)
                this.EntityIDSet.Add(entityID);
            else
            {
                foreach (Cap cp in this.Children)
                    cp.AddEntityToCap(capDistributionId, entityID);
            }
        }

        public bool IsCapped(int? psiId, int? clientCampusRelationshipId, int? countryId, int? channelId
                            , int? vendorId, int? applicationId, int? subChannelId, int? marketingUnitId, int? campusTypeId
                            , int? educationLevelId, int? militaryStatusId
                            , out CapReasonCode? crc
                            , LeadCreationType? leadCreationType
                            , int? stateId
                            , int? desiredStartDateId
                            , int? ageGroup
                            , out EntityMeta entityMeta
                            , int? leadScoreId
                            , int? leadTypeID = LEADTYPE_LEAD
                            )
        {
            crc = Reason;
            entityMeta = this.CapType;
            bool isCapped = false;
            Dictionary<int, HashSet<int>> capMUOverrides = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, HashSet<int>>>(MatchingCacheItem.CapMUOverrides);

            if (Capped)
            {
                if (marketingUnitId.HasValue && Reason.HasValue && Reason.Value == CapReasonCode.Normalization_Limit_Reached && capMUOverrides != null &&
                    capMUOverrides.ContainsKey(this.CapDistributionId) && capMUOverrides[CapDistributionId].Contains(marketingUnitId.Value))
                {
                    isCapped = false;
                }
                else if (this.CapType == EntityMeta.LeadType && leadTypeID == Cap.LEADTYPE_LEAD)
                {
                    isCapped = true;

                    if (this.ParentCapDistributionId != 0)
                    {
                        Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);

                        if (caps[this.ClientRelationProductMappingId].Children != null && caps[this.ClientRelationProductMappingId].Children.Any())
                        {
                            var lookForClickCap = caps[this.ClientRelationProductMappingId].Children.Where(c => c.CapType == EntityMeta.LeadType && c.EntityIDSet.Contains(Cap.LEADTYPE_CLICK));

                            if (lookForClickCap.Any())
                            {
                                if (!lookForClickCap.First().Capped)
                                {
                                    isCapped = false;
                                    crc = CapReasonCode.LeadType_Lead_Capped;
                                }
                                else
                                    crc = CapReasonCode.LeadType_Click_Capped;
                            }
                        }
                    }
                }
                else
                {
                    isCapped = true;
                }
            }

            if (!isCapped)
            {
                foreach (Cap c in Children)
                {
                    switch (c.CapType)
                    {
                        case EntityMeta.PSI:
                            if (psiId.HasValue && c.EntityIDSet.Contains(psiId.Value))

                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.ClientCampusRelationship:
                            if (clientCampusRelationshipId.HasValue && c.EntityIDSet.Contains(clientCampusRelationshipId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.CountryOfOrigin:
                            if (IsCountryOfOriginCap(countryId, c.EntityIDSet))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.Channel:
                            if (channelId.HasValue && c.EntityIDSet.Contains(channelId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.Vendor:
                            if (vendorId.HasValue && c.EntityIDSet.Contains(vendorId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.Application:
                            if (applicationId.HasValue && c.EntityIDSet.Contains(applicationId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.SubChanel:
                            if (subChannelId.HasValue && c.EntityIDSet.Contains(subChannelId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.MarketingUnit:
                            if (marketingUnitId.HasValue && c.EntityIDSet.Contains(marketingUnitId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        case EntityMeta.CampusType:
                            if (campusTypeId.HasValue && c.EntityIDSet.Contains(campusTypeId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;

                        //case EntityMeta.ChannelGroup:
                        //        if (channelId.HasValue)
                        //        {
                        //            Dictionary<int, HashSet<int>> channelGroups = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, HashSet<int>>>(MatchingCacheItem.ChannelGroups);
                        //            if (channelGroups.ContainsKey(channelId.Value) && channelGroups[channelId.Value].Contains(c.EntityId))
                        //                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, out crc);
                        //        }

                        //        break;
                        case EntityMeta.EducationLevel:
                            if (educationLevelId.HasValue && c.EntityIDSet.Contains(educationLevelId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;
                        case EntityMeta.MilitaryAffiliation:
                            if (militaryStatusId.HasValue && c.EntityIDSet.Contains((militaryStatusId.Value == NON_MILITARY_ID ? NO_MILITARY_AFFILIATION : HAS_MILITARY_AFFILIATION)))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;
                        case EntityMeta.LeadCreationType:
                            if (leadCreationType.HasValue && c.EntityIDSet.Contains((int)leadCreationType.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);
                            break;
                        case EntityMeta.LeadType:
                            if (c.EntityIDSet.Contains(leadTypeID.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;
                        case EntityMeta.State:
                            if (stateId.HasValue && c.EntityIDSet.Contains(stateId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);

                            break;
                        case EntityMeta.DesiredStartDate:
                            if (desiredStartDateId.HasValue && c.EntityIDSet.Contains(desiredStartDateId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);
                            break;
                        case EntityMeta.AgeGroup:
                            if (ageGroup.HasValue && c.EntityIDSet.Contains(ageGroup.GetValueOrDefault()))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);
                            break;
                        case EntityMeta.LeadPingLeadScore:
                            if (leadScoreId.HasValue && c.EntityIDSet.Contains(leadScoreId.GetValueOrDefault()))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);
                            break;
                        case EntityMeta.MilitaryStatus:
                            if (militaryStatusId.HasValue && c.EntityIDSet.Contains(militaryStatusId.Value))
                                isCapped = c.IsCapped(psiId, clientCampusRelationshipId, countryId, channelId, vendorId, applicationId, subChannelId, marketingUnitId, campusTypeId, educationLevelId, militaryStatusId, out crc, leadCreationType, stateId, desiredStartDateId, ageGroup, out entityMeta, leadScoreId, leadTypeID);
                            break;

                        default:
                            isCapped = false;
                            break;
                    }

                    if (isCapped)
                        break;
                }
            }

            return isCapped;
        }

        private bool IsCountryOfOriginCap(int? countryId, HashSet<int> entityIdSet)
        {
            bool returnVal = false;

            if (countryId.HasValue && entityIdSet.Count > 0)
            {
                if (entityIdSet.First() == 1) //international
                {
                    if (countryId.Value != 4 && countryId != 5) //US and Canada
                        returnVal = true;
                }
                else if (entityIdSet.First() == 2) //Domestic - US and Canada
                {
                    if (countryId.Value == 4 || countryId == 5) //US and Canada
                        returnVal = true;
                }
            }

            return returnVal;
        }
        public static string ReasonString(CapReasonCode? crc)
        {
            if (crc.HasValue)
            {
                switch (crc)
                {
                    case CapReasonCode.Cap_Limit_Reached:
                        return "Cap Limit Reached";
                    case CapReasonCode.Day_Parting_Limit_Reached:
                        return "Day Parting Cap Limit Reached";
                    case CapReasonCode.Forcefully_Capping_Out:
                        return "Forcefully Capping Out";
                    case CapReasonCode.Normalization_Limit_Reached:
                        return "Normalization Cap Limit Reached";
                    case CapReasonCode.Parent_Cap_Limit_Reached:
                        return "Parent Cap Limit Reached";
                    case CapReasonCode.Week_Parting_Limit_Reached:
                        return "Week Parting Cap Limit Reached";
                    case CapReasonCode.LeadType_Lead_Capped:
                        return "LeadType Lead Capped";
                    case CapReasonCode.LeadType_Click_Capped:
                        return "LeadType Click Capped";
                }
            }
            return "";
        }
    }
}
