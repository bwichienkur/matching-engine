using EDDY.IS.Common.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class CategoryDAO : VendorBaseDAO
    {
        public Entities.Category GetCategory(int categoryId)
        {
            Entities.Category category = null;
            try
            {
                category = this.getAllCategories().Where(c => c.CategoryId == categoryId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return category;
        }
    }
}
