using EDDY.IS.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.DTO
{
    public class AllocationWorkingData
    {        
        public SortedDictionary<int, gsNode> DrupalNodes { get; set; }
        public Dictionary<int, eRPL> eRPLList { get; set; }
    }
}
