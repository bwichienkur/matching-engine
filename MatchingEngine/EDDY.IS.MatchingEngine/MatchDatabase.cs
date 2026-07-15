using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.Core;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Reflection.Emit;
using System.Reflection;
using EDDY.IS.Base.Util;

namespace EDDY.IS.MatchingEngine
{
    public class MatchDatabase
    {
        /*****************************************************************************************************************
         * 
         *  The MatchDatabase contains the logic for how we filter programs based on the inputted criteria. 
         *  
         *  At the core of it are a bunch of Dictionaries of which:
         *  Dictionary Key - the id of the filter being applied (ex. SubjectId)
         *  Dictionary Value - is the Set of relevant matches that are linked to that key. For simplicity and for performance
         *                     the value is a HashSet<string>. The string that goes into the hash set is a key for the MatchItem
         *                     which is a combination of ApplicationId, ProgramProductId, CategoryId, SubjectId and SpecialtyId
         *                     this is because all of those values are needed to tie to a unique record.
         *  
         *  There are two other HashSet filters that aren't part of a Dictionary and those are:
         *      - OnlineCampusTypeProgramProducts : All MatchItems (by key) that belong to an online campus
         *      - CampusCampusTypeProgramProducts : All MatchItems (by key) that belong to a ground campus
         * 
         *  Here is a listing of the current Dictionaries:
         *      - ApplicationProgramProducts    : Lookup of all MatchItems tied to an Application
         *      - InstitutionProgramProducts    : Lookup of all MatchItems tied to a Institution
         *      - CampusProgramProducts         : Lookup of all MatchItems tied to a Campus
         *      - ProgramProgramProducts        : Lookup of all MatchItems tied to a Program
         *      - ProgramLevelProgramProducts   : Lookup of all MatchItems tied to a Program Level
         *      - ProgramTypeProgramProducts    : Lookup of all MatchItems tied to a Program Type
         *      - CategoryProgramProducts       : Lookup of all MatchItems tied to a Category
         *      - SubjectProgramProducts        : Lookup of all MatchItems tied to a Subject
         *      - SpecialtyProgramProducts      : Lookup of all MatchItems tied to a Specialty
         *      - TemplateProgramProducts       : Lookup of all MatchItems tied to a Template
         * 
         *  Once we are able to filter down to the resultant set of MatchItem keys then the ApplicationKeyMatchItems is utilized to
         *  get the actual MatchItem objects. 
         *****************************************************************************************************************/
        //public Dictionary<int, HashSet<string>> ApplicationProgramProducts { get; set; }
        private static readonly HashSet<int> PDProductIds = new HashSet<int>(new int[] { 1, 2, 8, 9, 16, 22, 48 });
        private static readonly HashSet<int> PDPlusGSProductIds = new HashSet<int>(new int[] { 1, 2, 8, 9, 16, 17, 22, 48, 73, 74 });
        private static readonly HashSet<int> PDPlusInternationalProductIds = new HashSet<int>(new int[] { 1, 2, 4, 8, 9, 16, 22, 48 });
        private static readonly HashSet<int> PDPlusGSAndInternationalProductIds = new HashSet<int>(new int[] { 1, 2, 4, 8, 9, 16, 17, 22, 48, 73, 74 });
        private static readonly HashSet<int> GraduateProgramLevels = new HashSet<int>(new int[] { 7, 8, 11 });
        private HashSet<int> PDProgramProducts { get; set; }

        private HashSet<int> GraduateProgramProducts { get; set; }

        private HashSet<int> PDPlusGSProgramProducts { get; set; }

        private HashSet<int> PDPlusInternationalProgramProducts { get; set; }

        private HashSet<int> PDPlusGSAndInternationalProgramProducts { get; set; }

        public HashSet<int> ProgramProductKeys { get; set; }
        public HashSet<int> PaidProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> PaidStatusTypeProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> InstitutionProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> CampusProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ProgramProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ProgramLevelProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ProgramTypeProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> CategoryProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> SubjectProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> SpecialtyProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> TemplateProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ProductProgramProducts { get; set; }
        public HashSet<int> OnlineCampusTypeProgramProducts { get; set; }
        public HashSet<int> CampusCampusTypeProgramProducts { get; set; }
        public Dictionary<int, MatchItemInternal> MatchItems { get; set; }
        public Dictionary<Base.InstitutionLeadTypes, Dictionary<int, List<MatchItemInternal>>> ThirdPartyMatches { get; set; }
        public Dictionary<int, HashSet<int>> ProgramIsHybridProgramProducts { get; set; }
        public HashSet<int> ClickOnlyProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ProgramGroups { get; set; }

        public Dictionary<int, Dictionary<int, HashSet<int>>> ProductCRPrograms { get; set; }
        //SAB Filters
        public Dictionary<int, HashSet<int>> WorkTypeProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> TermProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> DurationProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> PlacementAudienceProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> TeachAbroadTypeProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> LanguageProgramProducts { get; set; }

        public Dictionary<int, HashSet<int>> SFProductCodeProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> NonProfitProgramProducts { get; set; }
		public Dictionary<int, HashSet<int>> CampusOptionGroupProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> AdvertiserProgramProducts { get; set; }
        public Dictionary<int, HashSet<int>> ClientRelationshipProgramProducts { get; set; }
        public HashSet<int> TemplateUnassignedProgramProducts { get; set; }

        public MatchDatabase()
        {
            InstantiateObjects();
        }

        private void InstantiateObjects()
        {
            ProductCRPrograms = new Dictionary<int, Dictionary<int, HashSet<int>>>();
            ProgramProductKeys = new HashSet<int>();
            PDProgramProducts = new HashSet<int>();
            PDPlusGSAndInternationalProgramProducts = new HashSet<int>();
            PDPlusGSProgramProducts = new HashSet<int>();
            PDPlusInternationalProgramProducts = new HashSet<int>();
            GraduateProgramProducts = new HashSet<int>();
            ProductProgramProducts = new Dictionary<int, HashSet<int>>();
            InstitutionProgramProducts = new Dictionary<int, HashSet<int>>();
            CampusProgramProducts = new Dictionary<int, HashSet<int>>();
            ProgramProgramProducts = new Dictionary<int, HashSet<int>>();
            ProgramLevelProgramProducts = new Dictionary<int, HashSet<int>>();
            ProgramTypeProgramProducts = new Dictionary<int, HashSet<int>>();
            OnlineCampusTypeProgramProducts = new HashSet<int>();
            CampusCampusTypeProgramProducts = new HashSet<int>();
            CategoryProgramProducts = new Dictionary<int, HashSet<int>>();
            SubjectProgramProducts = new Dictionary<int, HashSet<int>>();
            SpecialtyProgramProducts = new Dictionary<int, HashSet<int>>();
            TemplateProgramProducts = new Dictionary<int, HashSet<int>>();
            MatchItems = new Dictionary<int, MatchItemInternal>();
            ThirdPartyMatches = new Dictionary<Base.InstitutionLeadTypes, Dictionary<int, List<MatchItemInternal>>>();
            ProgramIsHybridProgramProducts = new Dictionary<int, HashSet<int>>();
            ClickOnlyProgramProducts = new HashSet<int>();
            ProgramGroups = new Dictionary<int, HashSet<int>>();
            WorkTypeProgramProducts = new Dictionary<int, HashSet<int>>();
            TermProgramProducts = new Dictionary<int, HashSet<int>>();
            DurationProgramProducts = new Dictionary<int, HashSet<int>>();
            PlacementAudienceProgramProducts = new Dictionary<int, HashSet<int>>();
            TeachAbroadTypeProgramProducts = new Dictionary<int, HashSet<int>>();
            LanguageProgramProducts = new Dictionary<int, HashSet<int>>();
            SFProductCodeProgramProducts = new Dictionary<int, HashSet<int>>();
            NonProfitProgramProducts = new Dictionary<int, HashSet<int>>();
            TemplateUnassignedProgramProducts = new HashSet<int>();
			CampusOptionGroupProgramProducts = new Dictionary<int, HashSet<int>>();
            AdvertiserProgramProducts = new Dictionary<int, HashSet<int>>();
            ClientRelationshipProgramProducts = new Dictionary<int, HashSet<int>>();
        }

        private HashSet<int> FilterDictionaryLookup(int size, FilterType filter, List<int> filterList, Dictionary<int, HashSet<int>> lookupDictionary, Core.Logging.PerformanceLog pLog)
        {
            StartLogDetail(pLog, "FilterDictionaryLookup");
            HashSet<int> result = null;

            if (filterList.Count == 1)
            {
                if (lookupDictionary.ContainsKey(filterList[0]))
                    result = lookupDictionary[filterList[0]];
            }
            else if (filterList.Count > 1)
            {
                bool isSubsetOfGraduate = false;
                bool isSubsetOfPD = false;
                bool isSubsetOfPDPlusGSAndInternational = false;
                bool isSubsetOfPDPlusGS = false;
                bool isSubsetOfPDPlusInternational = false;
                bool complete = false;

                if (filter == FilterType.ProgramLevelList && GraduateProgramLevels.IsSubsetOf(filterList))
                {
                    if (filterList.Count == GraduateProgramLevels.Count)
                    {
                        complete = true;
                        result = GraduateProgramProducts;
                    }
                    else
                    {
                        result = new HashSet<int>(GraduateProgramProducts);
                        isSubsetOfGraduate = true;
                    }
                }
                else if (filter == FilterType.ProductList && PDPlusGSAndInternationalProductIds.IsSubsetOf(filterList))
                {
                    if (filterList.Count == PDPlusGSAndInternationalProductIds.Count)
                    {
                        complete = true;
                        result = PDPlusGSAndInternationalProgramProducts;
                    }
                    else
                    {
                        result = new HashSet<int>(PDPlusGSAndInternationalProgramProducts);
                        isSubsetOfPDPlusGSAndInternational = true;
                    }
                }
                else if (filter == FilterType.ProductList && PDPlusGSProductIds.IsSubsetOf(filterList))
                {
                    if (filterList.Count == PDPlusGSProductIds.Count)
                    {
                        complete = true;
                        result = PDPlusGSProgramProducts;
                    }
                    else
                    {
                        result = new HashSet<int>(PDPlusGSProgramProducts);
                        isSubsetOfPDPlusGS = true;
                    }
                }
                else if (filter == FilterType.ProductList && PDPlusInternationalProductIds.IsSubsetOf(filterList))
                {
                    if (filterList.Count == PDPlusInternationalProductIds.Count)
                    {
                        complete = true;
                        result = PDPlusInternationalProgramProducts;
                    }
                    else
                    {
                        result = new HashSet<int>(PDPlusInternationalProgramProducts);
                        isSubsetOfPDPlusInternational = true;
                    }
                }
                else if (filter == FilterType.ProductList && PDProductIds.IsSubsetOf(filterList))
                {
                    if (filterList.Count == PDProductIds.Count)
                    {
                        complete = true;
                        result = PDProgramProducts;
                    }
                    else
                    {
                        result = new HashSet<int>(PDProgramProducts);
                        isSubsetOfPD = true;
                    }
                }

                if (!complete)
                {
                    for (int i = 0; i < filterList.Count; i++)
                    {
                        if (lookupDictionary.ContainsKey(filterList[i]))
                        {
                            if (isSubsetOfGraduate && GraduateProgramLevels.Contains(filterList[i]))
                                continue;
                            else if (isSubsetOfPD && PDProductIds.Contains(filterList[i]))
                                continue;
                            else if (isSubsetOfPDPlusGS && PDPlusGSProductIds.Contains(filterList[i]))
                                continue;
                            else if (isSubsetOfPDPlusGSAndInternational && PDPlusGSAndInternationalProductIds.Contains(filterList[i]))
                                continue;
                            else if (isSubsetOfPDPlusInternational && PDPlusInternationalProductIds.Contains(filterList[i]))
                                continue;
                            else if (result == null)
                                result = new HashSet<int>(lookupDictionary[filterList[i]]);
                            else
                                result.UnionWith(lookupDictionary[filterList[i]]);
                        }
                    }
                }
            }
            EndLogDetail(pLog, null);
            return result;
        }



        //private IEnumerable<string> Intersect(IEnumerable<string> first, HashSet<string> second)
        //{
        //    IEnumerable<string> set = null;

        //    if (first != null && second != null)
        //    {
        //        set = (first.Count() > second.Count)
        //                  ? first.Intersect(second)
        //                  : second.Intersect(first);
        //    }

        //    return set;
        //}

        private HashSet<int> Intersect(HashSet<int> first, HashSet<int> second)
        {
            HashSet<int> set = new HashSet<int>();
            HashSet<int> smaller;
            HashSet<int> larger;

            if (first != null && second != null)
            {
                if (first.Count < second.Count)
                {
                    smaller = first;
                    larger = second;
                }
                else
                {
                    smaller = second;
                    larger = first;
                }

                foreach (var item in smaller)
                {
                    if (larger.Contains(item))
                        set.Add(item);
                }
            }

            return set;
        }

        protected void StartLogDetail(Core.Logging.PerformanceLog pLog, string methodName)
        {
            if (pLog != null)
            {
                pLog.StartLogDetail(methodName);
            }
        }

        protected void EndLogDetail(Core.Logging.PerformanceLog pLog, string methodName)
        {
            if (pLog != null)
            {
                pLog.EndLogDetail();
            }

            if (!String.IsNullOrEmpty(methodName))
                StartLogDetail(pLog, methodName);
        }

        private int FilterCount(List<int> filters, Dictionary<int, HashSet<int>> lookupDictionary)
        {
            int total = 0;

            foreach (int f in filters)
            {
                if (lookupDictionary.ContainsKey(f))
                    total += lookupDictionary[f].Count;
            }

            return total;
        }
        private IEnumerable<Filter> SetupFilters(List<int> institutionIdList, int? campusId,
                                                     List<int> programIdList, List<int> programLevels, List<int> programTypes,
                                                     CampusType? campusType, List<int> categories, List<int> subjects,
                                                     List<int> specialties, List<int> templates, List<int> campusList, bool? IsHybrid,
                                                     bool? onlyClick, int? programGroupId, List<int> workTypeList, List<int> termList,
                                                     List<int> durationList, List<int> placementAudienceList, List<int> teachAbroadTypeList,
                                                     List<int> languageList, List<int> productList, List<int> sfProductCodes, List<int> paidStatusType, bool? isNonProfit,
													 int? campusOptionGroupId, List<int> advertiserList, List<int> crList)
        {
            List<Filter> filters = new List<Filter>();

            if (institutionIdList != null && institutionIdList.Count > 0)
                filters.Add(new Filter() { Filters = institutionIdList, ItemCount = FilterCount(institutionIdList, InstitutionProgramProducts), Type = FilterType.InstitutionList });

            if (campusId.HasValue)
                filters.Add(new Filter() { Filters = new List<int> { campusId.Value }, ItemCount = FilterCount(new List<int> { campusId.Value }, CampusProgramProducts), Type = FilterType.CampusId });

            if (programIdList != null && programIdList.Count > 0)
                filters.Add(new Filter() { Filters = programIdList, ItemCount = FilterCount(programIdList, ProgramProgramProducts), Type = FilterType.ProgramList });

            if (programLevels != null && programLevels.Count > 0)
                filters.Add(new Filter() { Filters = programLevels, ItemCount = FilterCount(programLevels, ProgramLevelProgramProducts), Type = FilterType.ProgramLevelList });

            if (programTypes != null && programTypes.Count > 0)
                filters.Add(new Filter() { Filters = programTypes, ItemCount = FilterCount(programTypes, ProgramTypeProgramProducts), Type = FilterType.ProgramTypeList });

            //CampusType
            if (campusType.HasValue)
                filters.Add(new Filter() { Type = FilterType.CampusType, ItemCount = campusType.Value == CampusType.Ground ? CampusCampusTypeProgramProducts.Count : OnlineCampusTypeProgramProducts.Count });

            //Paid Status Type
            if (paidStatusType != null && paidStatusType.Count > 0)
                filters.Add(new Filter() { Filters = paidStatusType, ItemCount = FilterCount(paidStatusType, PaidStatusTypeProgramProducts), Type = FilterType.PaidStatusType });

            if (categories != null && categories.Count > 0)
                filters.Add(new Filter() { Filters = categories, ItemCount = FilterCount(categories, CategoryProgramProducts), Type = FilterType.CategoryList });

            if (subjects != null && subjects.Count > 0)
                filters.Add(new Filter() { Filters = subjects, ItemCount = FilterCount(subjects, SubjectProgramProducts), Type = FilterType.SubjectList });

            if (specialties != null && specialties.Count > 0)
                filters.Add(new Filter() { Filters = specialties, ItemCount = FilterCount(specialties, SpecialtyProgramProducts), Type = FilterType.SpecialtyList });

            if (templates != null && templates.Count > 0)
                filters.Add(new Filter() { Filters = templates, ItemCount = FilterCount(templates, TemplateProgramProducts), Type = FilterType.TemplateList });

            if (campusList != null && campusList.Count > 0)
                filters.Add(new Filter() { Filters = campusList, ItemCount = FilterCount(campusList, CampusProgramProducts), Type = FilterType.CampusList });

            if (IsHybrid.HasValue)
                filters.Add(new Filter() { BooleanFilter = IsHybrid, Type = FilterType.Hybrid, ItemCount = ProgramIsHybridProgramProducts[IsHybrid.Value ? 1 : 0].Count });

            if (onlyClick.HasValue && onlyClick.Value)
                filters.Add(new Filter() { BooleanFilter = onlyClick, Type = FilterType.ClickOnly, ItemCount = ClickOnlyProgramProducts.Count });

            if (programGroupId.HasValue)
                filters.Add(new Filter() { Filters = new List<int> { programGroupId.Value }, Type = FilterType.ProgramGroupId, ItemCount = FilterCount(new List<int> { programGroupId.Value }, ProgramGroups) });

            if (workTypeList != null && workTypeList.Count > 0)
                filters.Add(new Filter() { Filters = workTypeList, ItemCount = FilterCount(workTypeList, WorkTypeProgramProducts), Type = FilterType.WorkTypeList });

            if (termList != null && termList.Count > 0)
                filters.Add(new Filter() { Filters = termList, ItemCount = FilterCount(termList, TermProgramProducts), Type = FilterType.TermList });

            if (durationList != null && durationList.Count > 0)
                filters.Add(new Filter() { Filters = durationList, ItemCount = FilterCount(durationList, DurationProgramProducts), Type = FilterType.DurationList });

            if (placementAudienceList != null && placementAudienceList.Count > 0)
                filters.Add(new Filter() { Filters = placementAudienceList, ItemCount = FilterCount(placementAudienceList, PlacementAudienceProgramProducts), Type = FilterType.PlacementAudienceList });

            if (teachAbroadTypeList != null && teachAbroadTypeList.Count > 0)
                filters.Add(new Filter() { Filters = teachAbroadTypeList, ItemCount = FilterCount(teachAbroadTypeList, TeachAbroadTypeProgramProducts), Type = FilterType.TeachAbroadTypeList });

            if (languageList != null && languageList.Count > 0)
                filters.Add(new Filter() { Filters = languageList, ItemCount = FilterCount(languageList, LanguageProgramProducts), Type = FilterType.LanguageList });

            if (sfProductCodes != null && sfProductCodes.Count > 0)
                filters.Add(new Filter() { Filters = sfProductCodes, ItemCount = FilterCount(sfProductCodes, SFProductCodeProgramProducts), Type = FilterType.SFProductCodeList });

            if (productList != null && productList.Count > 0)
                filters.Add(new Filter() { Filters = productList, ItemCount = FilterCount(productList, ProductProgramProducts), Type = FilterType.ProductList });

            if (isNonProfit.HasValue)
                filters.Add(new Filter() { BooleanFilter = isNonProfit, Type = FilterType.NonProfit, ItemCount = NonProfitProgramProducts[isNonProfit.Value ? 1 : 0].Count });

			if(campusOptionGroupId.HasValue)
				filters.Add(new Filter() { Filters = new List<int> { campusOptionGroupId.Value }, ItemCount = FilterCount(new List<int> { campusOptionGroupId.Value }, CampusOptionGroupProgramProducts), Type = FilterType.CampusOptionGroupId });

            if (advertiserList != null && advertiserList.Count > 0)
                filters.Add(new Filter() { Filters = advertiserList, ItemCount = FilterCount(advertiserList, AdvertiserProgramProducts), Type = FilterType.AdvertiserList });

            if (crList != null && crList.Count > 0)
                filters.Add(new Filter() { Filters = crList, ItemCount = FilterCount(crList, ClientRelationshipProgramProducts), Type = FilterType.ClientRelationshipList });

            filters = filters.OrderBy(f => f.ItemCount).ToList();

            return filters;
        }

        private HashSet<int> ProcessFilters(IEnumerable<Filter> filters, HashSet<int> initialProgramProducts, CampusType? campusType,
                                               Core.Logging.PerformanceLog pLog)
        {
            HashSet<int> programProducts = initialProgramProducts;

            foreach (var filter in filters)
            {
                switch (filter.Type)
                {
                    case FilterType.InstitutionList:
                        StartLogDetail(pLog, "Institution Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, InstitutionProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.CampusId:
                        StartLogDetail(pLog, "Campus Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, CampusProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ProgramList:
                        StartLogDetail(pLog, "Program Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ProgramProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ProgramLevelList:
                        StartLogDetail(pLog, "Program Level Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ProgramLevelProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.CategoryList:
                        StartLogDetail(pLog, "Category Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, CategoryProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.SubjectList:
                        StartLogDetail(pLog, "Subject Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, SubjectProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.SpecialtyList:
                        StartLogDetail(pLog, "Specialty Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, SpecialtyProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ProgramTypeList:
                        StartLogDetail(pLog, "Program Type Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ProgramTypeProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.TemplateList:
                        StartLogDetail(pLog, "Template Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, TemplateProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.CampusList:
                        StartLogDetail(pLog, "Campus Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, CampusProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ProgramGroupId:
                        StartLogDetail(pLog, "Program Group Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ProgramGroups, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.WorkTypeList:
                        StartLogDetail(pLog, "Work Type Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, WorkTypeProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.TermList:
                        StartLogDetail(pLog, "Term Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, TermProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.DurationList:
                        StartLogDetail(pLog, "Duration Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, DurationProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.TeachAbroadTypeList:
                        StartLogDetail(pLog, "Teach Abroad Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, TeachAbroadTypeProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.LanguageList:
                        StartLogDetail(pLog, "Language Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, LanguageProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.PlacementAudienceList:
                        StartLogDetail(pLog, "Placement Audience Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, PlacementAudienceProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ProductList:
                        StartLogDetail(pLog, "Product Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ProductProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.SFProductCodeList:
                        StartLogDetail(pLog, "SF Product Code Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, SFProductCodeProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.CampusType:
                        StartLogDetail(pLog, "Campus Type Filter");
                        if (campusType.Value == CampusType.Online && OnlineCampusTypeProgramProducts != null)
                            programProducts = Intersect(programProducts, OnlineCampusTypeProgramProducts);
                        else if (campusType.Value == CampusType.Ground && CampusCampusTypeProgramProducts != null)
                            programProducts = Intersect(programProducts, CampusCampusTypeProgramProducts);
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.PaidStatusType:
                        StartLogDetail(pLog, "Paid Status Type Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, PaidStatusTypeProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break; 
                    case FilterType.Hybrid:
                        StartLogDetail(pLog, "Hybrid Filter");
                        programProducts = Intersect(programProducts, ProgramIsHybridProgramProducts[filter.BooleanFilter.Value ? 1 : 0]);
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ClickOnly:
                        StartLogDetail(pLog, "Click Only Filter");
                        programProducts = Intersect(programProducts, ClickOnlyProgramProducts);
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.NonProfit:
                        StartLogDetail(pLog, "NonProfit Filter");
                        programProducts = Intersect(programProducts, NonProfitProgramProducts[filter.BooleanFilter.Value ? 1 : 0]);
                        EndLogDetail(pLog, null);
                        break;
					case FilterType.CampusOptionGroupId:
						StartLogDetail(pLog, "CampusOptionGroup Filter");
						programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, CampusOptionGroupProgramProducts, pLog));
						EndLogDetail(pLog, null);
						break;
                    case FilterType.AdvertiserList:
                        StartLogDetail(pLog, "Advertiser Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, AdvertiserProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                    case FilterType.ClientRelationshipList:
                        StartLogDetail(pLog, "CR Filter");
                        programProducts = Intersect(programProducts, FilterDictionaryLookup(filter.ItemCount, filter.Type, filter.Filters, ClientRelationshipProgramProducts, pLog));
                        EndLogDetail(pLog, null);
                        break;
                }
            }

            return programProducts;
        }
        public List<MatchItem> FilterProgramProducts(List<int> institutionIdList, int? campusId,
                                                     List<int> programIdList, List<int> programLevels, List<int> programTypes,
                                                     CampusType? campusType, List<int> categories, List<int> subjects,
                                                     List<int> specialties, List<int> templates, string searchTerm,
                                                     List<int> campusList, bool? IsHybrid, bool? onlyClick, int? programGroupId, bool paidOnly,
                                                     List<int> workTypeList, List<int> termList, List<int> durationList,
                                                     List<int> placementAudienceList, List<int> teachAbroadTypeList, List<int> languageList,
                                                     List<int> productList, List<int> sfProductCodes, List<int> paidStatusType, bool? isNonProfit, 
													 int? campusOptionGroupId, List<int> advertiserList, List<int> crList, Core.Logging.PerformanceLog pLog = null)
        {
            //StartLogDetail("MatchDatabase.FilterProgramProducts");

            List<MatchItem> matches = new List<MatchItem>();

            HashSet<int> programProducts;
            //IEnumerable<string> programProducts;

            StartLogDetail(pLog, "Initial Union");
            if (paidOnly)
                programProducts = PaidProgramProducts;
            else
                programProducts = ProgramProductKeys;

            EndLogDetail(pLog, "Setup Filters");
            IEnumerable<Filter> filters = SetupFilters(institutionIdList, campusId, programIdList, programLevels, programTypes, campusType, categories,
                                                subjects, specialties, templates, campusList, IsHybrid, onlyClick, programGroupId, workTypeList,
                                                termList, durationList, placementAudienceList, teachAbroadTypeList, languageList, productList, sfProductCodes, 
												paidStatusType, isNonProfit, campusOptionGroupId, advertiserList, crList);
            EndLogDetail(pLog, "Process Filters");
            programProducts = ProcessFilters(filters, programProducts, campusType, pLog);

            EndLogDetail(pLog, "Create Match Items");

            bool isSearchTerm = !String.IsNullOrEmpty(searchTerm);

            if (!isSearchTerm)
            {
                //matches = new List<MatchItem>(programProducts.Count());
                matches.Capacity = programProducts.Count();

                foreach (int key in programProducts)
                    //matches.Add(ApplicationKeyMatchItems[applicationId][key].CloneMatchItem());
                    //matches.Add(MatchItems[key].CloneMatchItem());
                    //matches.Add(CloneObjectWithIL(MatchItems[key]));
                    matches.Add(new MatchItem(MatchItems[key]));
            }
            else
            {
                searchTerm = searchTerm.ToLower();
                foreach (int key in programProducts)
                {
                    //MatchItem mi = ApplicationKeyMatchItems[applicationId][key];
                    MatchItemInternal mi = MatchItems[key];
                    if (!String.IsNullOrEmpty(mi.ProgramName) && mi.ProgramName.ToLower().Contains(searchTerm))
                        matches.Add(new MatchItem(mi));
                }
            }
            EndLogDetail(pLog, null);

            //EndLogDetail();
            return matches;
        }


        public MatchItem GetMatchItemByProgramProductId(int programProductId)
        {
            if (MatchItems.ContainsKey(programProductId))
                return new MatchItem(MatchItems[programProductId]);
            else
                return null;
        }

        public List<MatchItem> GetMatchItemsByProgramProductId(int programProductId)
        {
            List<MatchItem> matches = new List<MatchItem>();

            if (MatchItems.ContainsKey(programProductId))
            {
                MatchItemInternal programProduct = MatchItems[programProductId];

                return GetMatchItemsByProgramId(programProduct.ProgramId);
            }

            return matches;
        }

        public List<MatchItem> GetMatchItemsByProgramId(int programId)
        {
            List<MatchItem> matches = new List<MatchItem>();

            HashSet<int> programProducts = new HashSet<int>();

            if (ProgramProgramProducts.ContainsKey(programId))
            {
                programProducts.UnionWith(ProgramProductKeys);
                programProducts.IntersectWith(ProgramProgramProducts[programId]);

                foreach (var key in programProducts)
                    matches.Add(new MatchItem(MatchItems[key]));//, null, null, null));
            }

            return matches;
        }

        public MatchItem GetThirdPartyMatch(Base.InstitutionLeadTypes ilt)
        {
            if (this.ThirdPartyMatches.ContainsKey(ilt))
            {
                int randMax = this.ThirdPartyMatches[ilt].Count();
                int itemIndex = 0;

                if (randMax > 1)
                {
                    Random rand = new Random();
                    itemIndex = rand.Next(0, randMax);
                }

                return new MatchItem(this.ThirdPartyMatches[ilt].ElementAt(itemIndex).Value[0]);
            }

            return null;
        }

        public List<MatchItem> GetThirdPartyMatches(Base.InstitutionLeadTypes[] leadTypeList)
        {
            List<MatchItem> matches = new List<MatchItem>();

            foreach (var leadType in leadTypeList)
            {
                if (this.ThirdPartyMatches.ContainsKey(leadType))
                {
                    foreach (int instId in this.ThirdPartyMatches[leadType].Keys)
                    {
                        foreach (var matchItem in this.ThirdPartyMatches[leadType][instId])
                            matches.Add(new MatchItem(matchItem, leadType));
                    }
                }
            }
            return matches;
        }

        private void LoadContentData()
        {
            MatchingContentData.JoinContentData(MatchItems.Values.ToList());

            foreach (Base.InstitutionLeadTypes ilt in ThirdPartyMatches.Keys)
            {
                foreach (int institutionId in ThirdPartyMatches[ilt].Keys)
                {
                    MatchingContentData.JoinContentData(ThirdPartyMatches[ilt][institutionId]);
                }
            }
        }

        //public void LoadTemplateAssignment()
        //{
        //    TemplateCacheItem tci = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);

        //    //foreach (int appId in ApplicationKeyMatchItems.Keys)
        //    //{
        //        foreach (MatchItemInternal mi in MatchItems.Values)
        //        {
        //            int templateId;

        //            if (tci.ProgramProductToTemplateAssignments.TryGetValue(mi.ProgramProductId, out templateId))
        //            {
        //                mi.TemplateId = templateId;

        //            if (TemplateProgramProducts.ContainsKey(templateId))
        //                    TemplateProgramProducts[templateId].Add(mi.Key);
        //                else
        //                    TemplateProgramProducts.Add(templateId, new HashSet<int> { mi.Key });
        //            }
        //        }
        //    //}
        //}

        public void SetPaidStatusTypeAndSFProductCode()
        {
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);

            //set Free based on product.
            Dictionary<int, VW_Matching_Product> products = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_Product>>(MatchingCacheItem.Products);

            //TODO: reevaluatate what it means to be free for SAB (if it has an SF Product code its not free???)

            //Set GS programs with no RPL and no Click Price to Free
            int count = (from mi in MatchItems.Values
                         join pr in products
                                 on mi.ProductId equals pr.Key
                         where pr.Value.IsFree == true || (pr.Value.AllowFree == true && mi.eRPL == 0 && mi.ClickPrice == 0 && mi.ProgramTypeId == (int)ProgramType.FullDegree)
                         select mi).Update(u => { u.PaidStatusTypeId = PaidStatusType.Free; });

            System.Diagnostics.Debug.Print("Count of Free Program Update: " + count);

            //Set everything that isn't free or doesn't allow free to paid status
            count = (from mi in MatchItems.Values
                     join pr in products
                                 on mi.ProductId equals pr.Key
                     where pr.Value.IsFree != true && pr.Value.AllowFree != true && pr.Value.AllowFraid != true
                     select mi).Update(u => { u.PaidStatusTypeId = PaidStatusType.Paid; });

            System.Diagnostics.Debug.Print("Count of Paid Program Update: " + count);

            //go through remaining GS programs    
            var productLevelMatches = MatchItems.Values.Where(m => m.PaidStatusTypeId == null).GroupBy(m => m.ClientRelationProductMappingId);
            count = 0;

            foreach (var productMatch in productLevelMatches)
            {
                if (caps.ContainsKey(productMatch.Key))
                {
                    //get matching product cap
                    Cap c = caps[productMatch.Key];

                    foreach (MatchItemInternal mi in productMatch)
                    {
                        mi.SFProductCode_CR = c.SFProductCode;

                        var childCap = c.Children.Where(e => e.CapType == EntityMeta.PSI && e.EntityIDSet.Contains(mi.PsiId)).FirstOrDefault();

                        if (childCap == null)
                            mi.PaidStatusTypeId = PaidStatusType.Free;
                        else
                        {
                            mi.SFProductCode_PSI = childCap.SFProductCode;

                            if (childCap.LimitType == CapLimitType.Fraid && childCap.IsCurrentlyFree)
                                mi.PaidStatusTypeId = PaidStatusType.Fraid;
                            else if (childCap.LimitType == CapLimitType.Fraid_Hotbutton && childCap.IsCurrentlyFree)
                            {
                                mi.PaidStatusTypeId = PaidStatusType.Fraid;
                                mi.SFProductCode_PSI = SFProductCode.SAB_HB;
                                mi.TemplateId = 345; //TODO: fix this hardcode - this is the required template for FraidHotbutton, otherwise it will go to the default free template
                            }
                            else
                                mi.PaidStatusTypeId = PaidStatusType.Paid;
                        }

                        count++;
                    }
                }
                else
                {
                    //No product cap exists for GS program so set to Free
                    foreach (MatchItemInternal mi in productMatch)
                        mi.PaidStatusTypeId = PaidStatusType.Free;
                }
            }
            System.Diagnostics.Debug.Print("Count of Fraid Program Update: " + count);
#if DEBUG
            count = MatchItems.Values.Where(m => m.PaidStatusTypeId == null).Count();
#endif
            System.Diagnostics.Debug.Print("Count of NULL Paid Status Type: " + count);

            //Need to have fraid hotbutton SAB programs be able to have click capabilities
            var paidAndFraidHotbuttonStatusMatches = MatchItems.Values.Where(m => m.PaidStatusTypeId == PaidStatusType.Paid || (m.PaidStatusTypeId == PaidStatusType.Fraid && m.SFProductCode_PSI == SFProductCode.SAB_HB));

            PaidProgramProducts = new HashSet<int>();

            foreach (var p in paidAndFraidHotbuttonStatusMatches)
            {
                if (p.PaidStatusTypeId == PaidStatusType.Paid)
                    PaidProgramProducts.Add(p.Key);

                if ((p.ClickPrice > 0 && !String.IsNullOrEmpty(p.ClickThroughUrl) &&
                    ((p.ProgramTypeId == (int)ProgramType.FullDegree || p.SFProductCode_PSI == SFProductCode.SAB_CPL || p.SFProductCode_PSI == SFProductCode.SAB_CPC)) ||
                    ( p.ProgramTypeId != (int)ProgramType.FullDegree && p.SFProductCode_PSI.HasValue && p.SFProductCode_PSI.Value != SFProductCode.SAB_CPL && p.SFProductCode_PSI.Value != SFProductCode.SAB_CPC)))
                {
                    p.HasClick = true;
                    this.ClickOnlyProgramProducts.Add(p.Key);
                }

            }

            var sfProductCodeMatches = MatchItems.Values.Where(m => m.SFProductCode_PSI.HasValue);

            foreach (var p in sfProductCodeMatches)
            {
                if (SFProductCodeProgramProducts.ContainsKey((int)p.SFProductCode_PSI.Value))
                    SFProductCodeProgramProducts[(int)p.SFProductCode_PSI.Value].Add(p.Key);
                else
                    SFProductCodeProgramProducts.Add((int)p.SFProductCode_PSI.Value, new HashSet<int> { p.Key });
            }

            //Load Free/Paid/Fraid Key sets
            HashSet<int> FraidProgramProducts = new HashSet<int>();
            HashSet<int> FreeProgramProducts = new HashSet<int>();
            FraidProgramProducts.UnionWith(MatchItems.Values.Where(x => x.PaidStatusTypeId == PaidStatusType.Fraid).Select(x => x.Key).ToList());
            PaidProgramProducts.UnionWith(MatchItems.Values.Where(x => x.PaidStatusTypeId == PaidStatusType.Paid).Select(x => x.Key).ToList());
            FreeProgramProducts.UnionWith(MatchItems.Values.Where(x => x.PaidStatusTypeId == PaidStatusType.Free).Select(x => x.Key).ToList());
            PaidStatusTypeProgramProducts = new Dictionary<int, HashSet<int>>();
            foreach (int id in FraidProgramProducts)
            {
                if (!PaidStatusTypeProgramProducts.ContainsKey((int)PaidStatusType.Fraid))
                    PaidStatusTypeProgramProducts.Add((int)PaidStatusType.Fraid, new HashSet<int> { id });
                else
                    PaidStatusTypeProgramProducts[(int)PaidStatusType.Fraid].Add(id);                
            }

            foreach (int id in PaidProgramProducts)
            {
                if (!PaidStatusTypeProgramProducts.ContainsKey((int)PaidStatusType.Paid))
                    PaidStatusTypeProgramProducts.Add((int)PaidStatusType.Paid, new HashSet<int> { id });
                else
                    PaidStatusTypeProgramProducts[(int)PaidStatusType.Paid].Add(id);
            }

            foreach (int id in FreeProgramProducts)
            {
                if (!PaidStatusTypeProgramProducts.ContainsKey((int)PaidStatusType.Free))
                    PaidStatusTypeProgramProducts.Add((int)PaidStatusType.Free, new HashSet<int> { id });
                else
                    PaidStatusTypeProgramProducts[(int)PaidStatusType.Free].Add(id);
            }
        }

        public void SetFactorValues()
        {
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);
            Dictionary<int, eRPL> eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);
            Dictionary<int, decimal> SABeRPCList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, decimal>>(MatchingCacheItem.SABPSIeRPC);

            foreach (MatchItemInternal mi in MatchItems.Values)
            {
                Cap parentCap;

                if (caps.TryGetValue(mi.ClientRelationProductMappingId, out parentCap))
                {
                    if (!parentCap.Capped)
                    {
                        mi.CapRoom = GetCapPercentFromCap(mi, parentCap, parentCap.CapPercentFromCap);

                        if (mi.CapRoom < 0)
                            mi.CapRoom = 0;
                    }
                    else
                    {
                        mi.CapRoom = 0;
                    }
                    mi.TreatAsMatch1 = parentCap.TreatAsMatch1;
                }

                if (SABeRPCList.ContainsKey(mi.PsiId))
                {
                    mi.eRPL = SABeRPCList[mi.PsiId];
                    mi.CapRoom = 1;

                    //Need the click price to accurately identify sab click customers
                    if (eRPLList.ContainsKey(mi.ProgramProductId))
                        mi.ClickPrice = eRPLList[mi.ProgramProductId].ClickPrice;
                }
                else if (eRPLList.ContainsKey(mi.ProgramProductId))
                {
                    eRPL rpl = eRPLList[mi.ProgramProductId];
                    mi.eRPL = rpl.eRPLFinal;
                    mi.RPL = rpl.RPL;
                    mi.ClickPrice = rpl.ClickPrice;
                }
            }
        }

        public decimal GetCapPercentFromCap(MatchItemInternal matchInternal, Cap cap, decimal parentCapPercentage)
        {
            decimal returnValue = parentCapPercentage;

            if (cap.Children != null && cap.Children.Any())
            {
                foreach (Cap childCap in cap.Children)
                {
                    //Use child % from cap if it has the parent's match interna's PSI id and is not full
                    if (childCap.CapType == EntityMeta.PSI && childCap.EntityIDSet.Contains(matchInternal.PsiId) && childCap.Capped == false)
                    {
                        returnValue = childCap.CapPercentFromCap;
                        break;
                    }
                    //Otherwise, go down an additional level into the tree if child has children.
                    else if (childCap.Children != null && childCap.Children.Any(c => !c.Capped))
                    {
                        returnValue = GetCapPercentFromCap(matchInternal, childCap, returnValue);
                    }
                }
            }

            return returnValue;
        }

        public bool IsCampusOnline(int campusId)
        {
            if (CampusProgramProducts.ContainsKey(campusId))
            {
                HashSet<int> programProducts = new HashSet<int>();
                programProducts.UnionWith(CampusProgramProducts[campusId]);
                programProducts.IntersectWith(OnlineCampusTypeProgramProducts);

                if (programProducts.Count > 0)
                    return true;
                else
                    return false;
            }

            return true;
        }

        public static MatchDatabase LoadMatchDatabase()
        {
            MatchDatabase md = new MatchDatabase();

            PopulateFromDb(ref md, MatchDatabaseDataService.GetAllProgramProductWithApplicationSubjectProd());

            PopulateFromDbThirdParties(ref md, MatchDatabaseDataService.GetAllThirdPartyMatchesProd());

            MatchDatabase mdCached = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            if (mdCached != null) //thus, this is a refresh and not initial load
                md.LoadFinalObjects();

            return md;
        }

        public void LoadFinalObjects()
        {
            LoadContentData();
            //LoadTemplateAssignment();
            SetFactorValues();
            SetPaidStatusTypeAndSFProductCode();
        }

        private static void AddToMatchDatabase(ref MatchDatabase md, MatchItemInternal mi)
        {
            //Key is the ProgramProductId
            int key = mi.Key;

            if (!md.ProductCRPrograms.ContainsKey(mi.ProductId))
                md.ProductCRPrograms.Add(mi.ProductId, new Dictionary<int, HashSet<int>>());

            if (!md.ProductCRPrograms[mi.ProductId].ContainsKey(mi.ClientRelationshipId))
                md.ProductCRPrograms[mi.ProductId].Add(mi.ClientRelationshipId, new HashSet<int> { key });
            else
                md.ProductCRPrograms[mi.ProductId][mi.ClientRelationshipId].Add(key);

            if (mi.ProgramDisplayGroupId.HasValue)
            {
                if (!md.ProgramGroups.ContainsKey(mi.ProgramDisplayGroupId.Value))
                    md.ProgramGroups.Add(mi.ProgramDisplayGroupId.Value, new HashSet<int> { key });
                else
                    md.ProgramGroups[mi.ProgramDisplayGroupId.Value].Add(key);
            }

            md.ProgramProductKeys.Add(key);

            if (!md.ProductProgramProducts.ContainsKey(mi.ProductId))
                md.ProductProgramProducts.Add(mi.ProductId, new HashSet<int> { key });
            else
                md.ProductProgramProducts[mi.ProductId].Add(key);

            if (PDProductIds.Contains(mi.ProductId))
                md.PDProgramProducts.Add(key);

            if (PDPlusGSAndInternationalProductIds.Contains(mi.ProductId))
                md.PDPlusGSAndInternationalProgramProducts.Add(key);

            if (PDPlusGSProductIds.Contains(mi.ProductId))
                md.PDPlusGSProgramProducts.Add(key);

            if (PDPlusInternationalProductIds.Contains(mi.ProductId))
                md.PDPlusInternationalProgramProducts.Add(key);

            // Load Campus -> MatchItem Key Set Dictionary
            if (!md.CampusProgramProducts.ContainsKey(mi.CampusId))
                md.CampusProgramProducts.Add(mi.CampusId, new HashSet<int> { key });
            else
                md.CampusProgramProducts[mi.CampusId].Add(key);

            // Load Institution -> MatchItem Key Set Dictionary
            if (!md.InstitutionProgramProducts.ContainsKey(mi.InstitutionId))
                md.InstitutionProgramProducts.Add(mi.InstitutionId, new HashSet<int> { key });
            else
                md.InstitutionProgramProducts[mi.InstitutionId].Add(key);

            // Load Program -> MatchItem Key Set Dictionary
            if (!md.ProgramProgramProducts.ContainsKey(mi.ProgramId))
                md.ProgramProgramProducts.Add(mi.ProgramId, new HashSet<int> { key });
            else
                md.ProgramProgramProducts[mi.ProgramId].Add(key);

            // Load Program Level -> MatchItem Key Set Dictionary
            if (!md.ProgramLevelProgramProducts.ContainsKey(mi.ProgramLevelId))
                md.ProgramLevelProgramProducts.Add(mi.ProgramLevelId, new HashSet<int> { key });
            else
                md.ProgramLevelProgramProducts[mi.ProgramLevelId].Add(key);

            if (GraduateProgramLevels.Contains(mi.ProgramLevelId))
                md.GraduateProgramProducts.Add(key);

            // Load Program Type -> MatchItem Key Set Dictionary
            if (!md.ProgramTypeProgramProducts.ContainsKey(mi.ProgramTypeId))
                md.ProgramTypeProgramProducts.Add(mi.ProgramTypeId, new HashSet<int> { key });
            else
                md.ProgramTypeProgramProducts[mi.ProgramTypeId].Add(key);

            // Load Category -> MatchItem Key Set Dictionary
            foreach (Tuple<int, int, int, bool> categoryMapping in mi.CategoryMappings)
            {
                if (!md.CategoryProgramProducts.ContainsKey(categoryMapping.Item1))
                    md.CategoryProgramProducts.Add(categoryMapping.Item1, new HashSet<int> { key });
                else
                    md.CategoryProgramProducts[categoryMapping.Item1].Add(key);

                if (!md.SubjectProgramProducts.ContainsKey(categoryMapping.Item2))
                    md.SubjectProgramProducts.Add(categoryMapping.Item2, new HashSet<int> { key });
                else
                    md.SubjectProgramProducts[categoryMapping.Item2].Add(key);

                if (!md.SpecialtyProgramProducts.ContainsKey(categoryMapping.Item3))
                    md.SpecialtyProgramProducts.Add(categoryMapping.Item3, new HashSet<int> { key });
                else
                    md.SpecialtyProgramProducts[categoryMapping.Item3].Add(key);
            }

            if (mi.TemplateId.HasValue && !md.TemplateProgramProducts.ContainsKey(mi.TemplateId.Value))
                md.TemplateProgramProducts.Add(mi.TemplateId.Value, new HashSet<int> { key });
            else if (mi.TemplateId.HasValue)
                md.TemplateProgramProducts[mi.TemplateId.Value].Add(key);
            else
                md.TemplateUnassignedProgramProducts.Add(key);

			if (mi.CampusOptionGroupId.HasValue && !md.CampusOptionGroupProgramProducts.ContainsKey(mi.CampusOptionGroupId.Value))
				md.CampusOptionGroupProgramProducts.Add(mi.CampusOptionGroupId.Value, new HashSet<int> { key });
			else if (mi.CampusOptionGroupId.HasValue)
				md.CampusOptionGroupProgramProducts[mi.CampusOptionGroupId.Value].Add(key);

            if (mi.AdvertiserId.HasValue && !md.AdvertiserProgramProducts.ContainsKey(mi.AdvertiserId.Value))
                md.AdvertiserProgramProducts.Add(mi.AdvertiserId.Value, new HashSet<int> { key });
            else if(mi.AdvertiserId.HasValue)
                md.AdvertiserProgramProducts[mi.AdvertiserId.Value].Add(key);

            if (!md.ClientRelationshipProgramProducts.ContainsKey(mi.ClientRelationshipId))
                md.ClientRelationshipProgramProducts.Add(mi.ClientRelationshipId, new HashSet<int> { key });
            else 
                md.ClientRelationshipProgramProducts[mi.ClientRelationshipId].Add(key);

            // Load Online/Campus Type -> MatchItem Key Set Dictionary
            if ((int)CampusType.Ground == mi.CampusCampusTypeId)
                md.CampusCampusTypeProgramProducts.Add(key);
            else
                md.OnlineCampusTypeProgramProducts.Add(key);

            // Load Hybrid
            if (!md.ProgramIsHybridProgramProducts.ContainsKey(mi.IsHybrid ? 1 : 0))
                md.ProgramIsHybridProgramProducts.Add(mi.IsHybrid ? 1 : 0, new HashSet<int> { key });
            else
                md.ProgramIsHybridProgramProducts[mi.IsHybrid ? 1 : 0].Add(key);

            if (!md.NonProfitProgramProducts.ContainsKey(mi.IsNonProfit ? 1 : 0))
                md.NonProfitProgramProducts.Add(mi.IsNonProfit ? 1 : 0, new HashSet<int> { key });
            else
                md.NonProfitProgramProducts[mi.IsNonProfit ? 1 : 0].Add(key);

            if (!md.MatchItems.ContainsKey(key))
                md.MatchItems.Add(key, mi);

            //SAB Filters

            if (mi.WorkTypeList.Count > 0)
            {
                foreach (var wt in mi.WorkTypeList)
                {
                    if (!md.WorkTypeProgramProducts.ContainsKey(wt))
                        md.WorkTypeProgramProducts.Add(wt, new HashSet<int> { key });
                    else
                        md.WorkTypeProgramProducts[wt].Add(key);
                }
            }

            if (mi.DurationList.Count > 0)
            {
                foreach (var d in mi.DurationList)
                {
                    if (!md.DurationProgramProducts.ContainsKey(d))
                        md.DurationProgramProducts.Add(d, new HashSet<int> { key });
                    else
                        md.DurationProgramProducts[d].Add(key);
                }
            }

            if (mi.TermList.Count > 0)
            {
                foreach (var t in mi.TermList)
                {
                    if (!md.TermProgramProducts.ContainsKey(t))
                        md.TermProgramProducts.Add(t, new HashSet<int> { key });
                    else
                        md.TermProgramProducts[t].Add(key);
                }
            }

            if (mi.PlacementAudienceList.Count > 0)
            {
                foreach (var p in mi.PlacementAudienceList)
                {
                    if (!md.PlacementAudienceProgramProducts.ContainsKey(p))
                        md.PlacementAudienceProgramProducts.Add(p, new HashSet<int> { key });
                    else
                        md.PlacementAudienceProgramProducts[p].Add(key);
                }
            }

            if (mi.TeachAbroadTypeList.Count > 0)
            {
                foreach (var teach in mi.TeachAbroadTypeList)
                {
                    if (!md.TeachAbroadTypeProgramProducts.ContainsKey(teach))
                        md.TeachAbroadTypeProgramProducts.Add(teach, new HashSet<int> { key });
                    else
                        md.TeachAbroadTypeProgramProducts[teach].Add(key);
                }
            }

            if (mi.LanguageId.HasValue)
            {
                if (!md.LanguageProgramProducts.ContainsKey(mi.LanguageId.Value))
                    md.LanguageProgramProducts.Add(mi.LanguageId.Value, new HashSet<int> { key });
                else
                    md.LanguageProgramProducts[mi.LanguageId.Value].Add(key);
            }
        }

        public static void PopulateFromDb(ref MatchDatabase md, Dictionary<int, List<VW_Matching_ProgramProduct>> programs)
        {
            foreach (int key in programs.Keys)
            {
                switch (key)
                {
                    case 1:
                    case 4:
                        //var sabTypeGroups = programs[key].GroupBy(p => new { p.ProgramProductId, p.CategoryId, p.SubjectId, p.SpecialtyId });
                        var sabTypeGroups = programs[key].GroupBy(p => p.ProgramProductId);

                        foreach (var sabTypeProgram in sabTypeGroups)
                        {
                            MatchItemInternal mi = Convert(sabTypeProgram.Select(g => g));
                            AddToMatchDatabase(ref md, mi);
                        }
                        break;
                    default:
                        var otherSabProgTypeGroups = programs[key].GroupBy(p => p.ProgramProductId);

                        foreach (var sabProgram in otherSabProgTypeGroups)
                        {
                            MatchItemInternal mi = Convert(sabProgram.Select(g => g));
                            AddToMatchDatabase(ref md, mi);
                        }
                        break;
                }
            }
        }

        public static void PopulateFromDbThirdParties(ref MatchDatabase md, List<DataModel.Entity.VW_Matching_ThirdPartyMatches> programs)
        {
            foreach (DataModel.Entity.VW_Matching_ThirdPartyMatches program in programs)
            {
                MatchItemInternal mi = Convert(program);

                Base.InstitutionLeadTypes ilt = (Base.InstitutionLeadTypes)program.InstitutionLeadTypeId;

                if (md.ThirdPartyMatches.ContainsKey(ilt))
                {
                    if (md.ThirdPartyMatches[ilt].ContainsKey(program.InstitutionId))
                        md.ThirdPartyMatches[ilt][program.InstitutionId].Add(mi);
                    else
                        md.ThirdPartyMatches[ilt].Add(program.InstitutionId, new List<MatchItemInternal>() { mi });
                }
                else
                {
                    Dictionary<int, List<MatchItemInternal>> tmp = new Dictionary<int, List<MatchItemInternal>>();
                    tmp.Add(program.InstitutionId, new List<MatchItemInternal>() { mi });

                    md.ThirdPartyMatches.Add(ilt, tmp);
                }
            }
        }

        private static MatchItemInternal Convert(IEnumerable<VW_Matching_ProgramProduct> groupItem)
        {
            MatchItemInternal mi = new MatchItemInternal();
            int i = 1;
            VW_Matching_ProgramProduct item = groupItem.First();

            mi.CampusCampusTypeId = item.CampusCampusTypeId;
            mi.CampusId = item.CampusId;
            //mi.CategoryId = item.CategoryId;
            mi.ClientCampusProductMappingId = item.ClientCampusProductMappingId;
            mi.ClientCampusRelationshipId = item.ClientCampusRelationshipId;
            mi.ClientRelationProductMappingId = item.ClientRelationProductMappingId;
            mi.ClientRelationshipId = item.ClientRelationshipId;
            mi.ClientId = item.ClientId;
            if (item.IncludeAllZipCodes.HasValue)
                mi.IncludeAllZipCodes = item.IncludeAllZipCodes.Value;
            mi.InstitutionId = item.InstitutionId;
            mi.ProductId = item.ProductId;
            mi.ProgramCampusTypeId = item.ProgramCampusTypeId;
            mi.ProgramId = item.ProgramId;
            mi.ProgramLevelId = item.ProgramLevelId;
            if (item.ProgramProductAllowableRadius.HasValue)
                mi.ProgramProductAllowableRadius = item.ProgramProductAllowableRadius.Value;
            mi.ProgramProductId = item.ProgramProductId;
            mi.ProgramProductRadiusZipCode = item.ProgramProductRadiusZipCode;
            if (item.ProgramProductUseZipCodeRules.HasValue)
                mi.ProgramProductUseZipCodeRules = item.ProgramProductUseZipCodeRules.Value;
            if (item.ProgramProductZipCodeExclusion.HasValue)
                mi.ProgramProductZipCodeExclusion = item.ProgramProductZipCodeExclusion.Value;
            if (item.ProgramProductZipCodeInclusion.HasValue)
                mi.ProgramProductZipCodeInclusion = item.ProgramProductZipCodeInclusion.Value;
            mi.ProgramTypeId = item.ProgramTypeId;
            mi.PsiId = item.PsiId;
            //mi.SubjectId = item.SubjectId;
            //mi.SpecialtyId = item.SpecialtyId;
            //mi.IsPrimarySubject = item.IsPrimarySubject;
            mi.CanSmartMatch = item.CanSmartMatch;
            mi.IsHybrid = item.IsHybrid;
            mi.InquiryDisabled = item.InquiryDisabled;
            mi.ClickThroughUrl = item.ClickThroughURL;
            mi.ProgramDisplayGroupId = item.ProgramDisplayGroupId;
            mi.RequiresSystemTemplateUse = item.RequiresSystemTemplateUse;
            mi.ShowLeadShare2U = item.ShowLeadShare2U;
            mi.LanguageId = item.LanguageId;
            mi.LeadRefinementEnabled = item.LeadRefinementEnabled;
            mi.SABSRAPosition_CR = item.SABSRAPosition_CR;
            mi.SABSRAPosition_PSI = item.SABSRAPosition_PSI;
            mi.ExcludeMatch1plusForFinAid = item.ExcludeMatch1plusForFinAid;
            mi.IsNonProfit = item.IsNonProfit;
            mi.DegreeAcronym = item.DegreeAcronym;

            //HARDCODE: Added for SAB & GS due to ticket 83365
            if (!item.TemplateId.HasValue && item.ProductId == 4)
                mi.TemplateId = 345;
            else if (!item.TemplateId.HasValue && item.ProductId == 17)
                mi.TemplateId = 230;
            else 
                mi.TemplateId = item.TemplateId;

            mi.AllowCrossSell = item.AllowCrossSell;

            mi.RequireJournayaLeadId = item.RequireJournayaLeadId;
			mi.CampusOptionGroupId = item.CampusOptionGroupId;
            mi.AdvertiserId = item.AdvertiserId;
            mi.UpsellOutboundTitanium = item.UpsellOutboundTitanium;
            mi.CustomTCPA = item.CustomTCPA;
            mi.CustomContactCenterTCPA = item.CustomContactCenterTCPA;

            foreach (var singleItem in groupItem)
            {
                if (singleItem.DurationId.HasValue)
                    mi.DurationList.Add(singleItem.DurationId.Value);
                if (singleItem.WorkTypeId.HasValue)
                    mi.WorkTypeList.Add(singleItem.WorkTypeId.Value);
                if (singleItem.TermId.HasValue)
                    mi.TermList.Add(singleItem.TermId.Value);
                if (singleItem.PlacementAudienceId.HasValue)
                    mi.PlacementAudienceList.Add(singleItem.PlacementAudienceId.Value);
                if (singleItem.TeachAbroadTypeId.HasValue)
                    mi.TeachAbroadTypeList.Add(singleItem.TeachAbroadTypeId.Value);
                if (singleItem.SpecialtyId.HasValue)
                {
                    mi.CategoryMappings.Add(new Tuple<int, int, int, bool>(singleItem.CategoryId.Value, singleItem.SubjectId.Value, singleItem.SpecialtyId.Value, singleItem.IsPrimarySubject));

                    if (singleItem.IsPrimarySubject || i == 1)
                    {
                        mi.PrimaryCategoryId = singleItem.CategoryId.Value;
                        mi.PrimarySpecialtyId = singleItem.SpecialtyId.Value;
                        mi.PrimarySubjectId = singleItem.SubjectId.Value;
                    }
                }

                i++;
            }

            return mi;
        }

        private static MatchItemInternal Convert(DataModel.Entity.VW_Matching_ThirdPartyMatches item)
        {
            MatchItemInternal mi = new MatchItemInternal();

            mi.CampusId = item.CampusId;
            mi.CampusCampusTypeId = item.CampusTypeId;
            mi.ClientCampusProductMappingId = item.ClientCampusProductMappingId;
            mi.ClientCampusRelationshipId = item.ClientCampusRelationshipId;
            mi.ClientRelationProductMappingId = item.ClientRelationProductMappingId;
            mi.ClientRelationshipId = item.ClientRelationshipId;
            mi.InstitutionId = item.InstitutionId;
            mi.ProductId = item.ProductId;
            mi.ProgramId = item.ProgramId;
            mi.ProgramTypeId = item.ProgramTypeId;
            mi.ProgramCampusTypeId = item.CampusTypeId;
            mi.ProgramProductId = item.ProgramProductId;
            mi.PsiId = item.PsiId;
            mi.PaidStatusTypeId = PaidStatusType.Paid;
            mi.ProgramCode = item.ProgramCode;

            Dictionary<int, eRPL> eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

            eRPL rpl;

            if (eRPLList.TryGetValue(item.ProgramProductId, out rpl))
            {
                mi.eRPL = rpl.eRPLFinal;
                mi.RPL = rpl.RPL;
            }

            return mi;
        }
    }
}
