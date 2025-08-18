
using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;
using Newtonsoft.Json;
using UnityEngine;


namespace Game
{
    public class GameSessionConnection : MonoSingleton<GameSessionConnection>
    {
        #region Define

        public const string GameSessionName = "GameSession";
        public const string ConnectionStatusChangeEvent = "CONNECTION_STATUS_CHANGE";
        public const string ConnectionErrorEvent = "CONNECTION_ERROR";

        /// <summary>
        /// 三个参数: HttpResponseProtocol response, string Service, string method.
        /// </summary>
        public const string MessageReceivedEvent = "MESSAGE_RECEIVED";

        #endregion

        #region Fields

        [Header("Settings")] 
        [SerializeField] private float timeout = 6.0f;
        // [SerializeField] private int retry = 3;

        #endregion

        #region Properties

        public bool DebugMessage { get; set; }

        #endregion

        #region Unity

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            var platformInterface = WebSocketConnection.Instance.GetInterface(GameSessionName);
            var heartbeat = gameObject.AddComponent<WebSocketHeartBeatBahaviour>();
            heartbeat.SetHeartBeatMessage(JsonConvert.SerializeObject(new HttpRequestProtocol
            {
                type = "heart-beat"
            }));
            platformInterface.AddBehaviour(heartbeat);

            if (DebugMessage)
            {
                var debugBehaviour = gameObject.AddComponent<WebScoketMessagePrintBehaviour>();
                platformInterface.AddBehaviour(debugBehaviour);
            }
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            Close();
        }

        #endregion

        #region API

        public bool ConnectToServer(string endPoint = "")
        {
            var platformInterface = WebSocketConnection.Instance.GetInterface(GameSessionName);
            platformInterface.BindEventReceiver(new GameSessionEventReceiver());
            return platformInterface.ConnectToServer(endPoint);
        }

        public bool IsConnected()
        {
            var platformInterface = WebSocketConnection.Instance.GetInterface(GameSessionName);
            return platformInterface.IsConnected();
        }

        public void Close()
        {
            var platformInterface = WebSocketConnection.Instance.GetInterface(GameSessionName);
            if (!platformInterface.IsConnected()) return;
            
            this.LogEditorOnly("WebSocket连接正在关闭");
            platformInterface.Close();
        }

        public void SendAsync(HttpRequestProtocol protocol)
        {
            var platformInterface = WebSocketConnection.Instance.GetInterface(GameSessionName);
            if (!platformInterface.IsConnected())
            {
                this.LogWarningEditorOnly("连接没有建立，无法通过WebSocket发送消息. ");
                return;
            }

            platformInterface.SendAsync(JsonConvert.SerializeObject(protocol));
        }

        #endregion
    }

    public class GameSessionEventReceiver : IWebSocketPlatformEventReciver
    {
        public void OnError(EWebSocketErrorType error)
        {
            EventDispatcher.TriggerEvent(GameSessionConnection.ConnectionErrorEvent, error);
        }

        public void OnMessage(string message)
        {
            var response = default(HttpResponseProtocol);
            var success = false;
            try
            {
                response = JsonConvert.DeserializeObject<HttpResponseProtocol>(message);
                success = true;
            }
            catch (Exception exception)
            {
                // ignored
                this.LogEditorOnly($"WebSocket序列化消息时报错: {message}");
                this.LogErrorEditorOnly(exception);
                NonmainThreadActionDispatcher.Instance.Enqueue(() =>
                {
                    EventDispatcher.TriggerEvent(GameSessionConnection.ConnectionErrorEvent, EWebSocketErrorType.SerializationError);
                });
            }

            if (success)
                NonmainThreadActionDispatcher.Instance.Enqueue(() =>
                {
                    EventDispatcher.TriggerEvent(GameSessionConnection.MessageReceivedEvent, response,
                        response.GetAttachmentAsString("service"), response.GetAttachmentAsString("method"));
                });
        }

        public void OnMessage(byte[] bytes)
        {
        }

        public void OnClose(bool errorClosed)
        {
            EventDispatcher.TriggerEvent(GameSessionConnection.ConnectionStatusChangeEvent, false, "CONNECTION-CLOSE");
        }

        public void OnOpen()
        {
            EventDispatcher.TriggerEvent(GameSessionConnection.ConnectionStatusChangeEvent, true, "");
        }
    }
}