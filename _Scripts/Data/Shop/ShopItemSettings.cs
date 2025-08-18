using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Data.Shop
{
    [CreateAssetMenu(fileName = "ItemSettings", menuName = "Scriptable Objects/ItemSettings")]
    public class ShopItemSettings : ScriptableObject
    {
        [SerializeField] private List<ShopDataRaw> shops;
    
        public IReadOnlyCollection<ShopDataRaw> GetShops() => shops.AsReadOnly();

        public IReadOnlyCollection<ShopDataRaw> GetShopsByType(EShopType type)
        {
            var res = new List<ShopDataRaw>();
            for (var i = 0; i < GetShops().Count; i++)
            {
                if (shops[i].shopType == type)
                {
                    res.Add(shops[i]);
                }
            }
            return res.AsReadOnly();
        }

        public ShopDataRaw GetShopItemById(string id)
        {
            for (var i = 0; i < GetShops().Count; i++)
            {
                if (shops[i].id == id)
                {
                    return shops[i];
                }
            }

            return default;
        }
    }

    [System.Serializable]
    public struct ShopDataRaw
    {
        public string id;
        public Sprite icon;
        public EShopType shopType;
        public long count;
        public long coin;
        public float money;
    }

    [System.Serializable]
    public enum EShopType
    {
        Item,
        Blueprint
    }
}