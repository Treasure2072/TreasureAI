using System;
using _Scripts.UI.Common;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class UIConfirmLayer : UILayer, IPointerClickHandler
    {
        #region Properties
        
        [Header("Settings")] 
        [SerializeField] private TextMeshProUGUI content;

        private Action<bool> OnResult { get; set; }
        
        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            
            this["ButtonOK"].As<UIBasicButton>().OnClickEvent?.AddListener(OnConfirmButtonPressed);
            this["ButtonCancel"].As<UIBasicButton>().OnClickEvent?.AddListener(OnCancelButtonPressed);
        }

        protected override void OnHide()
        {
            base.OnHide();
            OnResult = null;
        }

        #endregion
        
        #region API

        private void SetContents(string contentP, Action<bool> callback)
        {
            content.text = contentP;
            OnResult = callback;
        }
        
        public static void DisplayConfirmation(string content, Action<bool> callback)
        {
            var confirmLayer = UIManager.Instance.GetLayer<UIConfirmLayer>("UIConfirmLayer");
            confirmLayer?.SetContents(content, callback);
            confirmLayer?.Show();
        }

        #endregion

        #region Callbacks

        private void OnConfirmButtonPressed(UIBasicButton sender)
        {
            OnResult?.Invoke(true);
            Hide();
        }
        
        private void OnCancelButtonPressed(UIBasicButton sender)
        {
            OnResult?.Invoke(false);
            Hide();
        }

        #endregion
        
        #region Pointer

        public void OnPointerClick(PointerEventData eventData)
        {
            OnResult?.Invoke(false);
            Hide();
        }

        #endregion
    }
}