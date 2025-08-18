using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class UIWalletLayer : UILayer
    {
        #region Property

        [SerializeField] private string product = "dice-pack-01";
        [SerializeField] private string account = "0xa9507dDd4B05152609Ec071C753C851cDa7Bbcf1";
        [SerializeField] private string platform = "none";
        [SerializeField] private string id;
        [SerializeField] private string hash = "0xf0ffdc76594d0e6c6d483be1293b4840219d95b893f29e3e74c9e8ad8fbe73ef";

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["CreatePayment"].As<UIBasicButton>().OnClickEvent.AddListener(CreatePayment);
            this["UpdatePayment"].As<UIBasicButton>().OnClickEvent.AddListener(UpdatePayment);
            this["QueryPaymentById"].As<UIBasicButton>().OnClickEvent.AddListener(QueryPaymentById);
            this["QueryPaymentByUserId"].As<UIBasicButton>().OnClickEvent.AddListener(QueryPaymentByUserId);
            this["CancelPayment"].As<UIBasicButton>().OnClickEvent.AddListener(CancelPayment);
        }

        #endregion
        
        #region API

        public static UIWalletLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIWalletLayer>(nameof(UIWalletLayer));
            Debug.Assert(layer);
            return layer;
        }

        #endregion

        #region Callback

        private void CreatePayment(UIBasicButton sender)
        {
            // GameSessionAPI.PaymentAPI.CreatePayment(product, account,
            //     PlayerSandbox.Instance.RegisterAndLoginHandler.Email, platform,
            //     response =>
            //     {
            //         var amount = response.GetAttachmentAsInt("amount");
            //         id = response.GetAttachmentAsString("payment-id");
            //     });
        }

        private void UpdatePayment(UIBasicButton sender)
        {
            // GameSessionAPI.PaymentAPI.UpdatePayment(id, hash, response =>
            // {
            //     this.LogEditorOnly(JsonConvert.SerializeObject(response));
            // });
        }

        private void QueryPaymentById(UIBasicButton sender)
        {
            // GameSessionAPI.PaymentAPI.QueryPaymentById(id, response =>
            // {
            //     this.LogEditorOnly(JsonConvert.SerializeObject(response));
            // });
        }

        private void QueryPaymentByUserId(UIBasicButton sender)
        {
            // GameSessionAPI.PaymentAPI.QueryPaymentByUserId(PlayerSandbox.Instance.RegisterAndLoginHandler.Id,
            //     response =>
            //     {
            //         this.LogEditorOnly(JsonConvert.SerializeObject(response));
            //     });
        }

        private void CancelPayment(UIBasicButton sender)
        {
            // GameSessionAPI.PaymentAPI.CancelPayment(id, response =>
            // {
            //     this.LogEditorOnly(JsonConvert.SerializeObject(response));
            // });
        }

        #endregion
    }
}