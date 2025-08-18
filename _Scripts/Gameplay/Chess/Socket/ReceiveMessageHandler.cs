using System;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class ReceiveMessageHandler : MonoBehaviour
    {
        #region Properties

        public event Action<HttpResponseProtocol, string, string> OnReceiveMessageHandler;

        #endregion
        
        #region Unity

        private void Awake()
        {
            EventDispatcher.AddEventListener<HttpResponseProtocol, string, string>(GameSessionConnection.MessageReceivedEvent, OnReceiveMessageCallback);
        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<HttpResponseProtocol, string, string>(GameSessionConnection.MessageReceivedEvent, OnReceiveMessageCallback);
            OnReceiveMessageHandler = null;
        }

        #endregion

        #region Function

        private void OnReceiveMessageCallback(HttpResponseProtocol response, string service, string method)
        {
            if (gameObject.activeSelf)
            {
                OnReceiveMessageHandler?.Invoke(response, service, method);
            }
        }

        #endregion
    }
}