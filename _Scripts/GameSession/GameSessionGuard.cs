using System.Collections;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class GameSessionGuard : MonoBehaviour
    {
        #region Properties

        private bool IsGuarding { get; set; } = true;
        private bool ErrorHappened { get; set; }
        private float CancelTime { get; set; }
        private const int kRetryTimes = 3;
        private WaitForSeconds CheckTick { get; set; } = new(1);

        public const string OnStartReconnectEvent = "OnStartReconnectEvent";
        public const string OnReconnectResultEvent = "OnReconnectResultEvent";
        public const string OnReconnectTryEvent = "OnReconnectTryEvent";
        
        #endregion
        
        #region Unity

        private void Start()
        {
            StartCoroutine(ProcessGuarding());
        }

        #endregion

        #region API

        public void StopGuarding()
        {
            IsGuarding = false;
        }

        #endregion
        
        #region Functions

        private IEnumerator ProcessGuarding()
        {
            while (IsGuarding)
            {
                if (GameSessionConnection.Instance.IsConnected())
                {
                    yield return null;
                }
                else
                {
                    yield return StartCoroutine(ProcessReconnect());
                }
            }
        }
        
        private IEnumerator ProcessReconnect()
        {
            EventDispatcher.TriggerEvent(OnStartReconnectEvent);
            EventDispatcher.AddEventListener<EWebSocketErrorType>(GameSessionConnection.ConnectionErrorEvent, OnSessionErrorDuringReconnecting);
            var tries = kRetryTimes;
            while (tries-- > 0)
            {
                ErrorHappened = false;

                yield return null;
                EventDispatcher.TriggerEvent(OnReconnectTryEvent, kRetryTimes - tries, kRetryTimes);
                
                // 尝试重新连接到服务器
                // ...
                if (!GameSessionConnection.Instance.ConnectToServer())
                {
                    // 连接失败
                    yield break;
                }

                // Wait for timeout/error/connected
                // ...
                yield return StartCoroutine(WaitConnection());
                
                // Check if connection is successful
                // ...
                if (GameSessionConnection.Instance.IsConnected())
                {
                    yield break;
                }
            }
            
            EventDispatcher.TriggerEvent(OnReconnectResultEvent, GameSessionConnection.Instance.IsConnected());
            EventDispatcher.RemoveEventListener<EWebSocketErrorType>(GameSessionConnection.ConnectionErrorEvent, OnSessionErrorDuringReconnecting);
        }

        private IEnumerator WaitConnection()
        {
            CancelTime = Time.time + 6;
            while (CancelTime > Time.time || !ErrorHappened || GameSessionConnection.Instance.IsConnected())
            {
                yield return null;
            }
        }

        #endregion

        #region Callbacks

        private void OnSessionErrorDuringReconnecting(EWebSocketErrorType errorType)
        {
            ErrorHappened = true;
        }

        #endregion
    }
}


