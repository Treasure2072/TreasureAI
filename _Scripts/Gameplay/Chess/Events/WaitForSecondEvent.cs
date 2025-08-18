using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class WaitForSecondEvent : IQueueableEvent
    {
        private float FinishTs { get; set; }

        public WaitForSecondEvent(float delay)
        {
            FinishTs = Time.unscaledTime + delay;
        }

        public virtual void OnQueue() {}
        public virtual void OnExecute() {}
        public virtual void OnDequeue() {}
        public virtual void OnCancel() {}
        public virtual bool OnTick() { return Time.unscaledTime >= FinishTs; }
        public virtual void OnFinish() {}
    }
}
