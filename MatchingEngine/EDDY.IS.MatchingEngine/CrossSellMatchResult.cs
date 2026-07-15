using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine
{
    public class CrossSellMatchResult : MatchResult
    {
        public List<MatchItem> CrossSellSpecialtyMatchItemList { get; set; }
        public List<MatchItem> CrossSellSubjectMatchItemList { get; set; }
        public List<MatchItem> CrossSellCategoryMatchItemList { get; set; }
        public int? MaxProgramsToDisplay { get; set; }

        public int? MaxUserSelections { get; set; }

        public CampusType TemplateProgramCampusType { get; set; }

        public List<CrossSellMapping> CrossSellMappingList { get; set; }        

        public bool IsSAB { get; set; }
    }
}
