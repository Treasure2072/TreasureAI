using System;
using System.Collections.Generic;
using _Scripts.Utils;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class ChessTileChance : ChessTile
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private GameObject RibbonsEffectObject;

        #endregion

        #region Properties

        private bool ReceiveArriveMessage { get; set; } = false;
        
        private int EventId { get; set; }
        private int Dice { get; set; }
        private int Coin  { get; set; }
        private int Index { get; set; }
        private int Stand  { get; set; }
        
        private float Token { get; set; }

        #endregion
        
        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Happy();
            return new List<IQueueableEvent>
            {
                new CustomEvent(() =>
                {
                    UIStaticsLayer.HideUIStaticsLayer();
                    UIActivityLayer.HideUIActivityLayer();
                    UIChessboardLayer.HideLayer();
                }),
                new GameObjectVisibilityEvent(RibbonsEffectObject),
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new CustomEvent(() => { PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("stand-chance", 1); }),
                new CustomEvent(() =>
                {
                    var tilesCount = ChessGameBoard.GetChessGameBoard().GetTilesCount();
                    UIChanceLayer.ShowLayer(EventId, Coin, Dice, Token, (Stand + tilesCount - PlayerSandbox.Instance.ChessBoardHandler.StandIndex) % tilesCount);
                }),
                new CustomEvent(() =>
                {
                    CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                    {
                        if (EventId < 9)
                        {
                            SoundAPI.PlaySound(AudioInstance.Instance.Settings.goodSmall);
                        }
                        else if (EventId > 9)
                        {
                            SoundAPI.PlaySound(AudioInstance.Instance.Settings.bad);
                        }
                    }, 2f);
                }),
                new PlayFullscreenEffectEvent(EffectInstance.Instance.Settings.vfxChance),
                new WaitForTrueEvent(() => !UIChanceLayer.Showing()),
                EffectsAPI.CreateTip(() => Coin, () => Dice, () => Token),
                EffectsAPI.CreateSoundEffect(() => IsGoodEvent(EventId) ? EffectsAPI.EEffectType.Coin : EffectsAPI.EEffectType.None),
                EffectsAPI.CreateScreenFullEffect(() => Coin, () => Dice, () => Token),
                new CloseEffectCameraEvent(),
                new ChanceSpecialEvent(this, () => EventId, () => Index, () => Stand),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => Coin),
                new CustomEvent(() =>
                {
                    UIStaticsLayer.ShowUIStaticsLayer();
                    UIActivityLayer.ShowUIActivityLayer();
                    UIChessboardLayer.ShowLayer();
                }),
            };

        }
        #endregion

        #region Function

        private bool IsGoodEvent(int eventId)
        {
            return eventId <= 8;
        }

        #endregion
        
        #region Callbacks

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "chance") return;
            ReceiveArriveMessage = true;
            EventId = response.GetAttachmentAsInt("event");
            var gameMode = World.GetRegisteredObject<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey);
            if (gameMode.debugMode)
            {
                EventId = gameMode.chanceEventId;
            }

            if (EventId == 13)
            {
                Index = response.GetAttachmentAsInt("index");
            } 
            else if (EventId == 9)
            {
                Stand = response.GetAttachmentAsInt("stand");
            }
            else if (EventId == 8)
            {
                Index = response.GetAttachmentAsInt("index");
                Dice = response.GetAttachmentAsInt("dice");
                Token = response.GetAttachmentAsFloat("token");
                
                PlayerSandbox.Instance.CharacterHandler.Dice += Dice;
                PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(Token, -1) ? 0 : Token;
            }
            else
            {
                Dice = response.GetAttachmentAsInt("dice");
                Coin = response.GetAttachmentAsInt("coin");
                Token = response.GetAttachmentAsFloat("token");
                
                PlayerSandbox.Instance.CharacterHandler.Dice += Dice;
                PlayerSandbox.Instance.CharacterHandler.Coin += Coin;
                PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(Token, -1) ? 0 : Token;
            }
        }
        #endregion
    }

    internal class ChanceSpecialEvent : ChessTileEvent
    {
        private const float Timeout = 10f;
        private ChessTile Tile { get; set; }
        private Func<int> GetEventId { get; set; }
        private Func<int> GetIndex { get; set; }
        private Func<int> GetStand { get; set; }
        
        private float FinishTs { get; set; }
        private DragonLiCameraTopdown CameraTopdown { get; set; }

        public ChanceSpecialEvent(ChessTile tile, Func<int> getEventId, Func<int> getTileIndex, Func<int> getStand) : base(tile)
        {
            Tile = tile;
            GetEventId = getEventId;
            GetIndex = getTileIndex;
            GetStand = getStand;
            FinishTs = Time.unscaledTime + Timeout;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            var eventId = GetEventId();
            
            var gameMode = World.GetRegisteredObject<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey);
            var chessBord = World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey);
            switch (eventId)
            {
                case 8:
                    // 立刻完成修建
                    CameraTopdown = MoveCamera(ChessGameBoard.GetChessGameBoard().GetTileByIndex(GetIndex()).transform);
                    var upgradeTile = chessBord.GetTileByIndex(GetIndex());
                    (upgradeTile as ChessTileLand)?.UpgradeLevelHouse();
                    (upgradeTile as ChessTileLand)?.SetUpgradeEffectStatus(false);
                    break;
                case 9:
                    
                    // 跳跃到指定格子
                    gameMode.Jump(GetStand());
                    FinishTs = Time.unscaledTime + 1.8f;
                    break;
                case 13:
                    // 查封
                    CameraTopdown = MoveCamera(ChessGameBoard.GetChessGameBoard().GetTileByIndex(GetIndex()).transform);
                    var banTile = chessBord.GetTileByIndex(GetIndex());
                    (banTile as ChessTileLand)?.PerformLocked(true);
                    break;
                default:
                    FinishTs = Time.unscaledTime;
                    break;
            }
        }

        public override bool OnTick()
        {
            return Time.unscaledTime >= FinishTs;
        }

        public override void OnFinish()
        {
            base.OnFinish();

            if (CameraTopdown)
            {
                CameraTopdown.SetOverrideTarget(null);
            }
        }

        private DragonLiCameraTopdown MoveCamera(Transform target, float life = 3f)
        {
            FinishTs  = Time.unscaledTime + life;
            var cameraTrans = World.GetMainCamera();
            var camera = cameraTrans.GetComponent<DragonLiCameraTopdown>();
            camera.SetOverrideTarget(target);
            return camera;
        }
    }
}