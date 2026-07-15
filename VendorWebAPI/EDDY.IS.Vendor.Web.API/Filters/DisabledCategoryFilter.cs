using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class DisabledCategoryFilter : CategoryFilter
    {
        protected override void PerformValidation(string categoryIds, HttpActionContext actionContext, ref VendorResponseBase responseContent)
        {
            base.PerformValidation(categoryIds, actionContext, ref responseContent);
            if (!string.IsNullOrEmpty(categoryIds))
            {
                var categoryIdsArray = JsonConvert.DeserializeObject<string[]>(categoryIds);
                if (categoryIdsArray != null)
                {
                    foreach (string categoryIdString in categoryIdsArray)
                    {
                        int categoryId;
                        if (int.TryParse(categoryIdString, out categoryId))
                        {
                            var category = Categories.GetCategory(categoryId);
                            var categoryAvailable = category == null ? false : category.IsEnabled;
                            if (!categoryAvailable)
                            {
                                responseContent.MessageCodes.Add(InputValidation.MessageCodes.CategoryUnavailable);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}