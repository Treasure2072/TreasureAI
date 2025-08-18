using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class ShopDiceComponent : UIComponent
    {
        #region Property

        [SerializeField] private string id;
        [SerializeField] private TextMeshProUGUI tmpCount;
        [SerializeField] private TextMeshProUGUI tmpPrice;
        [SerializeField] private Button btnPurchase;

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            
            btnPurchase.onClick.AddListener(() =>
            {
                UIShopLayer.GetLayer()?.OnPurchaseAction(id);
            });
        }

        public override void OnShow()
        {
            base.OnShow();
            var info = PlayerSandbox.Instance.ChessBoardHandler.Shops.GetShopInfoById(id);
            tmpPrice.SetText($"{this.GetLocalizedText("usd")} {info.price}");
            tmpCount.SetText(info.count.ToString());
        }

        #endregion
    }
}