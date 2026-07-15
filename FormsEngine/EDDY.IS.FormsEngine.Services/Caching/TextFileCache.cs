using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Caching
{
    [Serializable]
    public class TextFileCache
    {
        public string FileStream { get; set; }
        public string FileStreamMinified { get; set; }
        public DateTime LastModified { get; set; }
    }
}