using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public enum FilterType
    {
        InstitutionList,
        CampusId,
        ProgramList,
        ProgramLevelList,
        ProgramTypeList,
        CampusType,
        CategoryList,
        SubjectList,
        SpecialtyList,
        TemplateList,
        CampusList,
        Hybrid,
        ClickOnly,
        ProgramGroupId,
        WorkTypeList,
        TermList,
        DurationList,
        PlacementAudienceList,
        TeachAbroadTypeList,
        LanguageList,
        ProductList,
        SFProductCodeList,
        NonProfit,
        PaidStatusType,
        FraidStatusType,
        FreeStatusType,
		CampusOptionGroupId,
        AdvertiserList,
        ClientRelationshipList
    }

    public class Filter
    {
        public FilterType Type { get; set; }
        public List<int> Filters { get; set; }

        public bool? BooleanFilter { get; set; }

        public int ItemCount { get; set; }

        public Filter()
        {
            Filters = new List<int>();
        }
    }
}
