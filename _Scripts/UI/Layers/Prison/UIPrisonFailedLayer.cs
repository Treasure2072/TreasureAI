using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIPrisonFailedLayer : UILayer
    {
        #region Fields

        [Header("References")]
        [SerializeField] private TextMeshProUGUI TMP_PunishCoin;

        #endregion

        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            // SetPunishCoin(0);
        }

        #endregion

        #region API

        public static UIPrisonFailedLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPrisonFailedLayer>("UIPrisonFailedLayer");
            Assert.IsNotNull(layer);
            return layer;
        }

        public void SetPunishCoin(int coin)
        {
            TMP_PunishCoin.text = $"{coin}";
        }

        #endregion
    }
}
