using System;
using _Scripts.UI.Common;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UIInputLayer : UILayer
    {
        #region Properties

        [Header("References")]
        [SerializeField] private TextMeshProUGUI tmpTitle;
        [SerializeField] private TMP_InputField tmpInputField;

        public event Action<string> OnSubmit;
        public UnityAction<string> OnSubmitAction { get; set; }
        public Func<bool> IsCorrectFormatAction { get; set; } = () => true;

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["ButtonOK"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnOKClickCallback);
            this["ButtonCancel"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCancelClickCallback);
        }

        protected override void OnShow()
        {
            base.OnShow();
            tmpInputField.text = null;
        }

        #endregion

        #region Function - API

        public static UIInputLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIInputLayer>("UIInputLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }

        #endregion

        #region Callabck

        private void OnOKClickCallback(UIBasicButton sender)
        {
            if (IsCorrectFormatAction())
            {
                OnSubmitAction?.Invoke(tmpInputField.text);
                OnSubmit?.Invoke(tmpInputField.text);
            }
            else
            {
                UITipLayer.DisplayTip("Error", "The format of the input is incorrect!");
            }
            
            Hide();
        }

        private void OnCancelClickCallback(UIBasicButton sender)
        {
            Hide();
        }

        #endregion
    }
}