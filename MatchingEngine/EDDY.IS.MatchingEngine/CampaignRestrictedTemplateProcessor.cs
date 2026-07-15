using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using EDDY.IS.Core;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine
{
    public class CampaignRestrictedTemplateProcessor
    {
        public CampaignRestrictedTemplateProcessor(){}

        #region public
        public static ConcurrentDictionary<Guid, HashSet<int>>  GetCampaignRestrictedTemplateList()
        {
            return CampaignDataService.GetCampaignRestrictedTemplateList();
        }

        public static List<MatchItem> ApplyFilter(Guid TrackId, List<MatchItem> matchItemList)
        {
            ConcurrentDictionary<Guid, HashSet<int>> campaignRestrictedTemplateToCampaign = StaticCacheProxyHost.CacheProxy.Get<ConcurrentDictionary<Guid, HashSet<int>>>(MatchingCacheItem.CampaignRestrictedTemplates);

            if (campaignRestrictedTemplateToCampaign != null 
                && campaignRestrictedTemplateToCampaign.Count > 0 
                && campaignRestrictedTemplateToCampaign.ContainsKey(TrackId)
                && campaignRestrictedTemplateToCampaign[TrackId].Count > 0)
                return matchItemList.Where(i => !campaignRestrictedTemplateToCampaign[TrackId].Contains(i.Match.TemplateId.HasValue ? i.Match.TemplateId.Value : 0)).ToList();

            return matchItemList;
        }
        #endregion

    }
}
