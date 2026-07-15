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
    internal class LeadScoreReservationDataService
    {
        public static List<VW_Matching_LeadScoreReservationConfiguration> GetLeadScoreReservationConfigurations()
        {
            List<DataModel.Entity.VW_Matching_LeadScoreReservationConfiguration> zList = new List<VW_Matching_LeadScoreReservationConfiguration>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_LeadScoreReservationConfiguration");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_LeadScoreReservationConfiguration>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_LeadScoreReservationConfiguration(dr));
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

        public static List<VW_Matching_LeadScoreReservationTierLevel> GetLeadScoreReservationTierLevels()
        {
            List<DataModel.Entity.VW_Matching_LeadScoreReservationTierLevel> zList = new List<Entity.VW_Matching_LeadScoreReservationTierLevel>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_LeadScoreReservationTierLevel");

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_LeadScoreReservationTierLevel(dr));
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

        public static List<VW_Matching_LeadScoreReservationSupplyUnitDefinition> GetScoreReservationSupplyUnitDefinition()
        {
            List<DataModel.Entity.VW_Matching_LeadScoreReservationSupplyUnitDefinition> zList = new List<VW_Matching_LeadScoreReservationSupplyUnitDefinition>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_LeadScoreReservationSupplyUnitDefinition");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_LeadScoreReservationSupplyUnitDefinition>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_LeadScoreReservationSupplyUnitDefinition(dr));
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
