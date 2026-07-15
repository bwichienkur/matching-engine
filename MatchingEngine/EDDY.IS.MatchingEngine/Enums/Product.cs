using EDDY.IS.MatchingEngine.DataModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Constants
{
    public enum ProductType
    {
        Premier = 1,
        Select = 2,
        UAB = 3,
        UAB_Free = 4,
        Campus = 8,
        Gold = 9,
        SC = 10,
        CE = 11,
        WarmTransfer = 12,
        TDC = 13,
        ThirdTier = 14,
        WarmTransfer_Silver = 15,
        Premier_Campus = 16,
        Lead_GS = 17,
        Exclusive = 20,
        Match1Exclusive = 48,
        WarmTransferTitanium = 52,
        WarmTransferTitaniumSMP = 72,
        Match1Plus = 71,
        WarmTransferSapphire = 92,
        EPLite = 105,
        BrandElect = 106,
        ThirdPartyAPI = 93,
        LiveTransfer = 117,
        LiveTransfer_Tier2 = 120
    }

    public class Product
    {
        public static readonly HashSet<int> ExclusiveProducts = new HashSet<int> { (int)ProductType.Match1Exclusive, (int)ProductType.Match1Plus };

        public static List<MatchItem> FilterWTMatchItems(bool hasAgentId, List<MatchItem> matchItems)
        {
            if(hasAgentId)
                return matchItems.Where(mi => IsWarmTransferProduct(mi.Match.ProductId) || IsEMSProduct(mi.Match.ProductId)).ToList();
            else
                return matchItems.Where(mi => !IsWarmTransferProduct(mi.Match.ProductId) || IsEMSProduct(mi.Match.ProductId)).ToList();
        }

        public static bool IsWarmTransferProduct(int productId)
        {
            Dictionary<int, VW_Matching_Product> products = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_Product>>(MatchingCacheItem.Products);

            if (products.ContainsKey(productId))
            {
                return products[productId].IsWTAllowed;
            }
            else
            {
                return false;
            }
        }

        public static bool IsEMSProduct(int productId)
        {
            Dictionary<int, VW_Matching_Product> products = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_Product>>(MatchingCacheItem.Products);

            if (products.ContainsKey(productId))
            {
                return products[productId].IsEMSProduct;
            }
            else
            {
                return false;
            }
        }

        public static bool IsWarmTransferSilverProduct(int productId)
        {
            if ((int)ProductType.WarmTransfer_Silver == productId)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}
