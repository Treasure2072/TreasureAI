using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Utils;
using DragonLi.Core;
using DragonLi.Network;
using Game;
using Newtonsoft.Json;

namespace Data
{
    public class ChessBoardHandler : SandboxHandlerBase, IMessageReceiver
    {
        private const string kPlayerInvestKey = "player-invest";
        private const string kPlayerInviteeKey = "player-invitee";
        private const string kPlayerInvitersKey = "player-inviters";
        private const string kPlayerChildrenKey = "player-children";
        private const string kPlayerStandKey = "player-stand-index";
        private const string kPlayerChessBordTileKey = "player-chess-bord-tile";
        private const string kPulledGameDataKey = "pulled-game-data";

        #region Property - Event

        public event Action<bool?, bool> OnPulledGameDataChanged;

        public event Action<int?, int> OnInvestChanged;

        public event Action<List<FEntityUser>, List<FEntityUser>> OnChildrenChanged;

        #endregion
        
        #region Properties - Default - Data

        public bool IsPulledGameData
        {
            get => SandboxValue.GetValue<bool>(kPulledGameDataKey);
            set => SandboxValue.SetValue(kPulledGameDataKey, value);
        }

        public List<FLandGrade> LandGrade { get; private set; }

        public List<FBuildArea> BuildAreas { get; private set; }
        
        public List<FCharacterShopInfo> CharactersShopInfos { get; private set; }
        
        public List<FChance> Chances { get; private set; }
        
        public List<FLandPrice> LandPrices { get; private set; }
        
        public List<FBuildSlot> BuildSlots { get; private set; }
        
        public List<FLand> Lands { get; private set; }
        
        public List<FShort> Shorts { get; private set; }
        
        public List<FChessBoard> ChessBoards { get; private set; }
        
        public List<FBuildSlotRate> BuildSlotRates { get; private set; }
        
        public List<FShopInfo> Shops { get; private set; }
        
        public List<FObjectiveDailyInfo> Objectives { get; private set; }
        
        public List<FCheckInInfo> CheckInInfos { get; private set; }
        
        public List<FObjectivePaymentInfo> PaymentInfos { get; private set; }

        #endregion

        #region Properties - Data

        /// <summary>
        /// 银行资金
        /// </summary>
        public int InvestCoin
        {
            get => SandboxValue.GetValue<int>(kPlayerInvestKey);
            set => SandboxValue.SetValue(kPlayerInvestKey, value);
        }

        /// <summary>
        /// 被邀请者
        /// </summary>
        public string Invitee
        {
            get => SandboxValue.GetValue<string>(kPlayerInviteeKey);
            set => SandboxValue.SetValue(kPlayerInviteeKey, value);
        }

        /// <summary>
        /// 已经邀请成功的人
        /// </summary>
        public FBankData Inviters
        {
            get => SandboxValue.GetValue<FBankData>(kPlayerInvitersKey);
            set => SandboxValue.SetValue(kPlayerInvitersKey, value);
        }

        /// <summary>
        /// 已经邀请成功的人
        /// </summary>
        public List<FEntityUser> Children
        {
            get => SandboxValue.GetValue<List<FEntityUser>>(kPlayerChildrenKey);
            set => SandboxValue.SetValue(kPlayerChildrenKey, value);
        }
        
        /// <summary>
        /// 当前站立的位置
        /// </summary>
        public int StandIndex
        {
            get => SandboxValue.GetValue<int>(kPlayerStandKey);
            set => SandboxValue.SetValue(kPlayerStandKey, value);
        }
        
        /// <summary>
        /// 棋盘格子数据
        /// items - 物品
        /// lands - 地块
        /// </summary>
        public FChessBoardData ChessBoardData
        {
            get => SandboxValue.GetValue<FChessBoardData>(kPlayerChessBordTileKey);
            set => SandboxValue.SetValue(kPlayerChessBordTileKey, value);
        }

        #endregion

        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }
            
            // TODO: 监听 sandbox 里面值的改变
            // ...
            sandboxCallbacks[kPulledGameDataKey] = (preValue, newValue) => OnPulledGameDataChanged?.Invoke((bool?)preValue, (bool)newValue);
            sandboxCallbacks[kPlayerInvestKey] = (preValue, newValue) =>OnInvestChanged?.Invoke((int?)preValue, (int)newValue);
            sandboxCallbacks[kPlayerChildrenKey] = (preValue, newValue) => OnChildrenChanged?.Invoke((List<FEntityUser>)preValue, (List<FEntityUser>)newValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            IsPulledGameData = false;
            
            GameSessionAPI.ChessBoardAPI.Query();
            GameSessionAPI.ChessBoardAPI.QueryBank(PlayerSandbox.Instance.RegisterAndLoginHandler.Email);
            GameSessionAPI.ChessBoardAPI.QueryGameData(response =>
            {
                if (!response.IsSuccess())
                {
                    this.LogErrorEditorOnly($"Failed to get chess board response: {response.error}");
                    return;
                }
                var landJson = response.GetAttachmentAsString("land");
                Lands = JsonConvert.DeserializeObject<List<FLand>>(landJson);
                
                var landGradeJson = response.GetAttachmentAsString("land-grade");
                LandGrade = JsonConvert.DeserializeObject<List<FLandGrade>>(landGradeJson);
                
                var chanceJson = response.GetAttachmentAsString("chance");
                Chances = JsonConvert.DeserializeObject<List<FChance>>(chanceJson);
                
                var shortJson = response.GetAttachmentAsString("short");
                Shorts = JsonConvert.DeserializeObject<List<FShort>>(shortJson);
                
                // var buildAreaJson = response.GetAttachmentAsString("build-area");
                // BuildAreas = JsonConvert.DeserializeObject<List<FBuildArea>>(buildAreaJson);
                
                // var buildSlotJson = response.GetAttachmentAsString("build-slot");
                // BuildSlots = JsonConvert.DeserializeObject<List<FBuildSlot>>(buildSlotJson);
                
                // var buildSlotRateJson = response.GetAttachmentAsString("build-slot-rate");
                // BuildSlotRates = JsonConvert.DeserializeObject<List<FBuildSlotRate>>(buildSlotRateJson);
                
                var characterJson = response.GetAttachmentAsString("character-level");
                CharactersShopInfos = JsonConvert.DeserializeObject<List<FCharacterShopInfo>>(characterJson);
                
                var chessboardJson = response.GetAttachmentAsString("chessboard");
                ChessBoards = JsonConvert.DeserializeObject<List<FChessBoard>>(chessboardJson);
                
                // var landPriceJson = response.GetAttachmentAsString("land-price");
                // LandPrices = JsonConvert.DeserializeObject<List<FLandPrice>>(landPriceJson);
                var shopJson = response.GetAttachmentAsString("shop");
                Shops = JsonConvert.DeserializeObject<List<FShopInfo>>(shopJson);
                
                var objectiveJson = response.GetAttachmentAsString("objective");
                Objectives = JsonConvert.DeserializeObject<List<FObjectiveDailyInfo>>(objectiveJson);
                
                var checkInJson = response.GetAttachmentAsString("check-in");
                CheckInInfos = JsonConvert.DeserializeObject<List<FCheckInInfo>>(checkInJson);

                var paymentJson = response.GetAttachmentAsString("payment-objective");
                PaymentInfos = JsonConvert.DeserializeObject<List<FObjectivePaymentInfo>>(paymentJson);
                IsPulledGameData = true;
            });
        }

        #endregion

        #region API

        /// <summary>
        /// 获取角色的最高等级
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public int GetMaxLevel(int characterId)
        {
            return CharactersShopInfos.Select(characterInfo => characterInfo.level).Prepend(1).Max();
        }

        #endregion

        #region Function - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName) return;
            if (method == GSChessBoardAPI.MethodQuery)
            {
                StandIndex = response.GetAttachmentAsInt("stand");
                ChessBoardData = JsonConvert.DeserializeObject<FChessBoardData>(response.GetAttachmentAsString("data"));

                // 还未升级完成
                // 服务端level已经+1， 客户端需要退回leve - 1的状态
                foreach (var (index, data) in ChessBoardData.lands)
                {
                    if (data.finishTs > TimeAPI.GetVietnamTimeStamp())
                    {
                        var fChessBoardLandTile = data;
                        fChessBoardLandTile.level--;
                        ChessBoardData.lands[index] = fChessBoardLandTile;
                    }
                }
            }
            else if (method == GSChessBoardAPI.MethodQueryBank)
            {
                InvestCoin = response.GetAttachmentAsInt("invest");
                Invitee = response.GetAttachmentAsString("inviter");
                
                var childrenJson = response.GetAttachmentAsString("children");
                Children = JsonConvert.DeserializeObject<List<FEntityUser>>(childrenJson);
                
                var bankJson = response.GetAttachmentAsString("bank"); 
                Inviters = JsonConvert.DeserializeObject<FBankData>(bankJson);
            }
        }

        #endregion

    }

}