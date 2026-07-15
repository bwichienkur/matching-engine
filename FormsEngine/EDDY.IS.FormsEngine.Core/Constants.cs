using EDDY.IS.FormsEngine.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core
{
    public static class Constants
    {
        public static readonly string NON_MILITARY_ID = "126";
        public static readonly int EMS_APPLICATION_ID = 27;

        public static readonly HashSet<int> ProductIdsToBeExcludedFromLogoPresentation = new HashSet<int> { (int)GradSchoolProduct.FREE };
    }
}
