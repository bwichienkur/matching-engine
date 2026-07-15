using EDDY.IS.FormsEngine.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{
    public class CampaignDataService
    {
        private Dictionary<Guid, int> CampaignApplicationIdDictionary
        {
            get
            {
                Dictionary<Guid, int> _campaignApplicationIdDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<Guid, int>>(Constants.CAMPAIGN_APPLICATION_ID_CACHE_KEY);
                if (_campaignApplicationIdDictionary == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.CAMPAIGN_APPLICATION_ID_CACHE_KEY, FormsEngineCacheHelper.GetCampaignApplicationIdDictionary(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheCampaignApplicationIdsTimeMinutes")));
                    _campaignApplicationIdDictionary = FormsEngineCacheProxy.Cache.Get<Dictionary<Guid, int>>(Constants.CAMPAIGN_APPLICATION_ID_CACHE_KEY);
                }
                return _campaignApplicationIdDictionary;
            }
        }

        public Dictionary<Guid, int> GetCampaignApplicationIdDictionary()
        {
            Dictionary<Guid, int> _campaignApplicationIdDictionary = new Dictionary<Guid, int>();
            List<Campaign> campaignList = new List<Campaign>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                campaignList = (from c in Context.Campaigns
                               where c.IsEnabled == 1
                               select c).ToList();
            }

            foreach (var campaign in campaignList)
            {
                if (!_campaignApplicationIdDictionary.ContainsKey(campaign.TrackId))
                {
                    _campaignApplicationIdDictionary.Add(campaign.TrackId, campaign.ApplicationId ?? 0);
                }
            }

            return _campaignApplicationIdDictionary;
        }

        public int GetCampaignApplicationIdByTrackId(Guid trackId)
        {
            CampaignApplicationIdDictionary.TryGetValue(trackId, out int applicationId);
            return applicationId;
        }

    }
}
