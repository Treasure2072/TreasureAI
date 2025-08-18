using System;
using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using _Scripts.Utils;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class BuildingAreaCard : GridBase
    {
        #region Fields

        [Header("References")]
        [SerializeField] private GameObject lockObject;
        [SerializeField] private GameObject upgradeObject;
        [SerializeField] private UIBuildAreaDescription description;

        #endregion

        #region Properties
        
        private GameObject UILock { get; set; }
        private GameObject UIUpgrade  { get; set; }
        private GameObject UITimer  { get; set; }
        
        private GameObject UIBoost  { get; set; }
        
        private GameObject UIWithDraw { get; set; }
        
        private UIBuildingGroup BuildingGroup { get; set; }
        
        private Button CardButton { get; set; }
        
        private int Index { get; set; }
        
        private (int startTs, int endTs) TimeSpan  { get; set; }

        private BuildingAreaType.EBuildAreaType BuildAreaType { get; set; }

        /// <summary>
        /// 选中操作
        /// </summary>
        public UnityEvent<BuildingAreaCard> OnSelectOperated { get; } = new();
        

        #endregion

        #region Unity

        private void FixedUpdate()
        {
            UpdateCardStatusByType(BuildAreaType);
        }

        #endregion

        #region GridBase

        protected override void OnInitialized()
        {
            base.OnInitialized();
            CardButton = GetComponent<Button>();
            CardButton?.onClick.RemoveAllListeners();
            CardButton?.onClick.AddListener(() =>
            {
                OnSelectOperated.Invoke(this);
                ClearWorldElements();
                OnUpdateUIWorldElementCallback(BuildAreaType);
            });
        }

        #endregion

        #region API

        public void Setup(int index)
        {
            Index = index;
            
            var gameMode = GetGameMode();
            if(!gameMode) return;
            var buildingArea = gameMode.GetBuildingArea(Index);
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            BuildingGroup = layer.SpawnWorldElement<UIBuildingGroup>(EffectInstance.Instance.Settings.uiBuildGroup, buildingArea.transform.position);
            BuildingGroup.RemoveAll();
        }

        /// <summary>
        /// 设置卡片类型
        /// </summary>
        /// <param name="buildAreaType">建筑区域当前状态类型</param>
        /// <param name="timeSpan">如果正在修建，还剩余完成的时间数据，包括剩余时间和开始时间</param>
        public void SetType(BuildingAreaType.EBuildAreaType buildAreaType, (int startTs, int endTs) timeSpan = default)
        {
            BuildAreaType = buildAreaType;
            TimeSpan = timeSpan;
            var gameMode = GetGameMode();
            var card = BuildingAreaInstance.Instance.Settings.GetCardByType(buildAreaType);
            var slotId = BuildingAreaAPI.GetSlotId(gameMode.GetBuildingAreaIndex(), GetIndex());
            var rate = BuildingAreaAPI.GetSlotRateBySlotId(slotId);
            var basic = BuildingAreaAPI.GetSlotByLevel(GetLevel());
            description.Setup(
                card.icon,
                GetLevel(),
                $"{this.GetLocalizedText("place")} {Index + 1}",
                Mathf.RoundToInt(basic.coin * rate.coin),
                Mathf.RoundToInt(basic.token * rate.token),
                Mathf.RoundToInt(basic.duration * rate.durationRate),
                Mathf.RoundToInt(basic.production * rate.production)
                );
            
            UpdateCardStatusByType(buildAreaType);
        }

        public void ClearWorldElements()
        {
            BuildingGroup.RemoveAll();
            UILock = null;
            UIUpgrade = null;
            UITimer = null;
            UIWithDraw = null;
            UIBoost = null;
        }

        public int GetIndex()
        {
            return Index;
        }

        #endregion

        #region Function

        private void UpdateCardStatusByType(BuildingAreaType.EBuildAreaType buildAreaType)
        {
            switch (buildAreaType)
            {
                case BuildingAreaType.EBuildAreaType.Locked:
                    lockObject.SetActive(true);
                    upgradeObject.SetActive(false);
                    break;
                case BuildingAreaType.EBuildAreaType.NotBuilt:
                case BuildingAreaType.EBuildAreaType.NotUpgraded:
                    lockObject.SetActive(false);
                    upgradeObject.SetActive(false);
                    break;
                case BuildingAreaType.EBuildAreaType.Upgrading:
                    lockObject.SetActive(false);
                    upgradeObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildAreaType), buildAreaType, null);
            }
        }
        
        /// <summary>
        /// 设置锁住状态
        /// </summary>
        private void SetLock()
        {
            // var gameMode = GetGameMode();
            // if(!gameMode) return;
            // var buildingArea = gameMode.GetBuildingArea(Index);
            // if (!buildingArea) return;
            // var worldElementLayer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            // var worldElement = worldElementLayer.SpawnWorldElement<UIWorldElement>(
            //     EffectInstance.Instance.Settings.uiBuildAreaUnlockButton, buildingArea.transform.position);
            var buttonUnlock = BuildingGroup.Spawn<Button>(EffectInstance.Instance.Settings.uiBuildAreaUnlockButton);
            buttonUnlock.onClick.RemoveAllListeners();
            buttonUnlock.onClick.AddListener(() =>
            {
                // 解锁操作回调
                // ...
                BuildingGroup.Remove(UILock);
                var slotId = GetSlotId(Index);
                GameSessionAPI.BuildAreaAPI.Unlock(slotId);
            });
            
            UILock = buttonUnlock.gameObject;
        }

        /// <summary>
        /// 设置未修建状态或者未升级状态
        /// </summary>
        private void SetNotBuildOrNotUpgrade()
        {
            var gameMode = GetGameMode();
            if(!gameMode) return;
            var buildingArea = gameMode.GetBuildingArea(Index);
            if (!buildingArea) return;
            
            // 等级达到上限
            // ...
            if(buildingArea.GetLevel() >= 3) return;
            
            // var worldElementLayer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            // var worldElement = worldElementLayer.SpawnWorldElement<UIWorldElement>(
            //     EffectInstance.Instance.Settings.uiBuildAreaUpgradeButton, buildingArea.transform.position);
            // var button = worldElement.GetComponent<Button>();
            
            var button = BuildingGroup.Spawn<Button>(EffectInstance.Instance.Settings.uiBuildAreaUpgradeButton);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                // 修建，升级操作回调
                // ...
                
                var slotId = GetSlotId(Index);
                var upgradeBase = BuildingAreaAPI.GetSlotByLevel(buildingArea.GetLevel() + 1);
                var upgradeRate = BuildingAreaAPI.GetSlotRateBySlotId(slotId);
                var coin = upgradeBase.coin * upgradeRate.coin;
                var token = upgradeBase.token * upgradeRate.token;
                var duration = upgradeBase.duration * upgradeRate.durationRate;
                UIPaymentLayer.ShowLayer(
                    coin: (long)coin,
                    token: token,
                    durationSec: (int)duration,
                    onConfirm: () =>
                    {
                        BuildingGroup.Remove(UIUpgrade);
                        GameSessionAPI.BuildAreaAPI.Upgrade(slotId);
                    });
            });
            
            UIUpgrade = button.gameObject;
        }


        private void SetTime((int remainTime, int fullTime) timeSpan = default)
        {
            var gameMode = GetGameMode();
            if(!gameMode) return;

            var buildingArea = gameMode.GetBuildingArea(Index);
            if (!buildingArea) return;
            var worldElement = BuildingGroup.Spawn<UIWSTimer>(EffectInstance.Instance.Settings.uiBuildAreaUpgradeTimer.gameObject);
            worldElement.Initialize(timeSpan.remainTime, timeSpan.fullTime, () =>
            {
                // 时间结束回调
                // ...
                BuildingGroup.Remove(UITimer);
                UITimer = null;
                BuildingGroup.Remove(UIBoost);
                UIBoost = null;
                buildingArea.UpgradeEnd();
                SetType(BuildingAreaType.EBuildAreaType.NotUpgraded);
            });
            
            UITimer = worldElement.gameObject;
        }

        private void SetBoost()
        {
            var buttonBoost = BuildingGroup.Spawn<Button>(EffectInstance.Instance.Settings.uiBuildAreaBoost);
            buttonBoost.onClick.RemoveAllListeners();
            buttonBoost.onClick.AddListener(() =>
            {
                // TODO: 使用物品加速
                // ...

                if (CanBoostBySlotId(GetSlotId(Index)))
                {
                    var content = string.Format(this.GetLocalizedText("item-use-fmt2"), 1, this.GetLocalizedText($"construction-bp-{GetSlotId(Index):D2}"));
                    UIConfirmLayer.DisplayConfirmation(content, option =>
                    {
                        if (option)
                        {
                            // TODO: 使用Boost
                            // ...
                        }
                    });
                }
                else
                {
                    UITipLayer.DisplayTip(this.GetLocalizedText("notice"), this.GetLocalizedText("notice_title"));
                }
            });
        }

        private void SetWithDraw()
        {
            if (GetLevel() < 1) return;
            
            var buttonWithDraw = BuildingGroup.Spawn<Button>(EffectInstance.Instance.Settings.uiBuildAreaWithDraw);
            buttonWithDraw.onClick.RemoveAllListeners();
            buttonWithDraw.onClick.AddListener(() =>
            {
                GameSessionAPI.BuildAreaAPI.WithdrawSlot(GetAreaIndex(), GetSlotId(Index));
            });
            
            UIWithDraw = buttonWithDraw.gameObject;
        }

        private bool CanBoostBySlotId(int slotId)
        {
            return PlayerSandbox.Instance.CharacterHandler.Items.ContainsKey($"construction-bp-{slotId:D2}");
        }
        
        private static BuildingAreaGameMode GetGameMode()
        {
            return GameMode.GetGameMode<BuildingAreaGameMode>();
        }

        /// <summary>
        /// 获得区域id
        /// </summary>
        /// <returns></returns>
        private int GetAreaIndex()
        {
            return GetGameMode().GetBuildingAreaIndex();;
        }

        /// <summary>
        /// 获得地块 id
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetSlotId(int index)
        {
            var areaIndex = GetGameMode().GetBuildingAreaIndex();
            return BuildingAreaAPI.GetSlotId(areaIndex, index);
        }

        /// <summary>
        /// 获取当前地块等级
        /// </summary>
        /// <returns></returns>
        private int GetLevel()
        {
            var gameMode = GetGameMode();
            var index = GetIndex();
            var build = gameMode.GetBuildingArea(index);
            return build.GetLevel();
        }

        #endregion

        #region Callback

        public void OnUpdateUIWorldElementCallback(BuildingAreaType.EBuildAreaType buildAreaType)
        {
            switch (buildAreaType)
            {
                case BuildingAreaType.EBuildAreaType.Locked:
                    SetLock();
                    break;
                case BuildingAreaType.EBuildAreaType.NotBuilt:
                case BuildingAreaType.EBuildAreaType.NotUpgraded:
                    SetNotBuildOrNotUpgrade();
                    SetWithDraw();
                    break;
                case BuildingAreaType.EBuildAreaType.Upgrading:
                    SetTime((TimeSpan.endTs - TimeAPI.GetUtcTimeStamp(), TimeSpan.endTs - TimeSpan.startTs));
                    // SetBoost();
                    SetWithDraw();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildAreaType), buildAreaType, null);
            }
        }

        #endregion
    }

}