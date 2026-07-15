using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class CrossSellModel: BaseModel
    {
        public CrossSellProgramResponse CrossSellProgramResponse { get; set; }

        public int MaxProgramsToDisplayTotal { get; set; }
        
        public bool InitialMatchWasValid { get; set; }
        public bool WasLeadCreated { get; set; }
        public bool IsForOptimizelyCrossSell { get; set; }
        public bool IsForTemplatePreview { get; set; }

        public string UserFullName { get; set; }
        public string UserSelectedInstitutionName { get; set; }
        public string UserSelectedProgramName { get; set; }
        public int MaxCrossSellUserSelections { get; set; }
        public string MaxCrossSellUserSelectionsAlpha { get; set; }

        public string Theme { get; set; }
        public int TemplateId { get; set; }
        public List<string> MobilePhones { get; set; }
        public EddyLogosDTO EddyLogos { get; set; }

        public CrossSellModel()
        {
            this.EddyLogos = new EddyLogosDTO();
        }

        #region #54902
        public string CrossSellHeaderMessage { get; set; }
        public string CrossSellSubHeaderMessage { get; set; }
        #endregion

    }


}
