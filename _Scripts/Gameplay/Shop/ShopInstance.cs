using _Scripts.Data.Shop;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class ShopInstance : Singleton<ShopInstance>
    {
        private ShopItemSettings _shopItemSettings;

        public ShopItemSettings ShopItemSettings
        {
            get
            {
                if (_shopItemSettings == null)
                {
                    _shopItemSettings = Resources.Load<ShopItemSettings>("ShopItemSettings");
                }
                return _shopItemSettings;
            }
            set
            {
                if(_shopItemSettings) return;
                _shopItemSettings = value;
            }
        }
    }

}