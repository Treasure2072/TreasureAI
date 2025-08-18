using Data.Data;
using DragonLi.Core;
using DragonLi.Network;
using Game;
using Newtonsoft.Json;

namespace Data
{
    public class PlayerSandbox : Singleton<PlayerSandbox>
    {
        #region Properties - Handler

        /// <summary>
        /// 登录注册相关数据缓存
        /// email, inviteCode, Token
        /// </summary>
        public RegisterAndLoginHandler RegisterAndLoginHandler { get; private set; } = new();

        /// <summary>
        /// socket 连接相关数据
        /// id, token
        /// </summary>
        public ConnectionHandler ConnectionHandler { get; private set; } = new();
        
        /// <summary>
        /// ai 相关数据
        /// </summary>
        public AIChatHandler AIChatHandler { get; private set; }
        
        /// <summary>
        /// 玩家相关信息
        /// 账户，角色，物品数据
        /// </summary>
        public CharacterHandler CharacterHandler { get; private set; }
        
        /// <summary>
        /// 1.游戏的一些不可修改的基础数据
        /// 2.棋盘，银行相关数据
        /// </summary>
        public ChessBoardHandler ChessBoardHandler { get; private set; }

        /// <summary>
        /// 建筑区域数据缓存
        /// </summary>
        public BuildingAreaHandler BuildingAreaHandler { get; private set; }
        
        /// <summary>
        /// 任务数据缓存
        /// </summary>
        public ObjectiveHandler ObjectiveHandler { get; private set; }
        
        /// <summary>
        /// 排行榜数据
        /// </summary>
        public RankHandler RankHandler { get; private set; }
        
        /// <summary>
        /// 区块链数据
        /// </summary>
        public BlockChainHandler BlockChainHandler { get; private set; }
        
        /// <summary>
        /// 游戏临时缓存
        /// </summary>
        public TemporaryHandler TemporaryHandler { get; private set; }

        #endregion

        #region Function
        
        private void InitData()
        {
            AIChatHandler = new AIChatHandler();
            CharacterHandler = new CharacterHandler();
            ChessBoardHandler = new ChessBoardHandler();
            BuildingAreaHandler = new BuildingAreaHandler();
            ObjectiveHandler = new ObjectiveHandler();
            RankHandler = new RankHandler();
            BlockChainHandler = new BlockChainHandler();
            TemporaryHandler = new TemporaryHandler();
        }
        
        #endregion

        #region API - Common
        
        public void InitReceiveListener()
        {
            EventDispatcher.AddEventListener<HttpResponseProtocol, string, string>(GameSessionConnection.MessageReceivedEvent, OnReceiveMessage);
        }

        public void InitializePlayerSandbox()
        {
            InitReceiveListener();
            InitData();
        }

        public void DebugInitializePlayerSandbox()
        {
#if UNITY_EDITOR
            InitializePlayerSandbox();
            CharacterHandler.ResetData();
#endif
        }

        #endregion

        #region Callback
        
        private void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service == "heart-beat") return;
            if (!response.IsSuccess())
            {
                this.LogErrorEditorOnly($"Received HTTP Error: {response.error} - {service} - {method}");
                return;
            }
            
            this.LogEditorOnly($"Received HTTP Message: {JsonConvert.SerializeObject(response.body)} - {service} - {method}");
            
            AIChatHandler?.OnReceiveMessage(response, service, method);
            CharacterHandler?.OnReceiveMessage(response, service, method);
            ChessBoardHandler?.OnReceiveMessage(response, service, method);
            BuildingAreaHandler?.OnReceiveMessage(response, service, method);
            ObjectiveHandler?.OnReceiveMessage(response, service, method);
        }
 
        #endregion
        
    }
}