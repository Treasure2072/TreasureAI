using System;
using System.Collections.Generic;
using _Scripts.Utils;
using Data.Type;
using DragonLi.Core;
using DragonLi.Network;
using Game;

namespace Data
{
    public class AIChatHandler : SandboxHandlerBase, IMessageReceiver
    {
        #region Properties - Event
        
        public event Action<AIChatType.TChatMessage> OnSessionMessage;

        #endregion

        #region Properties - Data

        private List<AIChatType.TChatMessage> Messages { get; set; }

        #endregion


        #region SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            
        }

        protected override void OnInit()
        {
            base.OnInit();
            Messages = new List<AIChatType.TChatMessage>();
        }

        #endregion

        public IReadOnlyList<AIChatType.TChatMessage> GetMessages()
        {
            return Messages;
        }

        /// <summary>
        /// 添加消息到缓存列表
        /// 只能缓存自己发送的消息，不包含服务端返回的消息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(AIChatType.TChatMessage message)
        {
            if(message.chatType != AIChatType.EChatType.Owner) return;
            Messages.Add(message);
            OnSessionMessage?.Invoke(message);
        }

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if(service != GameSessionAPI.AgentAPI.ServiceName || method != GSAgentAPI.MethodTalk) return;
            
            var message = new AIChatType.TChatMessage
            {
                timestamp = TimeAPI.GetUtcTimeStamp(),
                chatType = AIChatType.EChatType.Agent,
                message = response.GetAttachmentAsString("response")
            };
            Messages.Add(message);
            OnSessionMessage?.Invoke(message);
        }
    }
}