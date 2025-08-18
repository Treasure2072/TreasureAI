using System;
using _Scripts.UI.Common;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIBigCenterLayer : UILayer
    {
        #region Properties

        private Action<UIBasicButton> OnClickAction { get; set; }

        #endregion
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnCenter"].As<UIBasicButton>().OnClickEvent.AddListener(OnClick);
        }

        #endregion

        #region Function

        private void ShowLayer(Action<UIBasicButton> onClick)
        {
            OnClickAction = onClick;
            Show();
        }

        #endregion

        #region API

        public static void ShowUIBigCenterLayer(string layerName, Action<UIBasicButton> onClick)
        {
            var layer = UIManager.Instance.GetLayer<UIBigCenterLayer>(layerName);
            Assert.IsNotNull(layer);
            layer.ShowLayer(onClick);
        }

        public static void HideUIBigCenterLayer(string layerName)
        {
            var layer = UIManager.Instance.GetLayer<UIBigCenterLayer>(layerName);
            Assert.IsNotNull(layer);
            layer.OnClickAction = null;
            layer.Hide();
        }

        #endregion

        #region Callback

        private void OnClick(UIBasicButton button)
        {
            OnClickAction?.Invoke(button);
        }

        #endregion
    }

}