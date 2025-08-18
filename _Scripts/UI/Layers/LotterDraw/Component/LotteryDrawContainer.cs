using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using UnityEngine;

namespace Game
{
    public class LotteryDrawContainer : GridsContainerBase
    {
        #region Property

        

        #endregion
        
        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);

            var data = args[0] as List<ELotteryDrawType>;

            foreach (var v in data)
            {
                var grid = SpawnGrid<LotteryDrawSlot>();
                grid.SetGrid(v);
            }

        }

        #endregion

        #region API

        public int GetElementsCount() => Grids.Count;
        
        public GridBase GetGrid(int index) => Grids[index];

        #endregion
    }
}
