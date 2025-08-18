using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using Data;
using Ga;

namespace Game
{
    public class OrdersContainer : GridsContainerBase
    {
        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);
            var orders = args[0] as List<FPayment>;
            if (orders == null) return;
            foreach (var data in orders)
            {
                var slot = SpawnGrid<OrdersGrid>();
                slot.SetGrid(data);
            }
        }

        #endregion
    }
}