using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Game
{
    public static class BuildingAreaAPI
    {
        public static int GetSlotId(int areaId, int index)
        {
            return areaId switch
            {
                1 => index + 1,
                2 => 3 + index + 1,
                3 => 3 + 6 + index + 1,
                _ => 3 + 6 + 9
            };
        }

        public static (int, int) GetIndexBySlotId(int slotId)
        {
            /*
             * 1, 2, 3
             * 4, 5, 6, 7, 8, 9
             * 10, 11, 12, 13, 14, 15, 16, 17, 18
             */

            return slotId switch
            {
                <= 3 => (1, slotId - 1),
                <= 9 => (2, slotId - 3 - 1),
                _ => (3, slotId - 3 - 6 - 1)
            };
        }

        public static FBuildSlotRate GetSlotRateBySlotId(int slotId)
        {
            foreach (var slotRate in PlayerSandbox.Instance.ChessBoardHandler.BuildSlotRates.Where(slotRate => slotRate.slot == slotId))
            {
                return slotRate;
            }

            return default;
        }

        public static FBuildSlot GetSlotByLevel(int level)
        {
            foreach (var buildSlot in PlayerSandbox.Instance.ChessBoardHandler.BuildSlots.Where(slot => slot.level == level))
            {
                return buildSlot;
            }
            return default;
        }

        public static FBuildArea GetAreaByAreaId(int areaId)
        {
            foreach (var buildArea in PlayerSandbox.Instance.ChessBoardHandler.BuildAreas.Where(area => area.area == areaId))
            {
                return buildArea;
            }

            return default;
        }
        
        public static Dictionary<int, BuildAreaSlot> GetBuildingSlots(int areaId)
        {
            var buildAres = PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas;
            var areas = buildAres.areas ?? new Dictionary<int, BuildArea>();
            areas.TryGetValue(areaId, out var area);
            var slot = area.slots ?? new Dictionary<int, BuildAreaSlot>();
            return slot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaId">1,2,3</param>
        /// <param name="areaSlotId">从1开始</param>
        public static void UnlockBySlotId(int areaId, int areaSlotId)
        {
            PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas.areas.TryGetValue(areaId, out var area);
            if(area.slots == null) return;
            var succeed = area.slots.TryGetValue(areaSlotId, out var slot);
            if (!succeed)
            {
                area.slots.TryAdd(areaSlotId, default);
            }
        }
        
        public static void UpgradeBySlotId(int areaId, int areaSlotId)
        {
            PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas.areas.TryGetValue(areaId, out var area);
            if(area.slots == null) return;
            
            var succeed = area.slots.TryGetValue(areaSlotId, out var slot);
            if (!succeed) return;
            slot.level++;
            area.slots[areaSlotId] = slot;
        }
    }
}