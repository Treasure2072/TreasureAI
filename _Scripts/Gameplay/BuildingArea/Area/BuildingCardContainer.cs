using System;
using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using DragonLi.Frame;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class BuildingCardContainer : GridsContainerBase
    {
        #region Properties

        private Dictionary<int, BuildAreaSlot> BuildAreaData { get; set; }
        private UnityAction<BuildingAreaCard> SelectCallback { get; set; }
        
        private int SlotCount { get; set; }

        #endregion
        
        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);
            BuildAreaData = (Dictionary<int, BuildAreaSlot>)args[0];
            SelectCallback = args[1] as UnityAction<BuildingAreaCard>;
            SlotCount = (int)args[2];

            for (var i = 0; i < SlotCount; i++)
            {
                var id = i + 1;
                var card = SpawnGrid<BuildingAreaCard>();
                card.Setup(i);
                card.OnSelectOperated.RemoveAllListeners();
                card.OnSelectOperated.AddListener(SelectCallback);
                card.ClearWorldElements();
                
                if (!BuildAreaData.ContainsKey(id))
                {
                    // 未解锁
                    // ...
                    card.SetType(BuildingAreaType.EBuildAreaType.Locked);
                }
                else
                {
                    BuildAreaData.TryGetValue(id, out var buildAreaSlotData);
                    if (buildAreaSlotData.IsUpgrading())
                    {
                        card.SetType(BuildingAreaType.EBuildAreaType.Upgrading, buildAreaSlotData.GetTime());
                    }
                    else
                    {
                        var type = buildAreaSlotData.level == 0 ? BuildingAreaType.EBuildAreaType.NotBuilt : BuildingAreaType.EBuildAreaType.NotUpgraded;
                        card.SetType(type);
                    }
                }
            }
        }

        #endregion
        
        #region API

        /// <summary>
        /// 根据 index 设置卡片样式
        /// </summary>
        /// <param name="index">卡片索引</param>
        /// <param name="type">建筑区域当前状态类型</param>
        /// <param name="timeSpan">如果正在修建，还剩余完成的时间</param>
        public void SetCardTypeByIndex(int index, BuildingAreaType.EBuildAreaType type, (int startTs, int endTs) timeSpan = default)
        {
            var card = GetGridByIndex<BuildingAreaCard>(index);
            card.SetType(type, timeSpan);
            card.OnUpdateUIWorldElementCallback(type);
        }

        #endregion
    }
}