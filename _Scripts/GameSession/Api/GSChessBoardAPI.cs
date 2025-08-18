using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;

namespace Game
{
    public class GSChessBoardAPI : GameSessionAPIImpl
    {
        public static readonly string MethodQuery = "query";
        public static readonly string MethodMove = "move";
        public static readonly string MethodArrive = "arrive";
        public static readonly string MethodQueryBank = "query_bank";
        public static readonly string MethodWithdrawBank = "withdraw_bank";

        /// <summary>
        /// 玩家移动，调用这个函数会向服务器发送移动请求，服务端接受到请求后，会返回相应消息
        /// 必然会返回的消息：移动结果
        /// 可能会返回的消息：经过格子结果
        /// </summary>
        public void Move(int multiplier = 1)
        {
            var request = CreateRequest(MethodMove);
            request.AddBodyParams("mul", multiplier);
    
            SendMessage(request);
        }

        public void MoveDev(int diceA, int diceB, int multiplier = 1)
        {
            var request = CreateRequest("move-debug");
            request.AddBodyParams("mul", multiplier);
            request.AddBodyParams("a", diceA);
            request.AddBodyParams("b", diceB);
            SendMessage(request);
        }

        /// <summary>
        /// 停留一回合
        /// </summary>
        public void Stay(int multiplier = 1)
        {
            var request = CreateRequest("stay");
            request.AddBodyParams("mul", multiplier);
            SendMessage(request);
        }
        
        /// <summary>
        /// 在调用Move以后可以调用Arrive，此时会触发到达格子效果，服务端接受到请求后，会返回相应消息
        /// 一次Move对应一次Arrive，否则无效
        /// 一定会返回的消息：到达格子结果
        /// </summary>
        public void Arrive()
        {
            SendMessage(CreateRequest(MethodArrive));
        }

        /// <summary>
        /// 对目前站立的格子实施Option操作
        /// </summary>
        /// <param name="onCreateBody">可在此填写需要的参数列表</param>
        public void Option(Action<HttpRequestProtocol> onCreateBody)
        {
            var request = CreateRequest("option");
            onCreateBody?.Invoke(request);
            SendMessage(request);
        }

        /// <summary>
        /// 查询当前棋盘信息
        /// </summary>
        public void Query()
        {
            SendMessage(CreateRequest(MethodQuery));
        }
        
        /// <summary>
        /// 查询银行数据
        /// children: 
        /// invest: 拥有的银行资金
        /// bank: FBankData 邀请过得有效的玩家id，可以通过Https请求查询这些玩家的基本信息，名字等
        /// </summary>
        public void QueryBank(string email)
        {
            var request = CreateRequest(MethodQueryBank);
            request.AddBodyParams("email", email);
            SendMessage(request);
        }
        
        /// <summary>
        /// 提取银行资金
        /// 返回coin：所提取的数量
        /// 或者相关的错误信息
        /// </summary>
        public void WithdrawBank(string email)
        {
            var request = CreateRequest(MethodWithdrawBank);
            request.AddBodyParams("email", email);
            SendMessage(request);
        }

        /// <summary>
        /// Https请求查询这些玩家的基本信息url: user/queryUsers，传入参数为字符串users: string.
        /// 参数为查询的玩家列表，每个玩家用','隔开，如"sandy,tommy,paul"
        /// </summary>
        /// <param name="users"></param>
        /// <param name="finish"></param>
        public bool QueryUsers(string[] users, Action<HttpResponseProtocol> finish)
        {
            if (users == null || users.Length <= 0)
            {
                this.LogErrorEditorOnly("[QueryUsers] Users are not valid!");
                return false;
            }
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("users", string.Join(',', users));
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var queryRequest = new HttpRequest<HttpResponseProtocol>();
            return queryRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Get)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "user/queryUsers")
                .AddCallback(finish)
                .SendRequestAsync();
        }

        /// <summary>
        /// 拉取游戏数据
        /// </summary>
        /// <param name="finish"></param>
        public bool QueryGameData(Action<HttpResponseProtocol> finish)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new HttpRequest<HttpResponseProtocol>()
                .SetMethod(EHttpRequestMethod.Get)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "user/queryGameData")
                .AddCallback(finish)
                .SendRequestAsync();
        }
        
        public bool QueryGameRanks(string userId, Action<HttpResponseProtocol> finish)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new HttpRequest<HttpResponseProtocol>()
                .SetMethod(EHttpRequestMethod.Get)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + $"user/queryRanks?user={userId}")
                .AddCallback(finish)
                .SendRequestAsync();
        }
        
        protected override string GetServiceName()
        {
            return "chess";
        }
    }
}