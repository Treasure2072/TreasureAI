using _Scripts.UI.Common;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UIStartLayer : UILayer
    {
        #region Property

        public UnityEvent OnHideOperated { get; set; } = new();

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnStartGame"].As<UIBasicButton>().OnClickEvent.AddListener(OnStartGameClick);
        }

        #endregion

        #region API

        public static UIStartLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIStartLayer>(nameof(UIStartLayer));
            Debug.Assert(layer);
            return layer;
        }

        #endregion

        #region Callback

        private void OnStartGameClick(UIBasicButton sender)
        {
            OnHideOperated?.Invoke();
            OnHideOperated?.RemoveAllListeners();
            Hide();
        }

        #endregion
    }
}
