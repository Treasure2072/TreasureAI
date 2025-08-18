using System.Collections.Generic;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class ChessTileBuilding : ChessTile
    {
        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            return new List<IQueueableEvent>
            {

            };
        }

        #endregion
    }

}