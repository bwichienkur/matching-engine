using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class NoMatchModel : BaseModel
    {
        public string Theme { get; set; }
        public string UserFullName { get; set; }
        public bool GenericNoMatch { get; set; }
        
        public NoMatchModel(string theme)
        {
            Theme = string.IsNullOrWhiteSpace(theme) ? "default" : theme;
        }
    }
}