using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DTO.Extended;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DataModel
{
    public class LandingPageDataService
    {
        private List<VW_LandingPageSettings> LandingPageSettingsList
        {
            get
            {
                List<VW_LandingPageSettings> _landingPageSettingList = FormsEngineCacheProxy.Cache.Get<List<VW_LandingPageSettings>>(Constants.LANDING_PAGE_SETTING_CACHE_KEY);
                if (_landingPageSettingList == null)
                {
                    FormsEngineCacheProxy.Cache.Set(Constants.LANDING_PAGE_SETTING_CACHE_KEY, this.RetrieveLandingPageSettingList(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheResourceMetaDatasTimeMinutes")));
                    _landingPageSettingList = FormsEngineCacheProxy.Cache.Get<List<VW_LandingPageSettings>>(Constants.LANDING_PAGE_SETTING_CACHE_KEY);
                }
                return _landingPageSettingList;
            }
        }

        public VW_LandingPageSettings GetLandingPageByUrl(string url)
        {
            VW_LandingPageSettings result = null;

            if (IsValidUrl(url))
            {
                if (url.Contains("localhost"))
                {
                    url = ConfigurationManager.AppSettings.Get("TestingReplacementUrl");
                }

                Uri uri = new Uri(url);
                string domain = FormatDomain(uri.Host);
                string lastSegment = uri.Segments[uri.Segments.Length - 1].TrimEnd('/');

                result = LandingPageSettingsList.FirstOrDefault(a => a.DomainName == domain && a.Directory == lastSegment);
            }

            return result;
        }

        private static string FormatDomain(string domain)
        {
            string result = domain;

            if (IsTestEnvironment())
            {
                string environmentName = ConfigurationManager.AppSettings.Get("EnvironmentName");

                int index = domain.IndexOf(environmentName, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    domain = domain.Remove(index, environmentName.Length + 1);
                }

                result = domain.Replace(".local", ".com");
            }

            result = result.Replace("www.", string.Empty);

            return result;
        }

        private static bool IsTestEnvironment() 
        {
            return bool.Parse(ConfigurationManager.AppSettings.Get("IsTestEnvironment"));
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private List<VW_LandingPageSettings> RetrieveLandingPageSettingList()
        {
            List<VW_LandingPageSettings> Result = null;

            using (FEEntitiesContainer Context = new FEEntitiesContainer())
            {
                Result = (from m in Context.VW_LandingPageSettings
                          select m).ToList();
            }

            return Result; ;
        }
    }
}

