using System.Collections.Generic;
using _Scripts.Data.Shop;
using _Scripts.UI.Common.Grids;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ShopBlueprintContainer : GridsContainerBase
    {
        #region Properties

        private IReadOnlyCollection<ShopDataRaw> Blueprints { get; set; } 
        
        public UnityAction<string> PurchaseAction { get; set; }

        #endregion
        
        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);
            Blueprints = ShopInstance.Instance.ShopItemSettings.GetShopsByType(EShopType.Blueprint);
            foreach (var blueprint in Blueprints)
            {
                var grid = SpawnGrid<UIShopItem>();
                grid.SetGrid(blueprint, PurchaseAction);
            }
        }

        #endregion
    }
}
