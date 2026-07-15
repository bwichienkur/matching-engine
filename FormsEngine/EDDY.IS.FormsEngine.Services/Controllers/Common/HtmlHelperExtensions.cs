using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers.Common
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString InlineScriptBlock<TModel>(this HtmlHelper<TModel> htmlHelper, string path)
        {
            var builder = new TagBuilder("script");
            builder.Attributes.Add("type", "text/javascript");

            var physicalPath = htmlHelper.ViewContext.RequestContext.HttpContext.Server.MapPath(path);
            if (File.Exists(physicalPath))
            {
                builder.InnerHtml = File.ReadAllText(physicalPath);
            }

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}