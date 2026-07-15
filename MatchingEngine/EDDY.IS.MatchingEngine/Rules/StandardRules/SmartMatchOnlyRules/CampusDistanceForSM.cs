using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Configuration;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientCampusRelationship }
                            , new InputRequired[] { InputRequired.PostalCode }
                            , new RuleAttribute[] { RuleAttribute.ExecuteSmartMatchRules })]
    public class CampusDistanceForSM : Rule, ICRCampusProductRule
    {
        public CampusDistanceForSM(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipCampusProductRuleInput> input,
                                out List<ClientRelationshipCampusProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipCampusProductRuleInput>();

            double smMaxAllowedDistance = 0;
            GeoCodeProcessor geoCode = new GeoCodeProcessor();

            if (!Double.TryParse(ConfigurationManager.AppSettings["SM_MaxDistanceInMiles"], out smMaxAllowedDistance))
                smMaxAllowedDistance = 150;

            if (ruleInput.ZipCoordinate != null)
            {
                foreach (ClientRelationshipCampusProductRuleInput cr in input)
                {
                    if (ruleInput.Campaign.Allow150MileCampusFilter && cr.CampusType == CampusType.Ground)
                    {
                        double? campusDistance = geoCode.GetDistanceBetweenZipCodes(cr.CampusPostalCode, ruleInput.ZipCoordinate.ZipCode);

                        if (campusDistance.HasValue)
                        {
                            //If the match pref is ground/online/null && program is a ground program && a distance can be calculated && actual distance from lead to campus is greater than the maxallowed distance - then skip this campus
                            if (campusDistance.Value > smMaxAllowedDistance)
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.CampusDistanceSM;
                                cr.RuleName = "Campus Distance Exceeds SM Limit - " + smMaxAllowedDistance.ToString() + " miles.";
                                removed.Add(cr);
                            }
                        }
                    }
                }
            }
        }
    }
}
