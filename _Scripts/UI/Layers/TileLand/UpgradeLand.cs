using System;
using _Scripts.UI.Common.Grids;
using DragonLi.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UpgradeLand : GridBase
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private Image houseImage;
        [SerializeField] private TextMeshProUGUI tmpLevel;
        [SerializeField] private TextMeshProUGUI tmpCoin;

        #endregion

        #region API

        public void SetHouseColor(Color color)
        {
            houseImage.color = color;
        }

        public void SetLevel(int level)
        {
            tmpLevel.text = level.ToString();
        }

        public void SetCoin(long needCoin)
        {
            tmpCoin.text = NumberUtils.GetDisplayNumberString(needCoin);
        }

        #endregion
    }

}