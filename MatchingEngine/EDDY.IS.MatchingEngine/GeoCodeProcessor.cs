using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Base.Util;
using System.Text.RegularExpressions;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine
{
    public class GeoCodeProcessor : Engine
    {
        private readonly double _earthRadiusInMiles = 3959;

        public GeoCodeProcessor()
        { }

        public GeoCodeProcessor(EDDY.IS.Core.Logging.PerformanceLog perf)
            : base(perf)
        {  }

        private Dictionary<string, ZipCodeCoordinate> _zipCodeCoordinateList;

        private Dictionary<string, ZipCodeCoordinate> ZipCodeCoordinateList
        {
            get
            {
                if (_zipCodeCoordinateList == null)
                    _zipCodeCoordinateList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<string, ZipCodeCoordinate>>(MatchingCacheItem.ZipCodeCoordinates);

                return _zipCodeCoordinateList;
            }
        }

        private double RadianToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreesToRadians(double angle)
        {
            return angle * (Math.PI / 180.0);
        }

        public static Dictionary<string,ZipCodeCoordinate> GetAllZipCodeCoordinates()
        {
            Dictionary<string, ZipCodeCoordinate> zipCodes = new Dictionary<string, ZipCodeCoordinate>();

            List<VW_Matching_ZipCodeCoordinate> databaseZipCodes = GeoCodeDataService.GetAllZipCodeCoordinate();

            if (databaseZipCodes != null)
            {
                foreach (var zipItem in databaseZipCodes.DistinctBy(z => z.ZIPCode))
                {
                    ZipCodeCoordinate singleZip = new ZipCodeCoordinate();

                    if (!zipCodes.ContainsKey(zipItem.ZIPCode))
                    {
                        singleZip.ZipCode = zipItem.ZIPCode;
                        singleZip.Latitude = zipItem.Latitude;
                        singleZip.Longitude = zipItem.Longitude;
                        singleZip.ZipCodeId = zipItem.ZipCodeId;

                        zipCodes.Add(singleZip.ZipCode, singleZip);
                    }
                    else
                    {
                        try
                        {
                            Exception ex = new Exception(String.Format("GetAllZipCodeCoordinates duplicate key ZIPCode = {0}", zipItem.ZIPCode));
                            ISException isEx = new ISException(ex);
                            isEx.Save();
                        }
                        catch { }
                    }
                }
            }

            return zipCodes;
        }

        public static ZipCodeCoordinate GetZipCodeCoordinate(string zipCode)
        {
            Dictionary<string, ZipCodeCoordinate> ZipCodeCoordinateList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<string, ZipCodeCoordinate>>(MatchingCacheItem.ZipCodeCoordinates);

            if (IsValidZipCode(zipCode))
            {
                ZipCodeCoordinate coord;

                if(ZipCodeCoordinateList.TryGetValue(zipCode, out coord))
                    return coord;
                else
                    return null;
            }
            else
                return null;
        }

        public List<string> GetZipCodesWithinRadius(string zipCode, double radiusInMiles)
        {
            List<string> zipCodeList = new List<string>();

            var coord = ZipCodeCoordinateList.Where(z => z.Key == zipCode).FirstOrDefault();

            if (!coord.Equals(default(KeyValuePair<string, ZipCodeCoordinate>)))
            {
                double latitude = coord.Value.Latitude;
                double longitude = coord.Value.Longitude;

                double maxLatitude = latitude + RadianToDegrees(radiusInMiles / _earthRadiusInMiles);
                double minLatitude = latitude - RadianToDegrees(radiusInMiles / _earthRadiusInMiles);

                double maxLongitude = longitude + RadianToDegrees(radiusInMiles / _earthRadiusInMiles / Math.Cos(DegreesToRadians(latitude)));
                double minLongitude = longitude - RadianToDegrees(radiusInMiles / _earthRadiusInMiles / Math.Cos(DegreesToRadians(latitude)));


                var subSet = from z in ZipCodeCoordinateList
                             where (z.Value.Latitude > minLatitude && z.Value.Latitude < maxLatitude)
                                 && (z.Value.Longitude > minLongitude && z.Value.Longitude < maxLongitude)
                             select z;

                var finalSet = from z in subSet
                               where (Math.Acos(Math.Sin(DegreesToRadians(latitude)) * Math.Sin(DegreesToRadians(z.Value.Latitude)) + Math.Cos(DegreesToRadians(latitude)) * Math.Cos(DegreesToRadians(z.Value.Latitude)) * Math.Cos(DegreesToRadians(z.Value.Longitude) - DegreesToRadians(longitude))) * _earthRadiusInMiles) < radiusInMiles
                               select z;
                               
                               //select new { coord = z, distance = (Math.Acos(Math.Sin(DegreesToRadians(latitude)) * Math.Sin(DegreesToRadians(z.Value.Latitude)) + Math.Cos(DegreesToRadians(latitude)) * Math.Cos(DegreesToRadians(z.Value.Latitude)) * Math.Cos(DegreesToRadians(z.Value.Longitude) - DegreesToRadians(longitude))) * _earthRadiusInMiles) };

                zipCodeList = finalSet.Select(z => z.Value.ZipCode).ToList();
            }

            return zipCodeList;
        }

        public List<MatchItem> ExecuteGeoFilter(List<MatchItem> matchItemList, GeoTarget geoInfo, int applicationId, Campaign campaign, int? maxResultCount)
        {
            StartLogDetail("ExecuteGeoFilter");

            List<MatchItem> filteredMatchItemList = matchItemList;

            if (geoInfo.PostalCode != null)
                geoInfo.PostalCode = geoInfo.PostalCode.Trim();

            bool filterZip = ValidateGeoInput(geoInfo);
            bool filterCountry = (geoInfo.CountryList != null && geoInfo.CountryList.Count > 0) ? true : false;
            bool filterState = (geoInfo.StateList != null && geoInfo.StateList.Count > 0) ? true : false;
            bool filterCity = (geoInfo.CityList != null && geoInfo.CityList.Count > 0) ? true : false;

            List<CampusContent> campusContent = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);
            HashSet<int> filteredCampusIds = new HashSet<int>();
            bool filterExecuted = false;

            if (filterState || filterCountry || filterCity)
            {
                filterExecuted = true;

                if (filterCountry)
                {
                    //filteredCampusIds = new HashSet<int>(campusContent.Where(c => geoInfo.CountryList.Contains(Convert.ToInt32(c.CountryId))).Select(c => c.CampusId));
                    filteredMatchItemList = filteredMatchItemList.Where(mi => mi.Match.ProgramTypeId == (int)ProgramType.FullDegree && geoInfo.CountryList.Contains(mi.Match.CampusCountryId.HasValue ? mi.Match.CampusCountryId.Value : 0)).ToList();

                    if (applicationId == 20)
                    {
                        foreach (var countryId in geoInfo.CountryList)
                        {
                            filteredMatchItemList.AddRange(matchItemList.Where(
                                (mi => (mi.Match.ProgramTypeId != (int)ProgramType.FullDegree && mi.Match.ProgramAddressList.Any(pa => countryId == pa.CountryId)) || (mi.Match.CampusCountryId == countryId)
                                )));
                        }

                        //Filtering out program addresses that we don't wants                        
                        foreach (MatchItem mi in filteredMatchItemList)
                        {
                            IList<Address> addressesToKeep = new List<Address>();

                            foreach (Address address in mi.Match.ProgramAddressList)
                            {
                                if ((address.CountryId != null && geoInfo.CountryList.Contains((int)address.CountryId))
                                    || (address.StateId != null && geoInfo.CountryList.Contains((int)address.StateId)))
                                {
                                    addressesToKeep.Add(address);
                                }
                            }

                            mi.FilteredProgramAddressList = addressesToKeep.ToList();
                        }
                    }
                }
                if (filterState)
                {
                    //filteredCampusIds = new HashSet<int>(campusContent.Where(c => geoInfo.StateList.Contains(Convert.ToInt32(c.StateId))).Select(c => c.CampusId));

                    if (applicationId == 7)
                    {
                        var matchItemsToMarkGround = filteredMatchItemList.Where(mi => geoInfo.StateList.Contains(mi.Match.CampusStateId.HasValue ? mi.Match.CampusStateId.Value : 0));

                        MarkInAreaMatchItems(matchItemsToMarkGround.ToList(), filteredMatchItemList);

                        filteredMatchItemList = filteredMatchItemList.Where(mi => mi.IsGeotargeted == true).ToList();
                    }
                    else
                    {
                        filteredMatchItemList = filteredMatchItemList.Where(mi => mi.Match.ProgramTypeId == (int)ProgramType.FullDegree && geoInfo.StateList.Contains(mi.Match.CampusStateId.HasValue ? mi.Match.CampusStateId.Value : 0)).ToList();

                        if (applicationId == 20)
                        {
                            foreach (var stateId in geoInfo.StateList)
                                filteredMatchItemList.AddRange(matchItemList.Where(mi => mi.Match.ProgramTypeId != (int)ProgramType.FullDegree && mi.Match.ProgramAddressList.Any(pa => stateId == pa.StateId)));
                        }
                    }
                }
                if (filterCity)
                {
                    filteredMatchItemList = filteredMatchItemList.Where(mi => mi.Match.ProgramTypeId == (int)ProgramType.FullDegree && geoInfo.CityList.Contains(mi.Match.CampusCityId.HasValue ? mi.Match.CampusCityId.Value : 0)).ToList();

                    if (applicationId == 20)
                    {
                        foreach (var cityId in geoInfo.CityList)
                            filteredMatchItemList.AddRange(matchItemList.Where(mi => mi.Match.ProgramTypeId != (int)ProgramType.FullDegree && mi.Match.ProgramAddressList.Any(pa => cityId == pa.CityId)));
                    }
                }
            }

            if (filterZip)
            {
                //List<CampusContent> campusContent = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);
                HashSet<int> campusIds = new HashSet<int>(matchItemList.Where(mi => mi.Match.CampusCountryId.HasValue
                                                                && (mi.Match.CampusCountryId.Value == 4 || mi.Match.CampusCountryId.Value == 5) //US / Canada                                                                            
                                                         ).DistinctBy(c => c.Match.CampusId).Select(c => c.Match.CampusId));

                filterExecuted = true;
                var distanceList = (from c in campusIds
                                    join cc in campusContent
                                    on c equals cc.CampusId
                                    select new { CountryId = cc.CountryId, CampusId = cc.CampusId, ZipCode = cc.PostalCode, Distance = GetDistanceBetweenZipCodes(geoInfo.PostalCode, cc.PostalCode) }).ToList();

                HashSet<int> campusIdsToRemove = new HashSet<int>(distanceList.Where(dl => dl.Distance == null || dl.Distance > geoInfo.RadiusFromPostalCode).Select(dl => dl.CampusId));

                campusIdsToRemove.UnionWith(matchItemList.Where(mi => !mi.Match.CampusCountryId.HasValue
                                                                      || (mi.Match.CampusCountryId.Value != 4
                                                                      && mi.Match.CampusCountryId.Value != 5)).DistinctBy(c => c.Match.CampusId).Select(c => c.Match.CampusId));

                if (applicationId == 7)
                {
                    var matchItemsToMarkGround = filteredMatchItemList.Where(mi => !campusIdsToRemove.Contains(mi.Match.CampusId));

                    MarkInAreaMatchItems(matchItemsToMarkGround.ToList(), filteredMatchItemList);

                    filteredMatchItemList = filteredMatchItemList.Where(mi => mi.IsGeotargeted == true).ToList();
                }
                else
                    filteredMatchItemList = filteredMatchItemList.Where(mi => !campusIdsToRemove.Contains(mi.Match.CampusId)).ToList();
            }

            if (!filterExecuted)
                filteredMatchItemList = filteredMatchItemList.Where(mi => mi.Match.ProgramCampusTypeId != (int)CampusType.Ground).ToList();
            else
            {
                if (geoInfo.FreeInstitutionCount.HasValue && geoInfo.OnlineInstitutionCount.HasValue && maxResultCount.HasValue)
                {
                    int goodGeoMatchCount = filteredMatchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId == PaidStatusType.Paid && mi.Match.CampusCampusTypeId == (int)CampusType.Ground).DistinctBy(mi => mi.Match.InstitutionId).Count();

                    if (goodGeoMatchCount < maxResultCount.Value)
                    {
                        int freeCount = geoInfo.FreeInstitutionCount.Value;
                        int onlineCount = geoInfo.OnlineInstitutionCount.Value;
                               //2                  3
                        if (goodGeoMatchCount >= onlineCount)
                        {
                            freeCount = 0;
                            onlineCount = maxResultCount.Value - goodGeoMatchCount;
                        }
                        else
                        {
                            //onlineCount = onlineCount;
                            freeCount = maxResultCount.Value - goodGeoMatchCount - onlineCount;
                        }

                        //first get top however many requested unique institution programs from filteredMatchItemList - assign to filteredmatchitemlist
                        var freeMatches = new HashSet<int>(filteredMatchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId != PaidStatusType.Paid).GroupBy(mi => mi.Match.InstitutionId).Take(freeCount).Select(mi => mi.Key));

                        filteredMatchItemList.AddRange(filteredMatchItemList.Where(f => freeMatches.Contains(f.Match.InstitutionId)).ToList());

                        HashSet<int> onlineInstitutionList = new HashSet<int>(matchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId == PaidStatusType.Paid && mi.Match.ProgramCampusTypeId == (int)CampusType.Online).OrderByDescending(mi => mi.ProgramRankScore).GroupBy(mi => mi.Match.InstitutionId).Take(onlineCount).Select(mi => mi.Key));

                        var matchesToAdd = matchItemList.Where(mi => mi.Match.ProgramCampusTypeId == (int)CampusType.Online && mi.Match.PaidStatusTypeId == PaidStatusType.Paid && onlineInstitutionList.Contains(mi.Match.InstitutionId));

                        filteredMatchItemList.AddRange(matchesToAdd);
                    }
                }
                //else if(geoInfo.FreeInstitutionCount.HasValue && !geoInfo.OnlineInstitutionCount.HasValue && maxResultCount.HasValue)
                //{
                //    int passedValidationCount = filteredMatchItemList.Count(m => m.FailedValidation == false && m.Match.PaidStatusTypeId == PaidStatusType.Paid);

                //    if (passedValidationCount > 0 && passedValidationCount < maxResultCount.Value)
                //    {
                //        var freeMatches = filteredMatchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId == PaidStatusType.Free).OrderByDescending(mi => mi.ProgramRankScore).GroupBy(mi => mi.Match.InstitutionId).Take(geoInfo.FreeInstitutionCount.Value - passedValidationCount).Select(mi => mi.First());
                //        filteredMatchItemList.AddRange(freeMatches);
                //    }
                //}
                //For TFS #95780 - Localize National Online Advertiser
                else if (campaign != null && campaign.NumberOfOnlineBackfillOnGeoPages.HasValue && campaign.NumberOfOnlineBackfillOnGeoPages > 0)
                {
                    IEnumerable<int> groundInstitutionList = new List<int>();

                    if (filteredMatchItemList != null && filteredMatchItemList.Any())
                    {
                        var passedValidation = filteredMatchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId == PaidStatusType.Paid);

                        if (passedValidation.Any())
                            groundInstitutionList = passedValidation.GroupBy(mi => mi.Match.InstitutionId).Select(mi => mi.Key);
                    }

                    if (groundInstitutionList.Count() < 10)
                    {
                        HashSet<int> onlineInstitutionList = new HashSet<int>(matchItemList.Where(mi => mi.FailedValidation == false && mi.Match.PaidStatusTypeId == PaidStatusType.Paid && mi.Match.ProgramCampusTypeId == (int)CampusType.Online).OrderByDescending(mi => mi.ProgramRankScore).GroupBy(mi => mi.Match.InstitutionId).Where(mi => !groundInstitutionList.Contains(mi.Key)).Take(campaign.NumberOfOnlineBackfillOnGeoPages.Value).Select(mi => mi.Key));

                        var matchesToAdd = matchItemList.Where(mi => mi.Match.ProgramCampusTypeId == (int)CampusType.Online && mi.Match.PaidStatusTypeId == PaidStatusType.Paid && onlineInstitutionList.Contains(mi.Match.InstitutionId));
                        
                        foreach (var match in matchesToAdd)
                            match.IsNationalOnlineAdvertiser = true;

                        filteredMatchItemList.AddRange(matchesToAdd);
                    }
                }
            }

            EndLogDetail();
            return filteredMatchItemList;
        }

        public static void MarkInAreaMatchItems(List<MatchItem> matchItemsToMarkGround, List<MatchItem> filteredMatchItemList)
        {
            var institutionList = matchItemsToMarkGround.DistinctBy(mi => mi.Match.InstitutionId).Select(mi => mi.Match.InstitutionId).ToList();

            var matchitemsToMarkOnline = filteredMatchItemList.Where(mi => mi.Match.CampusCampusTypeId == (int)CampusType.Online && institutionList.Contains(mi.Match.InstitutionId)).ToList();

            matchItemsToMarkGround.All(mi => { mi.IsGeotargeted = true; return true; });

            matchitemsToMarkOnline.All(mi => { mi.IsGeotargeted = true; return true; });          
        }

        private bool ValidateGeoInput(GeoTarget geoInfo)
        {
            StartLogDetail("ValidateGeoInput");
            
            bool isValid = false;

            if(geoInfo != null)
                if (IsValidZipCode(geoInfo.PostalCode) && geoInfo.RadiusFromPostalCode.HasValue)
                    if (ZipCodeCoordinateList.Where(z => z.Key == geoInfo.PostalCode).FirstOrDefault().Value != default(ZipCodeCoordinate))
                        isValid = true;

            EndLogDetail();
            return isValid;
        }

        public double? GetDistanceBetweenZipCodes(string zipCode1, string zipCode2)
        {
            double? distance = null;

            if (IsValidZipCode(zipCode1) && IsValidZipCode(zipCode2))
            {
                ZipCodeCoordinate coord1 = null;
                ZipCodeCoordinate coord2 = null;

                ZipCodeCoordinateList.TryGetValue(zipCode1, out coord1);
                ZipCodeCoordinateList.TryGetValue(zipCode2, out coord2);

                if (coord1 != default(ZipCodeCoordinate) && coord2 != default(ZipCodeCoordinate))
                    distance = GetDistance(coord1.Latitude, coord1.Longitude, coord2.Latitude, coord2.Longitude);
            }

            return distance;
        }

        private double? GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double? distance = null;

            if (double.IsNaN(latitude1) || double.IsNaN(longitude1) || double.IsNaN(latitude2) || double.IsNaN(longitude2))
            {
                distance = null;
            }
            else
            {
                double latitude = latitude1 * 0.0174532925199433;
                double longitude = longitude1 * 0.0174532925199433;
                double num = latitude2 * 0.0174532925199433;
                double longitude3 = longitude2 * 0.0174532925199433;
                double num1 = longitude3 - longitude;
                double num2 = num - latitude;
                double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
                double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
                double num5 = 6376500 * num4;
                distance = num5 * 0.000621371; //meters to miles
            }

            return distance;
        }

        public static bool IsValidZipCode(string zipCode)
        {
            bool isValid = false;

            if(!String.IsNullOrWhiteSpace(zipCode))
                isValid = Regex.IsMatch(zipCode, @"^\d{5}$", RegexOptions.None);

            if (!String.IsNullOrWhiteSpace(zipCode) && !isValid)
                isValid = Regex.IsMatch(zipCode, @"^([ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ])\ {0,1}(\d[ABCEGHJKLMNPRSTVWXYZ]\d)$", RegexOptions.None);

            return isValid;
        }
    }

    public class ZipCodeCoordinate
    {
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZipCodeId { get; set; }
    }
}
