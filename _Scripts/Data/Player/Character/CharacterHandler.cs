using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;
using Game;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public class CharacterHandler : SandboxHandlerBase, IMessageReceiver
    {
        private const string kPlayerCoinKey = "player-coin";
        private const string kPlayerDiceKey = "player-dice";
        private const string kPlayerTokenKey = "player-token";
        private const string kPlayerUSDTKey = "player-usdt";
        private const string kCharacterIdKey = "character-id";
        private const string kCharacterLevelKey = "character-level";
        private const string kChessboardIdKey = "chessboard-id";
        private const string kChessboardsKey = "chessboards";
        private const string kCharactersKey = "characters";
        private const string kItemsKey = "items";
        private const string kBlueprintsKey = "blueprints";

        #region Properties - Event

        public event Action<int?, int> PlayerCoinChanged;
        public event Action<int?, int> PlayerDiceChanged;
        public event Action<float?, float> PlayerTokenChanged;
        
        public event Action<float?, float> PlayerUSDTChanged;
        
        public event Action<int?, int> CharacterIdChanged;
        public event Action<int?, int> ChessboardIdChanged;
        
        public event Action<int, int?, int> CharacterLevelChanged;
        public event Action<List<int>, List<int>> ChessboardsChanged;
        public event Action<List<int>, List<int>> CharactersChanged;
        public event Action<Dictionary<string, int>, Dictionary<string, int>> ItemsChanged;
        public event Action<List<int>, List<int>> BlueprintsChanged; 

        #endregion
        
        #region Properties - Currency - Data

        /// <summary>
        /// 玩家账户金币数量
        /// </summary>
        public int Coin
        {
            get => SandboxValue.GetValue<int>(kPlayerCoinKey);
            set => SandboxValue.SetValue(kPlayerCoinKey, value < 0 ? 0 : value);
        }
        
        /// <summary>
        /// 玩家账户骰子数量
        /// </summary>
        public int Dice
        {
            get => SandboxValue.GetValue<int>(kPlayerDiceKey);
            set => SandboxValue.SetValue(kPlayerDiceKey, value < 0 ? 0 : value);
        }
        
        /// <summary>
        /// 玩家账户代币数量
        /// </summary>
        public float Token
        {
            get => SandboxValue.GetValue<float>(kPlayerTokenKey);
            set => SandboxValue.SetValue(kPlayerTokenKey, value < 0 ? 0 : value);
        }
        
        public float USDT
        {
            get => SandboxValue.GetValue<float>(kPlayerUSDTKey);
            set => SandboxValue.SetValue(kPlayerUSDTKey, value);
        }

        #endregion

        #region Properties - Character - Data

        /// <summary>
        /// 玩家角色模型id
        /// </summary>
        public int CharacterId
        {
            get => SandboxValue.GetValue<int>(kCharacterIdKey);
            set => SandboxValue.SetValue(kCharacterIdKey, value);
        }

        public Dictionary<int, int> Levels
        {
            get => SandboxValue.GetValue<Dictionary<int, int>>(kCharacterLevelKey);
            set => SandboxValue.SetValue(kCharacterLevelKey, value);
        }
        
        /// <summary>
        /// 玩家当前处于棋盘的id
        /// </summary>
        public int ChessboardId
        {
            get => SandboxValue.GetValue<int>(kChessboardIdKey);
            set => SandboxValue.SetValue(kChessboardIdKey, value);
        }
        
        /// <summary>
        /// 玩家已经拥有的所有棋盘id
        /// </summary>
        public List<int> Chessboards
        {
            get => SandboxValue.GetValue<List<int>>(kChessboardsKey);
            set => SandboxValue.SetValue(kChessboardsKey, value);
        }
        
        /// <summary>
        /// 玩家已经拥有所有角色模型的id
        /// </summary>
        public List<int> Characters
        {
            get => SandboxValue.GetValue<List<int>>(kCharactersKey);
            set => SandboxValue.SetValue(kCharactersKey, value);
        }
        
        /// <summary>
        /// 玩家的物品数据
        /// </summary>
        public Dictionary<string, int> Items
        {
            get => SandboxValue.GetValue<Dictionary<string, int>>(kItemsKey);
            set => SandboxValue.SetValue(kItemsKey, value);
        }

        /// <summary>
        /// 玩家拥有的蓝图数据
        /// </summary>
        public List<int> Blueprints
        {
            get => SandboxValue.GetValue<List<int>>(kBlueprintsKey);
            set => SandboxValue.SetValue(kBlueprintsKey, value);
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
            sandboxCallbacks[kPlayerCoinKey] = (preValue, nowValue) => PlayerCoinChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kPlayerDiceKey] = (preValue, nowValue) => PlayerDiceChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kPlayerTokenKey] = (preValue, nowValue) => PlayerTokenChanged?.Invoke((float?)preValue, (float)nowValue);
            sandboxCallbacks[kPlayerUSDTKey] = (preValue, nowValue) => PlayerUSDTChanged?.Invoke((float?)preValue, (float)nowValue);
            sandboxCallbacks[kCharacterIdKey] = (preValue, nowValue) => CharacterIdChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kChessboardIdKey] = (preValue, nowValue) => ChessboardIdChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kChessboardsKey] = (preValue, nowValue) => ChessboardsChanged?.Invoke((List<int>)preValue, (List<int>)nowValue);
            sandboxCallbacks[kCharactersKey] = (preValue, nowValue) => CharactersChanged?.Invoke((List<int>)preValue, (List<int>)nowValue);
            sandboxCallbacks[kItemsKey] = (preValue, nowValue) => ItemsChanged?.Invoke((Dictionary<string, int>)preValue, (Dictionary<string, int>)nowValue);
            sandboxCallbacks[kBlueprintsKey] = (preValue, nowValue) => BlueprintsChanged?.Invoke((List<int>)preValue, (List<int>)nowValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            ResetData();
            GameSessionAPI.CharacterAPI.QueryCharacter();
            GameSessionAPI.CharacterAPI.QueryCurrency();
            GameSessionAPI.CharacterAPI.QueryUSDT(PlayerSandbox.Instance.RegisterAndLoginHandler.Email, response =>
            {
                if (!response.IsSuccess())
                {
                    this.LogErrorEditorOnly("USDT pull failed!");
                    return;
                }

                USDT = response.GetAttachmentAsFloat("balance");
            });
        }

        #endregion
        
        #region Function - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if(service != GameSessionAPI.CharacterAPI.ServiceName) return;
            if (method == GSCharacterAPI.MethodQueryCurrency)
            {
                var coin = response.GetAttachmentAsInt("coin");
                var dice = response.GetAttachmentAsInt("dice");
                var token = response.GetAttachmentAsFloat("token");
                Coin = coin;
                Dice = dice;
                Token = Mathf.Approximately(token, -1) ? 0 : token;
            }
            else if (method == GSCharacterAPI.MethodQueryCharacter)
            {
                
                var characterJson = response.GetAttachmentAsString("character");
                var character = JsonConvert.DeserializeObject<FCharacter>(characterJson);

                // CharacterId = character.characterId;
                ChessboardId = character.chessboardId;
                Chessboards = character.chessboards;
                Characters = character.characters;
                Levels = character.levels;
                Items = character.items;
                Blueprints = character.blueprints;
            }
        }

        #endregion

        #region Function - API

        public void ResetData()
        {
            Coin = 0;
            Dice = 0;
            Token = 0;
            CharacterId = 0;
            ChessboardId = -1;
        }

        /// <summary>
        /// 根据角色id，获取对应角色等级
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public int GetLevelById(int characterId)
        {
            return Levels.GetValueOrDefault(characterId, 1);
        }

        /// <summary>
        /// 获取当前角色等级
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            return GetLevelById(CharacterId);
        }
        
        /// <summary>
        /// 升级角色本地数据修改
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="level"></param>
        public void UpdateLocalDataOfUpgradeCharacter(int characterId, int level)
        {
            if (!Levels.ContainsKey(characterId))
            {
                Levels[characterId] = 1;
            }
            var preLevel = Levels[characterId];
            Levels[characterId] = level;
            CharacterLevelChanged?.Invoke(characterId, preLevel, level);
        }

        #endregion
    }
}