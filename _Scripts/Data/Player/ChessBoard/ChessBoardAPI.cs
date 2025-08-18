using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Game
{
    public static class ChessBoardAPI
    {
        private static Dictionary<int, string> ChessboardRouter { get; set; } = new()
        {
            { 0, "PeaceValle" },
            { 1, "ModernCity" },
            { 2, "CyberCity" }
        };

        public static IReadOnlyDictionary<int, string> GetChessboardRouter()
        {
            return ChessboardRouter;
        }
        
        public static string GetChessBoardByIndex(int index)
        {
            return GetChessboardRouter().GetValueOrDefault(index);
        }
        
        /// <summary>
        /// 得到当前主场景棋盘的场景名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentChessBoard()
        {
            return GetChessBoardByIndex(PlayerSandbox.Instance.CharacterHandler.ChessboardId);
        }

        public static FChessBoardLandTile GetDataByIndex(this Dictionary<int, FChessBoardLandTile> lands, int index)
        {
            if (!lands.ContainsKey(index))
            {
                Debug.LogWarning($"Tile uninclude {index}");
            }
            
            return lands.TryGetValue(index, out var value) ? value : new FChessBoardLandTile
            {
                level = 1,
                finishTs = 0,
                locked = false,
                startTs = 0,
            };;
        }

        public static void SetUpgradeTimeByIndex(this Dictionary<int, FChessBoardLandTile> lands, int index, int startTs, int finishTs)
        {
            var data = lands.GetDataByIndex(index);
            data.startTs = startTs;
            data.finishTs = finishTs;
            lands[index] = data;
        }

        public static void SetLockStatusByIndex(this Dictionary<int, FChessBoardLandTile> lands, int index, bool locked)
        {
            var data = lands.GetDataByIndex(index);
            data.locked = locked;
            lands[index] = data;
        }

        public static void SetLevelByIndex(this Dictionary<int, FChessBoardLandTile> lands, int index, int level)
        {
            var data = lands.GetDataByIndex(index);
            data.level = level;
            lands[index] = data;
        }
    }
}