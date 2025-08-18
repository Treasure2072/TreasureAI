using System;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [ExecuteInEditMode]
    public class RectSizeToTarget : UIComponent
    {
        [SerializeField] private RectTransformSizeTracker tracker;
        
        private RectTransform RectTrans { get; set; }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (!tracker) return;
            tracker.SizeYChanged += SizeYChanged;
            OnInit();
        }

        private void OnDisable()
        {
            if (!tracker) return;
            tracker.SizeYChanged -= SizeYChanged;
        }
#endif

        private void Awake()
        {
            if (!tracker) return;
            tracker.SizeYChanged += SizeYChanged;
        }

        private void OnDestroy()
        {
            if (!tracker) return;
            tracker.SizeYChanged -= SizeYChanged;
        }

        protected override void OnInit()
        {
            base.OnInit();
            this.LogEditorOnly("RectSizeToTarget init");
            RectTrans = GetComponent<RectTransform>();
        }

        private void SizeYChanged(float preVal, float newVal)
        {
            RectTrans.sizeDelta = RectTrans.sizeDelta.x * Vector2.right + Vector2.up * newVal;
        }
    }
}