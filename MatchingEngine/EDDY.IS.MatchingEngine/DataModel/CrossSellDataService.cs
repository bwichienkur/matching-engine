using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class CrossSellDataService
    {
        public static List<VW_Matching_CrossSellProgramLevelMapping> GetAllCrossSellProgramLevelMapping()
        {
            List<DataModel.Entity.VW_Matching_CrossSellProgramLevelMapping> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CrossSellProgramLevelMapping");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_CrossSellProgramLevelMapping>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_CrossSellProgramLevelMapping(dr));
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
