using DragonLi.Network;

namespace Game
{
    public class GSAgentAPI : GameSessionAPIImpl
    {
        public static readonly string MethodTalk = "talk";

        /// <summary>
        /// 与某个机器人交谈
        /// </summary>
        /// <param name="agent">机器人id</param>
        /// <param name="message">消息</param>
        public void Talk(string agent, string message)
        {
            var request = CreateRequest(MethodTalk);
            request.AddBodyParams("agent", agent);
            request.AddBodyParams("content", message);
            SendMessage(request);
        }
        
        protected override string GetServiceName()
        {
            return "ai";
        }
    }
}


