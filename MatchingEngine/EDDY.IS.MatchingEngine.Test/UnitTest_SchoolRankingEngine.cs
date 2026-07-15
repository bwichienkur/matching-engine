using System;
using EDDY.IS.MatchingEngine.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.MatchingEngine;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EDDY.IS.MatchingEngine.Test
{
    [TestClass]
    public class UnitTest_SchoolRankingEngine
    {
        [TestMethod]
        public void CanPickRankingModel_MatchOnlyOneCriteriaGroup()
        {

            SchoolRankingEngine ranking = new SchoolRankingEngine();

            Campaign campaign = new Campaign();
            campaign.ChannelId = 1;
            campaign.VendorId = 528;

            ProspectInput prospect = new ProspectInput();
            int applicationId = 6;
            int? subjectId = null;
            int? categoryId = null;
            int? programLevelId = null;

            var returnVal = ranking.PickRankingModel(campaign, prospect, applicationId, subjectId, categoryId, programLevelId);

            Assert.IsNotNull(returnVal);
        }

        [TestMethod]
        public void CanPickRankingModel_DontMatchAnyCriteriaGroup()
        {
            SchoolRankingEngine ranking = new SchoolRankingEngine();

            Campaign campaign = new Campaign();
            campaign.ChannelId = 1;
            campaign.VendorId = 112312;

            ProspectInput prospect = new ProspectInput();
            int applicationId = 6;
            int? subjectId = null;
            int? categoryId = null;
            int? programLevelId = null;

            var returnVal = ranking.PickRankingModel(campaign, prospect, applicationId, subjectId, categoryId, programLevelId);

            Assert.IsNotNull(returnVal);
        }

        [TestMethod]
        public void CanPickRankingModel_MatchesMultipleCriteriaGroupButOneHasMoreGroupItems()
        {
            SchoolRankingEngine ranking = new SchoolRankingEngine();

            Campaign campaign = new Campaign();
            campaign.ChannelId = 1;
            campaign.VendorId = 528;
            campaign.CampaignId = 2;

            ProspectInput prospect = new ProspectInput();
            int applicationId = 6;
            int? subjectId = null;
            int? categoryId = null;
            int? programLevelId = null;

            var returnVal = ranking.PickRankingModel(campaign, prospect, applicationId, subjectId, categoryId, programLevelId);

            Assert.IsNotNull(returnVal);
        }

        [TestMethod]
        public void CanPickRankingModel_MatchesAge()
        {
            SchoolRankingEngine ranking = new SchoolRankingEngine();

            Campaign campaign = new Campaign();
            //campaign.ChannelID = 1;
            //campaign.VendorID = 528;
            //campaign.CampaignID = 2;

            ProspectInput prospect = new ProspectInput();
            prospect.Age = 20;


            int applicationId = 6;
            int? subjectId = null;
            int? categoryId = null;
            int? programLevelId = null;

            var returnVal = ranking.PickRankingModel(campaign, prospect, applicationId, subjectId, categoryId, programLevelId);

            Assert.IsNotNull(returnVal);
        }

        [TestMethod]
        public void CanPickRankingModel_MatchesMultipleCriteriaGroupBothSameNumberOfItems()
        {
            SchoolRankingEngine ranking = new SchoolRankingEngine();

            Campaign campaign = new Campaign();
            campaign.ChannelId = 7;
            //campaign.VendorID = 528;
            //campaign.CampaignID = 2;

            ProspectInput prospect = new ProspectInput();
            //prospect.Age = 20;
            prospect.StateId = 1;
            prospect.EducationLevelId = 10;

            int applicationId = 6;
            int? subjectId = null;
            int? categoryId = 10;
            int? programLevelId = null;

            var returnVal = ranking.PickRankingModel(campaign, prospect, applicationId, subjectId, categoryId, programLevelId);

            Assert.IsNotNull(returnVal);
        }

        [TestMethod]
        public void CanGetDistance()
        {
            StaticCacheProxyHost.CacheProxy.PreloadEntireCache();

            GeoCodeProcessor proc = new GeoCodeProcessor();

            double? distance = proc.GetDistanceBetweenZipCodes("33073", "33428");

            Assert.IsTrue(distance != default(double?));
        }

        [TestMethod]
        public void CanGetProgramContent()
        {
            MatchingContentData.GetProgramContent();
        }

        [TestMethod]
        public void TestGetProgramContentADO()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif   
            using (SqlConnection conn = new SqlConnection(@"data source=EDDYDB01\DEV02;initial catalog=Nexus_FLL_new;integrated security=True;multipleactiveresultsets=True;"))
            {
                conn.Open();

                SqlCommand comm = new SqlCommand(@"select * from [VW_Matching_ProgramContent]", conn);
                SqlDataReader reader = comm.ExecuteReader();
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("TestGetProgramContentADO(): " + sw.ElapsedMilliseconds.ToString() + "ms");
#endif            
        }

        [TestMethod]
        public void CanGetAllMatchingContent()
        {
            var programs = MatchingContentData.GetProgramContent();
            var insts = MatchingContentData.GetInstitutionContent();
            var campuses = MatchingContentData.GetCampusContent();
        }
    }
}
