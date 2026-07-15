using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class CampusWithInstitutionModel
    {
        public string CampusName { get; set; }
        public int CampusId { get; set; }
        public CampusType CampusType { get; set; }
    }
}