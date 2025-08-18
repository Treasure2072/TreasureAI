using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class GSCharacterAPI : GameSessionAPIImpl
    {
        public static readonly string MethodQueryCurrency = "query_currency";
        public static readonly string MethodQueryCharacter = "query_character";
        public static readonly string MethodSetCharacter = "set_character";
        public static readonly string MethodSetChessboard = "set_chessboard";
        public static readonly string MethodCharacterUpgrade = "upgrade";
        public static readonly string MethodPurchase = "purchase";
        public static readonly string MethodDevDisconnect = "disconnect";
        public static readonly string MethodBindInviter = "bind_inviter";
        
        /// <summary>
        /// 装备角色
        /// </summary>
        /// <param name="character"></param>
        public void SetCharacter(int character)
        {
            var request = CreateRequest(MethodSetCharacter);
            request.AddBodyParams("id", character);
            SendMessage(request);
        }
        
        /// <summary>
        /// 装备棋盘
        /// </summary>
        /// <param name="chessboard"></param>
        public void SetChessboard(int chessboard)
        {
            var request = CreateRequest(MethodSetChessboard);
            request.AddBodyParams("id", chessboard);
            SendMessage(request);
        }
        
        /// <summary>
        /// 发送购买请求，传入shop id
        /// </summary>
        /// <param name="shopId">商店物品ID</param>
        public void Purchase(string shopId)
        {
            var request = CreateRequest(MethodPurchase);
            request.AddBodyParams("id", shopId);
            SendMessage(request);
        }
        
        /// <summary>
        /// 再设置界面使用，将自己和邀请者绑定起来，每一个用户只能有一个邀请者
        /// </summary>
        /// <param name="id">邀请者id</param>
        public void BindInviter(string id)
        {
            var request = CreateRequest(MethodBindInviter);
            request.AddBodyParams("id", id);
            SendMessage(request);
        }
        
        public void DevDisconnect()
        {
            SendMessage(CreateRequest(MethodDevDisconnect));
        }
        
        /// <summary>
        /// 查询货币
        /// </summary>
        public void QueryCurrency()
        {
            SendMessage(CreateRequest(MethodQueryCurrency));
        }
        
        /// <summary>
        /// 查询玩家数据，角色，棋盘，物品等
        /// </summary>
        public void QueryCharacter()
        {
            SendMessage(CreateRequest(MethodQueryCharacter));
        }

        /// <summary>
        /// 升级角色等级
        /// </summary>
        /// <param name="characterId"></param>
        public void UpgradeCharacter(int characterId = 5)
        {
            var request = CreateRequest(MethodCharacterUpgrade);
            request.AddBodyParams("id", characterId);
            SendMessage(request);
        }

        protected override string GetServiceName()
        {
            return "character";
        }
        
        public bool QueryUSDT(string email, Action<HttpResponseProtocol> finish)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new HttpRequest<HttpResponseProtocol>()
                .SetMethod(EHttpRequestMethod.Get)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + $"user/queryUSDT?email={email}")
                .AddCallback(finish)
                .SendRequestAsync();
        }
    }

    [Serializable]
    public struct FCharacter
    {
        /// <summary>
        /// 玩家当前角色
        /// </summary>
        public int characterId;
        
        /// <summary>
        /// 玩家当前棋盘
        /// </summary>
        public int chessboardId;
        
        /// <summary>
        /// 玩家拥有的棋盘，默认棋盘0不包含在这个列表
        /// </summary>
        public List<int> chessboards;
        
        /// <summary>
        /// 玩家拥有的角色，默认角色0不包含在这个列表
        /// </summary>
        public List<int> characters;
        
        /// <summary>
        /// 玩家所有角色等级信息
        /// </summary>
        public Dictionary<int, int> levels;
        
        // ReSharper disable once InconsistentNaming
        public Dictionary<string, int> items;
        
        /// <summary>
        /// 玩家拥有的蓝图
        /// </summary>
        public List<int> blueprints;
    }
    
    // [Serializable]
    // public struct FCharacterShopInfo
    // {
    //     public int id;
    //     public float coinMul;
    //     public int dice;
    //     public float price;
    //     public int rowId;
    // }
    
    [Serializable]
    public struct FCharacterShopInfo
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int level;
        
        /// <summary>
        /// 升级所需金币
        /// </summary>
        public long coinNeeded;
        
        /// <summary>
        /// 金币加成比例
        /// </summary>
        public float coinMultiplier;
        
        /// <summary>
        /// 骰子每天最多恢复数量
        /// </summary>
        public int diceAdded;
        
        /// <summary>
        /// 每日钻石
        /// </summary>
        public int tokenDay;
    }
}

