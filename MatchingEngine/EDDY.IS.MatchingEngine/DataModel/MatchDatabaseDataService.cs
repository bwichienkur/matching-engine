using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class MatchDatabaseDataService
    {
        public static HashSet<int> GetAllMatchResponseSearchVendorAllowedLogged()
        {
            HashSet<int> zList = new HashSet<int>();
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_MatchResponseSearchVendorAllowedLogged");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_MatchResponseSearchVendorAllowedLogged v = new VW_Matching_MatchResponseSearchVendorAllowedLogged(dr);

                        zList.Add(v.VendorId);
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

        public static Dictionary<int, List<VW_Matching_ProgramProduct>> GetAllProgramProductWithApplicationSubjectProd()
        {
            Dictionary<int, List<VW_Matching_ProgramProduct>> zList = null;

            SqlDataReader dr = null;
            SqlDataReader dr2 = null;

            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_ProgramProductWithApplicationSubject_Prod");

                zList = new Dictionary<int, List<VW_Matching_ProgramProduct>>();
                
                foreach(var programType in Enum.GetValues(typeof(ProgramType)))
                {
                    zList.Add((int)programType, new List<VW_Matching_ProgramProduct>());
                }

                if (dr.HasRows)
                {                   
                    while (dr.Read())
                    {
                        VW_Matching_ProgramProduct pp = new VW_Matching_ProgramProduct(dr);
                        zList[pp.ProgramTypeId].Add(pp);
                    }
                }

                dr2 = DataHelper.GetDataReader("Prod.VW_Matching_ProgramProductWithSABFilters_Prod");
            
                if (dr2.HasRows)
                {
                    while (dr2.Read())
                    {
                        VW_Matching_ProgramProduct pp = new VW_Matching_ProgramProduct(dr2);
                        zList[pp.ProgramTypeId].Add(pp);
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

                if (dr2 != null)
                {
                    dr2.Close();
                    dr2.Dispose();
                }
            }

            return zList;
        }

        public static List<VW_Matching_ThirdPartyMatches> GetAllThirdPartyMatchesProd()
        {
            List<DataModel.Entity.VW_Matching_ThirdPartyMatches> zList = new List<VW_Matching_ThirdPartyMatches>();

            SqlDataReader dr = null;
            
            try
            {
                dr = DataHelper.GetDataReader("Prod.VW_Matching_ThirdPartyMatches_Prod");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        zList.Add(new VW_Matching_ThirdPartyMatches(dr));
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
