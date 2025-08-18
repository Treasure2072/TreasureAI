using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Game
{
    public class ReconnectToServerComponent : MonoBehaviour
    {
        #region Editor

#if UNITY_EDITOR
        
        // void OnGUI ()
        // {
        //     GUI.Box(new Rect(Screen.width - 200 - 10, 10, 200, 100), "Debug");
        //
        //     if(GUI.Button(new Rect(Screen.width - 180 - 20, 40,180,80), "Disconnect"))
        //     {
        //         GameSessionAPI.CharacterAPI.DevDisconnect();
        //     }
        // }
#endif

        #endregion
        
        public static readonly string ReconnectionFailedKey = "RECONNECT_TO_SERVER_FAILED";
        
        #region Properties
        
        [Header("Debug")] 
        [SerializeField] private bool debugMessage = false;

        [Header("Settings - Reconnect to Server")]
        [SerializeField] private int maxReconnectAttempts = 3;
        [SerializeField] private float reconnectDelay = 0.5f;
        [SerializeField] private float reconnectInterval = 2f;
        [SerializeField] private float reconnectDelayMultiplier = 1.5f;
        [SerializeField] private float reconnectIntervalMultiplier = 1.5f;
        
        /// <summary>
        /// 当前重连次数
        /// </summary>
        private int _reconnections;

        private int Reconnections
        {
            get => _reconnections;
            set
            {
                if(_reconnections == value) return;
                OnReconnectionChanged(_reconnections, value);
                _reconnections = value;
            }
        }
        
        private bool ConnectionProcessing { get; set; }
        
        private bool CanCheck { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            EventDispatcher.AddEventListener<bool, string>(GameSessionConnection.ConnectionStatusChangeEvent, OnConnectionStatusChanged);
            
            _reconnections = 0;
            CanCheck = true;
            ConnectionProcessing = false;
            
            var config = Settings.GetConfiguration();
            TextCryptoUtils.SetDefaultVector(config.cryptoVector);
            TextCryptoUtils.SetDefaultPassword(config.cryptoPassword);
            TextCryptoUtils.SetDefaultKey(config.cryptoKey);
        }

        private void Start()
        {
            StartCoroutine(CheckTimeout());
        }

        private void OnDestroy()
        {
            StopCoroutine(CheckTimeout());
            EventDispatcher.RemoveEventListener<bool, string>(GameSessionConnection.ConnectionStatusChangeEvent, OnConnectionStatusChanged);
        }

        #endregion

        #region Function

        private IEnumerator CheckTimeout()
        {
            yield return null;
            
            while (true)
            {
                if (!CanCheck || ConnectionProcessing)
                {
                    yield return CoroutineTaskManager.Waits.TwoSeconds;
                    continue;
                }

                if (!GameSessionConnection.Instance.IsConnected())
                {
                    UIReconnectingLayer.ShowLayer(this.GetLocalizedText("reconnect-start-des"));
                    CanCheck = false;
                    yield return CoroutineTaskManager.Waits.TwoSeconds;
                    while (Reconnections < maxReconnectAttempts && !GameSessionConnection.Instance.IsConnected())
                    {
                        if (!ConnectionProcessing)
                        {
                            ConnectionProcessing = true;
                            yield return Reconnect();
                        }
                    }
                }
                
                yield return CoroutineTaskManager.Waits.TwoSeconds;
            }
        }

        private IEnumerator Reconnect()
        {
            if (GameSessionConnection.Instance.IsConnected())
            {
                CanCheck = true;
                ConnectionProcessing = false;
                UIReconnectingLayer.HideLayer();
                yield break;
            }
            
            Reconnections++;
            
            // 计算当前延迟和间隔
            var currentDelay = reconnectDelay * Mathf.Pow(reconnectDelayMultiplier, Reconnections);
            var currentInterval = reconnectInterval * Mathf.Pow(reconnectIntervalMultiplier, Reconnections);

            this.LogEditorOnly($"重连第 {Reconnections} 次，延迟: {currentDelay} 秒，间隔: {currentInterval} 秒");

            // 等待延迟时间
            yield return new WaitForSeconds(currentDelay);

            GameSessionConnection.Instance.DebugMessage = debugMessage;

            UnityWebRequest.ClearCookieCache();

            Login();

            // 等待当前间隔时间（可用于模拟再次重连的间隔）
            yield return new WaitForSeconds(currentInterval);
        }
        
        private void ConnectionFailed()
        {
            if (Reconnections >= maxReconnectAttempts)
            {
                EventDispatcher.TriggerEvent(ReconnectionFailedKey);
                SceneManager.Instance.AddSceneToLoadQueueByName("StartScene", 2);
                SceneManager.Instance.StartLoad();
                return;
            }

            StartCoroutine(Reconnect());
        }

        #endregion

        #region Callbacks

        private void Login() {

            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var id = PlayerSandbox.Instance.RegisterAndLoginHandler.Id;
            var token = PlayerSandbox.Instance.RegisterAndLoginHandler.Token;
            var result =  GameSessionConnection.Instance.ConnectToServer($"{connection.sessionServer}connect?token={token}&user={id}");
            this.LogEditorOnly("连接服务状态: " + result);
        }

        private void OnConnectionStatusChanged(bool status, string message)
        {
            if (!ConnectionProcessing) return;
            StopCoroutine(Reconnect());
            this.LogEditorOnly("连接服务结束, 成功: " + status);
            if (status)
            {
                Reconnections = 0;
                UIReconnectingLayer.SetContent(this.GetLocalizedText("reconnect-successful"));
                UIReconnectingLayer.HideLayer(2f);
            }
            else
            {
                this.LogErrorEditorOnly(message);
                ConnectionFailed();
                return;
            }
            
            ConnectionProcessing = false;
            CanCheck = true;
        }

        private void OnReconnectionChanged(int preValue, int newValue)
        {
            if (newValue == 0) return;
            
            if (UIReconnectingLayer.GetLayer().IsShowing)
            {
                UIReconnectingLayer.SetContent(newValue, maxReconnectAttempts);
            }
        }

        #endregion
        
    }
}