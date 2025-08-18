using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIBuildAreaDescription : MonoBehaviour
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI tmpLevel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI tmpCoin;
        [SerializeField] private TextMeshProUGUI tmpToken;
        [SerializeField] private TextMeshProUGUI tmpTime;
        [SerializeField] private TextMeshProUGUI tmpTokenProductionDaily;

        #endregion

        #region API

        public void Setup(Sprite icon, int level, string areaName, int coin, int token, int time,
            int tokenProductionDaily)
        {
            SetIcon(icon);
            SetLevel(level);
            SetName(areaName);
            SetCoin(coin);
            SetToken(token);
            SetTime(time);
            SetTokenProductionDaily(tokenProductionDaily);
        }
        
        public void SetIcon(Sprite icon)
        {
            if (!iconImage) return;
            iconImage.sprite = icon;
        }

        public void SetLevel(int level)
        {
            tmpLevel.text = level.ToString();
        }

        public void SetName(string areaName)
        {
            nameText.text = areaName;
        }

        public void SetCoin(int coin)
        {
            tmpCoin.text = NumberUtils.GetDisplayNumberStringAsCurrency(coin);
        }

        public void SetToken(int token)
        {
            tmpToken.text = NumberUtils.GetDisplayNumberStringAsCurrency(token);
        }

        public void SetTime(int time)
        {
            tmpTime.text = NumberUtils.GetDisplayNumberStringAsDuration(time);
        }

        public void SetTokenProductionDaily(int tokenProductionDaily)
        {
            tmpTokenProductionDaily.text = $"{NumberUtils.GetDisplayNumberStringAsCurrency(tokenProductionDaily)}/{this.GetLocalizedText("day-acronym")}";
        }

        public void SetDes(int level, string areaName, int coin, int token, int time, int tokenProductionDaily)
        {
            tmpLevel.text = $"{level}";
            nameText.text = areaName;
            tmpCoin.text = NumberUtils.GetDisplayNumberStringAsCurrency(coin);
            tmpToken.text = NumberUtils.GetDisplayNumberStringAsCurrency(token);
            tmpTime.text = NumberUtils.GetDisplayNumberStringAsDuration(time);
            tmpTokenProductionDaily.text = $"{NumberUtils.GetDisplayNumberStringAsCurrency(tokenProductionDaily)}/{this.GetLocalizedText("day-acronym")}";
        }

        #endregion
    }
}