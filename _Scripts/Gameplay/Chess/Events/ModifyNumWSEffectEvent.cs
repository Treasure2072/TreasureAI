using System;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class ModifyNumWSEffectEvent<T> : IQueueableEvent where T : IConvertible, IFormattable
    {
        #region Properties
        
        private Vector3 Target { get; set; }
        private UIWorldElement WSPrefab { get; set; } 
        private Func<T> GetNumber { get; }

        #endregion
        
        public ModifyNumWSEffectEvent(Vector3 target, UIWorldElement wsPrefab, Func<T> getNumber)
        {
            Target = target;
            WSPrefab = wsPrefab;
            GetNumber = getNumber;
        }

        public virtual void OnQueue() {}
        public void OnExecute()
        {
            if (Convert.ToDouble(GetNumber()) == 0)
            {
                return;
            }
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            var wsCoin = layer.SpawnWorldElement<UIWSNumber>(WSPrefab, Target + Vector3.up * 3.5f);
            wsCoin.SetNumber(GetNumber());
        }
        public virtual void OnDequeue() {}
        public virtual void OnCancel() {}
        public virtual bool OnTick() { return true; }
        public virtual void OnFinish() {}

    }
}