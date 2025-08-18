using System;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class GameSessionEvent : ChessTileEvent
    {
        #region Properties

        private bool Finish { get; set; } = false;

        private string Service { get; set; }
        private string Method { get; set; }
        private Action<HttpResponseProtocol, string, string> MessageCallback { get; set; }

        #endregion

        #region ChessTileEvent

        public GameSessionEvent(ChessTile tile, string service, string method, Action<HttpResponseProtocol, string, string> callback) : base(tile)
        {
            Service = service;
            Method = method;
            MessageCallback = callback;
            EventDispatcher.AddEventListener<HttpResponseProtocol, string, string>(GameSessionConnection.MessageReceivedEvent, OnReceiveMessage);
        }

        public override bool OnTick()
        {
            return Finish;
        }

        #endregion
        
        #region Callbacks

        private void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (Service != service || Method != method) return;
            if (response.IsSuccess())
            {
                MessageCallback(response, service, method);
                Finish = true;
            }
            else
            {
                this.LogErrorEditorOnly($"Received HTTP response: {response}");
                Finish = true;
            }
        }

        #endregion
        
    }
}
