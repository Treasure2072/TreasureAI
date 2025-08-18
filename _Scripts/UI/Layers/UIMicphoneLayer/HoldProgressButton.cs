using System;
using DG.Tweening;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class HoldProgressButton : UIComponent, IPointerDownHandler, IPointerUpHandler
    {

        #region Property
        
        [SerializeField] private float duration;

        private bool Press { get; set; }
        private Image Bar { get; set; }
        private float Timer  { get; set; }
        
        public event Action Pressed;
        public event Action Released;
        
        #endregion

        private void Awake()
        {
            Bar = GetComponent<Image>();
        }

        private void Update()
        {
            if (Press)
            {
                SetFillAmount((Time.unscaledTime - Timer) / duration);
            }
        }

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            Press = false;
        }

        public override void OnShow()
        {
            base.OnShow();
            SetFillAmount(0);
        }

        #endregion

        private void SetFillAmount(float value)
        {
            if(!Bar) return;
            Bar.fillAmount = value;
        }
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(Press) return;
            Press = true;
            Timer = Time.unscaledTime;
            transform.DOScale(Vector3.one * 1.2f, 0.5f ).SetEase(Ease.OutBounce);
            Pressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!Press) return;
            Press = false;
            transform.DOScale(Vector3.one, 0.5f ).SetEase(Ease.OutBounce);
            Released?.Invoke();
        }
    }

}