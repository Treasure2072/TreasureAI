using System;
using System.Collections.Generic;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIScratchLayer : UILayer
    {
        #region Define

        public enum EGroupType
        {
            Chest,
            Dollar,
            Coin
        }

        #endregion
        
        #region Fields

        [Header("References")]
        [SerializeField] private ScratchGroup chestGroup;
        [SerializeField] private ScratchGroup dollarGroup;
        [SerializeField] private ScratchGroup coinGroup;

        #endregion

        #region Properties

        private int ChestNumber { get; set; }
        private int DollarNumber { get; set; }
        private int CoinNumber { get; set; }

        #endregion

        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            ChestNumber = 0;
            DollarNumber = 0;
            CoinNumber = 0;
        }

        #endregion

        #region Function

        private void Add(EGroupType groupType)
        {
            switch (groupType)
            {
                case EGroupType.Chest:
                    ChestNumber++;
                    chestGroup.ActiveForNumber(ChestNumber);
                    break;
                case EGroupType.Dollar:
                    DollarNumber++;
                    dollarGroup.ActiveForNumber(DollarNumber);
                    break;
                case EGroupType.Coin:
                    CoinNumber++;
                    coinGroup.ActiveForNumber(CoinNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupType), groupType, null);
            }
        }

        #endregion

        #region API

        public void SetGroupsCoin((int, int, int) leaderboard)
        {
            // var baseCoin = val switch
            // {
            //     0 => coin / 3.5f,
            //     1 => coin / 2.5f,
            //     2 => coin / 1.5f,
            //     _ => throw new ArgumentOutOfRangeException(nameof(val), val, null)
            // };
            
            chestGroup.Init(leaderboard.Item1);
            dollarGroup.Init(leaderboard.Item2);
            coinGroup.Init(leaderboard.Item3);
        }
        
        public static UIScratchLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIScratchLayer>("UIScratchLayer");
            Assert.IsNotNull(layer);
            return layer;
        }

        public static void ShowUIScratchLayer()
        {
            var layer = GetLayer();
            layer.Show();
        }

        public static void HideUIScratchLayer()
        {
            var layer = GetLayer();
            layer.Hide();
        }

        public static void AddScratch(EGroupType groupType)
        {
            var layer = UIManager.Instance.GetLayer<UIScratchLayer>("UIScratchLayer");
            Assert.IsNotNull(layer);
            layer.Add(groupType);
        }

        #endregion
    }

}