using System;
using _Scripts.UI.Common;
using Data;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UISettingsLayer : UILayer, IMessageReceiver
    {
        #region Properties

        private TextMeshProUGUI TMPWalletButtonDes { get; set; }
        private TextMeshProUGUI TMPIntroducerButtonDes { get; set; }
        
        private string BindInviteCode { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChange;
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChange;
        }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
            this["BtnClose"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCloseClick);
            this["BtnWalletBind"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnBindWalletClick);
            this["BtnIntroducerBind"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnBindIntroducerClick);

            TMPWalletButtonDes = this["BtnWalletBind"].GetComponentInChildren<TextMeshProUGUI>();
            TMPIntroducerButtonDes = this["BtnIntroducerBind"].GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void OnShow()
        {
            base.OnShow();
            SetWallet();
            SetIntroducer();
        }

        #endregion

        #region Function

        private void SetWallet()
        {
            if(!TMPWalletButtonDes) return;
            TMPIntroducerButtonDes.text = this.GetLocalizedText("bind");
        }

        private void SetIntroducer()
        {
            if(!TMPIntroducerButtonDes) return;
            TMPIntroducerButtonDes.text = PlayerSandbox.Instance.ChessBoardHandler.Invitee.IsNullOrEmpty() 
                ? this.GetLocalizedText("bind") : PlayerSandbox.Instance.ChessBoardHandler.Invitee;
        }

        #endregion

        #region Function - API

        public static UISettingsLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UISettingsLayer>("UISettingsLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }

        #endregion

        #region Callback

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        private void OnBindWalletClick(UIBasicButton sender)
        {
            
        }

        private void OnBindIntroducerClick(UIBasicButton sender)
        {
            // TODO: 绑定邀请码， 缺少相关绑定接口
            // ...
            if(!PlayerSandbox.Instance.ChessBoardHandler.Invitee.IsNullOrEmpty()) return;

            var inputLayer = UIInputLayer.GetLayer();
            inputLayer.IsCorrectFormatAction = () => true;
            inputLayer.OnSubmitAction = code =>
            {
                GameSessionAPI.CharacterAPI.BindInviter(code);
                BindInviteCode = code;
            };
            UIInputLayer.ShowLayer();
        }

        #endregion

        #region Callback

        private void OnLanguageChange(string preVal, string newVal)
        {
            SetIntroducer();
        }

        #endregion

        #region Fuction - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (GameSessionAPI.CharacterAPI.ServiceName == service && GSCharacterAPI.MethodBindInviter == method)
            {
                // TODO: 邀请码绑定, 绑定成功需要将本地的绑定按钮内容修改为绑定的用户id
                // ...
                if (response.IsSuccess())
                {
                    PlayerSandbox.Instance.ChessBoardHandler.Invitee = BindInviteCode;
                    SetIntroducer();
                }
            }
        }
        
        #endregion
    }

}