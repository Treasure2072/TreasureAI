using _Scripts.UI.Common.Grids;
using Data;
using DragonLi.UI;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ga
{
    public class OrdersGrid : GridBase
    {
        #region Property

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI tmpStatus;
        [SerializeField] private TextMeshProUGUI tmpNumber;
        [SerializeField] private Button btn;

        private FPayment Payment { get; set; }
        
        #endregion

        #region GridBase

        protected override void OnInitialized()
        {
            base.OnInitialized();
            btn.onClick.AddListener(OnSelectClick);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            btn.onClick.RemoveListener(OnSelectClick);
        }

        public override void SetGrid(params object[] args)
        {
            base.SetGrid(args);
            var payment = (FPayment)args[0];
            Payment = payment;
            var status = (EPaymentStatus)payment.status;
            var order = UIOrdersLayer.GetLayer().Orders.GetOrder(status);
            SetIcon(order.icon);
            SetIconSize(order.sizeDelta);
            SetNumber(payment.paymentId);
            SetStatus(status);
        }

        #endregion

        #region Function

        private void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        private void SetIconSize(Vector2 size)
        {
            icon.rectTransform.sizeDelta = size;
        }

        private void SetNumber(string number)
        {
            tmpNumber.SetText($"{this.GetLocalizedText("no.")}{number[..10]}...");
        }

        private void SetStatus(EPaymentStatus status)
        {
            var statusLocalizationKey = status switch
            {
                EPaymentStatus.Canceled => "status-canceled",
                EPaymentStatus.Created => "status-created",
                EPaymentStatus.Paid => "status-paid",
                EPaymentStatus.Failed => "status-failed",
                _ => ""
            };
            tmpStatus.SetText($"{this.GetLocalizedText("status")}: {this.GetLocalizedText(statusLocalizationKey)}");
        }
        
        #endregion

        #region Callback

        private void OnSelectClick()
        {
            var layer = UIOrdersLayer.GetLayer();
            var details = layer.OrderDetails;
            details.SetNumber(Payment.paymentId);
            details.SetStatus((EPaymentStatus)Payment.status);
            
            var info = PlayerSandbox.Instance.ChessBoardHandler.Shops.GetShopInfoById(Payment.product);
            details.SetAmount(info.count);
            details.SetPublicKey(Payment.hash);
            details.SetProduct(Payment.product);
            details.SetCreateTime(Payment.time);
            details.SetPaymentTime(Payment.pay_time);
            layer?.OpenOrderDetails();
        }

        #endregion
    }
}