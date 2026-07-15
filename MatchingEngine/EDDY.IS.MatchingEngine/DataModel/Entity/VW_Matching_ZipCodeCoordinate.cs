using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ZipCodeCoordinate
    {
        public int ZipCodeId { get; set; }
        public string ZIPCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public VW_Matching_ZipCodeCoordinate(IDataReader dr)
        {
            ZipCodeId = System.Convert.ToInt32(dr["ZipCodeId"]);
            ZIPCode = System.Convert.ToString(dr["ZIPCode"]);
            Latitude = System.Convert.ToDouble(dr["Latitude"]);
            Longitude = System.Convert.ToDouble(dr["Longitude"]);
        }
    }
}
