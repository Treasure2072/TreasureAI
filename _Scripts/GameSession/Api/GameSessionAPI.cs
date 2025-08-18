using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class GameSessionAPI
    {
        public static readonly string MethodAttributeName = "method";

        public static GSLoginAPI LoginAPI { get; private set; } = new();
        public static GSCharacterAPI CharacterAPI { get; private set; } = new();
        public static GSChessBoardAPI ChessBoardAPI { get; private set; } = new();
        public static GSBuildAreaAPI BuildAreaAPI { get; private set; } = new();
        public static GSObjectiveAPI ObjectiveAPI { get; private set; } = new();
        public static GSAgentAPI AgentAPI { get; private set; } = new();
        
        public static GSPaymentAPI PaymentAPI { get; private set; } = new();
    }

    public class GameSessionAPIImpl
    {
        public string ServiceName => GetServiceName();

        protected static void SendMessage(HttpRequestProtocol request)
        {
            GameSessionConnection.Instance.SendAsync(request);
        }

        protected virtual string GetServiceName()
        {
            return "none";
        }

        protected HttpRequestProtocol CreateRequest(string method)
        {
            var request = new HttpRequestProtocol
            {
                type = GetServiceName()
            };
            request.AddBodyParams(GameSessionAPI.MethodAttributeName, method);
            return request;
        }
    }
}

