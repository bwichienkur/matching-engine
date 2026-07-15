using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Vendor.DataAccess.DataModels;
using EDDY.IS.Vendor.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.EmsLeadEngine.Entities;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.Vendor.Web.API.Tests.Controllers
{
    [TestClass]
    public class TestLeadUpdateController
    {

        // Lead Update 

        [TestMethod]
        public void MapToExchangeLead_ShouldMapClientStatusAndLeadState()
        {
            var dao = new DataExchangeServiceDAO();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStatus = "OldStatus"
            };

            var request = new LeadUpdateRequest
            {
                EMSInstitutionId = 136,
                LeadStateId = 2,
                ClosedReasonCode = "Test Lead Update",
                ClientStatus = "Starts",
                ClientNotes = "Updated notes"
            };

            var mapped = dao.MapToExchangeLead(originalLead, request);

            Assert.IsNotNull(mapped);
            Assert.IsNotNull(mapped.Lead);
            Assert.AreEqual(2, mapped.Lead.LeadStateId);
            Assert.AreEqual("Starts", mapped.Lead.ClientStatus);
            Assert.AreEqual(DateTime.Today, mapped.Lead.ClientStartDate.Value.Date); 
            Assert.AreEqual("Updated notes", mapped.Lead.ClientNotes);
            Assert.AreEqual(originalLead.LeadGUID, mapped.Lead.LeadGUID);
        }

        [TestMethod]
        public void MapToExchangeLead_ShouldNotDefaultClientStartDate_WhenOriginalDateAlreadyExists()
        {
            var dao = new DataExchangeServiceDAO();

            DateTime existingStartDate = new DateTime(2026, 1, 1);

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStartDate = existingStartDate
            };

            var request = new LeadUpdateRequest
            {
                EMSInstitutionId = 136,
                ClientStatus = "Starts"
            };

            var mapped = dao.MapToExchangeLead(originalLead, request);

            Assert.IsNotNull(mapped);
            Assert.IsNotNull(mapped.Lead);

            // It should NOT send a date update because original already has a value
            Assert.IsNull(mapped.Lead.ClientStartDate);
        }

        [TestMethod]
        public void MapToExchangeLead_ShouldUseRequestDate_WhenDateIsPassed()
        {
            var dao = new DataExchangeServiceDAO();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStartDate = null
            };

            var request = new LeadUpdateRequest
            {
                EMSInstitutionId = 136,
                ClientStatus = "Starts",
                ClientStartDate = "2026-02-01"
            };

            var mapped = dao.MapToExchangeLead(originalLead, request);

            Assert.IsNotNull(mapped);
            Assert.IsNotNull(mapped.Lead);
            Assert.IsTrue(mapped.Lead.ClientStartDate.HasValue);
            Assert.AreEqual(new DateTime(2026, 2, 1), mapped.Lead.ClientStartDate.Value.Date);
        }


        [TestMethod]
        public void LeadChanged_ShouldReturnFalse_WhenOnlyLookupFieldsChanged()
        {
            var dao = new DataExchangeServiceDAO();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStatus = "Starts"
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = originalLead.LeadGUID,
                Email = "newemail@test.com",
                Phone1 = "5555555555"
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsFalse(changed);
        }

        [TestMethod]
        public void LeadChanged_ShouldReturnTrue_WhenClientStatusChanged()
        {
            var dao = new DataExchangeServiceDAO();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStatus = "OldStatus"
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = originalLead.LeadGUID,
                ClientStatus = "Starts"
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsTrue(changed);
        }

        [TestMethod]
        public void LeadChanged_ShouldReturnTrue_WhenDateChanged()
        {
            var dao = new DataExchangeServiceDAO();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = Guid.NewGuid(),
                ClientStartDate = new DateTime(2026, 1, 1)
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = originalLead.LeadGUID,
                ClientStartDate = new DateTime(2026, 2, 1)
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsTrue(changed);
        }

        [TestMethod]
        public void LeadChanged_ShouldReturnFalse_WhenNoMappedFieldsChanged()
        {
            var dao = new DataExchangeServiceDAO();

            Guid leadGuid = Guid.NewGuid();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = leadGuid,
                LeadStateId = 2,
                ClientStatus = "Starts",
                ClientNotes = "Same notes"
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = leadGuid,
                LeadStateId = 2,
                ClientStatus = "Starts",
                ClientNotes = "Same notes"
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsFalse(changed);
        }
        [TestMethod]
        public void LeadChanged_ShouldReturnTrue_WhenLeadStateChanged()
        {
            var dao = new DataExchangeServiceDAO();

            Guid leadGuid = Guid.NewGuid();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = leadGuid,
                LeadStateId = 1
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = leadGuid,
                LeadStateId = 2
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsTrue(changed);
        }

        [TestMethod]
        public void LeadChanged_ShouldIgnoreNullUpdatedValues()
        {
            var dao = new DataExchangeServiceDAO();

            Guid leadGuid = Guid.NewGuid();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = leadGuid,
                ClientStatus = "Existing Status",
                ClientNotes = "Existing Notes"
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = leadGuid,
                ClientStatus = null,
                ClientNotes = null
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsFalse(changed);
        }

        [TestMethod]
        public void LeadChanged_ShouldIgnoreEmptyStringUpdatedValues()
        {
            var dao = new DataExchangeServiceDAO();

            Guid leadGuid = Guid.NewGuid();

            var originalLead = new VW_EMSLead
            {
                InstitutionId = 136,
                LeadGUID = leadGuid,
                ClientStatus = "Existing Status",
                ClientNotes = "Existing Notes"
            };

            var updatedLead = new EDDY.IS.EmsLeadEngine.Entities.Common.ExchangeLead
            {
                EMSInstitutionId = 136,
                LeadGUID = leadGuid,
                ClientStatus = "",
                ClientNotes = "   "
            };

            bool changed = dao.LeadChanged(originalLead, updatedLead);

            Assert.IsFalse(changed);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void LookUpLead_ShouldFindLead_ByEMSLeadId()
        {
            var dao = new DataExchangeServiceDAO();

            var request = new LeadUpdateRequest
            {
                EMSInstitutionId = 136,
                LookUpKeyList = new List<int> { (int)ExchangeLeadUniqueKey.EMSLeadId },
                EMSLeadId = 27259894 // replace with real QA EMSLeadId
            };

            var lead = dao.LookUpLead(request);

            Assert.IsNotNull(lead);
        }

    }
}
