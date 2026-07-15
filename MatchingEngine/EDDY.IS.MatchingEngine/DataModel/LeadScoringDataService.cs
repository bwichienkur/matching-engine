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
    internal class LeadScoringDataService
    {
        public static List<VW_Matching_Product> GetAllProduct()
        {
            List<DataModel.Entity.VW_Matching_Product> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Product");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_Product>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_Product(dr));
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

        public static List<VW_Matching_Application> GetAllApplication()
        {
            List<DataModel.Entity.VW_Matching_Application> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Application");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_Application>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_Application(dr));
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

        public static List<VW_Matching_ApplicationToProduct> GetApplicationToProducts()
        {
            List<DataModel.Entity.VW_Matching_ApplicationToProduct> zList = null;

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ApplicationToProduct");

                if (dr.HasRows)
                {
                    zList = new List<Entity.VW_Matching_ApplicationToProduct>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.VW_Matching_ApplicationToProduct(dr));
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

        public static List<LeadPingLeadScore> GetLeadPingLeadScores()
        {
            List<DataModel.Entity.LeadPingLeadScore> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("LeadPingLeadScore");

                if (dr.HasRows)
                {
                    zList = new List<Entity.LeadPingLeadScore>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.LeadPingLeadScore(dr));
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

        public static List<LeadPingLeadScoreCPL> GetLeadPingLeadScoreCPLs()
        {
            List<DataModel.Entity.LeadPingLeadScoreCPL> zList = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("LeadPingLeadScoreCPL");

                if (dr.HasRows)
                {
                    zList = new List<Entity.LeadPingLeadScoreCPL>();

                    while (dr.Read())
                    {
                        zList.Add(new Entity.LeadPingLeadScoreCPL(dr));
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
