using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    /// <summary>
    /// 计时器组件
    /// </summary>
    public class CountdownComponent : MonoBehaviour
    {
        #region Property

        [SerializeField] private float lifeTime;
        [SerializeField] private UnityEvent onEndEvent = new();
        [SerializeField] private UnityEvent<float> onValueChanged = new();

        public event Action<float> OnValueChanged;

        public event Action OnStart;
        public event Action OnEnd;

        public bool IsEnd { get; private set; } = true;
        
        private float EndTime { get; set; }
        private float CurrentTime => Time.unscaledTime;

        #endregion

        #region Unity

        private void FixedUpdate()
        {
            if (!IsEnd)
            {
                onValueChanged.Invoke(GetRemainingTime());
                OnValueChanged?.Invoke(GetRemainingTime());
            }

            if (!IsEnd && GetRemainingTime() <= 0)
            {
                IsEnd = true;
                onEndEvent?.Invoke();
                OnEnd?.Invoke();
            }
        }

        #endregion

        #region Function

        private float GetRemainingTime()
        {
            return EndTime - CurrentTime;
        }

        #endregion

        #region API

        public void StartCountdown()
        {
            IsEnd = false;
            EndTime = CurrentTime + lifeTime;
            OnStart?.Invoke();
        }

        public void StopCountdown()
        {
            IsEnd = true;
            onEndEvent?.Invoke();
            OnEnd?.Invoke();
        }

        #endregion
    }
}