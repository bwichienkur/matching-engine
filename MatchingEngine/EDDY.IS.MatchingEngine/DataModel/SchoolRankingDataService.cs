using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class SchoolRankingDataService
    {
        public static int GetWeightSubjectIdFromDatabase(BusinessModelTestCacheItem item)
        {
            int finalBusinessModelWeightSubjectId = item.WeightSubjectId;

            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Nexus"].ConnectionString))
                {

                    SqlParameter param1 = new SqlParameter("@TrackingDeviceGUID", item.TrackingDeviceGuid);
                    SqlParameter param2 = new SqlParameter("@BusinessModelId", item.BusinessModelId);
                    SqlParameter param3 = new SqlParameter("@BusinessModelWeightSubjectId", item.WeightSubjectId);
                    SqlParameter param4 = new SqlParameter() { ParameterName = "@FoundBusinessModelWeightSubjectId", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

                    using (SqlCommand cmd = new SqlCommand("EDDY_ME_BusinessModelTestCache_Selectsert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        cmd.Parameters.Add(param4);

                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();

                        finalBusinessModelWeightSubjectId = Convert.ToInt32(param4.Value);
                    }
                }
            }
            catch(Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            return finalBusinessModelWeightSubjectId;
        }

        public static List<VW_Matching_ProgramProductStrategic> GetAllStrategic()
        {
            List<DataModel.Entity.VW_Matching_ProgramProductStrategic> zList = new List<Entity.VW_Matching_ProgramProductStrategic>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramProductStrategic");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramProductStrategic(dr));
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

        public static List<VW_Matching_ProgramProductRPL> GetAllProgramProductRPL()
        {
            List<DataModel.Entity.VW_Matching_ProgramProductRPL> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramProductRPL");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ProgramProductRPL>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ProgramProductRPL(dr));
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

        public static List<VW_Matching_SABPSIeRPC> GetAllSABPSIeRPC()
        {
            List<DataModel.Entity.VW_Matching_SABPSIeRPC> zList = new List<Entity.VW_Matching_SABPSIeRPC>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_SABPSIeRPC");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_SABPSIeRPC(dr));
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

        public static Dictionary<int, VW_Matching_ProgramProductScrubRate> GetAllProgramProductScrubRate()
        {
            Dictionary<int, VW_Matching_ProgramProductScrubRate> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramProductScrubRate");

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, VW_Matching_ProgramProductScrubRate>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramProductScrubRate item = new Entity.VW_Matching_ProgramProductScrubRate(dr);

                        if (zList.ContainsKey(item.ProgramProductId))
                            zList[item.ProgramProductId] = item;
                        else
                            zList.Add(item.ProgramProductId, item);
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

        public static List<VW_Matching_BusinessModelWeightSubject> GetAllBusinessModelWeightSubject()
        {
            List<DataModel.Entity.VW_Matching_BusinessModelWeightSubject> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_BusinessModelWeightSubject");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_BusinessModelWeightSubject>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_BusinessModelWeightSubject(dr));
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

        public static List<VW_Matching_BusinessModelCriteriaGroup> GetAllBusinessModelCriteriaGroup()
        {
            List<DataModel.Entity.VW_Matching_BusinessModelCriteriaGroup> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_BusinessModelCriteriaGroup");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_BusinessModelCriteriaGroup>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_BusinessModelCriteriaGroup(dr));
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

        public static Dictionary<int, Dictionary<int,VW_Matching_ProgramProductPriceByState>> GetAllProgramProductPriceByState()
        {
            Dictionary<int, Dictionary<int, VW_Matching_ProgramProductPriceByState>> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ProgramProductPriceByState");

                if (dr.HasRows)
                {
                    zList = new Dictionary<int, Dictionary<int, VW_Matching_ProgramProductPriceByState>>();

                    while (dr.Read())
                    {
                        VW_Matching_ProgramProductPriceByState item = new Entity.VW_Matching_ProgramProductPriceByState(dr);

                        if (zList.ContainsKey(item.ProgramProductId))
                        {
                            if (!zList[item.ProgramProductId].ContainsKey(item.StateId))
                                zList[item.ProgramProductId].Add(item.StateId, item);
                        }
                        else
                            zList.Add(item.ProgramProductId, new Dictionary<int, VW_Matching_ProgramProductPriceByState>() { { item.StateId, item } });
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
