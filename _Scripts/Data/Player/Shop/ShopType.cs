using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public static class ShopType
    {
        public static FShopInfo GetShopInfoById(this List<FShopInfo> infos, string id)
        {
            if (infos == null)
            {
                Debug.LogError($"shop's data is null!!!");
                return default;
            }
            return (from info in infos where info.id == id select info).FirstOrDefault();
        }
    }

    [System.Serializable]
    public struct FShopInfo
    {
        public string id;
        public decimal price;
        public decimal coinPrice;
        public string item;
        public int count;
    }
}