using DragonLi.Network;
using UnityEngine;

namespace Data
{
    public interface IMessageReceiver
    {
        void OnReceiveMessage(HttpResponseProtocol response, string service, string method);
    }
}