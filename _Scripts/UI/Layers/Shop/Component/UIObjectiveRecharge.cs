using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIObjectiveRecharge : UIComponent
    {
        #region Field

        [Header("Reference")]
        [SerializeField] private Image imgFill;
        [SerializeField] private TextMeshProUGUI tmpContent;

        #endregion

        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnRechargeChanged += OnRechargeChanged;
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnRechargeChanged -= OnRechargeChanged;
        }

        #endregion

        #region UIComponent

        public override void OnShow()
        {
            base.OnShow();
            Refresh();
        }

        #endregion

        #region Function

        private void Refresh()
        {
            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentLimitByRecharge(recharge);
            SetContent(info.sum - recharge, info.token);
            SetFill(recharge / info.sum);
        }
        
        private void SetContent(float sum, long token)
        {
            tmpContent.SetText(string.Format(this.GetLocalizedText("shop-payment-fmt"), sum, token));
        }

        private void SetFill(float amount)
        {
            imgFill.fillAmount = amount;
        }

        #endregion

        #region Callback

        private void OnLanguageChanged(string previous, string current)
        {
            Refresh();
        }

        private void OnRechargeChanged(float? preVal, float newVal)
        {
            Refresh();
        }

        #endregion
    }
}