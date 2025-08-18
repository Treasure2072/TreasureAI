using Data;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class SelfRankComponent : UIComponent
    {
        #region Property

        [SerializeField] private TextMeshProUGUI tmpName;
        [SerializeField] private TextMeshProUGUI tmpRank;
        [SerializeField] private TextMeshProUGUI tmpCoin;

        #endregion
        
        #region UIComponent

        public override void OnShow()
        {
            base.OnShow();
            tmpName.SetText(PlayerSandbox.Instance.RegisterAndLoginHandler.Name);
            tmpRank.SetText($"No.{PlayerSandbox.Instance.RankHandler.SelfRank}".ToUpper());
            tmpCoin.SetText($"{this.GetLocalizedText("dice")}:{NumberUtils.GetDisplayNumberStringAsCurrency(PlayerSandbox.Instance.RankHandler.SelfUsedDice)}");
        }

        #endregion
    }
}