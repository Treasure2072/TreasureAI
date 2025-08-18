using System;
using DG.Tweening;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    // ReSharper disable once InconsistentNaming
    public class UIWSNumber : UIWorldElement
    {
        [Header("Settings")] 
        [SerializeField] private RectTransform animatedRoot;
        [SerializeField] private CanvasGroup animatedGroup;
        [SerializeField] private UIAnimatedNumberText animatedNumber;
        
        [Header("Animation")]
        [SerializeField] private float duration = 3.0f;
        [SerializeField] private float popup = 0.3f;
        [SerializeField] private float fade = 0.3f;

        private void Animate()
        {
            animatedRoot.localPosition = Vector3.zero;
            animatedRoot.DOAnchorPosY(80, duration + fade);

            animatedRoot.localScale = Vector3.one * 0.8f;
            animatedRoot.DOScale(Vector3.one, popup);
            
            animatedGroup.alpha = 0;
            animatedGroup.DOFade(1, popup);
            animatedGroup.DOFade(0, fade).SetDelay(duration);
        }
        
        public void SetNumber<T>(T number) where T : IConvertible, IFormattable
        {
            Animate();
            animatedNumber.SetValue(number);
        }
    }
}


