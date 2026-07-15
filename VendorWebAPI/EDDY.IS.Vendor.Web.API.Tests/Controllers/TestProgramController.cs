using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using System.Web.Http.Controllers;
using System.Net.Http;
using EDDY.IS.Vendor.Web.API.Filters;

namespace EDDY.IS.Vendor.Web.API.Tests.Controllers
{
    [TestClass]
    public class TestProgramController
    {
        [TestMethod]
        public void GetProgramDetails_ShouldNotBeNull()
        {
            Programs programs = new Programs();
            DirectoryRequest directoryRequest = new DirectoryRequest();
            directoryRequest.APIKey = Guid.Parse("8696D456-4F79-4AEF-BAC6-D1D25B521704");
            directoryRequest.ProgramId = 7243;
            directoryRequest.CampusId = 4919;
            Program program = programs.GetProgramDetails(directoryRequest);
            Assert.IsNotNull(program);
        }

        [TestMethod]
        public void GetProgramForm_ShouldNotBeNull()
        {
            Programs programs = new Programs();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            contactRequest.TemplateId = 3;
            FormTemplate formTemplate = programs.GetProgramForm(contactRequest);
            Assert.IsNotNull(formTemplate);
        }

        [TestMethod]
        public void GetProgramForm_ShouldReturnSteps()
        {
            Programs programs = new Programs();
            ContactRequest contactRequest = new ContactRequest();
            contactRequest.APIKey = Guid.Parse("5395975D-0E9E-479C-9382-11EBABED5873");
            contactRequest.TemplateId = 3;
            FormTemplate formTemplate = programs.GetProgramForm(contactRequest);


            Assert.IsTrue(formTemplate.Fields.Count > 0, "We have Steps");


        }
        
    }
}
