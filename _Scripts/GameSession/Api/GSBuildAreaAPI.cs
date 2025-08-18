using System;
using System.Collections.Generic;
using _Scripts.Utils;

namespace Game
{
    public class GSBuildAreaAPI : GameSessionAPIImpl
    {
        public static readonly string MethodQuery = "query";
        
        /// <summary>
        /// 查询建造区域信息 返回 BuildAreas
        /// </summary>
        public void Query()
        {
            SendMessage(CreateRequest(MethodQuery));
        }

        /// <summary>
        /// 解锁槽位，前提是区域已解锁
        /// coin: 花费的Coin
        /// token: 花费的Token
        /// finish-time: 完成时间
        /// </summary>
        /// <param name="slotId">槽位的id，目前是1-18</param>
        public void Unlock(int slotId)
        {
            var request = CreateRequest("unlock");
            request.AddBodyParams("slot", slotId);
            SendMessage(request);
        }

        /// <summary>
        /// 解锁区域，前提是上一个区域已经完全建造
        /// coin: 花费的Coin
        /// </summary>
        /// <param name="areaId">区域id</param>
        public void UnlockArea(int areaId)
        {
            var request = CreateRequest("unlock_area");
            request.AddBodyParams("area", areaId);
            SendMessage(request);
        }
        
        /// <summary>
        /// 升级槽位，前提是已经解锁
        /// coin: 花费的Coin
        /// token: 花费的Token
        /// finish-time: 完成时间
        /// </summary>
        /// <param name="slotId">槽位ID</param>
        public void Upgrade(int slotId)
        {
            var request = CreateRequest("upgrade");
            request.AddBodyParams("slot", slotId);
            SendMessage(request);
        }
        
        /// <summary>
        /// 收集一个区域的所有槽位的收益，返回列表[3, 6, 9]
        /// tokens: list(int)
        /// </summary>
        /// <param name="areaId"></param>
        [Obsolete]
        public void Withdraw(int areaId)
        {
            var request = CreateRequest("withdraw");
            request.AddBodyParams("area", areaId);
            SendMessage(request);
        }

        /// <summary>
        /// 收集一个槽位
        /// token: int
        /// </summary>
        /// <param name="areaId">1-3</param>
        /// <param name="slotId">1-18</param>
        public void WithdrawSlot(int areaId, int slotId)
        {
            var request = CreateRequest("withdraw");
            request.AddBodyParams("area", areaId);
            request.AddBodyParams("slot", slotId);
            SendMessage(request);
        }
                
        protected override string GetServiceName()
        {
            return "build";
        }
    }
    
    [Serializable]
    public struct BuildAreas
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnassignedField.Global
        public Dictionary<int, BuildArea> areas;

        public bool IsUnlocked(int areaId)
        {
            return areas.ContainsKey(areaId);
        }

        public void Unlock(int areaId)
        {
            if (areas.ContainsKey(areaId)) return;
            var newArea = new BuildArea
            {
                slots = new Dictionary<int, BuildAreaSlot>()
            };
            areas.Add(areaId, newArea);
        }
    }
    
    [Serializable]
    public struct BuildArea
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnassignedField.Global
        public Dictionary<int, BuildAreaSlot> slots;
    }
    
    [Serializable]
    public struct BuildAreaSlot
    {
        public int level;
        public int startTime;
        public int endTime;
        public int harvestTime;

        /// <summary>
        /// 是否在修建中
        /// </summary>
        /// <returns></returns>
        public bool IsUpgrading()
        {
            return TimeAPI.GetUtcTimeStamp() < endTime;
        }

        /// <summary>
        /// 获取当前时间和总时间
        /// </summary>
        /// <returns></returns>
        public (int startTs, int endTs) GetTime()
        {
            return (startTime, endTime);
        }

        /// <summary>
        /// 是否可以收获
        /// </summary>
        /// <returns></returns>
        public bool IsHarvest()
        {
            return TimeAPI.GetUtcTimeStamp() >= harvestTime;
        }
    }
    
    /// <summary>
    /// 建筑区系数表
    /// </summary>
    [Serializable]
    public struct FBuildSlotRate
    {
        public int slot;
        public int area;
        public int subId;
        public float coin;
        public float token;
        public float durationRate;
        public float production;
        public int rowId;
    }

    /// <summary>
    /// 建筑区基数表
    /// </summary>
    [Serializable]
    public struct FBuildSlot
    {
        public int level;
        public int coin;
        public int token;
        public int duration;
        public int production;
        public int productionMax;
        public int rowId;
    }

    [Serializable]
    public struct FBuildArea
    {
        public int area;
        public int coin;
        public int token;
        public int rowId;
    }
    
}

