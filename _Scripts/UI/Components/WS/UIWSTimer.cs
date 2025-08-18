using System;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    
    public class UIWSTimer : MonoBehaviour
    {
        #region Properties

        [Header("Settings")]
        [SerializeField] private Animator animatorRef;
        [SerializeField] private UIAnimatedNumberText timeText;
        [SerializeField] private Image progress;
        
        private Action FinishCallback { get; set; }
        private float RemainingTime { get; set; }
        private float FullTime { get; set; }
        
        /// <summary>
        /// Update是否更新时间
        /// </summary>
        private float LockTime { get; set; }
        private bool Terminating { get; set; }
        private static readonly int AnimatorIDBoost = Animator.StringToHash("Boost");

        #endregion

        #region API

        public void Initialize(int time, int fullTime, Action finishedCallback = null)
        {
            Terminating = false;
            timeText.SetNumberDirectly(time);
            RemainingTime = time;
            FullTime = fullTime;
            FinishCallback = finishedCallback;
        }
        
        public void UpdateTime(int time)
        {
            animatorRef.SetTrigger(AnimatorIDBoost);
            timeText.SetNumber(time);
            RemainingTime = time;
            LockTime = Time.time + timeText.duration;
        }

        public float GetRemainingTime()
        {
            return RemainingTime;
        }

        #endregion

        #region Unreal

        protected void Update()
        {
            if (Terminating)
            {
                return;
            }
            
            if (RemainingTime <= 0)
            {
                SpawnManager.Instance.AddObjectToPool(gameObject, 2.0f);
                FinishCallback?.Invoke();
                FinishCallback = null;
                Terminating = true;
                return;
            }

            RemainingTime -= Time.deltaTime;
            progress.fillAmount = RemainingTime / FullTime;
            if (LockTime > Time.time)
            {
                return;
            }
            
            timeText.SetNumberDirectly((int)RemainingTime);
            // if (RemainingTime <= 0)
            // {
            //     SpawnManager.Instance.AddObjectToPool(gameObject);
            //     FinishCallback?.Invoke();
            // }
        }

        #endregion
    }
}


