using EDDY.IS.FormsEngine.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EDDY.IS.FormsEngine.DataModel
{
    public class OpenMailDataService
    {
        public OpenMailProfile GetOpenMailProfile(int profileId) {

            OpenMailProfile Result = null;

            if (OpenMailProfileDictionary().ContainsKey(profileId))
            {
                Result = OpenMailProfileDictionary()[profileId];
            }

            return Result;

        }
        public Dictionary<int, OpenMailProfile> OpenMailProfileDictionary()
        {
            Dictionary<int, OpenMailProfile> openMailProfiles = FormsEngineCacheProxy.Cache.Get<Dictionary<int, OpenMailProfile>>(Constants.OPENMAILPROFILE_CACHE_KEY);

            if (openMailProfiles == null)
            {
                FormsEngineCacheProxy.Cache.Set(Constants.OPENMAILPROFILE_CACHE_KEY, FormsEngineCacheHelper.GetOpenMailDictionary, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                openMailProfiles = FormsEngineCacheProxy.Cache.Get<Dictionary<int, OpenMailProfile>>(Constants.OPENMAILPROFILE_CACHE_KEY);
            }
            return openMailProfiles;
        }

        public Dictionary<int, OpenMailProfile> GetOpenMailProfileDictionary()
        {
            Dictionary<int, OpenMailProfile> openMailProfiles = new Dictionary<int, OpenMailProfile>();

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                var profiles = Context.OpenMailProfiles
                    .Include("OpenMailRules.StandardControlCode")
                    .Where(p => p.IsActive)
                    .ToList();
                foreach (var profile in profiles)
                {
                    openMailProfiles.Add(profile.OpenMailProfileId, profile);
                }
            }
            return openMailProfiles;
        }
    }
}
