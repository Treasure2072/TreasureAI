using System;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UILoadingLayer : UILayer
    {
        #region Property

        private UnityEvent HideEvent { get; } = new();

        #endregion

        #region UILayer

        protected override void OnHide()
        {
            base.OnHide();
            HideEvent?.Invoke();
            HideEvent?.RemoveAllListeners();
        }

        #endregion
        
        #region API

        public static UILoadingLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UILoadingLayer>("UILoadingLayer");
            Debug.Assert(layer);
            return layer;
        }

        public void ShowLayer(Action onHideCallback = null, float disappearTime = -1f)
        {
            HideEvent?.RemoveAllListeners();
            if (onHideCallback != null)
            {
                HideEvent?.AddListener(() => onHideCallback?.Invoke());
            }
            Show();

            if (disappearTime < 0) return;

            CoroutineTaskManager.Instance.WaitSecondTodo(Hide, disappearTime);
        }

        public void HideLayer(Action onHideCallback = null)
        {
            HideEvent?.RemoveAllListeners();
            if (onHideCallback != null)
            {
                HideEvent?.AddListener(() => onHideCallback?.Invoke());
            }
            Hide();
        }

        #endregion
    }
}