using System;
using System.Collections.Generic;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class ConditionalEvent : IQueueableEvent
    {
        private Func<bool> Condition { get; }
        private Func<List<IQueueableEvent>> TrueEvents { get; }
        private Func<List<IQueueableEvent>> FalseEvents { get; }

        public ConditionalEvent(
            Func<bool> condition, 
            Func<List<IQueueableEvent>> trueEvents = null, 
            Func<List<IQueueableEvent>> falseEvents = null
            )
        {
            Condition = condition;
            TrueEvents = trueEvents;
            FalseEvents = falseEvents;
        }
        
        public virtual void OnQueue() {}
        
        public virtual void OnExecute()
        {
            var events = Condition() ? TrueEvents?.Invoke() : FalseEvents?.Invoke();
            if(events != null && events.Count > 0) 
            {
                EventQueue.Instance.Enqueue(events);
            }
        }

        public virtual void OnDequeue() {} 
        public virtual void OnCancel() {}
        public virtual bool OnTick() { return true; }
        public virtual void OnFinish() {}
    }
}