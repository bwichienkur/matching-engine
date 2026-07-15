using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine
{
    public class FeatureProcessor
    {
        public static List<Feature> GetFeatures()
        {
            List<Feature> featureList = new List<Feature>();

            List<VW_Matching_Feature> dbFeature = FeatureDataService.GetAllFeatures();

            var groupedByFeatureId = dbFeature.GroupBy(f => f.FeatureId);

            foreach (var featureGroup in groupedByFeatureId)
            {
                var firstFeature = featureGroup.First();
                Feature newFeature = new Feature();

                newFeature.FeatureId = firstFeature.FeatureId;
                newFeature.FeatureCode = firstFeature.FeatureCode;
                newFeature.FeatureType = (FeatureType)firstFeature.FeatureTypeId;
                newFeature.FeatureDetailList = new List<int>();

                foreach (var featureItem in featureGroup)
                {
                    newFeature.FeatureDetailList.Add(featureItem.EntityId);
                }

                featureList.Add(newFeature);
            }

            return featureList;
        }

        public static List<MatchItem> FilterByFeatureId(List<MatchItem> matchItemList, int featureId)
        {
            List<MatchItem> filteredItems = matchItemList;
            
            List<Feature> featureList = StaticCacheProxyHost.CacheProxy.Get<List<Feature>>(MatchingCacheItem.Features);

            Feature selectedFeature = featureList.Where(f => f.FeatureId == featureId).FirstOrDefault();

            if (selectedFeature != default(Feature))
            {
                switch (selectedFeature.FeatureType)
                {
                    case FeatureType.Institution:
                        filteredItems = (from mi in matchItemList
                                    join f in selectedFeature.FeatureDetailList
                                    on mi.Match.InstitutionId equals f
                                    select mi).ToList();
                        break;
                    case FeatureType.Campus:
                        filteredItems = (from mi in matchItemList
                                         join f in selectedFeature.FeatureDetailList
                                         on mi.Match.CampusId equals f
                                         select mi).ToList();
                        break;
                    case FeatureType.Program:
                        filteredItems = (from mi in matchItemList
                                         join f in selectedFeature.FeatureDetailList
                                         on mi.Match.ProgramId equals f
                                         select mi).ToList();
                        break;
                    default:
                        break;
                }
            }

            return filteredItems;
        }
    }

    public enum FeatureType
    {
        Institution = 1,
        Campus = 2,
        Program = 3
    }

    public class Feature
    {
        public int FeatureId { get; set; }
        public string FeatureCode { get; set; }
        public FeatureType FeatureType { get; set; }
        public List<int> FeatureDetailList { get; set; }
    }
}
