using System;
using System.Globalization;
using _Scripts.Utils;
using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class OrdersDetailsComponent : UIComponent
    {
        #region Property

        [SerializeField] private TextMeshProUGUI tmpNumber;
        [SerializeField] private TextMeshProUGUI tmpStatus;
        [SerializeField] private Image imgStatus;
        [SerializeField] private TextMeshProUGUI tmpPublicKey;
        [SerializeField] private TextMeshProUGUI tmpProduct;
        [SerializeField] private TextMeshProUGUI tmpAmount;
        [SerializeField] private TextMeshProUGUI tmpCreateTime;
        [SerializeField] private TextMeshProUGUI tmpPaymentTime;
        [SerializeField] private Button btnRefresh;
        [SerializeField] private Button btnCancel;

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            btnRefresh.onClick.AddListener(OnRefreshClick);
            btnCancel.onClick.AddListener(OnCancelClick);
        }

        #endregion

        #region API

        

        #endregion

        #region Function

        public void SetNumber(string number)
        {
            tmpNumber.SetText($"{this.GetLocalizedText("no.")}{number[..10]}...");
        }

        public void SetStatus(EPaymentStatus status)
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
            var order = UIOrdersLayer.GetLayer().Orders.GetOrder(status);
            imgStatus.sprite = order.icon;
            imgStatus.rectTransform.sizeDelta = order.sizeDelta * 0.6f;
        }

        public void SetPublicKey(string publicKey)
        {
            var key = publicKey != null ? $"{publicKey[..10]}..." : "";
            tmpPublicKey.SetText($"{this.GetLocalizedText("public-key")}: {key}");
        }

        public void SetProduct(string localizationKey)
        {
            tmpProduct.SetText($"{this.GetLocalizedText("production")}: {this.GetLocalizedText(localizationKey)}");
        }

        public void SetAmount(int amount)
        {
            tmpAmount.SetText($"{this.GetLocalizedText("amount")}: {amount}");
        }

        public void SetCreateTime(int time)
        {
            tmpCreateTime.text = $"{this.GetLocalizedText("creation-time")}: {TimeAPI.ConvertVietnamTimestampToLocalTime(time)}";
        }

        public void SetPaymentTime(int time)
        {
            var timeStr = time != 0 ? TimeAPI.ConvertVietnamTimestampToLocalTime(time).ToString(CultureInfo.CurrentCulture) : "— — —";
            tmpPaymentTime.text = $"{this.GetLocalizedText("payment-time")}: {timeStr}";
        }

        #endregion

        #region Callback

        private void OnRefreshClick()
        {
            
        }

        private void OnCancelClick()
        {
            
        }

        #endregion
    }
}