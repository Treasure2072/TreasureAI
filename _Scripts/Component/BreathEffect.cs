using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class BreathEffect : MonoBehaviour
    {
        #region Property

        [Header("Reference")]
        [SerializeField] private Transform target;
        
        [Header("Setting")]
        [SerializeField] private bool playOnAwake = false;
        [SerializeField] private float endValue = 1.1f;
        [SerializeField] private float duration = 1f;
        

        private Tween BreathTween { get; set; }

        #endregion
        
        #region Unity

        private void Awake()
        {
            if (playOnAwake)
            {
                SetBreath(playOnAwake);
            }
        }

        #endregion

        #region API

        public void SetBreath(bool isBreath)
        {
            target.localScale = Vector3.one;
            if (BreathTween != null && BreathTween.IsActive())
            {
                BreathTween.Kill();
                BreathTween = null;
            }

            if (isBreath)
            {
                BreathTween = target.DOScale(endValue, duration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        #endregion
    }
}