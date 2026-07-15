using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class StateGeoCacheItem
    {
        public Dictionary<int, List<int>> ClientRelationProductMappingList { get; private set; }
        public Dictionary<int, List<int>> ClientCampusProductMappingList { get; private set; }
        public Dictionary<int, List<int>> ProgramProductMappingList { get; private set; }

        public StateGeoCacheItem()
        {
            ClientCampusProductMappingList = new Dictionary<int, List<int>>();
            ClientRelationProductMappingList = new Dictionary<int, List<int>>();
            ProgramProductMappingList = new Dictionary<int, List<int>>();
        }
    }
}
