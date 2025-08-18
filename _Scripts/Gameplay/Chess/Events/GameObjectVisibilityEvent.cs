using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class GameObjectVisibilityEvent : IQueueableEvent
    {
        #region Properties

        private GameObject EffectObject { get; set; }
        
        private float DelayDisappear { get; set; }
        
        #endregion
        
        #region ChessTileEvent

        public GameObjectVisibilityEvent(GameObject effectObject, float delayDisappear = 3f)
        {
            EffectObject = effectObject;
            DelayDisappear = delayDisappear;
        }

        public void OnQueue() { }

        public void OnExecute()
        {
            EffectObject.SetActive(true);
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                EffectObject.SetActive(false);
            }, DelayDisappear);
        }

        public void OnDequeue() { }

        public void OnCancel() { }

        public bool OnTick()
        {
            return true;
        }

        public void OnFinish() { }

        #endregion
    }
}
