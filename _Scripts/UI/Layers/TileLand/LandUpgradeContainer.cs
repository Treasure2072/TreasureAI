using System;
using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using Data;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class LandUpgradeContainer : GridsContainerBase
    {
        #region Properties

        /// <summary>
        /// 所有插槽    level - slot
        /// </summary>
        private Dictionary<int, UpgradeLand> UpgradeLands { get; set; } = new();
        
        /// <summary>
        /// land数据  level - data
        /// </summary>
        private Dictionary<int, FLand> FLands { get; set; } = new();

        #endregion
        #region GridsContainerBase

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);
            
            UpgradeLands.Clear();
            FLands.Clear();
            foreach (var land in PlayerSandbox.Instance.ChessBoardHandler.Lands)
            {
                FLands.TryAdd(land.level, land);
            }

            for (var level = 1; level <= FLands.Count; level++)
            {
                var slot = SpawnGrid<UpgradeLand>();
                slot.SetLevel(level);
                slot.SetCoin(GetNeedCoin(level));
                slot.transform.localScale = Vector3.one;
                UpgradeLands.TryAdd(level, slot);
            }
        }


        #endregion

        #region Function

        private ChessGameBoard GetChessBord()
        {
            return World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey);
        }

        private long GetNeedCoin(int level)
        {
            var chessBoard = GetChessBord();
            var standIndex = PlayerSandbox.Instance.ChessBoardHandler.StandIndex;
            var standTile = chessBoard.GetTileByIndex(standIndex);

            var rate = 1f;
            if (standTile is ChessTileLand land)
            {
                rate += land.GetFeeMarkupRate();
            }
            
            return (long)Math.Round(FLands[level].upgradeMulCoin * 10000f * rate);
        }

        #endregion

        #region API

        public UpgradeLand GetUpgradeLandByLevel(int level)
        {
            return UpgradeLands.GetValueOrDefault(level);
        }

        #endregion
    }

}