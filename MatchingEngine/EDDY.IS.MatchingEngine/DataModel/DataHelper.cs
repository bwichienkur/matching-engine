using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel
{
    public class DataHelper
    {
        public static SqlDataReader GetDataReader(string viewName)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Nexus"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + viewName, conn);
            cmd.CommandTimeout = 300;

            cmd.Connection.Open();
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public static SqlDataReader GetDataReader(string viewName, string filter)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Nexus"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + viewName + " WHERE " + filter, conn);
            cmd.CommandTimeout = 300;

            cmd.Connection.Open();
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        public static SqlDataReader GetDataReaderFromSp(string filter)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Nexus"].ConnectionString);
            SqlCommand cmd = new SqlCommand("dbo.GetEMSDuplicateInfoByEmail", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = filter;
            cmd.CommandTimeout = 500;

            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
