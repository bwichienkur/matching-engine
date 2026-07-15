using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Controllers.Common
{
    [Serializable]
    public class DataBindResultItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public string Group { get; set; }
        public bool Selected { get; set; }
    }
}