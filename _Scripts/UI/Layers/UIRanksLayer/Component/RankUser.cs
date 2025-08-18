using _Scripts.UI.Common.Grids;
using Data.Type;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class RankUser : GridBase
    {
        #region Properties
        
        [Header("Settings - First")]
        [SerializeField] private Sprite avatarFirst;
        [SerializeField] private Sprite bgFirst;
        [SerializeField] private Color colFirst;
        
        [Header("Settings - Second")]
        [SerializeField] private Sprite avatarSecond;
        [SerializeField] private Sprite bgSecond;
        [SerializeField] private Color colSecond;
        
        [Header("Settings - Third")]
        [SerializeField] private Sprite avatarThird;
        [SerializeField] private Sprite bgThird;
        [SerializeField] private Color colThird;
        
        [Header("Settings - Other")]
        [SerializeField] private Sprite avatarOther;
        [SerializeField] private Sprite bgOther;
        [SerializeField] private Color colOther;
        
        [Header("References")]
        [SerializeField] private RectTransform signTransform;
        [SerializeField] private TextMeshProUGUI tmpRank;
        [SerializeField] private Image imgRank;
        [SerializeField] private Image imgBg;
        [SerializeField] private TextMeshProUGUI tmpName;
        [SerializeField] private TextMeshProUGUI tmpNum;

        #endregion

        #region GridBase

        public override void SetGrid<T>(params object[] args)
        {
            base.SetGrid<T>(args);
            var data = args[0] as RankHandlerType.FUser? ?? default;
            var rank = (int)args[1];
            SetName(data.name);
            SetNum(data.data);
            SetRank(rank);
        }

        #endregion

        #region Function

        private void SetRank(int rank)
        {
            if(tmpRank == null) return;
            rank = Mathf.Clamp(rank, 0, int.MaxValue);
            tmpRank.text = rank < 999 ? $"No.{rank + 1}" : "No.999+";
            
            SetRankSign(rank);
            SetRankBg(rank);
            SetColorOfText(rank);
        }

        private void SetRankSign(int rank)
        {
            if(imgRank == null) return;
            imgRank.sprite = rank switch
            {
                0 => avatarFirst,
                1 => avatarSecond,
                2 => avatarThird,
                _ => avatarOther
            };

            if (rank <= 2 && rank >= 0)
            {
                signTransform.sizeDelta = Vector2.one * 150;
                signTransform.anchoredPosition = new Vector2(-351.8f, 3.2f);
            }
            else
            {
                signTransform.sizeDelta = Vector2.one * 128;
                signTransform.anchoredPosition = new Vector2(-340f, 8f);
            }
        }

        private void SetRankBg(int rank)
        {
            if(imgBg == null) return;
            imgBg.sprite = rank switch
            {
                0 => bgFirst,
                1 => bgSecond,
                2 => bgThird,
                _ => bgOther
            };
        }

        private void SetColorOfText(int rank)
        {
            var color = rank switch
            {
                0 => colFirst,
                1 => colSecond,
                2 => colThird,
                _ => colOther
            };
            tmpRank.color = color;
            tmpName.color = color;
            tmpNum.color = color;
        }

        private void SetName(string userName)
        {
            if(tmpName == null) return;
            tmpName.SetText(userName);
        }

        private void SetNum(long num)
        {
            if(tmpNum == null) return;
            var dice = num <= 99999 ? $"{NumberUtils.GetDisplayNumberStringAsCurrency(num)}" : $"{NumberUtils.GetDisplayNumberStringAsCurrency(99999)}+";
            tmpNum.text = $"{this.GetLocalizedText("dice")} : {dice}";
        }

        #endregion
    }
}