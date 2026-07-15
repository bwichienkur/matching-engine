using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class FeatureDataService
    {
        public static List<VW_Matching_Feature> GetAllFeatures()
        {
            List<DataModel.Entity.VW_Matching_Feature> zList = new List<Entity.VW_Matching_Feature>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Feature");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_Feature(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return zList;
        }
    }
}
