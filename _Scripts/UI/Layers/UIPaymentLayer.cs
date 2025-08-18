using System;
using _Scripts.UI.Common;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIPaymentLayer : UILayer
    {
        #region Properties

        [Header("Settings")] 
        [SerializeField] private GameObject coinNode;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameObject tokenNode;
        [SerializeField] private TextMeshProUGUI tokenText;
        [SerializeField] private GameObject durationNode;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private GameObject moneyNode;
        [SerializeField] private TextMeshProUGUI moneyText;

        private Action OnConfirmCallback { get; set; }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["ButtonOK"].As<UIBasicButton>().OnClickEvent.AddListener(OnButtonOkPressed);
            this["ButtonCancel"].As<UIBasicButton>().OnClickEvent.AddListener(OnButtonCancelPressed);
        }

        protected override void OnHide()
        {
            base.OnHide();
            OnConfirmCallback = null;
        }

        #endregion
        
        #region API

        public static UIPaymentLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPaymentLayer>("UIPaymentLayer");
            Debug.Assert(layer);
            return layer;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="coin">游戏金币</param>
        /// <param name="token">游戏代币</param>
        /// <param name="durationSec">时间</param>
        /// <param name="currency">现金</param>
        /// <param name="onConfirm"></param>
        public static void ShowLayer(long coin = -1, float token = -1, int durationSec = -1, decimal currency = -1, Action onConfirm = null)
        {
            GetLayer()?.SetupPayment(coin, token, durationSec, currency, onConfirm);
            GetLayer()?.Show();
        }

        public void Confirm()
        {
            OnButtonOkPressed(this["ButtonOK"].As<UIBasicButton>());
        }

        #endregion

        #region Function

        /// <summary>
        /// 设置支付账单，设置为-1表示无此项
        /// </summary>
        /// <param name="coin">金币</param>
        /// <param name="token">Token</param>
        /// <param name="durationSec">消耗的时间，秒为单位</param>
        /// <param name="currency">货币</param>
        /// <param name="onConfirm"></param>
        private void SetupPayment(long coin = -1, float token = -1, int durationSec = -1, decimal currency = -1, Action onConfirm = null)
        {
            coinNode.SetActive(coin > 0);
            coinText.text = $"x{NumberUtils.GetDisplayNumberStringAsCurrency(coin)}";
            
            tokenNode.SetActive(token > 0);
            tokenText.text = $"x{NumberUtils.GetDisplayNumberStringAsCurrency(token, 2)}";
            
            durationNode.SetActive(durationSec > 0);
            durationText.text = $"{NumberUtils.GetDisplayNumberStringAsDuration(durationSec)}";
            
            moneyNode.SetActive(currency > 0);
            moneyText.text = $"{currency:F}";

            OnConfirmCallback = onConfirm;
        }

        #endregion

        #region Callbacks

        private void OnButtonCancelPressed(UIBasicButton sender)
        {
            Hide();
        }

        private void OnButtonOkPressed(UIBasicButton sender)
        {
            OnConfirmCallback?.Invoke();
            Hide();
        }

        #endregion
    }
}


