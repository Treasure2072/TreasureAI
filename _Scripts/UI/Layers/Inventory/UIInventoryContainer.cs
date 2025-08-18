using _Scripts.UI.Common.Grids;
using Data;
using UnityEngine;

namespace Game
{
    public class UIInventoryContainer : GridsContainerBase
    {
        #region Function - GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);

            foreach (var (id, count) in PlayerSandbox.Instance.CharacterHandler.Items)
            {
                var grid = SpawnGrid<UIInventoryItem>();
                grid.SetIcon(ShopInstance.Instance.ShopItemSettings.GetShopItemById(id).icon);
                grid.SetNum(count);
            }
        }

        #endregion
    }
}
