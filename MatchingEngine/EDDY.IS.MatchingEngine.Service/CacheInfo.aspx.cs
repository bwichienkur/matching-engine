using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EDDY.IS.MatchingEngine.Service
{
    public partial class CacheInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //GridView1.DataSource = GetCacheMetadata();
            //GridView1.DataBind();

            Response.Write(GetCacheMetadata());
        }

        public string GetCacheMetadata()
        {
            var sb = new StringBuilder();
            sb.Append("<table class='table-grid' border=1 style='width:100%; table-layout:fixed'><colgroup><col style='width:88%' /><col style='width:12%' /></colgroup><tr><td>Key</td><td>Approximate Object Size (KB)</td></tr>");

            DataTable dt = new DataTable();
            dt.Columns.Add("key", typeof(string));
            dt.Columns.Add("Size(KB)", typeof(string));

            try
            {
                var appCache = HttpRuntime.Cache;

                foreach (DictionaryEntry cachedObject in appCache)
                {
                    object obj = appCache.Get(cachedObject.Key.ToString());

                    long size = 0;
                    decimal sizeKB = 0;


                    //dt.Rows.Add(strApplicationId, cachedObject.Key.ToString(), size);
                    sb.Append(string.Format("<tr><td style='word-wrap:break-word;'>{0}</td><td>{1}</td></tr>", cachedObject.Key.ToString(), sizeKB));
                }
            }
            catch (Exception ex)
            {
                //ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }

            return sb.Append("</table>").ToString();
            //return dt;
        }
    }
}