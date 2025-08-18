using System;
using DragonLi.UI;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformSizeTracker : UIComponent
    {
#if UNITY_EDITOR
        
        private void OnEnable()
        {
            OnInit();
            EditorApplication.update += UpdateInEditorMode;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateInEditorMode;
        }
        
        private void UpdateInEditorMode()
        {
            UpdateListenerSize();
        }
#endif
        
        private RectTransform rectTransform;
        private Vector2 previousSize;
        
        public event Action<Vector2, Vector2> OnSizeChanged;
        public event Action<float, float> SizeXChanged;
        public event Action<float, float> SizeYChanged;



        protected override void OnInit()
        {
            base.OnInit();
            rectTransform = GetComponent<RectTransform>();
            previousSize = rectTransform.sizeDelta;
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying) return;
            UpdateListenerSize();
        }

        private void UpdateListenerSize()
        {
            if(!rectTransform) return;

            if (!Mathf.Approximately(rectTransform.sizeDelta.x, previousSize.x))
            {
                SizeXChanged?.Invoke(previousSize.x, rectTransform.sizeDelta.x);
                OnSizeChanged?.Invoke(previousSize, rectTransform.sizeDelta);
                previousSize = rectTransform.sizeDelta;
            }

            if (!Mathf.Approximately(rectTransform.sizeDelta.y, previousSize.y))
            {
                SizeYChanged?.Invoke(previousSize.y, rectTransform.sizeDelta.y);
                OnSizeChanged?.Invoke(previousSize, rectTransform.sizeDelta);
                previousSize = rectTransform.sizeDelta;
            }
        }
    }
}