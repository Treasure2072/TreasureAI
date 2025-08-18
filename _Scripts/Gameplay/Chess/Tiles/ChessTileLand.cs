using System;
using System.Collections.Generic;
using _Scripts.Utils;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ChessTileLand : ChessTile
    {
        #region Fields
        [Header("Settings")]
        [SerializeField] private ChessTilesType.EChessTileLandType type;
        
        [Header("Settings - Level")]
        [SerializeField] private TextMeshProUGUI tmpLevel;
        [SerializeField] private Color[] levelColors;
        
        [Header("References")]
        [SerializeField] private Transform elementTransform;
        [SerializeField] private LevelComponent[] levelComponents;

        [Header("Effects")] 
        [SerializeField] private GameObject cashEffectObject;
        [SerializeField] private GameObject landBanObject;
        [SerializeField] private GameObject landUpgradeObject;
        
        #endregion

        #region Properties
        
        // private string Item { get; set; }
        
        private bool bReceiveArriveMessage { get; set; } = false;

        public int Coin { get; set; }
        public int Need { get; set; }
        public int Finish { get; set; }
        public string Option { get; set; }

        private UIWSTimer Timer { get; set; }
        private UIWorldElement LockedWorldElement { get; set; }

        #endregion

        #region Properties - TilesData

        private FChessBoardLandTile TileData => PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.GetDataByIndex(TileIndex);

        private int Level
        {
            get => TileData.level;
            set
            {
                var level = TileData.level;
                if (level != value)
                {
                    PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.SetLevelByIndex(TileIndex, value);
                }
            }
        }

        private bool Locked
        {
            get => TileData.locked;
            set
            {
                var locked = TileData.locked;
                if (locked != value)
                {
                    PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.SetLockStatusByIndex(TileIndex, value);
                }
            }
        }

        #endregion

        #region API
        
        public void InitializedData()
        {
            // TileData = data;
            
            if (TimeAPI.GetVietnamTimeStamp() < TileData.finishTs)
            {
                SetUpUpgradeTimer(TileData.finishTs - TimeAPI.GetVietnamTimeStamp(), TileData.finishTs - TileData.startTs);
            }
            PerformLocked(TileData.locked);
            SetHousesLevel(Level);
            UpdateLevelText();
        }

        // public void InitializedData(string item)
        // {
        //     Item = item;
        // }

        public void SetUpUpgradeTimer(int remainTime, int fullTime)
        {
            SetUpgradeEffectStatus(true);
            if (!Timer)
            {
                var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
                var element = layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiEffectLandUpgradeTimer, elementTransform.position);
                Timer = element.GetComponent<UIWSTimer>();
            }
            Timer.Initialize(remainTime, fullTime, () =>
            {
                Timer = null;
                FinishUpgrade();
            });
        }

        public FChessBoardLandTile GetTileData()
        {
            return TileData;
        }

        public ChessTilesType.EChessTileLandType GetLandType()
        {
            return type;
        }

        public void PerformLocked(bool locked)
        {
            Locked = locked;
            SetBanEffectStatus(locked);
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");

            if (locked)
            {
                if (LockedWorldElement) return;
                LockedWorldElement = layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiEffectLandLocked, GetStandPosition());
            }
            else
            {
                if (!LockedWorldElement) return;
                layer.RemoveElement(LockedWorldElement);
                LockedWorldElement = null;
            }
        }

        public void UpgradeLevelHouse()
        {
            SetHousesLevel(Level);
        }
        
        public void SetUpgradeEffectStatus(bool display)
        {
            landUpgradeObject.SetActive(display);
        }

        #endregion

        #region Function

        private void SetBanEffectStatus(bool display)
        {
            landBanObject.SetActive(display);
        }        

        #endregion

        #region Functions - Level

        private void InitHouses()
        {
            foreach (var t in levelComponents)
            {
                t.SetState(LevelComponent.EHouseLocationType.Disappear);
            }
        }

        private void UpdateLevelText()
        {
            if (!tmpLevel) return;
            tmpLevel.SetText(string.Format(this.GetLocalizedText("level-fmt"), Level));
        }

        private Color GetColorByLevel(int level)
        {
            var index = (level - 1) / 2;
            return levelColors[index];
        }

        private LevelComponent GetSlotByIndex(int index)
        {
            return levelComponents[index];
        }

        private void SetHousesLevel(int level)
        {
            InitHouses();
            var levelInfo = GetHouseRepresentation(level);
            foreach (var info in levelInfo)
            {
                var slot = GetSlotByIndex(info.Key);
                slot.SetColor(info.Value.color);
                slot.SetState(info.Value.half ? LevelComponent.EHouseLocationType.Part : LevelComponent.EHouseLocationType.All);
            }
        }
        
        private void FinishUpgrade()
        {
            PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("upgrade-land", 1);
            Level++;
            UpdateLevelText();
            UpgradeLevelHouse();
            SetUpgradeEffectStatus(false);
        }

        private Dictionary<int, (Color color, bool half)> GetHouseRepresentation(int level)
        {
            var result = new Dictionary<int, (Color color, bool half)>();
            var levelArray = TilesAPI.GetHouseLevels(level);
            for (var i = 0; i < 4; i++)
            {
                var slotLevel = levelArray[i];
                if (slotLevel <= 0) continue;
                var color = GetColorByLevel(slotLevel);
                result[i] = (color, slotLevel % 2 == 1);
            }

            return result;
        }
        
        #endregion

        #region ChessTile

        public override void Initialize(int tileIndex)
        {
            base.Initialize(tileIndex);
            InitHouses();
            InitializedData();
        }

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            var layer = UITutorialLayer.GetLayer("UITutorialLayer-Land");
            return new List<IQueueableEvent>
            {
                new CustomEvent(() =>
                {
                    if (TutorialHandler.IsTriggerable(TutorialHandler.FirstStandLandKey))
                    {
                        UIStaticsLayer.HideUIStaticsLayer();
                        UIActivityLayer.HideUIActivityLayer();
                        UIChessboardLayer.HideLayer();
                        layer.OnHidEvent.AddListener(() =>
                        {
                            TutorialHandler.Triggered(TutorialHandler.FirstStandLandKey);
                            UIStaticsLayer.ShowUIStaticsLayer();
                            UIActivityLayer.ShowUIActivityLayer();
                            UIChessboardLayer.ShowLayer();
                        });
                        layer.Show();
                    }
                }),
                new WaitForTrueEvent(() => !TutorialHandler.IsTriggerable(TutorialHandler.FirstStandLandKey)),
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new CustomEvent(() => { SoundAPI.PlaySound(AudioInstance.Instance.Settings.goodSmall); }),
                new GameObjectVisibilityEvent(cashEffectObject, 3f),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => Coin),
                new CustomEvent(() => { PlayerSandbox.Instance.CharacterHandler.Coin += Coin; }),
                new LandTileOptionEvent(this, Timer),
            };
        }

        #endregion
        
        #region Callbacks

        protected override void OnLanguageChanged(string previousLanguage, string newLanguage)
        {
            base.OnLanguageChanged(previousLanguage, newLanguage);
            UpdateLevelText();
        }

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if(service != GameSessionAPI.ChessBoardAPI.ServiceName) return;
            if(response.GetAttachmentAsString("tile") != "land") return;
            OnReceiveArriveMessage(response, service, method);
            OnOptionMessage(response, service, method);
            
        }

        private void OnReceiveArriveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (method != GSChessBoardAPI.MethodArrive) return;
            JsonConvert.SerializeObject(response.body);
            bReceiveArriveMessage = true;
            Coin = response.GetAttachmentAsInt("coin");
            Need = response.GetAttachmentAsInt("need");
            Finish = response.GetAttachmentAsInt("finish");
            Option = response.GetAttachmentAsString("option");
        }

        private void OnOptionMessage(HttpResponseProtocol response, string service, string method)
        {
            if(method != "option") return;
            JsonConvert.SerializeObject(response.body);
            var coinNeed = response.GetAttachmentAsInt("coin_need");
            PlayerSandbox.Instance.CharacterHandler.Coin -= coinNeed;
            // 解锁
            PerformLocked(false);

            var task = new List<IQueueableEvent>
            {
                new ModifyNumWSEffectEvent<int>(transform.position,
                    EffectInstance.Instance.Settings.uiEffectMinusCoinNumber, () => -Math.Abs(coinNeed))
            };
            
            EventQueue.Instance.Enqueue(task);
        }

        #endregion
    }

    internal class LandTileOptionEvent : ChessTileEvent
    {
        #region Properties

        private UIWSTimer Time { get; set; }

        private ChessTile Tile { get; set; }
        
        private Func<int> GetNeed { get; set; }
        private Func<int> GetFinish { get; set; }
        private Func<string> GetOption { get; set; }

        #endregion
        
        #region ChessTileEvent

        public LandTileOptionEvent(ChessTile tile, UIWSTimer upgradeTime ) : base(tile)
        {
            Tile = tile;
            GetNeed = () => ((ChessTileLand)tile).Need;
            GetFinish = () => ((ChessTileLand)tile).Finish;
            GetOption = () => ((ChessTileLand)tile).Option;
            Time = upgradeTime;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            OptionLogic();
        }
        
        #endregion

        #region Functions

        private void OptionLogic()
        {
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            var landTile = Tile as ChessTileLand;
            Debug.Assert(landTile);

            var tipLocation = landTile.GetStandPosition();
            switch (GetOption())
            {
                case "acc":
                    if (Time)
                    {
                        var remainTime = (int)Time.GetRemainingTime() - 600;
                        if(remainTime < 0) remainTime = 0;
                        Time.UpdateTime(remainTime);
                        
                        // 存储数据到本地
                        var data = PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.GetDataByIndex(Tile.TileIndex);
                        PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.SetUpgradeTimeByIndex(Tile.TileIndex, data.startTs, data.finishTs - 600);
                    }
                    break;
                case "upgrade":
                    var task = new List<IQueueableEvent>()
                    {
                        new CustomEvent(execute: () =>
                        {
                            UILandUpgradeLayer.ShowUILandUpgradeLayer(GetNeed(), finish =>
                            {
                                var remainTime = (int)finish - TimeAPI.GetVietnamTimeStamp();
                                landTile.SetUpUpgradeTimer(remainTime, remainTime);
                                PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands.SetUpgradeTimeByIndex(Tile.TileIndex, TimeAPI.GetVietnamTimeStamp(), (int)finish);
                            });
                            
                            if (GameInstance.Instance.HostingHandler.Hosting)
                            {
                                UILandUpgradeLayer.GetLayer().OnConfirmClick(null);
                            }
                        }),
                        new WaitForTrueEvent(() => !UILandUpgradeLayer.GetLayer().IsShowing)
                    };
                    EventQueue.Instance.Enqueue(task);
                    break;
                case "need_coin":
                    layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiEffectLandNoCoin, tipLocation);
                    break;
                case "limit":
                    layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiEffectLandLvLimit, tipLocation);
                    break;
                case "locked":
                    landTile.PerformLocked(true);
                    break;
                case "unlock":
                    // TODO: 地块解锁确认
                    // ...
                    UIPaymentLayer.ShowLayer(coin:(long)(10000 * PlayerSandbox.Instance.ChessBoardHandler.Lands.GetLandInfoByLevel(landTile.GetTileData().level).standMul), onConfirm: () =>
                    {
                        GameSessionAPI.ChessBoardAPI.Option(null);
                        // landTile.PerformLocked(false);
                    });

                    if (GameInstance.Instance.HostingHandler.Hosting)
                    {
                        UIPaymentLayer.GetLayer().Confirm();
                    }
                    break;
                case "max":
                    layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiEffectLandLvMax, tipLocation);
                    break;
            }
        }

        #endregion
    }

}