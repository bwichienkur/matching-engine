using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Core.Logging.DataModel;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.MatchingEngine.Constants;

namespace EDDY.IS.MatchingEngine
{
    public class CrossSellProcessor : Engine
    {
        public CrossSellProcessor(EDDY.IS.Core.Logging.PerformanceLog pLog)
            : base(pLog)
        {
        }

        public static List<CrossSellMapping> GetAllCrossSellMappings()
        {
            List<CrossSellMapping> xSell = new List<CrossSellMapping>();

            List<VW_Matching_CrossSellProgramLevelMapping> databaseXSell = CrossSellDataService.GetAllCrossSellProgramLevelMapping();

            foreach (var xSellItem in databaseXSell)
            {
                CrossSellMapping singleXSell = new CrossSellMapping();

                singleXSell.EducationLevelId = xSellItem.EducationLevelId;
                singleXSell.InitialProgramLevelId = xSellItem.InitialProgramLevelId;
                singleXSell.SearchProgramLevelId = xSellItem.SearchProgramLevelId;
                singleXSell.Priority = xSellItem.Priority;
                singleXSell.InitialLeadValidMessage = xSellItem.InitialLeadValidMessage;
                singleXSell.InitialLeadInvalidMessage = xSellItem.InitialLeadInvalidMessage;

                xSell.Add(singleXSell);
            }

            return xSell;
        }

        public List<CrossSellMapping> GetSearchProgramLevels(int educationLevelId, int initialProgramLevelId)
        {
            StartLogDetail("GetSearchProgramLevels");

            List<CrossSellMapping> crossSellMapping = StaticCacheProxyHost.CacheProxy.Get<List<CrossSellMapping>>(MatchingCacheItem.CrossSellMapping);

            crossSellMapping = crossSellMapping.Where(csm => csm.EducationLevelId == educationLevelId && csm.InitialProgramLevelId == initialProgramLevelId).ToList();

            EndLogDetail();
            return crossSellMapping;
        }

        public List<CrossSellMapping> GetSearchProgramLevels(int educationLevelId, List<int> initialProgramLevelList)
        {
            StartLogDetail("GetSearchProgramLevels");

            List<CrossSellMapping> crossSellMapping = StaticCacheProxyHost.CacheProxy.Get<List<CrossSellMapping>>(MatchingCacheItem.CrossSellMapping);

            crossSellMapping = crossSellMapping.Where(csm => csm.EducationLevelId == educationLevelId && initialProgramLevelList.Contains(csm.InitialProgramLevelId)).ToList();

            EndLogDetail();
            return crossSellMapping;
        }

        private CrossSellResults GetFullDegreeCrossSells(List<MatchItem> matchItemList, MatchItem programProduct, int? educationLevelId, int? countryId)
        {
            CrossSellResults finalResults = new CrossSellResults();
            finalResults.IsSAB = false;
            //TemplateCacheItem programAssignments = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);

            int? subjectId = null;
            int? categoryId = null;
            CampusType? programCampusType = null;
            int? programLevelId = null;
            int? specialtyId = null;

            if (programProduct != null)
            {
                programLevelId = programProduct.Match.ProgramLevelId;
                programCampusType = (CampusType)programProduct.Match.ProgramCampusTypeId;

                subjectId = programProduct.Match.PrimarySubjectId;
                categoryId = programProduct.Match.PrimaryCategoryId;
                specialtyId = programProduct.Match.PrimarySpecialtyId;
            }
            
            matchItemList = matchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
                                                                                    m.Match.ProductId != (int)ProductType.Match1Exclusive &&
                                                                                    m.Match.ProductId != (int)ProductType.Match1Plus &&
                                                                                    m.Match.ProductId != (int)ProductType.Exclusive &&
                                                                                    m.Match.InquiryDisabled == false &&
                                                                                    m.FailedValidation == false).ToList();

            if (!educationLevelId.HasValue && programLevelId.HasValue)
            {
                finalResults.CrossSellMappingList = new List<CrossSellMapping>();
                
                finalResults.CrossSellMappingList.Add(new CrossSellMapping
                {
                    EducationLevelId = 0,
                    InitialLeadInvalidMessage = null,
                    InitialLeadValidMessage = null,
                    InitialProgramLevelId = programLevelId.Value,
                    Priority = 1,
                    SearchProgramLevelId = programLevelId.Value
                });

                matchItemList = matchItemList.Where(mi => mi.Match.ProgramLevelId == programLevelId.Value).ToList();
            }
            else if (programLevelId.HasValue)
            {
                //get appropriate program levels for cross sell, and add as filter to view
                List<CrossSellMapping> crossSellMapping = this.GetSearchProgramLevels(educationLevelId.Value, programLevelId.Value);

                finalResults.CrossSellMappingList = crossSellMapping;

                if (crossSellMapping != null && crossSellMapping.Count > 0)
                {
                    var programLevelList = crossSellMapping.Select(xs => xs.SearchProgramLevelId);

                    if (programLevelList != null && programLevelList.Count() > 0)
                        matchItemList = matchItemList.Where(mi => programLevelList.Contains(mi.Match.ProgramLevelId)).ToList(); //filter by only allowed program levels from XSell mapping
                }
            }            

            if (programCampusType.HasValue)
            {
                finalResults.TemplateProgramCampusType = programCampusType.Value;
            }

            if(countryId.HasValue && countryId.Value == 4) //Original lead from US, should only cross-sell to US
            {
                matchItemList = matchItemList.Where(mi => mi.Match.CampusCountryId == 4).ToList();
            }

            //matchItemList = FilterBySubjectMappings(matchItemList, categoryId, subjectId, specialtyId);
            //finalResults.MatchItemList = matchItemList.Where(mi => !mi.Match.SFProductCode_PSI.HasValue || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPL || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPC).ToList();
            //List<MatchItem> mainMatchItemList = FilterBySubjectMappings(matchItemList, categoryId, subjectId, specialtyId);
            //finalResults.CategoryMatchItemList = mainMatchItemList.Where(mi => !mi.Match.SFProductCode_PSI.HasValue || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPL || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPC).ToList();

            //TFS-101295 - Create a crosssell backfill list in the event the original crosssell list isnt filled up.
            //Create the list based on category, subject, and specialty mappings.
            finalResults.CategoryMatchItemList = new List<MatchItem>();
            finalResults.SubjectMatchItemList = new List<MatchItem>();
            finalResults.SpecialtyMatchItemList = new List<MatchItem>();
            if (categoryId.HasValue)
            {
                finalResults.CategoryMatchItemList = matchItemList.Where(mi => mi.Match.PrimaryCategoryId == categoryId.Value).ToList();

                if (subjectId.HasValue)
                {
                    finalResults.SubjectMatchItemList = finalResults.CategoryMatchItemList.Where(mi => mi.Match.PrimarySubjectId == subjectId.Value).ToList();
                }

                if (specialtyId.HasValue)
                {
                    if (subjectId.HasValue)
                        finalResults.SpecialtyMatchItemList = finalResults.SubjectMatchItemList.Where(mi => mi.Match.PrimarySpecialtyId == specialtyId.Value).ToList();
                    else
                        finalResults.SpecialtyMatchItemList = finalResults.CategoryMatchItemList.Where(mi => mi.Match.PrimarySpecialtyId == specialtyId.Value).ToList();
                }
            }
            finalResults.CategoryMatchItemList = finalResults.CategoryMatchItemList.Where(mi => !mi.Match.SFProductCode_PSI.HasValue || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPL || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPC).ToList();
            finalResults.SubjectMatchItemList = finalResults.SubjectMatchItemList.Where(mi => !mi.Match.SFProductCode_PSI.HasValue || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPL || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPC).ToList();
            finalResults.SpecialtyMatchItemList = finalResults.SpecialtyMatchItemList.Where(mi => !mi.Match.SFProductCode_PSI.HasValue || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPL || mi.Match.SFProductCode_PSI.Value == SFProductCode.SAB_CPC).ToList();            

            return finalResults;
        }

        private CrossSellResults GetSABCrossSells(List<MatchItem> matchItemList, MatchItem matchItem, List<KeyValuePair<string, int>> kvcodedata)
        {
            CrossSellResults finalResults = new CrossSellResults();
            finalResults.CategoryMatchItemList = new List<MatchItem>();
            finalResults.SubjectMatchItemList = new List<MatchItem>();
            finalResults.SpecialtyMatchItemList = new List<MatchItem>();
            finalResults.IsSAB = true;
            IEnumerable<int> programCountryIds = null;

            matchItemList = matchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
                                                                                    m.Match.InquiryDisabled == false &&
                                                                                    m.FailedValidation == false).ToList();

            if (matchItem.Match.ProgramAddressList != null && matchItem.Match.ProgramAddressList.Any())
                programCountryIds = matchItem.Match.ProgramAddressList.Select(pa => pa.CountryId.Value).Distinct();

            if (kvcodedata != null && kvcodedata.Any(c => c.Key == "CountryOfInterest" || c.Key == "CEA_DestinationOfInterest" || c.Key == "API_DestinationOfInterest"))
            {
                //get the kvcodeid
                int cname = kvcodedata.Where(c => c.Key == "CountryOfInterest" || c.Key == "CEA_DestinationOfInterest" || c.Key == "API_DestinationOfInterest").Select(c => c.Value).FirstOrDefault();
                //get the kvcode item so we can get the value
                VW_Matching_KVCodeDataCache kvCodeData = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_KVCodeDataCache>>(MatchingCacheItem.REKVCodeData)[cname];
                //match the value to a name of a country
                List<int> country = StaticInternalObject.Countries.Where(c => c.Value == kvCodeData.Value).Select(c => c.Key).ToList();
                if(country != null && country.Count > 0) //if we found something use it
                    programCountryIds = country;
            }

            IEnumerable<MatchItem> workingList1 = matchItemList.Where(mi => mi.Match.ProgramTypeId == matchItem.Match.ProgramTypeId && mi.Match.SFProductCode_PSI.HasValue);

            if (programCountryIds != null)
            {
                workingList1 = workingList1.Where(mi => mi.Match.ProgramAddressList != null && mi.Match.ProgramAddressList.Any());
                workingList1 = workingList1.Where(mi => mi.Match.ProgramAddressList.Select(pa => pa.CountryId.Value).Intersect(programCountryIds).Any());
            }

            //TFS-101295 - Create a crosssell backfill list in the event the original crosssell list isnt filled up.
            //Specialty match item list is the new working list
            //finalResults.CategoryMatchItemList = workingList1.ToList();
            finalResults.SpecialtyMatchItemList = workingList1.ToList();
            return finalResults;
        }

        public CrossSellResults ExecuteCrossSellRules(List<MatchItem> matchItemList, int institutionId, MatchItem programProduct, int? educationLevelId, bool initialLeadSuccess, List<KeyValuePair<string, int>> kvcodedata)
        {
            StartLogDetail("ExecuteCrossSellRules");

            CrossSellResults finalResults = new CrossSellResults();

            IEnumerable<MatchItem> tempList = matchItemList;

            //var test = tempList.Where(mi => mi.Match.ProgramId == 297011).ToList();

            //TFS #82820 - only remove the intial lead institution if saved successfully, otherwise just remove the program
            if (initialLeadSuccess)
                tempList = tempList.Where(mi => mi.Match.InstitutionId != institutionId);
            else
                tempList = tempList.Where(mi => mi.Match.ProgramId != programProduct.Match.ProgramId);

            matchItemList = tempList.ToList();

            if (programProduct.Match.ProgramTypeId == 1)
                finalResults = GetFullDegreeCrossSells(matchItemList, programProduct, educationLevelId, programProduct.Match.CampusCountryId);
            else
                finalResults = GetSABCrossSells(matchItemList, programProduct, kvcodedata);

            EndLogDetail();
            return finalResults;
        }
    }

    public class CrossSellResults
    {
        public List<MatchItem> CategoryMatchItemList { get; set; }
        public List<MatchItem> SubjectMatchItemList { get; set; }
        public List<MatchItem> SpecialtyMatchItemList { get; set; }
        public CampusType TemplateProgramCampusType { get; set; }
        public List<CrossSellMapping> CrossSellMappingList { get; set; }        
        public bool IsSAB { get; set; }
    }

    public class CrossSellMapping
    {
        public int EducationLevelId { get; set; }
        public int InitialProgramLevelId { get; set; }
        public int SearchProgramLevelId { get; set; }
        public int Priority { get; set; }
        public string InitialLeadValidMessage { get; set; }
        public string InitialLeadInvalidMessage { get; set; }
    }
}
