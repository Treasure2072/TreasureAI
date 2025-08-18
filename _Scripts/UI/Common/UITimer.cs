using System;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(UIAnimatedNumberText))]
    public class UITimer : MonoBehaviour
    {
        #region Properties

        private UIAnimatedNumberText TimeText { get; set; }
        private Action FinishCallback { get; set; }
        private float RemainingTime { get; set; }
        
        /// <summary>
        /// Update是否更新时间
        /// </summary>
        private float LockTime { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            TimeText = GetComponent<UIAnimatedNumberText>();
        }

        private void FixedUpdate()
        {
            if (RemainingTime <= 0)
            {
                FinishCallback?.Invoke();
                FinishCallback = null;
                return;
            }
            RemainingTime -= Time.fixedDeltaTime;

            if (LockTime > Time.time)
            {
                return;
            }
            
            TimeText.SetNumberDirectly((int)RemainingTime);
        }

        #endregion

        #region API

        public void Initialize(int time, Action finishCallback = null)
        {
            if (!TimeText)
            {
                TimeText = GetComponent<UIAnimatedNumberText>();
            }
            
            RemainingTime = time;
            FinishCallback = finishCallback;
            TimeText.SetNumberDirectly(time);
        }

        public void UpdateTime(int time)
        {
            RemainingTime = time;
            TimeText.SetNumber(time);
            LockTime = Time.time + TimeText.duration;
        }

        public float GetRemainingTime()
        {
            return RemainingTime;
        }

        #endregion
    }
}