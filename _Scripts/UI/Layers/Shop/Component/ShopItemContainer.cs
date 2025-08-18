using System.Collections.Generic;
using _Scripts.Data.Shop;
using _Scripts.UI.Common.Grids;
using Data;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ShopItemContainer : GridsContainerBase
    {
        #region Properties
        
        public UnityAction<string> PurchaseAction { get; set; }

        #endregion

        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);

            List<FShopInfo> shopInfos = PlayerSandbox.Instance.ChessBoardHandler.Shops;
            foreach (var info in shopInfos)
            {
                var grid = SpawnGrid<UIShopItem>();
                grid.SetGrid(info, PurchaseAction);
            }
        }

        #endregion
    }
}