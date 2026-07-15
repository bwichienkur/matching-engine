using EDDY.IS.MatchingEngine.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.UnitTests.MatchDatabase
{
    [TestClass]
    public class GetCapPercentFromCapFixture
    {
        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfNoChildren()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(60, 100);
            
            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.4d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturn1stChildPercentFromCapIfFirstChildHasNonFullCapAndIsEntityTypePSIAndMatchingPSI()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(60, 100);

            Cap firstChildCap = new Cap(45, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.55d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfFirstChildHasNonFullCapAndIsEntityTypePSIAndNonMatchingPSI()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(90, 100);

            Cap firstChildCap = new Cap(60, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(456);
            parentCap.Children.Add(firstChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.10d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfFirstChildHasFullCapAndParentDoesNot()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(25, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.75d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturn2ndChildPercentFromCapIf1stChildIsFull()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(90, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(20, 100);
            secondChildCap.CapType = DTO.EntityMeta.PSI;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.80d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturn2ndChildPercentFromCapIf1stChildIsNotAPSI()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(90, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.Channel;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(10, 100);
            secondChildCap.CapType = DTO.EntityMeta.PSI;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.90d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfFirstAndSecondChildrenAreFull()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(75, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(100, 100);
            secondChildCap.CapType = DTO.EntityMeta.PSI;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.25d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfAllChildrenAreNotPSIs()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(65, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.MarketingUnit;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(100, 100);
            secondChildCap.CapType = DTO.EntityMeta.Channel;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.35d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturn3ndChildPercentFromCapIfParentsAreFull()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(90, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(100, 100);
            secondChildCap.CapType = DTO.EntityMeta.PSI;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            Cap thirdChildCap = new Cap(25, 100);
            thirdChildCap.CapType = DTO.EntityMeta.PSI;
            thirdChildCap.EntityIDSet.Add(123);
            secondChildCap.Children.Add(thirdChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.75d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturn3rdChildPercentFromCapIfParentsAreNotAPSI()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(90, 100);

            Cap firstChildCap = new Cap(50, 100);
            firstChildCap.CapType = DTO.EntityMeta.Channel;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(75, 100);
            secondChildCap.CapType = DTO.EntityMeta.MarketingUnit;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            Cap thirdChildCap = new Cap(25, 100);
            thirdChildCap.CapType = DTO.EntityMeta.PSI;
            thirdChildCap.EntityIDSet.Add(123);
            secondChildCap.Children.Add(thirdChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.75d, usedPercentageFromCap, 0.0000001);
        }

        [TestMethod]
        public void MatchingService_GetCapPercentFromCap_ShouldReturnParentPercentFromCapIfFirstSecondAndThirdChildrenAreFull()
        {
            EDDY.IS.MatchingEngine.MatchDatabase matchDatabase = new EDDY.IS.MatchingEngine.MatchDatabase();
            MatchItemInternal testMatch = new MatchItemInternal();
            testMatch.PsiId = 123;

            Cap parentCap = new Cap(80, 100);

            Cap firstChildCap = new Cap(100, 100);
            firstChildCap.CapType = DTO.EntityMeta.PSI;
            firstChildCap.EntityIDSet.Add(123);
            parentCap.Children.Add(firstChildCap);

            Cap secondChildCap = new Cap(100, 100);
            secondChildCap.CapType = DTO.EntityMeta.PSI;
            secondChildCap.EntityIDSet.Add(123);
            firstChildCap.Children.Add(secondChildCap);

            Cap thirdChildCap = new Cap(100, 100);
            thirdChildCap.CapType = DTO.EntityMeta.PSI;
            thirdChildCap.EntityIDSet.Add(123);
            secondChildCap.Children.Add(thirdChildCap);

            double usedPercentageFromCap = (double)matchDatabase.GetCapPercentFromCap(testMatch, parentCap, parentCap.CapPercentFromCap);

            Assert.AreEqual(0.20d, usedPercentageFromCap, 0.0000001);
        }
    }
}
