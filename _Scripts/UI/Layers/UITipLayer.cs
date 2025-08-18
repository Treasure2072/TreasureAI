using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class UITipLayer : UILayer, IPointerClickHandler
    {
        #region Define

        public enum ETipType
        {
            Normal,
            Bad,
            Good
        }

        #endregion
        
        #region Properties

        [Header("Settings")] 
        [SerializeField] private Color normalColor;
        [SerializeField] private Color badColor;
        [SerializeField] private Color goodColor;
        
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI content;
        
        private float DisappearTime { get; set; } = 0;

        private UnityEvent HideOperation { get; set; } = new();
        
        public List<IQueueableEvent> OnHideEvents { get; private set; } = new();
        
        #endregion

        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            OnHideEvents.Clear();
            DisappearTime = Time.unscaledTime + 1;
        }

        protected override void OnHide()
        {
            base.OnHide();
            EventQueue.Instance.Enqueue(OnHideEvents);
            if (HideOperation != null)
            {
                HideOperation.Invoke();
                HideOperation.RemoveAllListeners();
            }
        }

        #endregion
        
        #region API

        private void SetContents(string titleP, string contentP, ETipType tipType = ETipType.Normal)
        {
            this.title.text = titleP;
            this.content.text = contentP;
            background.color = tipType switch
            {
                ETipType.Normal => normalColor,
                ETipType.Bad => badColor,
                ETipType.Good => goodColor,
                _ => background.color
            };
        }

        public static UITipLayer GetLayer()
        {
            var tipLayer = UIManager.Instance.GetLayer<UITipLayer>("UITipsLayer");
            Debug.Assert(tipLayer);
            return tipLayer;
        }
        
        public static void DisplayTip(string title, string content, ETipType tipType = ETipType.Normal, UnityAction hideCallback = null)
        {
            var tipLayer = GetLayer();
            if (hideCallback != null)
            {
                tipLayer.HideOperation.AddListener(hideCallback);
            }
            tipLayer?.SetContents(title, content, tipType);
            tipLayer?.Show();
        }

        #endregion

        #region Pointer

        public void OnPointerClick(PointerEventData eventData)
        {
            if (DisappearTime > Time.unscaledTime)
            {
                return;
            }
            
            Hide();
        }

        #endregion
    }
}


