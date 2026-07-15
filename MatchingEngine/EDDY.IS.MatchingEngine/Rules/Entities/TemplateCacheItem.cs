//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EDDY.IS.MatchingEngine
//{
//    public class TemplateCacheItem
//    {
//        public Dictionary<int, int> ProgramProductToTemplateAssignments { get; set; }
//        public HashSet<int> UnassignedProgramProducts { get; set; }

//        public TemplateCacheItem()
//        {
//            ProgramProductToTemplateAssignments = new Dictionary<int, int>();
//            UnassignedProgramProducts = new HashSet<int>();
//        }

//        public bool LoadUnassignedProgramProducts()
//        {
//            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

//            if (md == null || md.ProgramProductKeys.Count == 0)
//                return false;

//            HashSet<int> boundProgramProducts = new HashSet<int>(ProgramProductToTemplateAssignments.Keys);
//            HashSet<int> allProgramProducts = new HashSet<int>(md.ProgramProductKeys);

//            allProgramProducts.ExceptWith(boundProgramProducts);
//            UnassignedProgramProducts = allProgramProducts;

//            return true;
//        }
//    }
//}
