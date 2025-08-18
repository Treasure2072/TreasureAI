using System.Collections.Generic;
using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Game
{
    public class LoginDirectlyComponent : UIComponent
    {
        #region Propertiy

        [Header("Settings")]
        [SerializeField] private float timeout = 15f;
        
        [Header("Reference")]
        [SerializeField] private TextMeshProUGUI tmpAccount;
        [SerializeField] private Button btnLogin;
        [SerializeField] private Button btnCancel;

        public string Email { get; set; }
        
        #endregion
        
        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            btnLogin.onClick.AddListener(OnLoginDirectlyClick);
            btnCancel.onClick.AddListener(OnCancelClick);
        }

        public override void OnShow()
        {
            base.OnShow();

            if (GetValidFirstAccount() == null)
            {
                OnCancelClick();
            }
            else
            {
                // SetEmail();
            }
            
        }

        #endregion

        #region Function

        // private void SetEmail()
        // {
        //     tmpAccount.SetText(PlayerSandbox.Instance.RegisterAndLoginHandler.Email);
        // }

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

        private void OnLoginDirectlyClick()
        {
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, timeout);
            GameSessionAPI.LoginAPI.SendLogin(Email,
                PlayerSandbox.Instance.RegisterAndLoginHandler.LoggedEmails.GetValueOrDefault(Email, ""),
                response =>
                {
                    UILoadingLayer.GetLayer()?.HideLayer();
                    UILoginLayer.GetLayer()?.OnVerifyResponseCallback(response, Email);
                    if (!response.IsSuccess())
                    {
                        UILoginLayer.GetLayer().OpenLoginEmailModal();
                    }
                });
        }

        private void OnCancelClick()
        {
            UILoginLayer.GetLayer().OpenLoginEmailModal();
        }

        #endregion
    }
}