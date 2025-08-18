using System;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class CustomEvent : IQueueableEvent
    {
        public Action Execute { get; }
        public Func<bool> Tick { get; }
        public Action Queue { get; }
        public Action Dequeue { get; }
        public Action Cancel { get; }
        public Action Finish { get; }
        
        public CustomEvent(
            Action execute, 
            Func<bool> tick = null, 
            Action queue = null, 
            Action dequeue = null, 
            Action cancel = null, 
            Action finish = null
            )
        {
            Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            Tick = tick ?? (() => true);
            Queue = queue ?? (() => { });
            Dequeue = dequeue ?? (() => { });
            Cancel = cancel ?? (() => { });
            Finish = finish ?? (() => { });
        }

        public void OnQueue()
        {
            Queue?.Invoke();
        }

        public void OnExecute()
        {
            Execute?.Invoke();
        }

        public void OnDequeue()
        {
            Dequeue?.Invoke();
        }

        public void OnCancel()
        {
            Cancel?.Invoke();
        }

        public bool OnTick()
        {
            return Tick?.Invoke() ?? false;
        }

        public void OnFinish()
        {
            Finish?.Invoke();
        }
    }
}
