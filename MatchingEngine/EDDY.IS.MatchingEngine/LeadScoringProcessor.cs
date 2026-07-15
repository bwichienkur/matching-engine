using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine
{   
    public class LeadScoringProcessor
    {
        public static Dictionary<int, int> GetLeadScoreProducts()
        {
            Dictionary<int, int> products = new Dictionary<int, int>();

            List<VW_Matching_Product> dbProducts = LeadScoringDataService.GetAllProduct();

            foreach (var product in dbProducts)
            {
                if(product.ProductQualityRanking.HasValue)
                    products.Add(product.ProductId, product.ProductQualityRanking.Value);
            }

            return products;
        }

        public static Dictionary<int, VW_Matching_Application> GetApplications()
        {
            Dictionary<int, VW_Matching_Application> applications = new Dictionary<int, VW_Matching_Application>();

            List<VW_Matching_Application> dbApps = LeadScoringDataService.GetAllApplication();

            foreach(var app in dbApps)
            {
                applications.Add(app.ApplicationId, app);
            }

            return applications;
        }

        public static Dictionary<int, VW_Matching_Product> GetProducts()
        {
            Dictionary<int, VW_Matching_Product> products = new Dictionary<int, VW_Matching_Product>();

            List<VW_Matching_Product> dbProducts = LeadScoringDataService.GetAllProduct();

            foreach (var product in dbProducts)
            {
                products.Add(product.ProductId, product);
            }

            return products;
        }

        public static Dictionary<int, HashSet<int>> GetApplicationToProducts()
        {
            Dictionary<int, HashSet<int>> applicationToProducts = new Dictionary<int, HashSet<int>>();

            List<VW_Matching_ApplicationToProduct> dbAppToProducts = LeadScoringDataService.GetApplicationToProducts();

            foreach (var appToProduct in dbAppToProducts)
            {
                if (applicationToProducts.ContainsKey(appToProduct.ApplicationId))
                    applicationToProducts[appToProduct.ApplicationId].Add(appToProduct.ProductId);
                else
                    applicationToProducts.Add(appToProduct.ApplicationId, new HashSet<int> { appToProduct.ProductId });
            }

            return applicationToProducts;
        }

        public List<MatchItem> Execute(List<MatchItem> matchItemList, int? leadScoringTierLevel, Guid campaignTrackGuid, IS.Base.ISApplication? application)
        {
            List<MatchItem> matchResults = null;

            Campaign campaign = Campaign.Get(campaignTrackGuid);

            if (campaign != null)
            {
                matchResults = campaign.ApplyProductFilter(matchItemList, application).OrderByDescending(mi => mi.ProgramRankScore).ToList();
                matchResults = campaign.ApplyPSIFilter(matchItemList, application).OrderByDescending(mi => mi.ProgramRankScore).ToList(); ;
            }

            return  matchResults;
        }
        /*
        public Tuple<bool,List<MatchItem>> Execute(List<MatchItem> matchItemList, LeadScoringProductAssignment productAssignment, Guid campaignTrackGuid)
        {
            List<MatchItem> matchResults = null;

            Campaign campaign = Campaign.Get(campaignTrackGuid);
            bool leadScoringApplied = false;

            if (productAssignment != null && productAssignment.ChosenModelProductId > 0 && campaign.LeadScoringProductSelectionEnabled)
            {
                Dictionary<int, int> productList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, int>>(MatchingCacheItem.Products);

                //get the product quality ranking of the product chosen by leadscoring
                int selectedQualityRanking = productList.Where(pl => pl.Key == productAssignment.ChosenModelProductId).FirstOrDefault().Value;

                //join all products from match item list to get their quality rankings
                List<Tuple<MatchItem, int>> matchItemWithProductRankings = JoinProductQualityRankings(matchItemList, productList);

                List<Tuple<MatchItem, int>> crMatchItems = new List<Tuple<MatchItem, int>>();
                List<int> crIdList = new List<int>();

                if (productAssignment.CrProductAssignmentList != null && productAssignment.CrProductAssignmentList.Any())
                {
                    crMatchItems = FilterCrSpecificProduct(productAssignment.CrProductAssignmentList, matchItemWithProductRankings, productList);
                    crIdList = productAssignment.CrProductAssignmentList.Select(cr => cr.CrId).ToList();
                }

                //remove "better" products, ie if lead scoring thinks this should only be a SELECT lead, then remove PREMIER, GOLD etc
                //exclude cr specific ones as well
                var filteredMatchResults = matchItemWithProductRankings.Where(mi => mi.Item2 >= selectedQualityRanking && !crIdList.Contains(mi.Item1.ClientRelationshipId)).ToList();

                //add cr specific items (they were excluded from default filter)
                filteredMatchResults.AddRange(crMatchItems);

                if (filteredMatchResults.Count() > 0)
                {
                    //order by the product quality first, then SRA
                    matchResults = filteredMatchResults.OrderBy(mi => mi.Item2).ThenByDescending(mi => mi.Item1.ProgramRankScore).Select(mi => mi.Item1).ToList();

                    //lead scoring has its own allowed product list, apply that here
                    matchResults = campaign.ApplyLeadScoringProductFilter(matchResults);

                    //revert to campaign settings if lead scoring would result in a "limbo", keeping in mind that the campaign product filters were skipped prior
                    if (matchResults.Count() == 0)
                        matchResults = campaign.ApplyProductFilter(matchItemList, null).OrderByDescending(mi => mi.ProgramRankScore).ToList();
                    else
                        leadScoringApplied = true;
                }
                else //revert to campaign settings if lead scoring would result in a "limbo", keeping in mind that the campaign product filters were skipped prior
                {
                    matchResults = campaign.ApplyProductFilter(matchItemList, null).OrderByDescending(mi => mi.ProgramRankScore).ToList();
                }
            }
            else //the campaign does not have product selection enabled, default campaign product filter should have been applied, just return back same list of items as was passed in
                matchResults = campaign.ApplyProductFilter(matchItemList, null).OrderByDescending(mi => mi.ProgramRankScore).ToList();

            return new Tuple<bool,List<MatchItem>>(leadScoringApplied, matchResults);
        }

        private List<Tuple<MatchItem, int>> FilterCrSpecificProduct(List<CrProductAssignment> crProductList, List<Tuple<MatchItem, int>> matchItemWithProductRankings, Dictionary<int, int> productList)
        {
            List<Tuple<MatchItem, int>> crMatchItems = new List<Tuple<MatchItem, int>>();

            foreach (var crProduct in crProductList)
            {
                int crQualityRanking = productList.Where(pl => pl.Key == crProduct.ProductId).FirstOrDefault().Value;

                crMatchItems.AddRange(matchItemWithProductRankings.Where(mi => mi.Item2 >= crQualityRanking && mi.Item1.ClientRelationshipId == crProduct.CrId));
            }

            return crMatchItems;
        }

        private List<Tuple<MatchItem, int>> JoinProductQualityRankings(List<MatchItem> matchItemList, Dictionary<int, int> productList)
        {
            var rankings = from m in matchItemList
                           join p in productList
                           on m.ProductId equals p.Key
                           select new Tuple<MatchItem, int>(m, p.Value);

            return rankings.ToList();
        }
         */

        public static Dictionary<int, LeadPingLeadScore> GetLeadPingLeadScores()
        {
            Dictionary<int, LeadPingLeadScore> leadScores = new Dictionary<int, LeadPingLeadScore>();

            foreach (LeadPingLeadScore leadScore in LeadScoringDataService.GetLeadPingLeadScores())
            {
                leadScores.Add(leadScore.LeadPingLeadScoreId, leadScore);
            }
            return leadScores;
        }

        public static Dictionary<int, LeadPingLeadScoreCPL> GetLeadPingLeadScoreCPLs()
        {
            Dictionary<int, LeadPingLeadScoreCPL> leadPingLeadScoreCPLs = new Dictionary<int, LeadPingLeadScoreCPL>();

            foreach (LeadPingLeadScoreCPL leadPingLeadScoreCPL in LeadScoringDataService.GetLeadPingLeadScoreCPLs())
            {
                leadPingLeadScoreCPLs.Add(leadPingLeadScoreCPL.LeadPingLeadScoreCPLId, leadPingLeadScoreCPL);
            }
            return leadPingLeadScoreCPLs;
        }
    }
}
