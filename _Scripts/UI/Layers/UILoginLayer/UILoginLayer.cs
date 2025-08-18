using System;
using _Scripts.UI.Common;
using Data;
using DragonLi.Android;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Game
{
    public class UILoginLayer : UILayer
    {
        #region Properties - UI Component

        private LanguageComponent LanguageComp { get; set; }
        
        private LoginDirectlyComponent LoginDirectlyComp { get; set; }

        private LoginEmailComponent LoginEmailComp { get; set; }

        private RegisterEmailComponent RegisterEmailComp { get; set; }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            LanguageComp = this["Language"].As<LanguageComponent>();
            LoginDirectlyComp = this["Login_Directly"].As<LoginDirectlyComponent>();
            LoginEmailComp = this["Login_Email"].As<LoginEmailComponent>();
            RegisterEmailComp = this["Register_Email"].As<RegisterEmailComponent>();
            
            this["BtnLanguage"].As<UIBasicButton>().OnClickEvent.AddListener(LanguageComp.OnNextLanguageClick);
            
            this["BtnLoginEmailInput"].As<UIBasicButton>().OnClickEvent.AddListener(LoginEmailComp.OnInputEmailClick);
            this["BtnLoginVerifyInput"].As<UIBasicButton>().OnClickEvent.AddListener(LoginEmailComp.OnInputVerifyClick);
            this["BtnLoginEmailVerify"].As<UIBasicButton>().OnClickEvent.AddListener(LoginEmailComp.OnGetVerifyClick);
            this["BtnLogin"].As<UIBasicButton>().OnClickEvent.AddListener(LoginEmailComp.OnLoginClick);
            this["BtnRegister"].As<UIBasicButton>().OnClickEvent.AddListener(LoginEmailComp.OnRegisterClick);
            
            this["BtnRegisterEmailInput"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnInputEmailClick);
            this["BtnRegisterEmailCodeInput"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnInputCodeClick);
            this["BtnRegisterEmailPaste"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnPasteCodeClick);
            this["BtnRegisterVerifyInput"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnInputVerifyClick);
            this["BtnRegisterEmailVerify"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnGetVerifyClick);

            this["BtnRegisterEmailNext"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnNextClick);
            this["BtnRegisterEmailBack"].As<UIBasicButton>().OnClickEvent.AddListener(RegisterEmailComp.OnBackClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (GetValidFirstAccount() != null)
            {
                OpenLoginDirectlyModal();
            }
        }

        protected override void OnTick()
        {
            base.OnTick();
            this["BtnRegisterEmailPaste"].gameObject.SetActive(false);

#if UNITY_ANDROID
            this["BtnRegisterEmailPaste"].gameObject.SetActive(true);
            this["BtnRegisterEmailPaste"].GetComponent<Button>().interactable =
                !AndroidExternalAPI.GetFromClipboard().IsNullOrEmpty();
#endif
        }

        #endregion

        #region Function
        
        private string GetValidFirstAccount()
        {
            foreach (var account in PlayerSandbox.Instance.RegisterAndLoginHandler.LoggedEmails)
            {
                if (MathAPI.IsValidEmail(account.Key) && !account.Value.IsNullOrEmpty())
                {
                    return account.Key;
                }
            }
            
            return null;
        }

        private void PopTipError(string localizationErrorKey, UnityEngine.Events.UnityAction callback = null)
        {
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                this.GetLocalizedText(localizationErrorKey),
                UITipLayer.ETipType.Bad,
                callback);
        }

        #endregion
        
        #region API

        public static UILoginLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UILoginLayer>("UILoginLayer");
            Debug.Assert(layer);
            return layer;
        }
        
        public void OpenLoginDirectlyModal() 
        {
            LoginDirectlyComp.gameObject.SetActive(true);
            LoginEmailComp.gameObject.SetActive(false);
            RegisterEmailComp.gameObject.SetActive(false);
        }

        public void OpenLoginEmailModal()
        {
            LoginDirectlyComp.gameObject.SetActive(false);
            LoginEmailComp.gameObject.SetActive(true);
            RegisterEmailComp.gameObject.SetActive(false);
        }

        public void OpenRegisterEmailModal()
        {
            LoginDirectlyComp.gameObject.SetActive(false);
            LoginEmailComp.gameObject.SetActive(false);
            RegisterEmailComp.gameObject.SetActive(true);
        }

        public void PopTipEmailInvalid()
        {
            PopTipError("email-invalid");
        }

        public void PopTipInviteCodeInvalid()
        {
            PopTipError("invite-code-invalid");
        }

        public void PopTipVerifyCodeInvalid()
        {
            PopTipError("verify-code-invalid");
        }

        #endregion

        #region Callback - Login

        /// <summary>
        /// 登录或注册验证回调
        /// </summary>
        /// <param name="response"></param>
        /// <param name="email"></param>
        public void OnVerifyResponseCallback(HttpResponseProtocol response, string email)
        {
            if (response.IsSuccess())
            {
                this.LogEditorOnly(JsonConvert.SerializeObject(response));
                var id = response.GetAttachmentAsString("id");
                var token = response.GetAttachmentAsString("token");
                var userName = response.GetAttachmentAsString("name");
                var inviterName = response.GetAttachmentAsString("inviter-name");
                var inviteCode = response.GetAttachmentAsString("invite-code");
                PlayerSandbox.Instance.RegisterAndLoginHandler.Email = email;
                PlayerSandbox.Instance.RegisterAndLoginHandler.Id = id;
                PlayerSandbox.Instance.RegisterAndLoginHandler.Token = token;
                PlayerSandbox.Instance.RegisterAndLoginHandler.Name = userName;
                PlayerSandbox.Instance.RegisterAndLoginHandler.InviterName = inviterName;
                PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode = inviteCode;

                // 区分正常用户，和游客用户
                // 游客用户不需要缓存数据到本地
                if (MathAPI.IsValidEmail(email))
                {
                    PlayerSandbox.Instance.RegisterAndLoginHandler.LoggedEmails[email] = token;
                    PlayerSandbox.Instance.RegisterAndLoginHandler.SaveToLocal();
                }
                
                StartCoroutine(GameMode.GetGameMode<StartGameMode>()?.GameStart());
            }
            else
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"),
                    this.GetLocalizedText(response.GetError()),
                    UITipLayer.ETipType.Bad);
            }
        }

        #endregion
        
    }
}


