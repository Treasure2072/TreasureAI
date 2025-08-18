using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    
    [Serializable]
    public struct FChessBoardData
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnassignedField.Global
        public Dictionary<int, string> items;
        
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnassignedField.Global
        public Dictionary<int, FChessBoardLandTile> lands;
    }

    [Serializable]
    public struct FChessBoardLandTile
    {
        public int level;
        public int startTs;
        public int finishTs;
        public bool locked;
    }

    [Serializable]
    public struct FChessBoardBankLogs
    {
        public List<FChessBoardBankLog> logs;
    }
        
    [Serializable]
    public struct FChessBoardBankLog
    {
        public int time;
        public int coin;
        public string user;
        public string name;
        public int avt;
    }
    
    [Serializable]
    public struct FLandGrade
    {
        public int level;
        public float priceMul;
        public float rewardMul;
    }

    [Serializable]
    public struct FLand
    {
        public int level;
        public float standMul;
        public float upgradeMulCoin;
        public float upgradeMulToken;
        public int upgradeDuration;
        public int requireLevel;
    }

    [Serializable]
    public struct FLandPrice
    {
        public int level;
        public int required;
        public int price;
        public int rowId;
    }
    
    [Serializable]
    public struct FShort
    {
        public int index;
        
        /// <summary>
        /// 抢夺倍率
        /// </summary>
        public float mul;
        
        /// <summary>
        /// 出现概率（这个值在客户端无作用）
        /// </summary>
        public float chance;

        public int rowId;
    }

    [Serializable]
    public struct FChance
    {
        public int index;
        public float coinMul;
        public int dice;
        public float chance;
        public bool special;
        public int rowId;
    }

    [Serializable]
    public struct FChessBoard
    {
        public int id;
        public float coinMul;
        public int dice;
        public float price;
        public int rowId;
    }
        
    [Serializable]
    public struct FBankData
    {
        private List<FChessBoardBankLog> logs;
        private List<string> invites;

        public IReadOnlyList<FChessBoardBankLog> GetLogs()
        {
            return logs == null ? new List<FChessBoardBankLog>() : logs.AsReadOnly();
        }
        
        public IReadOnlyList<string> GetInvites()
        {
            return invites == null ? new List<string>() : invites.AsReadOnly();
        }
    }

    [Serializable]
    public struct FEntityUser
    {
        public string id;
        public string name;
        public string email;
        public string code;
        public string inviterEmail;
    }
    public static class ChessBoardType
    {

    }

}