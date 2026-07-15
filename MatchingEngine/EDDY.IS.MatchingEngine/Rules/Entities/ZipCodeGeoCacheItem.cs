using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class ZipCodeGeoCacheItem
    {
        public ZipCodeCacheItem ClientRelationProductMappingList { get; private set; }
        public ZipCodeCacheItem ClientCampusProductMappingList { get; private set; }
        public ZipCodeCacheItem ProgramProductMappingList { get; private set; }

        public ZipCodeGeoCacheItem()
        {
            ClientCampusProductMappingList = new ZipCodeCacheItem();
            ClientRelationProductMappingList = new ZipCodeCacheItem();
            ProgramProductMappingList = new ZipCodeCacheItem();
        }
    }

    public class ZipCodeCacheItem
    {
        public List<int> ExclusionList { get; private set; }
        public List<int> InclusionList { get; private set; }

        public ZipCodeCacheItem()
        {
            ExclusionList = new List<int>();
            InclusionList = new List<int>();
        }
    }
}
