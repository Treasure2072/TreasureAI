using System;
using DG.Tweening;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [RequireComponent(typeof(RectTransform))]
    public class TweenMoveActive : MonoBehaviour
    {
        
        [Header("Settings - Active")]
        public float ActiveDuring = 0.5f;
        public float ActiveDelay = 0.0f;
        public Ease ActiveEaseType = Ease.OutQuart;
        
        [Header("Settings - InsActive")]
        public float InactiveDuring = 0.5f;
        public float InactiveDelay = 0.0f;
        public Ease InactiveEaseType = Ease.InQuart;
        
        [Header("Tween Active")]
        [SerializeField] private Vector2 Insactive;
        [SerializeField] private Vector2 Active;

        private RectTransform RectTrans { get; set; }

        private void Awake()
        {
            RectTrans = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            OnPlayForward();
        }

        private void OnDisable()
        {
            OnPlayBack();
        }

        #region UITweener

        private void OnPlayForward()
        {
            if (!RectTrans) return;
            RectTrans.DOComplete();
            RectTrans.anchoredPosition = Insactive;
            
            var tween = RectTrans.DOAnchorPos(Active, ActiveDuring).SetEase(ActiveEaseType);
            if (ActiveDelay > 0.0f) tween.SetDelay(ActiveDelay);
        }

        private void OnPlayBack()
        {
            if (!RectTrans) return;
            RectTrans.DOComplete();
            RectTrans.anchoredPosition = Active;
            
            var tween = RectTrans.DOAnchorPos(Insactive, InactiveDuring).SetEase(InactiveEaseType);
            if (InactiveDelay > 0.0f) tween.SetDelay(InactiveDelay);
        }

        #endregion
        
#if UNITY_EDITOR
        
        [ContextMenu("Mark As Active")]
        private void MarkAsActiveLocation()
        {
            var rectTrans = GetComponent<RectTransform>();
            Active = rectTrans.anchoredPosition;
        }
        
        [ContextMenu("Mark As Inactive")]
        private void MarkAsHideLocation()
        {
            var rectTrans = GetComponent<RectTransform>();
            Insactive = rectTrans.anchoredPosition;
        }
        
#endif
    }

}