using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public enum CriteriaType
    {
        Application = 0,
        Age = 1,
        State = 2,
        IsMilitary = 3,
        EducationLevel = 4,
        Campaign = 5,
        Vendor = 6,
        Channel = 7,
        Subject = 8,
        Category = 9,
        ProgramLevel = 10
    }

    public enum FactorType
    {
        Cap = 1,
        eRPL = 2,
        CampusDistance = 3,
        RPC = 4,
        Strategic = 5
    }

    public enum BusinessModelRollupType
    {
        TopScore = 1,
        AverageScore = 2,
        MedianScore = 3
    }
    
    public enum BusinessModelAggregateType
    {
        Max = 1,
        Min = 2
    }
    
    public class BusinessModel
    {
        public int BusinessModelId { get; set; }
        public string BusinessModelName { get; set; }
        public int? BusinessModelTestId { get; set; }
        public bool IsDefault { get; set; }
        public BusinessModelRollupType RollupType { get; set; }
        public bool UseDistanceCliffFormula { get; set; }
        public int? DistanceCliffValue { get; set; }
        public int? DistanceCapMultiplier { get; set; }

        public List<CriteriaGroup> CriteriaGroupList { get; set; }
        public List<WeightSubject> WeightSubjectList { get; set; }

        public int? ChoosenWeightSubjectId { get; set; }
        public List<FactorAggregate> FactorAggregateList { get; set; }

        public BusinessModel Clone()
        {
            return new BusinessModel()
            {
                BusinessModelId = this.BusinessModelId,
                BusinessModelName = this.BusinessModelName,
                BusinessModelTestId = this.BusinessModelTestId,
                ChoosenWeightSubjectId = this.ChoosenWeightSubjectId,
                IsDefault = this.IsDefault,
                RollupType = this.RollupType,
                CriteriaGroupList = this.CriteriaGroupList,
                WeightSubjectList = this.WeightSubjectList,
                UseDistanceCliffFormula = this.UseDistanceCliffFormula,
                DistanceCliffValue = this.DistanceCliffValue,
                DistanceCapMultiplier = this.DistanceCapMultiplier
            };
        }
    }

    public class FactorAggregate
    {
        public FactorType BusinessModelFactorId { get; set; }
        public BusinessModelAggregateType BusinessModelAggregateTypeId { get; set; }
        public decimal? AggregateValue { get; set; }
        public int ProgramProductId { get; set; }
        public List<FactorAggregateValue> FactorAggregateValueList { get; set; }
    }

    public class FactorAggregateValue
    {
        public FactorType BusinessModelFactorId { get; set; }
        public decimal? AggregateValue { get; set; }
    }

    public class CriteriaGroup
    {
        public int CriteriaGroupId { get; set; }
        public List<Criteria> CriteriaList { get; set; }
    }

    public class Criteria
    {
        public int CriteriaId { get; set; }
        public CriteriaType CriteriaType { get; set; }
        public string Operand { get; set; }
        public List<CriteriaValue> CriteriaValueList { get; set; }
    }

    public class CriteriaValue
    {
        public int CriteriaValueId { get; set; }
        public string Value { get; set; }
    }

    public class WeightSubject
    {
        public int WeightSubjectId { get; set; }
        public bool IsBaseline { get; set; }
        public int PercentToShow { get; set; }
        public List<FactorWeight> FactorWeightList { get; set; }
    }

    public class FactorWeight
    {
        public int FactorWeightId { get; set; }
        public FactorType FactorType { get; set; }
        public decimal OnlineWeight { get; set; }
        public decimal CampusWeight { get; set; }
    }

    public class BusinessModelTestCacheItem
    {
        private const string cacheItemPrefix = "ME_BMTest_";

        public Guid TrackingDeviceGuid { get; set; }
        public int BusinessModelId { get; set; }
        public int WeightSubjectId { get; set; }

        public string CacheItemKeyName
        {
            get { return cacheItemPrefix + TrackingDeviceGuid.ToString() + "_" + BusinessModelId.ToString(); }
        }
    }
}
