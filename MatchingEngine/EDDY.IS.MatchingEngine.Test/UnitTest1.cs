using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDDY.IS.MatchingEngine.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetCategories()
        {
            MatchingService.MatchingServiceClient client = new MatchingService.MatchingServiceClient();
            client.Open();
            DTO.DirectoryMatchRequest request = new DTO.DirectoryMatchRequest();
            request.TrackGuid = new Guid("38ca993e-865f-4d87-8b37-8420489fcd31");
            request.Application = EDDY.IS.Core.ISApplication.ExpressDirectories;
            request.ApplicationId = 2;
            request.ProgramTypeList = new System.Collections.Generic.List<DTO.ProgramType>(new DTO.ProgramType[] { DTO.ProgramType.Degrees });
            request.SortMethod = DTO.EntitySortMethod.Alphabetical;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            client.GetCategories(request);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 200)
                Assert.Fail("Too long");
        }

        [TestMethod]
        public void GetInstitutions()
        {
            MatchingService.MatchingServiceClient client = new MatchingService.MatchingServiceClient();
            client.Open();
            DTO.DirectoryMatchRequest request = new DTO.DirectoryMatchRequest();
            request.TrackGuid = new Guid("38ca993e-865f-4d87-8b37-8420489fcd31");
            request.Application = EDDY.IS.Core.ISApplication.ExpressDirectories;
            request.ApplicationId = 2;
            request.SortMethod = DTO.EntitySortMethod.RankScore;
            request.MaxResultsCount = 10;
            request.CampusType = DTO.CampusType.Online;
            request.RemoveInvalidEntities = true;
            request.MaxNestedProgramCount = 1;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            client.GetInstitutions(request, false);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 200)
                Assert.Fail("Too long");
        }

        [TestMethod]
        public void GetSubject()
        {
            MatchingService.MatchingServiceClient client = new MatchingService.MatchingServiceClient();
            client.Open();
            DTO.DirectoryMatchRequest request = new DTO.DirectoryMatchRequest();
            request.TrackGuid = new Guid("38ca993e-865f-4d87-8b37-8420489fcd31");
            request.Application = EDDY.IS.Core.ISApplication.ExpressDirectories;
            request.ApplicationId = 2;
            request.ProgramTypeList = new System.Collections.Generic.List<DTO.ProgramType>(new DTO.ProgramType[] { DTO.ProgramType.Degrees });
            request.SortMethod = DTO.EntitySortMethod.Alphabetical;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            client.GetSubjects(request);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 200)
                Assert.Fail("Too long");
        }
    }
}
