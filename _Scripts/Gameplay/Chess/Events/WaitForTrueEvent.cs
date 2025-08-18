using System;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class WaitForTrueEvent : IQueueableEvent
    {
        #region Properties
        
        private Func<bool> Condition { get; set; }

        #endregion
        
        #region ChessTileEvent

        public WaitForTrueEvent(Func<bool> condition)
        {
            Condition = condition;
        }

        public void OnQueue() { }

        public void OnExecute() { }

        public void OnDequeue() { }

        public void OnCancel() { }
        public bool OnTick()
        {
            return Condition();
        }

        public void OnFinish() { }

        #endregion
    }
}
