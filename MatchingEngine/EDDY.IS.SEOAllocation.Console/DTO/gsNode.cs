using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.DTO
{
    public class gsNode
    {
        public List<int> campus_type { get; set; }
        public List<int> level { get; set; }
        public List<int> category { get; set; }
        public List<int> subject { get; set; }
        public List<int> specialty { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string changed { get; set; }
    }
}
